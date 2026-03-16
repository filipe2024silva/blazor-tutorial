using f10.pulsar.sv.data;
using f10.pulsar.sv.data.mariadb;
using Microsoft.EntityFrameworkCore;

public interface IBanhoReceitaRepository
{
    Task<List<BanhoReceita>> GetAllAsync();
    Task<BanhoReceita?> GetByIdAsync(int id);
    Task AddAsync(BanhoReceita model);
    Task UpdateAsync(BanhoReceita model);
}

public class BanhoReceitaRepository : IBanhoReceitaRepository
{
    private readonly IDbContextFactory<PulsarDataContext> _factory;

    public BanhoReceitaRepository(IDbContextFactory<PulsarDataContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<BanhoReceita>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.BanhoReceitas.AsNoTracking().ToListAsync();
    }

    public async Task<BanhoReceita?> GetByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.BanhoReceitas.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(BanhoReceita model)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.BanhoReceitas.Add(model);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(BanhoReceita model)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.BanhoReceitas.Update(model);
        await db.SaveChangesAsync();
    }
}