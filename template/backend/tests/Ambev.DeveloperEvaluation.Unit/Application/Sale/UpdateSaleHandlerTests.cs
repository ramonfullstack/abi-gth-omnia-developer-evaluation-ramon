using Ambev.DeveloperEvaluation.Application.Eventing;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Dto;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sale
{
    /// <summary>
    /// Contains unit tests for the <see cref="UpdateSaleHandler"/> class.
    /// </summary>
    public class UpdateSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly UpdateSaleHandler _handler;
        private readonly ILogger<UpdateSaleHandler> _logger;
        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSaleHandlerTests"/> class.
        /// Sets up the test dependencies and creates fake data generators.
        /// </summary>
        public UpdateSaleHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
            _eventPublisher = Substitute.For<IEventPublisher>();
            _handler = new UpdateSaleHandler(_saleRepository, _mapper, _logger, _eventPublisher);
        }

        /// <summary>
        /// Tests for update sale and return successfuly with id.
        /// </summary>
        [Fact(DisplayName = "Handle should update sale and return successfuly with id")]
        public async Task Handle_Should_Update_Sale_And_Return_Result_Success_With_Data()
        {
            // Arrange
            var command = new UpdateSaleCommand
            {
                Id = Guid.NewGuid(),
                Customer = "UpdatedCustomer",
                Branch = "UpdatedBranch",
                Products = new List<UpdateProductSaleDto>
            {
                new UpdateProductSaleDto { Name = "UpdatedProduct", Quantity = 5, UnitPrice = 15.0M }
            },
                IsCanceled = true
            };

            var saleEntity = new SaleEntity
            {
                Id = command.Id,
                Customer = "OriginalCustomer",
                Branch = "OriginalBranch"
            };

            _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
              .Returns(saleEntity);

            _saleRepository.UpdateAsync(Arg.Any<SaleEntity>(), Arg.Any<CancellationToken>())
             .Returns(saleEntity);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(command.Id, result.Id);
        }


        [Fact(DisplayName = "Handle should throw exception when sale not found")]
        public async Task Handle_Should_Throw_Exception_When_Sale_Not_Found()
        {
            var command = new UpdateSaleCommand { Id = Guid.NewGuid() };
            _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((SaleEntity)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

        [Fact(DisplayName = "Handle should apply 10% discount for quantities between 4 and 9")]
        public async Task Handle_Should_Apply_10_Percent_Discount()
        {
            var command = new UpdateSaleCommand
            {
                Id = Guid.NewGuid(),
                Customer = "UpdatedCustomer",
                Branch = "UpdatedBranch",
                Products = new List<UpdateProductSaleDto>
                {
                    new UpdateProductSaleDto { Name = "UpdatedProduct", Quantity = 5, UnitPrice = 10.0M }
                },
                IsCanceled = false
            };

            var saleEntity = new SaleEntity { Id = command.Id };
            _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(saleEntity);
            _saleRepository.UpdateAsync(Arg.Any<SaleEntity>(), Arg.Any<CancellationToken>()).Returns(saleEntity);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(9.0M, command.Products.First().UnitPrice);
        }

        [Fact(DisplayName = "Handle should apply 20% discount for quantities between 10 and 20")]
        public async Task Handle_Should_Apply_20_Percent_Discount()
        {
            var command = new UpdateSaleCommand
            {
                Id = Guid.NewGuid(),
                Customer = "UpdatedCustomer",
                Branch = "UpdatedBranch",
                Products = new List<UpdateProductSaleDto>
                {
                    new UpdateProductSaleDto { Name = "UpdatedProduct", Quantity = 10, UnitPrice = 10.0M }
                },
                IsCanceled = false
            };

            var saleEntity = new SaleEntity { Id = command.Id };
            _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(saleEntity);
            _saleRepository.UpdateAsync(Arg.Any<SaleEntity>(), Arg.Any<CancellationToken>()).Returns(saleEntity);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(8.0M, command.Products.First().UnitPrice);
        }

        [Fact(DisplayName = "Handle should throw exception when quantity exceeds 20")]
        public async Task Handle_Should_Throw_Exception_When_Quantity_Exceeds_20()
        {
            var command = new UpdateSaleCommand
            {
                Id = Guid.NewGuid(),
                Customer = "UpdatedCustomer",
                Branch = "UpdatedBranch",
                Products = new List<UpdateProductSaleDto>
                {
                    new UpdateProductSaleDto { Name = "UpdatedProduct", Quantity = 21, UnitPrice = 10.0M }
                },
                IsCanceled = false
            };

            var saleEntity = new SaleEntity { Id = command.Id };
            _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(saleEntity);
            _saleRepository.UpdateAsync(Arg.Any<SaleEntity>(), Arg.Any<CancellationToken>()).Returns(saleEntity);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

    }
}
