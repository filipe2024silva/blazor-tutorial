using f10.pulsar.sv.data;
using f10.pulsar.sv.data.mariadb;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;


public interface IBanhoRepository
{
    Task<List<Banho>> GetAllAsync();
    Task<Banho?> GetByIdAsync(int id);
    Task AddAsync(Banho model);
    Task UpdateAsync(Banho model);
}

public class BanhoRepository : IBanhoRepository
{
    private readonly IDbContextFactory<PulsarDataContext> _factory;

    public BanhoRepository(IDbContextFactory<PulsarDataContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<Banho>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Banhos.AsNoTracking().ToListAsync();
    }

    public async Task<Banho?> GetByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Banhos.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Banho model)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Banhos.Add(model);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Banho model)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Banhos.Update(model);
        await db.SaveChangesAsync();
    }
}

