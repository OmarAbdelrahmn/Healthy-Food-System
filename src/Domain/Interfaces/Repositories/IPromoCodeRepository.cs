using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories;
public interface IPromoCodeRepository
{
    Task<PromoCode?> GetByCodeAsync(string code);
    Task<PromoCode?> GetByIdAsync(Guid id);
    Task<IEnumerable<PromoCode>> GetAllAsync();
    Task AddAsync(PromoCode promoCode);
    Task UpdateAsync(PromoCode promoCode);
    Task DeleteAsync(PromoCode promoCode);
    Task<bool> ExistsAsync(string code);
    Task SaveChangesAsync();
}
