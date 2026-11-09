using UberSy.Application.Common.Errors;
using UberSy.Application.Features.ServiceRequests.Offers.Commands.RejectOffer;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Domain.ServiceRequests.Offers;
using UberSy.Domain.ServiceRequests.Offers.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.RejectOffer;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class RejectOfferCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    private async Task<(Guid OfferId, Guid OrderId)> SeedOfferForDriver1Async(OfferStatus status = OfferStatus.Sent)
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        var offer = await ServiceRequestTestData.SeedOfferAsync(Context, order.Id, driver.Id, status);

        return (offer.Id, order.Id);
    }

    [Fact]
    public async Task Handle_ShouldMarkOfferRejected_WhenDriverRejects()
    {
        var (offerId, orderId) = await SeedOfferForDriver1Async();
        AsDriver1();

        var result = await Mediator.Send(new RejectOfferCommand(offerId), CancellationToken.None);

        AssertSuccess(result);
        var saved = await ServiceRequestTestData.LoadOfferAsync(Context, offerId);
        Assert.Equal(OfferStatus.Rejected, saved.Status);
        Assert.NotNull(saved.RespondedAt);

        // Rejecting an offer must not touch the order
        var order = await ServiceRequestTestData.LoadOrderAsync(Context, orderId);
        Assert.Null(order.DriverId);
    }

    [Theory]
    [InlineData(OfferStatus.Accepted)]
    [InlineData(OfferStatus.Rejected)]
    [InlineData(OfferStatus.Expired)]
    public async Task Handle_ShouldReturnAlreadyRespondedTo_WhenOfferIsNotSent(OfferStatus status)
    {
        var (offerId, _) = await SeedOfferForDriver1Async(status);
        AsDriver1();

        var result = await Mediator.Send(new RejectOfferCommand(offerId), CancellationToken.None);

        AssertError(result, OfferErrors.OfferAlreadyRespondedTo);
    }

    [Fact]
    public async Task Handle_ShouldReturnOfferNotFound_WhenOfferMissing()
    {
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        AsDriver1();

        var result = await Mediator.Send(new RejectOfferCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.OfferNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenOfferBelongsToAnotherDriver()
    {
        var (offerId, _) = await SeedOfferForDriver1Async();
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id);
        AsDriver2();

        var result = await Mediator.Send(new RejectOfferCommand(offerId), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
        Assert.Equal(OfferStatus.Sent, (await ServiceRequestTestData.LoadOfferAsync(Context, offerId)).Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenUserIsNotADriver()
    {
        var (offerId, _) = await SeedOfferForDriver1Async();
        AsCustomer();

        var result = await Mediator.Send(new RejectOfferCommand(offerId), CancellationToken.None);

        AssertError(result, ApplicationErrors.DriverNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var (offerId, _) = await SeedOfferForDriver1Async();

        var result = await Mediator.Send(new RejectOfferCommand(offerId), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenOfferIdEmpty()
    {
        // KNOWN GAP: RejectOfferCommand has no validator (AcceptOfferCommand has one), so Guid.Empty reaches the
        // database and comes back as OfferNotFound instead of a validation error on "OfferId".
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        AsDriver1();

        var result = await Mediator.Send(new RejectOfferCommand(Guid.Empty), CancellationToken.None);

        AssertErrorCode(result, "OfferId");
    }
}
