using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using OrderHub.Application.DTOs.Payments;
using OrderHub.Application.Interfaces.Repositories;
using OrderHub.Application.Services;
using OrderHub.Shared.Results;

namespace OrderHub.Tests.Services
{
    public sealed class ClientPaymentServiceTests
    {
        private readonly IClientPaymentRepository _paymentRepository = Substitute.For<IClientPaymentRepository>();
        private readonly ClientPaymentService _sut;

        public ClientPaymentServiceTests()
        {
            this._sut = new(
                this._paymentRepository,
                NullLogger<ClientPaymentService>.Instance
            );
        }

        // ─── GetDailyPaymentsAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task GetDailyPaymentsAsync_WhenRepositorySucceeds_ReturnsPayments()
        {
            // Arrange
            const long clientId = 1;
            var from = new DateOnly(2025, 1, 1);
            var to = new DateOnly(2025, 1, 31);

            var payments = new List<DailyPaymentDto>
            {
                new() { PaymentDate = new(2025, 1, 10), Amount = 1500.00m },
                new() { PaymentDate = new(2025, 1, 20), Amount = 2300.50m }
            };

            this._paymentRepository
                .GetDailyPaymentsAsync(clientId, from, to)
                .Returns(Result.Success<IEnumerable<DailyPaymentDto>>(payments));

            // Act
            var result = await this._sut.GetDailyPaymentsAsync(clientId, from, to);

            // Assert
            Assert.True(result.IsSuccess);
            var items = result.Value.ToList();
            Assert.Equal(2, items.Count);
            Assert.Equal(1500.00m, items[0].Amount);
            Assert.Equal(2300.50m, items[1].Amount);
        }

        [Fact]
        public async Task GetDailyPaymentsAsync_WhenRepositoryFails_ReturnsFailure()
        {
            // Arrange
            const long clientId = 99;
            var from = new DateOnly(2025, 1, 1);
            var to = new DateOnly(2025, 1, 31);

            var error = Error.InternalServerError("Query timeout");
            this._paymentRepository
                .GetDailyPaymentsAsync(clientId, from, to)
                .Returns(Result.Failure<IEnumerable<DailyPaymentDto>>(error));

            // Act
            var result = await this._sut.GetDailyPaymentsAsync(clientId, from, to);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(error.Code, result.Error.Code);
        }

        [Fact]
        public async Task GetDailyPaymentsAsync_WhenNoPaymentsExist_ReturnsEmptyCollection()
        {
            // Arrange
            const long clientId = 2;
            var from = new DateOnly(2025, 6, 1);
            var to = new DateOnly(2025, 6, 30);

            this._paymentRepository
                .GetDailyPaymentsAsync(clientId, from, to)
                .Returns(Result.Success<IEnumerable<DailyPaymentDto>>([]));

            // Act
            var result = await this._sut.GetDailyPaymentsAsync(clientId, from, to);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }
    }
}
