//using System.Net;
//using System.Net.Http.Json;
//using UberSy.Api.IntegrationTests.Common;
//using UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;
//using UberSy.Contract.Requests.Menu;
//using UberSy.Domain.Identity;
//using UberSy.Test.Common.Security;
//using Xunit;

//namespace UberSy.Api.IntegrationTests.Controllers;

//[Collection(WebAppFactoryCollection.CollectionName)]
//public class AddOnsControllerTests(WebAppFactory factory) : AppTestBase(factory)
//{
//    // AppHttpClient needs to be initialized using the factory
//    private readonly AppHttpClient _client = factory.CreateAppHttpClient();

//    [Fact]
//    public async Task GetAddOnById_WithValidId_ShouldReturnAddOn()
//    {
//        // Arrange
//        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
//        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
//        var token = await _client.GenerateTokenAsync(TestUsers.Owner1);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.GetAsync($"/api/v1.0/addons/{addOn.Id}", CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

//        var result = await response.Content.ReadFromJsonAsync<AddOnDto>(CancellationToken.None);
//        Assert.NotNull(result);
//        Assert.Equal(addOn.Id, result.Id);
//        Assert.Equal(addOn.NameEn, result.NameEn);
//    }

//    [Fact]
//    public async Task GetAddOnById_WithInvalidId_ShouldReturnNotFound()
//    {
//        // Arrange
//        var token = await _client.GenerateTokenAsync(TestUsers.Owner1);
//        _client.SetAuthorizationHeader(token);
//        var randomId = Guid.NewGuid();

//        // Act
//        var response = await _client.GetAsync($"/api/v1.0/addons/{randomId}", CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//    }

//    [Fact]
//    public async Task CreateAddOn_WithValidRequest_ShouldCreateAddOn()
//    {
//        // Arrange
//        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
//        var token = await _client.GenerateTokenAsync(TestUsers.Owner1);
//        AsOwner1();
//        _client.SetAuthorizationHeader(token);

//        var request = new CreateAddOnRequest("جبنة إضافية", "Extra Cheese", 5.0m);

//        // Act
//        var response = await _client.PostAsJsonAsync(
//            $"/api/v1.0/restaurants/{restaurant.Id}/addons",
//            request,
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

//        var result = await response.Content.ReadFromJsonAsync<AddOnDto>(CancellationToken.None);
//        Assert.NotNull(result);
//        Assert.Equal(request.NameAr, result.NameAr);
//        Assert.Equal(request.Price, result.Price);
//    }

//    [Fact]
//    public async Task CreateAddOn_WithoutOwnerRole_ShouldReturnForbidden()
//    {
//        // Arrange
//        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

//        // Using a customer token instead of owner
//        AsCustomer();
//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        var request = new CreateAddOnRequest("جبنة", "Cheese", 5.0m);

//        // Act
//        var response = await _client.PostAsJsonAsync(
//            $"/api/v1.0/restaurants/{restaurant.Id}/addons",
//            request,
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//    }

//    [Fact]
//    public async Task UpdateAddOn_WithValidRequest_ShouldUpdateAddOn()
//    {
//        // Arrange
//        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
//        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);

//        AsOwner1();
//        var token = await _client.GenerateTokenAsync(TestUsers.Owner1);
//        _client.SetAuthorizationHeader(token);

//        var request = new UpdateAddOnRequest("Updated AR", "Updated EN", 15.0m);

//        // Act
//        var response = await _client.PutAsJsonAsync(
//            $"/api/v1.0/addons/{addOn.Id}",
//            request,
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
//    }

//    [Fact]
//    public async Task ActivateAddOn_WithValidId_ShouldActivateAddOn()
//    {
//        // Arrange
//        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
//        // Seed as inactive so we can activate it
//        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: false);

//        AsOwner1();
//        var token = await _client.GenerateTokenAsync(TestUsers.Owner1);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.PostAsJsonAsync(
//            $"/api/v1.0/addons/{addOn.Id}/activate",
//            new { }, // Empty body
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
//    }

//    [Fact]
//    public async Task DeactivateAddOn_WithValidId_ShouldDeactivateAddOn()
//    {
//        // Arrange
//        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
//        // Seed as active (default) so we can deactivate it
//        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);

//        AsOwner1();
//        var token = await _client.GenerateTokenAsync(TestUsers.Owner1);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.PostAsJsonAsync(
//            $"/api/v1.0/addons/{addOn.Id}/deactivate",
//            new { }, // Empty body
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
//    }

//    [Fact]
//    public async Task GetAddOnsByRestaurant_WithoutAuthentication_ShouldReturnUnauthorized()
//    {
//        // Arrange
//        var restaurantId = Guid.NewGuid();

//        // Act
//        var response = await _client.GetAsync(
//            $"/api/v1.0/restaurants/{restaurantId}/addons",
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//    }
//}
