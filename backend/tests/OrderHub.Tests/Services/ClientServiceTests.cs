using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using OrderHub.Application.DTOs.Clients;
using OrderHub.Application.Interfaces.Repositories;
using OrderHub.Application.Services;
using OrderHub.Domain.Entities;
using OrderHub.Shared.Pagination;
using OrderHub.Shared.Results;

namespace OrderHub.Tests.Services
{
    public sealed class ClientServiceTests
    {
        private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();
        private readonly ClientService _sut;

        public ClientServiceTests()
        {
            this._sut = new(
                this._clientRepository,
                NullLogger<ClientService>.Instance
            );
        }

        // ─── CreateAsync ────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_WhenRepositorySucceeds_ReturnsClientDto()
        {
            // Arrange
            var dto = new CreateClientDto { Name = "Acme Corp" };
            _clientRepository.AddAsync(Arg.Any<Client>()).Returns(Result.Success());

            // Act
            var result = await _sut.CreateAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Acme Corp", result.Value.Name);
        }

        [Fact]
        public async Task CreateAsync_WhenRepositoryFails_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateClientDto { Name = "Acme Corp" };
            var error = Error.AlreadyExists(nameof(Client), nameof(Client.Name), dto.Name);
            _clientRepository.AddAsync(Arg.Any<Client>()).Returns(Result.Failure(error));

            // Act
            var result = await _sut.CreateAsync(dto);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(error.Code, result.Error.Code);
        }

        // ─── GetPagedAsync ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetPagedAsync_ReturnsMappedPagedResult()
        {
            // Arrange
            var pageRequest = new PageRequest { Page = 2, PageSize = 5 };

            var clients = new List<Client>
            {
                new() { Id = 1, Name = "Client A" },
                new() { Id = 2, Name = "Client B" }
            };

            var pagedClients = new PagedResult<Client>
            {
                Items = clients,
                Page = 2,
                PageSize = 5,
                TotalCount = 7
            };

            _clientRepository.GetPagedAsync(pageRequest).Returns(pagedClients);

            // Act
            var result = await _sut.GetPagedAsync(pageRequest);

            // Assert
            Assert.Equal(7, result.TotalCount);
            Assert.Equal(2, result.Page);
            Assert.Equal(5, result.PageSize);
            Assert.Equal(2, result.Items.Count());

            var items = result.Items.ToList();
            Assert.Equal(1, items[0].Id);
            Assert.Equal("Client A", items[0].Name);
            Assert.Equal(2, items[1].Id);
            Assert.Equal("Client B", items[1].Name);
        }

        [Fact]
        public async Task GetPagedAsync_WhenNoClients_ReturnsEmptyPage()
        {
            // Arrange
            var pageRequest = new PageRequest { Page = 1, PageSize = 20 };

            var emptyPage = new PagedResult<Client>
            {
                Items = [],
                Page = 1,
                PageSize = 20,
                TotalCount = 0
            };

            _clientRepository.GetPagedAsync(pageRequest).Returns(emptyPage);

            // Act
            var result = await _sut.GetPagedAsync(pageRequest);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
            Assert.False(result.HasNextPage);
            Assert.False(result.HasPreviousPage);
        }
    }
}
