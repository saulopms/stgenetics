using System.Collections.Concurrent;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;

namespace GoodHamburger.Infrastructure.Repositories;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _store = new();

    public Task<IEnumerable<Order>> GetAllAsync() =>
        Task.FromResult<IEnumerable<Order>>(_store.Values.OrderBy(o => o.CreatedAt).ToList());

    public Task<Order?> GetByIdAsync(Guid id) =>
        Task.FromResult(_store.TryGetValue(id, out var order) ? order : null);

    public Task AddAsync(Order order)
    {
        _store[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order)
    {
        _store[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
