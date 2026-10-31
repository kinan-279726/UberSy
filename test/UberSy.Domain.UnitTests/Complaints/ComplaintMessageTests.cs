using UberSy.Domain.Complaints;
using Xunit;

namespace UberSy.Domain.UnitTests.Complaints;

public class ComplaintMessageTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbComplaintMessage CreateValidComplaintMessage()
    {
        return TbComplaintMessage.Create(
            id: Guid.NewGuid(),
            complaintId: Guid.NewGuid(),
            senderUserId: Guid.NewGuid(),
            message: "This is a valid test message."
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenSenderUserIdIsEmpty()
    {
        var result = TbComplaintMessage.Create(
            id: Guid.NewGuid(),
            complaintId: Guid.NewGuid(),
            senderUserId: Guid.Empty,
            message: "This is a valid test message.");

        Assert.False(result.IsSuccess);
        Assert.Equal(ComplaintErrors.UserIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenMessageIsEmpty()
    {
        var result = TbComplaintMessage.Create(
            id: Guid.NewGuid(),
            complaintId: Guid.NewGuid(),
            senderUserId: Guid.NewGuid(),
            message: "");

        Assert.False(result.IsSuccess);
        Assert.Equal(ComplaintErrors.MessageRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenMessageIsWhiteSpace()
    {
        var result = TbComplaintMessage.Create(
            id: Guid.NewGuid(),
            complaintId: Guid.NewGuid(),
            senderUserId: Guid.NewGuid(),
            message: "   ");

        Assert.False(result.IsSuccess);
        Assert.Equal(ComplaintErrors.MessageRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenMessageIsTooLong()
    {
        var result = TbComplaintMessage.Create(
            id: Guid.NewGuid(),
            complaintId: Guid.NewGuid(),
            senderUserId: Guid.NewGuid(),
            message: new string('A', 2001));

        Assert.False(result.IsSuccess);
        Assert.Equal(ComplaintErrors.MessageTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var complaintId = Guid.NewGuid();
        var senderUserId = Guid.NewGuid();
        var rawMessage = "  This message needs trimming.  ";

        // Act
        var result = TbComplaintMessage.Create(
            id: id,
            complaintId: complaintId,
            senderUserId: senderUserId,
            message: rawMessage);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify properties are set correctly
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(complaintId, result.Value.ComplaintId);
        Assert.Equal(senderUserId, result.Value.SenderUserId);

        // Verify the message was trimmed
        Assert.Equal("This message needs trimming.", result.Value.Message);
    }
}
