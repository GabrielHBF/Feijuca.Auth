using AutoFixture;
using Feijuca.Auth.Application.Commands.User;
using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Entities;
using Feijuca.Auth.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Feijuca.Auth.Api.UnitTests.Command.Auth
{
    public class RefreshTokenCommandHandlerTests
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly RefreshTokenCommandHandler _handler;

        public RefreshTokenCommandHandlerTests()
        {
            _handler = new RefreshTokenCommandHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Given_ValidRefreshToken_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var refreshTokenCommand = _fixture.Create<RefreshTokenCommand>();
            var cancellationToken = _fixture.Create<CancellationToken>();
            var tokenDetails = _fixture.Create<TokenDetails>();
            var refreshTokenResult = Result<TokenDetails>.Success(tokenDetails);

            _userRepositoryMock
                .Setup(repo => repo.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshTokenResult);

            // Act
            var result = await _handler.HandleAsync(refreshTokenCommand, cancellationToken);

            // Assert
            result
                .IsSuccess
                .Should()
                .BeTrue();

            _userRepositoryMock.Verify(repo => repo.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            _userRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Given_InvalidRefreshToken_When_HandleIsCalled_Then_ShouldReturnFailure()
        {
            // Arrange
            var refreshTokenCommand = _fixture.Create<RefreshTokenCommand>();
            var cancellationToken = _fixture.Create<CancellationToken>();
            var refreshTokenResult = Result<TokenDetails>.Failure(UserErrors.InvalidRefreshToken);

            _userRepositoryMock
                .Setup(repo => repo.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshTokenResult);

            // Act
            var result = await _handler.HandleAsync(refreshTokenCommand, cancellationToken);

            // Assert
            result
                .Error
                .Should()
                .Be(UserErrors.InvalidRefreshToken);

            _userRepositoryMock.Verify(repo => repo.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            _userRepositoryMock.VerifyNoOtherCalls();
        }
    }
}
