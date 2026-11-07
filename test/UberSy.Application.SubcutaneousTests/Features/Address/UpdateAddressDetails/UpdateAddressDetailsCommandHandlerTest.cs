using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Application.SubcutaneousTests.Common.Factories;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Address.UpdateAddressDetails;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateAddressDetailsCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public UpdateAddressDetailsCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Handle_WithValidData_Should_Update_Address_Details()
    {
        // Arrange
        var customer = TestUsers.Customer;
        _currentUser.Returns(customer);

        var address = AddressFactory.CreateValidAddress(userId: customer.Id);
        _context.TbAddresses.Add(address);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(
            addressId: address.Id,
            street: "New Street",
            city: "Aleppo",
            latitude: 36.2021,
            longitude: 37.1343,
            label: "Work",
            floor: "7",
            phone: "+963912345679");

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(
            result.IsSuccess,
            result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var updated = await _context.TbAddresses.AsNoTracking().FirstAsync(a => a.Id == address.Id);

        Assert.Equal("New Street", updated.Street);
        Assert.Equal("Aleppo", updated.City);
        Assert.Equal("Work", updated.Label);
        Assert.Equal("7", updated.Floor);
        Assert.Equal("+963912345679", updated.Phone);
        Assert.Equal(36.2021, updated.Boundary.Y, precision: 6);
        Assert.Equal(37.1343, updated.Boundary.X, precision: 6);
    }

    [Fact]
    public async Task Handle_WithValidData_Should_Not_Change_Other_Users_Addresses()
    {
        // Arrange
        var customer = TestUsers.Customer;
        var otherCustomer = TestUsers.Customer2;
        _currentUser.Returns(customer);

        var myAddress = AddressFactory.CreateValidAddress(userId: customer.Id);
        var otherAddress = AddressFactory.CreateValidAddress(userId: otherCustomer.Id, street: "Other Street");
        _context.TbAddresses.Add(myAddress);
        _context.TbAddresses.Add(otherAddress);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(
            addressId: myAddress.Id,
            street: "Changed Street");

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var untouched = await _context.TbAddresses.AsNoTracking().FirstAsync(a => a.Id == otherAddress.Id);
        Assert.Equal("Other Street", untouched.Street);
    }

    [Fact]
    public async Task Handle_When_Address_Not_Found_Should_Return_AddressNotFound_Error()
    {
        // Arrange
        _currentUser.Returns(TestUsers.Customer);

        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(addressId: Guid.NewGuid());

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AddressNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_When_User_Does_Not_Own_The_Address_Should_Return_AccessDenied_And_Not_Update()
    {
        // Arrange
        var owner = TestUsers.Customer2;
        var address = AddressFactory.CreateValidAddress(userId: owner.Id, street: "Original Street");
        _context.TbAddresses.Add(address);
        await _context.SaveChangesAsync(CancellationToken.None);

        _currentUser.Returns(TestUsers.Customer);

        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(
            addressId: address.Id,
            street: "Hacked Street");

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);

        var unchanged = await _context.TbAddresses.AsNoTracking().FirstAsync(a => a.Id == address.Id);
        Assert.Equal("Original Street", unchanged.Street);
    }

    [Fact]
    public async Task Handle_With_Invalid_Command_Should_Return_Error_And_Not_Update()
    {
        // Arrange
        var customer = TestUsers.Customer;
        _currentUser.Returns(customer);

        var address = AddressFactory.CreateValidAddress(userId: customer.Id, street: "Original Street");
        _context.TbAddresses.Add(address);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(
            addressId: address.Id,
            street: "");

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);

        var unchanged = await _context.TbAddresses.AsNoTracking().FirstAsync(a => a.Id == address.Id);
        Assert.Equal("Original Street", unchanged.Street);
    }
}
