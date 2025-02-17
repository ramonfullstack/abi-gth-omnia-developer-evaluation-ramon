﻿using Ambev.DeveloperEvaluation.Application.Eventing;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sale
{
    /// <summary>
    /// Contains unit tests for the <see cref="DeleteSaleHandler"/> class.
    /// </summary>
    public class DeleteSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly DeleteSaleHandler _handler;
        private readonly ILogger<DeleteSaleHandler> _logger;
        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteSaleHandlerTests"/> class.
        /// Sets up the test dependencies and creates fake data generators.
        /// </summary>
        public DeleteSaleHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _logger = Substitute.For<ILogger<DeleteSaleHandler>>();
            _eventPublisher = Substitute.For<IEventPublisher>();
            _handler = new DeleteSaleHandler(_saleRepository, _logger, _eventPublisher);
        }

        /// <summary>
        /// Tests for delete sale and return successfuly.
        /// </summary>
        [Fact(DisplayName = "Handle should delete sale and return successfuly")]
        public async Task Handle_Should_Delete_Sale_And_Return_Result_Success_With_Data()
        {
            // Arrange
            var command = new DeleteSaleCommand(Guid.NewGuid());

            _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
        }
    }
}
