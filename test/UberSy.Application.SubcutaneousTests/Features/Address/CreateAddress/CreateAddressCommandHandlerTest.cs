using MediatR;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Addresses.Commands.CreateAddress;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Application.SubcutaneousTests.Common.Factories;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Address.CreateAddress;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class CreateAddressCommandHandlerTest : IAsyncLifetime
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly WebAppFactory _factory;
    private readonly TestCurrentUser _currentUser;

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    public CreateAddressCommandHandlerTest(WebAppFactory factory)
    {
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
        _factory = factory;
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed_And_SetFirstAddressAsDefault()
    {
        // Arrange
        var customer = TestUsers.Customer;
        _currentUser.Returns(customer);

        var address = AddressFactory.CreateValidAddress(userId: customer.Id);

        var command = new CreateAddressCommand(
            Street: address.Street,
            City: address.City,
            Latitude: address.Boundary.Y,
            Longitude: address.Boundary.X,
            Label: address.Label,
            Floor: address.Floor,
            Phone: address.Phone);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var returnedAddress = result.Value;

        Assert.Equal(customer.Id, returnedAddress.UserId);
        Assert.Equal(command.Street, returnedAddress.Street);
        Assert.Equal(command.City, returnedAddress.City);
        Assert.Equal(command.Latitude, returnedAddress.Latitude, precision: 6);
        Assert.Equal(command.Longitude, returnedAddress.Longitude, precision: 6);
        Assert.Equal(command.Label, returnedAddress.Label);
        Assert.Equal(command.Floor, returnedAddress.Floor);
        Assert.Equal(command.Phone, returnedAddress.Phone);

        Assert.True(returnedAddress.IsDefault);
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Authenticated_Should_Return_Unauthorized()
    {
        // Arrange
        var address = AddressFactory.CreateValidAddress();

        var command = new CreateAddressCommand(
            Street: address.Street,
            City: address.City,
            Latitude: address.Boundary.Y,
            Longitude: address.Boundary.X,
            Label: address.Label,
            Floor: address.Floor,
            Phone: address.Phone);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.Unauthorized.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_When_User_Has_Existing_Address_Should_Not_Set_New_As_Default()
    {
        // Arrange
        var customer = TestUsers.Customer2;
        _currentUser.Returns(customer);

        var firstAddressCommand = new CreateAddressCommand(
            Street: "First Street",
            City: "Damascus",
            Latitude: 33.5138,
            Longitude: 36.2765,
            Label: "Home",
            Floor: "1",
            Phone: "+963912345678");

        var firstResult = await _mediator.Send(firstAddressCommand);
        Assert.True(firstResult.IsSuccess);
        Assert.True(firstResult.Value.IsDefault);

        var secondAddressCommand = new CreateAddressCommand(
            Street: "Second Street",
            City: "Damascus",
            Latitude: 33.5140,
            Longitude: 36.2770,
            Label: "Work",
            Floor: "2",
            Phone: "+963912345679");

        // Act
        var secondResult = await _mediator.Send(secondAddressCommand);

        // Assert
        Assert.True(secondResult.IsSuccess);
        var returnedAddress = secondResult.Value;

        Assert.Equal(customer.Id, returnedAddress.UserId);

        Assert.False(returnedAddress.IsDefault);
    }
}
