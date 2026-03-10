using AutoFixture;
using Feijuca.Auth.Application.Commands.User;
using Feijuca.Auth.Common.Errors;
using Feijuca.Auth.Domain.Interfaces;
using Mattioli.Configurations.Models;
using Moq;

namespace Feijuca.Auth.Api.UnitTests.Command.Users
{
    public class ChangePasswordHandlerTest
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly ChangePasswordCommandHandler _handler;

        public ChangePasswordHandlerTest()
        {
            _handler = new ChangePasswordCommandHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Should_ReturnSuccessResult_WhenChangePasswordSucceeds()
        {
            // Arrange
            var changePasswordCommand = _fixture.Create<ChangePasswordCommand>();
            var cancellationToken = _fixture.Create<CancellationToken>();

            _userRepositoryMock
                .Setup(repo => repo.ResetPasswordAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(true));

            // Act
            var result = await _handler.HandleAsync(changePasswordCommand, cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            _userRepositoryMock.Verify(repo => repo.ResetPasswordAsync(
                changePasswordCommand.ChangePasswordRequest.Id,
                changePasswordCommand.ChangePasswordRequest.NewPassword,
                cancellationToken), Times.Once);

            _userRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Should_ReturnFailureResult_WhenChangePasswordFails()
        {
            // Arrange
            var changePasswordCommand = _fixture.Create<ChangePasswordCommand>();
            var cancellationToken = _fixture.Create<CancellationToken>();

            _userRepositoryMock
                .Setup(repo => repo.ResetPasswordAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Failure(UserErrors.ChangePasswordError));

            // Act
            var result = await _handler.HandleAsync(changePasswordCommand, cancellationToken);

            // Assert
            Assert.Equal(UserErrors.ChangePasswordError.Code, result.Error.Code);

            Assert.False(result.IsSuccess);
            _userRepositoryMock.Verify(repo => repo.ResetPasswordAsync(
                changePasswordCommand.ChangePasswordRequest.Id,
                changePasswordCommand.ChangePasswordRequest.NewPassword,
                cancellationToken), Times.Once);

            _userRepositoryMock.VerifyNoOtherCalls();
        }

    }
}