using Application.Interfaces;
using Domain.Models.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;
public class PromoCodeRepository : BaseRepository<PromoCode>, IPomoCodeRepository
{
    public PromoCodeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PromoCode?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(p => p.Usages)
            .FirstOrDefaultAsync(pc => pc.Code == code.ToUpper(), cancellationToken);
    }
    public async Task<List<PromoCode>> GetActivePromoCodesAsync(CancellationToken cancellationToken)
    {
        return await _dbSet
            .Where(pc => pc.IsActive && pc.ValidFrom <= DateTime.UtcNow && pc.ValidUntil >= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, CancellationToken cancellationToken = default)
    {
        return !await _dbSet.AnyAsync(pc => pc.Code == code.ToUpper(), cancellationToken);
    }
}
