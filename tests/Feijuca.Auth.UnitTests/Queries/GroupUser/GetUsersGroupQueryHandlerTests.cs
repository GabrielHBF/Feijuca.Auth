using AutoFixture;
using Feijuca.Auth.Application.Queries.GroupUser;
using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Entities;
using Feijuca.Auth.Domain.Filters;
using Feijuca.Auth.Domain.Interfaces;
using Feijuca.Auth.Providers;

using FluentAssertions;
using Moq;

namespace Feijuca.Auth.Api.UnitTests.Queries.GroupUser
{
    public class GetUsersGroupQueryHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<ITenantProvider> _tenantServiceMock = new();
        private readonly GetUsersGroupQueryHandler _handler;

        public GetUsersGroupQueryHandlerTests()
        {
            _handler = new GetUsersGroupQueryHandler(_groupRepositoryMock.Object, _userRepositoryMock.Object, _tenantServiceMock.Object);
        }

        [Fact(Skip = "FIX LATER")]
        public async Task Given_ValidGroupId_WhenGroupDoesNotExist_Then_ReturnFailureResult()
        {
            // Arrange
            var userGroupQuery = _fixture.Create<GetUsersGroupQuery>();
            var cancellationToken = _fixture.Create<CancellationToken>();
            var groupsResult = Result<IEnumerable<Group>>.Success([]);

            _groupRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(groupsResult);

            // Act
            var result = await _handler.HandleAsync(userGroupQuery, cancellationToken);

            // Assert
            result
                .Error
                .Should()
                .Be(GroupErrors.GetUsersInGroupsError);

            _groupRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            _groupRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact(Skip = "FIX LATER")]
        public async Task Given_ValidGroupId_WhenRepositoryReturnsFailure_Then_ReturnFailureResult()
        {
            // Arrange
            var userGroupQuery = _fixture.Create<GetUsersGroupQuery>();
            var cancellationToken = _fixture.Create<CancellationToken>();
            var groupsResult = Result<IEnumerable<Group>>.Failure(GroupErrors.GetUsersInGroupsError);

            _groupRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(groupsResult);

            // Act
            var result = await _handler.HandleAsync(userGroupQuery, cancellationToken);

            // Assert
            result
                .Error
                .Should()
                .Be(GroupErrors.GetUsersInGroupsError);

            _groupRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            _groupRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact(Skip = "Necessary solve soon")]
        public async Task Given_ValidGroupId_WhenGroupAndUsersExist_Then_ReturnSuccessResult()
        {
            // Arrange
            var userGroupQuery = _fixture.Create<GetUsersGroupQuery>();
            var cancellationToken = _fixture.Create<CancellationToken>();
            var group = _fixture.Build<Group>()
                                .With(g => g.Id, userGroupQuery.GetUsersGroupRequest.GroupId)
                                .Create();

            var groupsResult = Result<IEnumerable<Group>>.Success([group]);
            var usersResult = Result<IEnumerable<User>>.Success(_fixture.CreateMany<User>(5));

            _groupRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(groupsResult);

            _groupRepositoryMock
                .Setup(repo => repo.GetUsersInGroupAsync(group.Id, It.IsAny<UserFilters>(), It.IsAny<int>(), cancellationToken))
                .ReturnsAsync(usersResult);

            _userRepositoryMock
                .Setup(repo => repo.GetTotalAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(100);

            // Act
            var result = await _handler.HandleAsync(userGroupQuery, cancellationToken);

            // Assert
            result
                .IsSuccess
                .Should()
                .BeTrue();

            result
                .Data
                .Should()
                .NotBeNull();

            _groupRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            _groupRepositoryMock.Verify(repo => repo.GetUsersInGroupAsync(group.Id, It.IsAny<UserFilters>(), It.IsAny<int>(), cancellationToken), Times.Once());
            _groupRepositoryMock.VerifyNoOtherCalls();

            _userRepositoryMock.Verify(repo => repo.GetTotalAsync(It.IsAny<CancellationToken>()), Times.Once());
            _userRepositoryMock.VerifyNoOtherCalls();
        }
    }
}
