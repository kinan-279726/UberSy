using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Application.SubcutaneousTests.Common.Factories;
using UberSy.Test.Common.Security;
using Xunit;

namespace UberSy.Application.SubcutaneousTests.Features.Address.MarkAddressAsDefault;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class MarkAddressAsDefaultCommandHandlerTest
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public MarkAddressAsDefaultCommandHandlerTest(WebAppFactory factory)
    {
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    [Fact]
    public async Task Handle_WithValidData_Should_Set_As_Default_And_Unmark_Others()
    {
        // Arrange
        var customer = TestUsers.Customer;
        _currentUser.Returns(customer);

        var address1 = AddressFactory.CreateValidAddress(userId: customer.Id);
        var address2 = AddressFactory.CreateValidAddress(userId: customer.Id);

        address1.MarkAsDefault();

        _context.TbAddresses.Add(address1);
        _context.TbAddresses.Add(address2);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = new MarkAddressAsDefaultCommand(address2.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var updatedAddress1 = await _context.TbAddresses.AsNoTracking().FirstAsync(a => a.Id == address1.Id);
        var updatedAddress2 = await _context.TbAddresses.AsNoTracking().FirstAsync(a => a.Id == address2.Id);

        Assert.False(updatedAddress1.IsDefault); 
        Assert.True(updatedAddress2.IsDefault);
    }

    [Fact]
    public async Task Handle_When_Address_Is_Already_Default_Should_Return_Success_Without_Changes()
    {
        // Arrange
        var customer = TestUsers.Customer;
        _currentUser.Returns(customer);

        var address = AddressFactory.CreateValidAddress(userId: customer.Id);
        address.MarkAsDefault();

        _context.TbAddresses.Add(address);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = new MarkAddressAsDefaultCommand(address.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var updatedAddress = await _context.TbAddresses.FirstAsync(a => a.Id == address.Id);
        Assert.True(updatedAddress.IsDefault);
    }

    [Fact]
    public async Task Handle_When_Address_Not_Found_Should_Return_AddressNotFound_Error()
    {
        // Arrange
        var customer = TestUsers.Customer;
        _currentUser.Returns(customer);

        var randomAddressId = Guid.NewGuid();
        var command = new MarkAddressAsDefaultCommand(randomAddressId);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AddressNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_When_User_Does_Not_Own_The_Address_Should_Return_AccessDenied_Error()
    {
        // Arrange
        var otherCustomer = TestUsers.Customer2;
        var address = AddressFactory.CreateValidAddress(userId: otherCustomer.Id);

        _context.TbAddresses.Add(address);
        await _context.SaveChangesAsync(CancellationToken.None);

        var currentCustomer = TestUsers.Customer;
        _currentUser.Returns(currentCustomer);

        var command = new MarkAddressAsDefaultCommand(address.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
    }
}
