using Microsoft.EntityFrameworkCore;
using MiniMartPOS.Data;
using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Where(c => c.Status)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customer?> GetCustomerByPhoneAsync(string phone)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == phone && c.Status);
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            customer.CreatedDate = DateTime.Now;
            customer.Status = true;
            customer.LoyaltyPoints = 0;
            customer.OutstandingBalance = 0;
            
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            
            return customer;
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            var existing = await _context.Customers.FindAsync(customer.Id);
            if (existing == null)
            {
                throw new InvalidOperationException("Customer not found");
            }

            existing.CustomerName = customer.CustomerName;
            existing.Phone = customer.Phone;
            existing.Email = customer.Email;
            existing.Address = customer.Address;
            existing.Status = customer.Status;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                customer.Status = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddLoyaltyPointsAsync(int customerId, int points)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer != null)
            {
                customer.LoyaltyPoints += points;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RedeemLoyaltyPointsAsync(int customerId, int points)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer != null && customer.LoyaltyPoints >= points)
            {
                customer.LoyaltyPoints -= points;
                await _context.SaveChangesAsync();
            }
        }
    }
}
