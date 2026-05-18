using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Models;
using OrderManagementAPI.Data;
using OrderManagementAPI.Repositories.Interfaces;

namespace OrderManagementAPI.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetActiveOrdersSortedByDateAsync()
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(order => order.IsActive)
                .OrderBy(order => order.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(order => order.Id == id);
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        // public void Update(Order order)
        // {
        //     _context.Orders.Update(order);
        // }

        // public void Delete(Order order)
        // {
        //     _context.Orders.Remove(order);
        // }

        // public async Task<bool> SaveChangesAsync()
        // {
        //     return await _context.SaveChangesAsync() > 0;
        // }
    }
}
