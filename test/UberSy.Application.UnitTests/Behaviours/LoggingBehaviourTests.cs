using Microsoft.Extensions.Logging;
using NSubstitute;
using UberSy.Application.Common.Behaviours;
using UberSy.Application.Common.Interfaces;
using Xunit;

namespace UberSy.Application.UnitTests.Behaviours;

public class LoggingBehaviourTests
{
    private readonly ILogger<DummyRequest> _logger = Substitute.For<ILogger<DummyRequest>>();
    private readonly IUser _user = Substitute.For<IUser>();
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();

    private readonly LoggingBehaviour<DummyRequest> _sut;

    public LoggingBehaviourTests()
    {
        _sut = new LoggingBehaviour<DummyRequest>(_logger, _user, _identityService);
    }

    [Fact]
    public async Task Process_WithUserId_LogsRequestWithUserName()
    {
        // Arrange
        var request = new DummyRequest();
        _user.Id.Returns("00000000-0000-0000-0000-000000000000");
        _identityService.GetUserNameAsync(Guid.Parse("00000000-0000-0000-0000-000000000000")).Returns("kinan");

        // Act
        await _sut.Process(request, CancellationToken.None);

        // Assert
        await _identityService.Received(1).GetUserNameAsync(Guid.Parse("00000000-0000-0000-0000-000000000000"));

        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Request")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task Process_WithoutUserId_LogsRequestWithEmptyUserName()
    {
        // Arrange
        var request = new DummyRequest();
        _user.Id.Returns((string?)null);

        // Act
        await _sut.Process(request, CancellationToken.None);

        // Assert
        await _identityService.DidNotReceive().GetUserNameAsync(Arg.Any<Guid>());

        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Request")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    public class DummyRequest;
}
