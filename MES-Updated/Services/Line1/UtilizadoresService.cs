using f10.pulsar.mes;
using f10.pulsar.sv.data;
using f10.pulsar.sv.data.mariadb;
using Microsoft.EntityFrameworkCore;

public class UtilizadoresService
{
    private readonly IDbContextFactory<PulsarDataContext> _factory;
    private readonly Utils _utils;

    public UtilizadoresService(IDbContextFactory<PulsarDataContext> factory, Utils utils)
    {
        _factory = factory;
        _utils = utils;
    }


    public async Task<List<Utilizadores>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Utilizadores
                        .AsNoTracking()
                        .OrderBy(x => x.Nome)
                        .ToListAsync();
    }

    public async Task<Utilizadores?> GetByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Utilizadores.AsNoTracking()
                                     .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task SaveAsync(Utilizadores model, int id)
    {
        await using var db = await _factory.CreateDbContextAsync();

        if (id == 0)
        {
            if (string.IsNullOrWhiteSpace(model.Pass))
                throw new InvalidOperationException("Password obrigatória");

            db.Utilizadores.Add(new Utilizadores
            {
                User = model.User,
                Nome = model.Nome,
                Email = model.Email,
                CardId = model.CardId,
                Nivel = model.Nivel,
                Pass = _utils.Hash(model.Pass)
            });

            _utils.UserLog($"Inseriu o utilizador '{model.Nome}'", "MES");
        }
        else
        {
            var entity = await db.Utilizadores.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new InvalidOperationException("Utilizador não encontrado");

            entity.User = model.User;
            entity.Nome = model.Nome;
            entity.Email = model.Email;
            entity.CardId = model.CardId;
            entity.Nivel = model.Nivel;

            if (!string.IsNullOrWhiteSpace(model.Pass))
                entity.Pass = _utils.Hash(model.Pass);

            _utils.UserLog($"Editou o utilizador '{model.Nome}'", "MES");
        }

        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var entity = await db.Utilizadores.FirstAsync(x => x.Id == id);
        db.Utilizadores.Remove(entity);

        await db.SaveChangesAsync();

        _utils.UserLog($"Eliminou o utilizador '{entity.Nome}'", "MES");
    }
}
