using AutoFixture;
using Feijuca.Auth.Application.Commands.GroupUser;
using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Feijuca.Auth.Api.UnitTests.Command.GroupUser
{
    public class RemoveUserFromGroupCommandHandlerTests
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<IGroupUsersRepository> _groupUsersRepositoryMock = new();
        private readonly RemoveUserFromGroupCommandHandler _handler;

        public RemoveUserFromGroupCommandHandlerTests()
        {
            _handler = new RemoveUserFromGroupCommandHandler(_groupUsersRepositoryMock.Object);
        }

        [Fact]
        public async Task Given_ValidUserAndGroup_When_RemoveUserFromGroupIsCalled_Then_ShouldAddUserSuccessfully()
        {
            // Arrange
            var deleteUserGroupCommand = _fixture.Create<RemoveUserFromGroupCommand>();
            var cacellationToken = _fixture.Create<CancellationToken>();
            var deleteUserGroupResult = Result<bool>.Success(true);

            _groupUsersRepositoryMock
                .Setup(repo => repo.RemoveUserFromGroupAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(deleteUserGroupResult);

            // Act
            var result = await _handler.HandleAsync(deleteUserGroupCommand, cacellationToken);

            // Assert
            result
                .IsSuccess
                .Should()
                .BeTrue();

            _groupUsersRepositoryMock.Verify(repo => repo.RemoveUserFromGroupAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once());
            _groupUsersRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Given_InvalidUserAndGroup_When_RemoveUserFromGroupIsCalled_Then_ShouldRetornFails()
        {
            // Arrange
            var deleteUserGroupCommand = _fixture.Create<RemoveUserFromGroupCommand>();
            var cacellationToken = _fixture.Create<CancellationToken>();
            var deleteUserGroupResult = Result<bool>.Failure(UserGroupErrors.ErrorAddUserToGroup);

            _groupUsersRepositoryMock
                .Setup(repo => repo.RemoveUserFromGroupAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(deleteUserGroupResult);

            // Act
            var result = await _handler.HandleAsync(deleteUserGroupCommand, cacellationToken);

            // Assert
            result
                .Error
                .Should()
                .Be(UserGroupErrors.ErrorAddUserToGroup);

            _groupUsersRepositoryMock.Verify(repo => repo.RemoveUserFromGroupAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once());
            _groupUsersRepositoryMock.VerifyNoOtherCalls();
        }
    }
}
