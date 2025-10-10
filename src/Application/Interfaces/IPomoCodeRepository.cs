using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;
public interface IPomoCodeRepository : IRepository<PromoCode>
{
    Task<PromoCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> IsCodeUniqueAsync(string code, CancellationToken cancellationToken = default);

    Task<List<PromoCode>> GetActivePromoCodesAsync(CancellationToken cancellationToken = default);
    Task<bool> MaxUserUsedPromoCodeAsync(string userId, string promoCode, CancellationToken cancellationToken = default);
}

public interface  IUnitOfWork : IDisposable
{
    IPomoCodeRepository PromoCodes { get; }
    Task<int> SaveChangesAsync( CancellationToken cancellationToken = default );
}