﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Eventing;
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
    /// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
    /// </summary>
    public class CreateSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly CreateSaleHandler _handler;
        private readonly ILogger<CreateSaleHandler> _logger;
        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSaleHandlerTests"/> class.
        /// Sets up the test dependencies and creates fake data generators.
        /// </summary>
        public CreateSaleHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<CreateSaleHandler>>();
            _eventPublisher = Substitute.For<IEventPublisher>();
            _handler = new CreateSaleHandler(_saleRepository, _mapper, _logger, _eventPublisher);
        }

        /// <summary>
        /// Tests for create sale and return successfuly with id.
        /// </summary>
        [Fact(DisplayName = "Handle should create sale and return successfuly with id")]
        public async Task Handle_Should_Create_Sale_And_Return_Result_Success_With_Data()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                Customer = "Customer1",
                Branch = "Branch1",
                Products = new List<CreateProductSaleDto>
            {
                new CreateProductSaleDto { Name = "Product1", Quantity = 2, UnitPrice = 10.0M }
            }
            };

            var saleEntity = new SaleEntity
            {
                Customer = command.Customer,
                Branch = command.Branch,
                Products = new List<ProductSale>
            {
                new ProductSale { Name = "Product1", Quantity = 2, UnitPrice = 10.0M }
            }
            };

            _saleRepository.CreateAsync(Arg.Any<SaleEntity>(), Arg.Any<CancellationToken>())
                .Returns(saleEntity);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(saleEntity.Id, result.Id);
        }
    }
}
