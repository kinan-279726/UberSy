using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Addresses.Commands.DeleateAddress;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Application.SubcutaneousTests.Common.Factories;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Address.DeleateAddress;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class DeleteAddressCommandHandlerTest
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public DeleteAddressCommandHandlerTest(WebAppFactory factory)
    {
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    [Fact]
    public async Task Handle_WithValidData_Should_Delete_Address_Successfully()
    {
        // Arrange
        var customer = TestUsers.Customer;
        _currentUser.Returns(customer);

        var address = AddressFactory.CreateValidAddress(userId: customer.Id);

        _context.TbAddresses.Add(address);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteAddressCommand(address.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var deletedAddress = await _context.TbAddresses
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == address.Id);

        Assert.NotNull(deletedAddress);
        Assert.True(result.IsSuccess, result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");
    }

    [Fact]
    public async Task Handle_When_Address_Not_Found_Should_Return_NotFound_Error()
    {
        // Arrange
        var customer = TestUsers.Customer;
        _currentUser.Returns(customer);

        var randomAddressId = Guid.NewGuid();
        var command = new DeleteAddressCommand(randomAddressId);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AddressNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_When_User_Does_Not_Own_The_Address_Should_Return_Forbidden_Error()
    {
        // Arrange
        var otherCustomer = TestUsers.Customer2;
        var address = AddressFactory.CreateValidAddress(userId: otherCustomer.Id);

        _context.TbAddresses.Add(address);
        await _context.SaveChangesAsync(CancellationToken.None);

        var currentCustomer = TestUsers.Customer;
        _currentUser.Returns(currentCustomer);

        var command = new DeleteAddressCommand(address.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
    }
}
