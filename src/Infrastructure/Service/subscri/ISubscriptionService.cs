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
}
