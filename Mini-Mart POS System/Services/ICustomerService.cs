using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer?> GetCustomerByPhoneAsync(string phone);
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<Customer> UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
        Task AddLoyaltyPointsAsync(int customerId, int points);
        Task RedeemLoyaltyPointsAsync(int customerId, int points);
    }
}
