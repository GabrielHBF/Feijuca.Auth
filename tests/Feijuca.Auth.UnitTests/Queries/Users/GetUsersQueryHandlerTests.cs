using AutoFixture;
using Feijuca.Auth.Application.Queries.Users;
using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Entities;
using Feijuca.Auth.Domain.Filters;
using Feijuca.Auth.Domain.Interfaces;
using Feijuca.Auth.Providers;
using FluentAssertions;
using Moq;

namespace Feijuca.Auth.Api.UnitTests.Queries.Users
{
    public class GetUsersQueryHandlerTests
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<ITenantProvider> _tenantServiceMock = new();
        private readonly GetUsersQueryHandler _handler;

        public GetUsersQueryHandlerTests()
        {
            _handler = new GetUsersQueryHandler(_userRepositoryMock.Object, _tenantServiceMock.Object);
        }

        [Fact(Skip = "Necessary solve soon")]
        public async Task Given_UsersExist_WhenGetAllUsersCalled_Then_ReturnsPagedUsersSuccessfully()
        {
            var usersQuery = _fixture.Create<GetUsersQuery>();
            var cancellationToken = _fixture.Create<CancellationToken>();
            var users = _fixture.CreateMany<User>();
            var userResult = Result<IEnumerable<User>>.Success(users);

            _userRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<UserFilters>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userResult);

            _userRepositoryMock
                .Setup(repo => repo.GetTotalAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users.Count());

            // Act
            var result = await _handler.HandleAsync(usersQuery, cancellationToken);

            // Assert
            result
                .IsSuccess
                .Should()
                .BeTrue();

            _userRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<UserFilters>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once());
            _userRepositoryMock.Verify(repo => repo.GetTotalAsync(It.IsAny<CancellationToken>()), Times.Once());
            _userRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact(Skip = "FIX LATER")]
        public async Task Given_UsersNotExist_WhenGetAllUsersCalled_Then_ReturnsFailure()
        {
            var usersQuery = _fixture.Create<GetUsersQuery>();
            var cancellationToken = _fixture.Create<CancellationToken>();
            var userResult = Result<IEnumerable<User>>.Failure(UserErrors.GetAllUsersError);

            _userRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<UserFilters>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userResult);
            
            _userRepositoryMock
                .Setup(repo => repo.GetTotalAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_fixture.Create<int>());

            // Act
            var result = await _handler.HandleAsync(usersQuery, cancellationToken);

            // Assert
            result
                .Error
                .Should()
                .Be(UserErrors.GetAllUsersError);

            _userRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<UserFilters>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once());
            _userRepositoryMock.Verify(repo => repo.GetTotalAsync(It.IsAny<CancellationToken>()), Times.Once());
            _userRepositoryMock.VerifyNoOtherCalls();
        }
    }
}
