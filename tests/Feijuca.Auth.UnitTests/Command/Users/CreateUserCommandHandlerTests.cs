using AutoFixture;
using Feijuca.Auth.Application.Commands.User;
using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Entities;
using Feijuca.Auth.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Feijuca.Auth.Api.UnitTests.Command.Users
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly AddUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _handler = new AddUserCommandHandler(_userRepositoryMock.Object);
        }

        [Fact(Skip = "Necessary solve soon")]
        public async Task Should_ReturnFailureResult_WhenCreateUserFails()
        {
            // Arrange
            var createUserCommand = _fixture.Create<AddUserCommand>();
            var createUserResult = Result<bool>.Failure(UserErrors.UserCreationError);
            var cancellationToken = _fixture.Create<CancellationToken>();

            _userRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createUserResult);

            // Act
            var result = await _handler.Handle(createUserCommand, cancellationToken);

            // Assert
            result
                .Error
                .Should()
                .Be(UserErrors.UserCreationError);

            _userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact(Skip = "Necessary solve soon")]
        public async Task Should_ReturnFailureResult_WhenResetPasswordFails()
        {
            // Arrange
            var createUserCommand = _fixture.Create<AddUserCommand>();
            var createUserResult = Result<bool>.Success(true);

            var mockUser = _fixture.Build<User>()
                                   .With(u => u.Username, createUserCommand.AddUserRequest.Username)
                                   .With(u => u.Password, createUserCommand.AddUserRequest.Password)
                                   .Create();

            var getUserResult = Result<User>.Success(mockUser);
            var resetPasswordResult = Result<bool>.Failure(UserErrors.WrongPasswordDefinition);
            var cancellationToken = _fixture.Create<CancellationToken>();

            _userRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createUserResult);

            _userRepositoryMock
                .Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(getUserResult);

            _userRepositoryMock
                .Setup(repo => repo.ResetPasswordAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resetPasswordResult);

            // Act
            var result = await _handler.Handle(createUserCommand, cancellationToken);

            // Assert
            result
                .Error
                .Should()
                .Be(UserErrors.WrongPasswordDefinition);

            _userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(repo => repo.ResetPasswordAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact(Skip = "Necessary solve soon")]
        public async Task Should_CreateUser_Successfully_WhenResultIsSuccessful()
        {
            // Arrange
            var createUserCommand = _fixture.Create<AddUserCommand>();
            var createUserResult = Result<bool>.Success(true);

            var mockUser = _fixture.Build<User>()
                                   .With(u => u.Username, createUserCommand.AddUserRequest.Username)
                                   .With(u => u.Password, createUserCommand.AddUserRequest.Password)
                                   .Create();

            var getUserResult = Result<User>.Success(mockUser);
            var resetPasswordResult = Result<bool>.Success(true);
            var cancellationToken = _fixture.Create<CancellationToken>();

            _userRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createUserResult);

            _userRepositoryMock
                .Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(getUserResult);

            _userRepositoryMock
                .Setup(repo => repo.ResetPasswordAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resetPasswordResult);

            // Act
            var result = await _handler.Handle(createUserCommand, cancellationToken);

            // Assert
            result
                .IsSuccess
                .Should()
                .BeTrue();

            _userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(repo => repo.ResetPasswordAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.VerifyNoOtherCalls();
        }
    }
}
