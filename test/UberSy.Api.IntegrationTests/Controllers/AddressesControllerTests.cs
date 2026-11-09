
//using System.Net;
//using System.Net.Http.Json;
//using UberSy.Api.IntegrationTests.Common;
//using UberSy.Application.Features.Addresses.Dtos;
//using UberSy.Application.SubcutaneousTests.Common.Factories;
//using UberSy.Contract.Requests.Addresses;
//using UberSy.Domain.Addresses;
//using UberSy.Test.Common.Security;
//using Xunit;

//namespace UberSy.Api.IntegrationTests.Controllers;

//[Collection(WebAppFactoryCollection.CollectionName)]
//public class AddressesControllerTests(WebAppFactory factory) : AppTestBase(factory)
//{
//    private readonly AppHttpClient _client = factory.CreateAppHttpClient();

//    // ---------- REQUEST HELPERS ----------

//    private static CreateAddressRequest CreateRequest(TbAddress address)
//        => new(
//            Street: address.Street,
//            City: address.City,
//            Latitude: address.Boundary.X,
//            Longitude: address.Boundary.Y,
//            Label: address.Label,
//            Floor: address.Floor,
//            Phone: address.Phone);

//    private static UpdateAddressRequest UpdateRequest(TbAddress address)
//        => new(
//            Street: address.Street,
//            City: address.City,
//            Latitude: address.Boundary.X,
//            Longitude: address.Boundary.Y,
//            Label: address.Label,
//            Floor: address.Floor,
//            Phone: address.Phone);

//    // ---------- GET MINE ----------

//    [Fact]
//    public async Task GetMine_WithExistingAddresses_ShouldReturnMyAddresses()
//    {
//        // Arrange
//        AsCustomer();

//        var address = AddressFactory.CreateValidAddress(
//            userId: TestUsers.Customer.Id);

//        var otherAddress = AddressFactory.CreateValidAddress(
//            userId: TestUsers.Owner1.Id);

//        Context.TbAddresses.AddRange(address, otherAddress);
//        await Context.SaveChangesAsync(CancellationToken.None);

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.GetAsync(
//            "/api/v1.0/addresses",
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

//        var result = await response.Content.ReadFromJsonAsync<List<AddressDto>>(
//            CancellationToken.None);

//        Assert.NotNull(result);
//        Assert.Single(result);
//        Assert.Equal(address.Id, result[0].Id);
//    }

//    [Fact]
//    public async Task GetMine_WithoutAuthentication_ShouldReturnUnauthorized()
//    {
//        // Act
//        var response = await _client.GetAsync(
//            "/api/v1.0/addresses",
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//    }

//    // ---------- GET BY ID ----------

//    [Fact]
//    public async Task GetById_WithValidId_ShouldReturnAddress()
//    {
//        // Arrange
//        AsCustomer();

//        var address = AddressFactory.CreateValidAddress(
//            userId: TestUsers.Customer.Id);

//        Context.TbAddresses.Add(address);
//        await Context.SaveChangesAsync(CancellationToken.None);

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.GetAsync(
//            $"/api/v1.0/addresses/{address.Id}",
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

//        var result = await response.Content.ReadFromJsonAsync<AddressDto>(
//            CancellationToken.None);

//        Assert.NotNull(result);
//        Assert.Equal(address.Id, result.Id);
//    }

//    [Fact]
//    public async Task GetById_WithInvalidId_ShouldReturnNotFound()
//    {
//        // Arrange
//        AsCustomer();

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        var randomId = Guid.NewGuid();

//        // Act
//        var response = await _client.GetAsync(
//            $"/api/v1.0/addresses/{randomId}",
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//    }

//    [Fact]
//    public async Task GetById_WithAnotherUsersAddress_ShouldReturnNotFound()
//    {
//        // Arrange
//        AsCustomer();

//        var address = AddressFactory.CreateValidAddress(
//            userId: TestUsers.Owner1.Id);

//        Context.TbAddresses.Add(address);
//        await Context.SaveChangesAsync(CancellationToken.None);

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.GetAsync(
//            $"/api/v1.0/addresses/{address.Id}",
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//    }

//    // ---------- CREATE ----------

//    [Fact]
//    public async Task Create_WithValidRequest_ShouldCreateAddress()
//    {
//        // Arrange
//        AsCustomer();

//        var address = AddressFactory.CreateValidAddress();
//        var request = CreateRequest(address);

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.PostAsJsonAsync(
//            "/api/v1.0/addresses",
//            request,
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

//        var result = await response.Content.ReadFromJsonAsync<AddressDto>(
//            CancellationToken.None);

//        Assert.NotNull(result);
//        Assert.NotEqual(Guid.Empty, result.Id);
//        Assert.Equal(request.Street, result.Street);
//        Assert.Equal(request.City, result.City);
//        Assert.Equal(request.Label, result.Label);
//        Assert.Equal(request.Phone, result.Phone);
//    }

//    [Fact]
//    public async Task Create_WithoutAuthentication_ShouldReturnUnauthorized()
//    {
//        // Arrange
//        var address = AddressFactory.CreateValidAddress();
//        var request = CreateRequest(address);

//        // Act
//        var response = await _client.PostAsJsonAsync(
//            "/api/v1.0/addresses",
//            request,
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//    }

//    // ---------- UPDATE ----------

//    [Fact]
//    public async Task Update_WithValidRequest_ShouldUpdateAddress()
//    {
//        // Arrange
//        AsCustomer();

//        var address = AddressFactory.CreateValidAddress(
//            userId: TestUsers.Customer.Id);

//        Context.TbAddresses.Add(address);
//        await Context.SaveChangesAsync(CancellationToken.None);

//        // Create another valid address to obtain update data
//        var updatedAddress = AddressFactory.CreateValidAddress();
//        var request = UpdateRequest(updatedAddress);

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.PutAsJsonAsync(
//            $"/api/v1.0/addresses/{address.Id}",
//            request,
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
//    }

//    [Fact]
//    public async Task Update_WithInvalidId_ShouldReturnNotFound()
//    {
//        // Arrange
//        AsCustomer();

//        var address = AddressFactory.CreateValidAddress();
//        var request = UpdateRequest(address);

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        var randomId = Guid.NewGuid();

//        // Act
//        var response = await _client.PutAsJsonAsync(
//            $"/api/v1.0/addresses/{randomId}",
//            request,
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//    }

//    [Fact]
//    public async Task Update_WithAnotherUsersAddress_ShouldReturnNotFound()
//    {
//        // Arrange
//        AsCustomer();

//        var address = AddressFactory.CreateValidAddress(
//            userId: TestUsers.Owner1.Id);

//        Context.TbAddresses.Add(address);
//        await Context.SaveChangesAsync(CancellationToken.None);

//        var updatedAddress = AddressFactory.CreateValidAddress();
//        var request = UpdateRequest(updatedAddress);

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.PutAsJsonAsync(
//            $"/api/v1.0/addresses/{address.Id}",
//            request,
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//    }

//    // ---------- MARK AS DEFAULT ----------

//    [Fact]
//    public async Task MarkAsDefault_WithValidId_ShouldMarkAddressAsDefault()
//    {
//        // Arrange
//        AsCustomer();

//        var address = AddressFactory.CreateValidAddress(
//            userId: TestUsers.Customer.Id);

//        Context.TbAddresses.Add(address);
//        await Context.SaveChangesAsync(CancellationToken.None);

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.PostAsJsonAsync(
//            $"/api/v1.0/addresses/{address.Id}/default",
//            new { },
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
//    }

//    [Fact]
//    public async Task MarkAsDefault_WithInvalidId_ShouldReturnNotFound()
//    {
//        // Arrange
//        AsCustomer();

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        var randomId = Guid.NewGuid();

//        // Act
//        var response = await _client.PostAsJsonAsync(
//            $"/api/v1.0/addresses/{randomId}/default",
//            new { },
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//    }

//    [Fact]
//    public async Task MarkAsDefault_WithAnotherUsersAddress_ShouldReturnNotFound()
//    {
//        // Arrange
//        AsCustomer();

//        var address = AddressFactory.CreateValidAddress(
//            userId: TestUsers.Owner1.Id);

//        Context.TbAddresses.Add(address);
//        await Context.SaveChangesAsync(CancellationToken.None);

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.PostAsJsonAsync(
//            $"/api/v1.0/addresses/{address.Id}/default",
//            new { },
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//    }

//    // ---------- DELETE ----------

//    [Fact]
//    public async Task Delete_WithValidId_ShouldDeleteAddress()
//    {
//        // Arrange
//        AsCustomer();

//        var address = AddressFactory.CreateValidAddress(
//            userId: TestUsers.Customer.Id);

//        Context.TbAddresses.Add(address);
//        await Context.SaveChangesAsync(CancellationToken.None);

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.DeleteAsync(
//            $"/api/v1.0/addresses/{address.Id}",
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
//    }

//    [Fact]
//    public async Task Delete_WithInvalidId_ShouldReturnNotFound()
//    {
//        // Arrange
//        AsCustomer();

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        var randomId = Guid.NewGuid();

//        // Act
//        var response = await _client.DeleteAsync(
//            $"/api/v1.0/addresses/{randomId}",
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//    }

//    [Fact]
//    public async Task Delete_WithAnotherUsersAddress_ShouldReturnNotFound()
//    {
//        // Arrange
//        AsCustomer();

//        var address = AddressFactory.CreateValidAddress(
//            userId: TestUsers.Owner1.Id);

//        Context.TbAddresses.Add(address);
//        await Context.SaveChangesAsync(CancellationToken.None);

//        var token = await _client.GenerateTokenAsync(TestUsers.Customer);
//        _client.SetAuthorizationHeader(token);

//        // Act
//        var response = await _client.DeleteAsync(
//            $"/api/v1.0/addresses/{address.Id}",
//            CancellationToken.None);

//        // Assert
//        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//    }
//}
