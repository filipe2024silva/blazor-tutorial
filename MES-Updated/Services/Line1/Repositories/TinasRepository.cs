using f10.pulsar.sv.data;
using f10.pulsar.sv.data.mariadb;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;


public interface ITinasRepository
{
    Task<List<Tina>> GetAllAsync();
    Task<Tina?> GetByIdAsync(int id);
    Task UpdateAsync(Tina model);
}

public class TinasRepository : ITinasRepository
{
    private readonly IDbContextFactory<PulsarDataContext> _factory;

    public TinasRepository(IDbContextFactory<PulsarDataContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<Tina>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Tinas.AsNoTracking().ToListAsync();
    }

    public async Task<Tina?> GetByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Tinas.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateAsync(Tina model)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Tinas.Update(model);
        await db.SaveChangesAsync();
    }
}

