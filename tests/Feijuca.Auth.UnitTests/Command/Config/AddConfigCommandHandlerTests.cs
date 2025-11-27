using AutoFixture;
using Feijuca.Auth.Application.Commands.Config;
using Feijuca.Auth.Common.Errors;
using Feijuca.Auth.Domain.Entities;
using Feijuca.Auth.Domain.Interfaces;
using Feijuca.Auth.Providers;

using FluentAssertions;
using Moq;

namespace Feijuca.Auth.Api.UnitTests.Command.Config
{
    public class AddConfigCommandHandlerTests
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<IConfigRepository> _configRepositoryMock = new();
        private readonly Mock<ITenantProvider> _tenantService = new();
        private readonly AddOrUpdateConfigCommandHandler _handler;

        public AddConfigCommandHandlerTests()
        {
            _handler = new AddOrUpdateConfigCommandHandler(_configRepositoryMock.Object, _tenantService.Object);
        }

        [Fact(Skip = "FIX LATER")]
        public async Task Given_ValidConfiguration_When_AddConfigAsync_Then_ShouldReturnSuccess()
        {
            // Arrange
            var addConfigCommand = _fixture.Create<AddOrUpdateConfigCommand>();
            var cancellationToken = _fixture.Create<CancellationToken>();

            _configRepositoryMock
                .Setup(repo => repo.AddConfigAsync(It.IsAny<KeycloakSettingsEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result =  await _handler.HandleAsync(addConfigCommand, cancellationToken);

            // Assert
            result
                .IsSuccess
                .Should()
                .BeTrue();

            _configRepositoryMock.Verify(repo => repo.AddConfigAsync(It.IsAny<KeycloakSettingsEntity>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(Skip = "FIX LATER")]
        public async Task Given_InvalidConfiguration_When_AddConfigAsync_Then_ShouldReturnFailure()
        {
            // Arrange
            var addConfigCommand = _fixture.Create<AddOrUpdateConfigCommand>();
            var cancellationToken = _fixture.Create<CancellationToken>();

            _configRepositoryMock
                .Setup(repo => repo.AddConfigAsync(It.IsAny<KeycloakSettingsEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.HandleAsync(addConfigCommand, cancellationToken);

            // Assert
            result
                .Error
                .Should()
                .Be(ConfigErrors.InsertConfig);

            _configRepositoryMock.Verify(repo => repo.AddConfigAsync(It.IsAny<KeycloakSettingsEntity>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
