using UberSy.Domain.Complaints;
using UberSy.Domain.Complaints.Enums;
using Xunit;

namespace UberSy.Domain.UnitTests.Complaints;

public class ComplaintTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbComplaint CreateValidComplaint()
    {
        return TbComplaint.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            serviceRequestId: Guid.NewGuid(),
            subject: "مشكلة في الطلب",
            message: "الطلب وصل متأخراً جداً."
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenUserIdIsEmpty()
    {
        var result = TbComplaint.Create(
            id: Guid.NewGuid(),
            userId: Guid.Empty,
            serviceRequestId: Guid.NewGuid(),
            subject: "مشكلة في الطلب",
            message: "الطلب وصل متأخراً جداً.");

        Assert.False(result.IsSuccess);
        Assert.Equal(ComplaintErrors.UserIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenSubjectIsEmpty()
    {
        var result = TbComplaint.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            serviceRequestId: null,
            subject: "",
            message: "الطلب وصل متأخراً جداً.");

        Assert.False(result.IsSuccess);
        Assert.Equal(ComplaintErrors.SubjectRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenSubjectIsTooLong()
    {
        var result = TbComplaint.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            serviceRequestId: null,
            subject: new string('A', 201),
            message: "الطلب وصل متأخراً جداً.");

        Assert.False(result.IsSuccess);
        Assert.Equal(ComplaintErrors.SubjectTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenMessageIsEmpty()
    {
        var result = TbComplaint.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            serviceRequestId: null,
            subject: "مشكلة في الطلب",
            message: "");

        Assert.False(result.IsSuccess);
        Assert.Equal(ComplaintErrors.MessageRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenMessageIsTooLong()
    {
        var result = TbComplaint.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            serviceRequestId: null,
            subject: "مشكلة في الطلب",
            message: new string('B', 2001));

        Assert.False(result.IsSuccess);
        Assert.Equal(ComplaintErrors.MessageTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_AndSetStatusToOpen_WhenDataIsValid()
    {
        var result = TbComplaint.Create(
            id: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            serviceRequestId: Guid.NewGuid(),
            subject: "  مشكلة في الطلب  ",
            message: "  الطلب وصل متأخراً جداً.  ");

        Assert.True(result.IsSuccess);
        Assert.Equal("مشكلة في الطلب", result.Value.Subject);
        Assert.Equal("الطلب وصل متأخراً جداً.", result.Value.Message);
        Assert.Equal(ComplaintStatus.Open, result.Value.Status);
    }

    // ============================================================
    // State Transition Tests (MarkInProgress)
    // ============================================================

    [Fact]
    public void MarkInProgress_ShouldReturnSuccess_AndSetStatusToInProgress()
    {
        var complaint = CreateValidComplaint();

        var result = complaint.MarkInProgress();

        Assert.True(result.IsSuccess);
        Assert.Equal(ComplaintStatus.InProgress, complaint.Status);
    }

    [Fact]
    public void MarkInProgress_ShouldReturnError_WhenComplaintIsClosed()
    {
        var complaint = CreateValidComplaint();
        complaint.Close();

        var result = complaint.MarkInProgress();

        Assert.False(result.IsSuccess);
        Assert.Equal(ComplaintErrors.ComplaintAlreadyClosed.Code, result.TopError.Code);
    }

    // ============================================================
    // State Transition Tests (Resolve)
    // ============================================================

    [Fact]
    public void Resolve_ShouldReturnSuccess_AndSetStatusToResolved()
    {
        var complaint = CreateValidComplaint();

        var result = complaint.Resolve();

        Assert.True(result.IsSuccess);
        Assert.Equal(ComplaintStatus.Resolved, complaint.Status);
    }

    [Fact]
    public void Resolve_ShouldReturnError_WhenComplaintIsClosed()
    {
        var complaint = CreateValidComplaint();
        complaint.Close();

        var result = complaint.Resolve();

        Assert.False(result.IsSuccess);
        Assert.Equal(ComplaintErrors.ComplaintAlreadyClosed.Code, result.TopError.Code);
    }

    // ============================================================
    // State Transition Tests (Close)
    // ============================================================

    [Fact]
    public void Close_ShouldReturnSuccess_AndSetStatusToClosed()
    {
        var complaint = CreateValidComplaint();

        var result = complaint.Close();

        Assert.True(result.IsSuccess);
        Assert.Equal(ComplaintStatus.Closed, complaint.Status);
    }

    [Fact]
    public void Close_ShouldReturnError_WhenComplaintIsAlreadyClosed()
    {
        var complaint = CreateValidComplaint();
        complaint.Close();

        var result = complaint.Close();

        Assert.False(result.IsSuccess);
        Assert.Equal(ComplaintErrors.ComplaintAlreadyClosed.Code, result.TopError.Code);
    }

    // ============================================================
    // State Transition Tests (Reopen)
    // ============================================================

    [Fact]
    public void Reopen_ShouldReturnSuccess_AndSetStatusToOpen()
    {
        var complaint = CreateValidComplaint();
        complaint.Close();
        Assert.Equal(ComplaintStatus.Closed, complaint.Status);

        var result = complaint.Reopen();

        Assert.True(result.IsSuccess);
        Assert.Equal(ComplaintStatus.Open, complaint.Status);
    }
}
