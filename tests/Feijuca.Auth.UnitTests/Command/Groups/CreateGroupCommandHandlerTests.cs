using AutoFixture;
using Feijuca.Auth.Application.Commands.Group;
using Feijuca.Auth.Common.Errors;
using Mattioli.Configurations.Models;
using Feijuca.Auth.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Feijuca.Auth.Providers;

namespace Feijuca.Auth.Api.UnitTests.Command.Groups
{
    public class CreateGroupCommandHandlerTests
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
        private readonly Mock<ITenantProvider> _tenantProviderMock = new();
        private readonly AddGroupCommandHandler _handler;

        public CreateGroupCommandHandlerTests()
        {
            _handler = new AddGroupCommandHandler(_groupRepositoryMock.Object, _tenantProviderMock.Object);
        }

        [Fact]
        public async Task Given_ValidRequest_When_CreateGroupIsCalled_Then_ShouldReturnSuccess()
        {
            // Arrange
            var createGroupCommand = _fixture.Create<AddGroupCommand>();
            var cancellationToken = _fixture.Create<CancellationToken>();

            _groupRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string[]>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _handler.Handle(createGroupCommand, cancellationToken);

            // Assert
            result
                .IsSuccess
                .Should()
                .BeTrue();

            _groupRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string[]>>(), It.IsAny<CancellationToken>()), Times.Once);
            _groupRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Given_InvalidRequest_When_CreateGroupFails_Then_ShouldReturnFailure()
        {
            // Arrange
            var createGroupCommand = _fixture.Create<AddGroupCommand>();
            var cancellationToken = _fixture.Create<CancellationToken>();

            var failureResult = Result.Failure(GroupErrors.CreationGroupError);

            _groupRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string[]>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failureResult);

            // Act
            var result = await _handler.Handle(createGroupCommand, cancellationToken);

            // Assert
            result
                .Error
                .Should()
                .Be(GroupErrors.CreationGroupError);

            _groupRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string[]>>(), It.IsAny<CancellationToken>()), Times.Once);
            _groupRepositoryMock.VerifyNoOtherCalls();
        }
    }
}
