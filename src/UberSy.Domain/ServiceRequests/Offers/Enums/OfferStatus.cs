namespace UberSy.Domain.ServiceRequests.Offers.Enums;

public enum OfferStatus : byte
{
    Sent = 1,
    Accepted = 2,
    Rejected = 3,
    Expired = 4,
    LostToAnotherDriver = 5,
}