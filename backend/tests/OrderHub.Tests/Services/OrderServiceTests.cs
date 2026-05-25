using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using OrderHub.Application.DTOs.Orders;
using OrderHub.Application.Filters;
using OrderHub.Application.Interfaces.Repositories;
using OrderHub.Application.Interfaces.Services;
using OrderHub.Application.Services;
using OrderHub.Domain.Entities;
using OrderHub.Domain.Enums;
using OrderHub.Shared.Pagination;
using OrderHub.Shared.Results;

namespace OrderHub.Tests.Services
{
    public sealed class OrderServiceTests
    {
        private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
        private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();
        private readonly IOrderNotificationService _notificationService = Substitute.For<IOrderNotificationService>();
        private readonly OrderService _sut;

        public OrderServiceTests()
        {
            this._sut = new(
                this._orderRepository,
                this._clientRepository,
                this._notificationService,
                NullLogger<OrderService>.Instance
            );
        }

        // ─── GetByIdAsync ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_WhenOrderExists_ReturnsSuccessWithMappedDto()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = OrderServiceTests.BuildOrder(orderId, clientName: "Acme");
            this._orderRepository.GetByIdAsync(orderId).Returns(Result.Success(order));

            // Act
            var result = await this._sut.GetByIdAsync(orderId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(orderId, result.Value.Id);
            Assert.Equal(order.OrderNumber, result.Value.OrderNumber);
            Assert.Equal(order.Description, result.Value.Description);
            Assert.Equal(order.Status, result.Value.Status);
            Assert.Equal("Acme", result.Value.ClientName);
        }

        [Fact]
        public async Task GetByIdAsync_WhenOrderNotFound_ReturnsFailure()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var notFound = Error.NotFound(nameof(Order), orderId);
            this._orderRepository.GetByIdAsync(orderId).Returns(Result.Failure<Order>(notFound));

            // Act
            var result = await this._sut.GetByIdAsync(orderId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(notFound.Code, result.Error.Code);
        }

        // ─── GetPagedAsync ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetPagedAsync_ReturnsMappedPagedResult()
        {
            // Arrange
            var pageRequest = new PageRequest { Page = 1, PageSize = 10 };
            var filter = new OrderFilter();

            var orders = new List<Order>
            {
                OrderServiceTests.BuildOrder(Guid.NewGuid(), orderNumber: "ORD-001"),
                OrderServiceTests.BuildOrder(Guid.NewGuid(), orderNumber: "ORD-002")
            };

            var pagedOrders = new PagedResult<Order>
            {
                Items = orders,
                Page = 1,
                PageSize = 10,
                TotalCount = 2
            };

            this._orderRepository.GetPagedAsync(pageRequest, filter).Returns(pagedOrders);

            // Act
            var result = await this._sut.GetPagedAsync(pageRequest, filter);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.Items.Count());
            Assert.Contains(result.Items, dto => dto.OrderNumber == "ORD-001");
            Assert.Contains(result.Items, dto => dto.OrderNumber == "ORD-002");
        }

        // ─── CreateAsync ────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_WhenClientDoesNotExist_ReturnsNotFoundFailure()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                ClientId = 42,
                OrderNumber = "ORD-999",
                Description = "Some order"
            };

            this._clientRepository.ExistsAsync(dto.ClientId).Returns(false);

            // Act
            var result = await this._sut.CreateAsync(dto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"{nameof(Client)}.NotFound", result.Error.Code);
            await this._orderRepository.DidNotReceive().AddAsync(Arg.Any<Order>());
        }

        [Fact]
        public async Task CreateAsync_WhenRepositoryFails_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                ClientId = 1,
                OrderNumber = "ORD-001",
                Description = "Test order"
            };

            var repoError = Error.InternalServerError("DB write failed");
            this._clientRepository.ExistsAsync(dto.ClientId).Returns(true);
            this._orderRepository.AddAsync(Arg.Any<Order>()).Returns(Result.Failure(repoError));

            // Act
            var result = await this._sut.CreateAsync(dto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(repoError.Code, result.Error.Code);
        }

        [Fact]
        public async Task CreateAsync_WhenSuccessful_ReturnsOrderDtoWithCreatedStatus()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                ClientId = 1,
                OrderNumber = "ORD-001",
                Description = "Test order"
            };

            this._clientRepository.ExistsAsync(dto.ClientId).Returns(true);
            this._orderRepository.AddAsync(Arg.Any<Order>()).Returns(Result.Success());

            // Act
            var result = await this._sut.CreateAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dto.ClientId, result.Value.ClientId);
            Assert.Equal(dto.OrderNumber, result.Value.OrderNumber);
            Assert.Equal(dto.Description, result.Value.Description);
            Assert.Equal(OrderStatus.Created, result.Value.Status);
            Assert.NotEqual(Guid.Empty, result.Value.Id);
        }

        // ─── UpdateStatusAsync ──────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateStatusAsync_WhenRepositoryFails_ReturnsFailureAndDoesNotNotify()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var dto = new UpdateOrderStatusDto { Status = OrderStatus.Shipped };
            var repoError = Error.NotFound(nameof(Order), orderId);

            this._orderRepository.UpdateStatusAsync(orderId, dto.Status)
                .Returns(Result.Failure<UpdateOrderStatusResult>(repoError));

            // Act
            var result = await this._sut.UpdateStatusAsync(orderId, dto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(repoError.Code, result.Error.Code);
            await _notificationService.DidNotReceive().NotifyStatusChangedAsync(
                Arg.Any<Guid>(), Arg.Any<OrderStatus>(), Arg.Any<DateTime>()
            );
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenSuccessful_ReturnsResultAndSendsNotification()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var dto = new UpdateOrderStatusDto { Status = OrderStatus.Delivered };
            var updatedAt = DateTime.UtcNow;

            var statusResult = new UpdateOrderStatusResult
            {
                Id = orderId,
                Status = OrderStatus.Delivered,
                UpdatedAt = updatedAt
            };

            this._orderRepository.UpdateStatusAsync(orderId, dto.Status)
                .Returns(Result.Success(statusResult));

            // Act
            var result = await this._sut.UpdateStatusAsync(orderId, dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(orderId, result.Value.Id);
            Assert.Equal(OrderStatus.Delivered, result.Value.Status);
            Assert.Equal(updatedAt, result.Value.UpdatedAt);

            await this._notificationService.Received(1).NotifyStatusChangedAsync(
                orderId, OrderStatus.Delivered, updatedAt, Arg.Any<CancellationToken>()
            );
        }

        // ─── helpers ────────────────────────────────────────────────────────────────

        private static Order BuildOrder(
            Guid id,
            string orderNumber = "ORD-001",
            string description = "Test",
            OrderStatus status = OrderStatus.Created,
            long clientId = 1,
            string? clientName = null
        ) => new()
        {
            Id = id,
            ClientId = clientId,
            OrderNumber = orderNumber,
            Description = description,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Client = clientName is not null ? new Client { Id = clientId, Name = clientName } : null
        };
    }
}
