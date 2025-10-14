using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.subscri;
public interface ISubscriptionService
{
    Task<SubscriptionStatisticsDto> GetSubscriptionStatisticsAsync();
    Task<PagedResult<CustomerDetailsDtos>> GetCustomersAsync(CustomerFilterDto filter);
    Task<CustomerDetailsDtos> GetCustomerByIdAsync(Guid id);
    Task<CustomerDetailsDtos> AddCustomerAsync(CreateCustomerDto dto);
    Task<bool> UpdateCustomerAsync(Guid id, CreateCustomerDto dto);
    Task<bool> DeleteCustomerAsync(Guid id);
    Task<bool> PauseSubscriptionAsync(Guid id);
    Task<bool> ResumeSubscriptionAsync(Guid id);
    Task<List<PromoCodeDto>> GetActivePromoCodesAsync();
    Task<byte[]> ExportCustomersAsync(CustomerFilterDto filter);
    Task<bool> FreezeSubscriptionAsync(string userId, string reason = null, DateTime? freezeUntil = null);
    Task<bool> UnfreezeSubscriptionAsync(string userId);
    Task<bool> IsFrozenAsync(string userId);
    Task<UserSubscriptionStatusDto> GetFreezeStatusAsync(string userId);
    Task<List<UserSubscriptionStatusDto>> GetAllFrozenUsersAsync();
    Task<int> GetFrozenSubscriptionsCountAsync();
}
