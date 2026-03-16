using f10.pulsar.sv.data;
using f10.pulsar.sv.data.mariadb;
using Microsoft.EntityFrameworkCore;

public interface IBanhoReceitaParametroRepository
{
    Task<List<BanhoReceitaParametro>> GetByReceitaIdAsync(int receitaId);
    Task<BanhoReceitaParametro?> GetByIdAsync(int id);

    Task AddAsync(BanhoReceitaParametro model);
    Task UpdateAsync(BanhoReceitaParametro model);
    Task DeleteAsync(int id);
}

public class BanhoReceitaParametroRepository : IBanhoReceitaParametroRepository
{
    private readonly IDbContextFactory<PulsarDataContext> _factory;

    public BanhoReceitaParametroRepository(
        IDbContextFactory<PulsarDataContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<BanhoReceitaParametro>> GetByReceitaIdAsync(int receitaId)
    {
        await using var db = await _factory.CreateDbContextAsync();

        return await db.BanhoReceitaParametros
            .AsNoTracking()
            .Where(p => p.BanhoReceitaId == receitaId)
            .OrderBy(p => p.ParameterName)
            .ToListAsync();
    }

    public async Task<BanhoReceitaParametro?> GetByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();

        return await db.BanhoReceitaParametros
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(BanhoReceitaParametro model)
    {
        await using var db = await _factory.CreateDbContextAsync();

        db.BanhoReceitaParametros.Add(model);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(BanhoReceitaParametro model)
    {
        await using var db = await _factory.CreateDbContextAsync();

        db.BanhoReceitaParametros.Update(model);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var entity = await db.BanhoReceitaParametros.FindAsync(id);
        if (entity == null)
            return;

        db.BanhoReceitaParametros.Remove(entity);
        await db.SaveChangesAsync();
    }
}