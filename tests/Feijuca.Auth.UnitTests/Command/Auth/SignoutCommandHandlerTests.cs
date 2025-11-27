using AutoFixture;
using Feijuca.Auth.Application.Commands.User;
using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Feijuca.Auth.Api.UnitTests.Command.Auth
{
    public class SignoutCommandHandlerTests
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly SignoutCommandHandler _handler;

        public SignoutCommandHandlerTests()
        {
            _handler = new SignoutCommandHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Given_ValidRefreshToken_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            var signoutCommand = _fixture.Create<SignoutCommand>();
            var cancellationToken = _fixture.Create<CancellationToken>();
            var signoutResult = Result<bool>.Success(true);

            _userRepositoryMock
                .Setup(repo => repo.SignoutAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(signoutResult);

            // Act
            var result = await _handler.HandleAsync(signoutCommand, cancellationToken);

            // Assert
            result
                .IsSuccess
                .Should()
                .BeTrue();

            _userRepositoryMock.Verify(repo => repo.SignoutAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            _userRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Given_InvalidRefreshToken_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            var signoutCommand = _fixture.Create<SignoutCommand>();
            var cancellationToken = _fixture.Create<CancellationToken>();
            var signoutResult = Result<bool>.Failure(UserErrors.InvalidRefreshToken);

            _userRepositoryMock
                .Setup(repo => repo.SignoutAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(signoutResult);

            // Act
            var result = await _handler.HandleAsync(signoutCommand, cancellationToken);

            // Assert
            result
                .Error
                .Should()
                .Be(UserErrors.InvalidRefreshToken);

            _userRepositoryMock.Verify(repo => repo.SignoutAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            _userRepositoryMock.VerifyNoOtherCalls();
        }
    }
}
