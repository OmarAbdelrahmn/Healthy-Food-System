using Domain.Interfaces.Repositories;
using Domain.Models.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;
public class PromoCodeRepository : IPromoCodeRepository
{
    private readonly ApplicationDbContext _context;
    public PromoCodeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PromoCode?> GetByCodeAsync (string code)
    {
        return await _context.PromoCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(pc => pc.Code == code.ToUpper());
    }
    public async Task<PromoCode?> GetByIdAsync(Guid id)
    {
        return await _context.PromoCodes
            .FindAsync(id);
    }

    public async Task<IEnumerable<PromoCode>> GetAllAsync()
    {
        return await _context.PromoCodes
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(PromoCode promoCode)
    {
        await _context.PromoCodes.AddAsync(promoCode);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(PromoCode promoCode)
    {
        _context.PromoCodes.Update(promoCode);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(PromoCode promoCode)
    {
        _context.PromoCodes.Remove(promoCode);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string code)
    {
        return await _context.PromoCodes
            .AnyAsync(pc => pc.Code == code);
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}
