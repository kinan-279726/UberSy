using UberSy.Domain.Common.Results;

namespace UberSy.Domain.ServiceRequests.Offers;

public static class OfferErrors
{
    public static Error DriverIdRequired =>
        Error.Validation("Driver_Id_Required", "Driver Id Is Required");

    public static Error OfferAlreadyRespondedTo =>
        Error.Conflict("Offer_Already_Responded_To", "This Offer Has Already Been Responded To");

    public static Error OfferExpired =>
        Error.Conflict("Offer_Expired", "This Offer Has Expired");
}