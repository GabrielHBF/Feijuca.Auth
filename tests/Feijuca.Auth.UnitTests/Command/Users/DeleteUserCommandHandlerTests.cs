using AutoFixture;
using Feijuca.Auth.Application.Commands.User;
using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Feijuca.Auth.Api.UnitTests.Command.Users
{
    public class DeleteUserCommandHandlerTests
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly DeleteUserCommandHandler _handler;

        public DeleteUserCommandHandlerTests()
        {
            _handler = new DeleteUserCommandHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenPassIdValid()
        {
            // Assert
            var deleteRequest = _fixture.Create<DeleteUserCommand>();
            var cancellationToken = _fixture.Create<CancellationToken>();

            _userRepositoryMock
                .Setup(repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(true));

            // Act
            var result = await _handler.HandleAsync(deleteRequest, cancellationToken);

            // Assert
            result
                .IsSuccess
                .Should()
                .BeTrue();

            _userRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once());
            _userRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenDeleteAsyncReturnError()
        {
            // Assert
            var deleteRequest = _fixture.Create<DeleteUserCommand>();
            var cancellationToken = _fixture.Create<CancellationToken>();
            var errorResult = Result<bool>.Failure(UserErrors.DeletionUserError);

            _userRepositoryMock
                .Setup(repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(errorResult);

            // Act
            var result = await _handler.HandleAsync(deleteRequest, cancellationToken);

            // Assert
            result
                .Error
                .Should()
                .Be(UserErrors.DeletionUserError);

            _userRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once());
            _userRepositoryMock.VerifyNoOtherCalls();
        }
    }
}
