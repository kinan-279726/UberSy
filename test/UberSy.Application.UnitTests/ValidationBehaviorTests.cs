using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using UberSy.Application.Common.Behaviours;
using UberSy.Application.Features.Restaurants.Commands.CreateRestaurant;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Application.Features.Restaurants.Mappers;
using UberSy.Domain.Common.Results;
using UberSy.Test.Common.Restaurants;

using Xunit;

namespace UberSy.Application.UnitTests;

public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<CreateRestaurantCommand, Result<RestaurantDto>> _validationBehavior;
    private readonly IValidator<CreateRestaurantCommand> _mockValidator;
    private readonly RequestHandlerDelegate<Result<RestaurantDto>> _mockNextBehavior;

    public ValidationBehaviorTests()
    {
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<Result<RestaurantDto>>>();
        _mockValidator = Substitute.For<IValidator<CreateRestaurantCommand>>();

        _validationBehavior = new(_mockValidator);
    }

    [Fact]
    public async Task InvokeValidationBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange
        var createRestaurantCommand = RestaurantCommandFactory.CreateRestaurantCommand();
        var RestaurantResponse = RestaurantFactory.CreateRestaurant().Value.ToDto();

        _mockValidator
            .ValidateAsync(createRestaurantCommand, CancellationToken.None)
            .Returns(new ValidationResult());

        _mockNextBehavior.Invoke(CancellationToken.None).Returns(RestaurantResponse);

        // Act
        var result = await _validationBehavior.Handle(createRestaurantCommand, _mockNextBehavior, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(RestaurantResponse, result.Value);
    }

    [Fact]
    public async Task InvokeValidationBehavior_WhenValidatorResultIsNotValid_ShouldReturnListOfErrors()
    {
        // Arrange
        var createRestaurantCommand = RestaurantCommandFactory.CreateRestaurantCommand();

        List<ValidationFailure> validationFailures = [new(propertyName: "property1", errorMessage: "property1 is invalid")];

        _mockValidator
            .ValidateAsync(createRestaurantCommand, CancellationToken.None)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _validationBehavior.Handle(createRestaurantCommand, _mockNextBehavior, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("property1", result.TopError.Code);
        Assert.Equal("property1 is invalid", result.TopError.Description);
    }

    [Fact]
    public async Task InvokeValidationBehavior_WhenNoValidator_ShouldInvokeNextBehavior()
    {
        // Arrange
        var createRestaurantCommand = RestaurantCommandFactory.CreateRestaurantCommand();
        var validationBehavior = new ValidationBehavior<CreateRestaurantCommand, Result<RestaurantDto>>();

        var restaurant = RestaurantFactory.CreateRestaurant().Value;

        var restaurantResponse = RestaurantFactory.CreateRestaurant().Value.ToDto();

        _mockNextBehavior.Invoke(CancellationToken.None).Returns(restaurantResponse);

        // Act
        var result = await validationBehavior.Handle(createRestaurantCommand, _mockNextBehavior, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(restaurantResponse, result.Value);
    }
}
