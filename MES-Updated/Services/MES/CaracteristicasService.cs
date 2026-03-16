using f10.pulsar.mes.data;
using f10.pulsar.sv.data.mariadb;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

public class CaracteristicasService
{
    private readonly IDbContextFactory<MySqlDataContext> _factory;

    public CaracteristicasService(IDbContextFactory<MySqlDataContext> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Devolve tuplo de caracteristicas ativas/desativas
    /// </summary>
    /// <returns></returns>
    public async Task<(List<Caracteristicas>, List<Caracteristicas>)> GetAllAsync()
    {
        try
        {
            await using var db = await _factory.CreateDbContextAsync();

            var all = await db.Caracteristicas
                               .AsNoTracking()
                               .OrderBy(x => x.Caracteristica)
                               .ToListAsync();

            var ativas = all.Where(x => x.Ativo).ToList();
            var inativas = all.Where(x => !x.Ativo).ToList();

            return (ativas, inativas);
        }
        catch (MySqlException)
        {
            throw;
        }
    }

    /// <summary>
    /// Devolve o objeto by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Caracteristicas?> GetByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Caracteristicas.AsNoTracking().FirstOrDefaultAsync(x => x.IdCaracteristica == id);
    }

    /// <summary>
    /// Save create ou update do objeto
    /// </summary>
    /// <param name="model"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task SaveAsync(Caracteristicas model, int id)
    {
        await using var db = await _factory.CreateDbContextAsync();

        if (id == 0)
        {
            db.Caracteristicas.Add(new Caracteristicas
            {
                Caracteristica = model.Caracteristica,
                Valor = model.Valor,
                Unidade = model.Unidade,
                Ativo = true
            });
        }
        else
        {
            var entity = await db.Caracteristicas.FirstAsync(x => x.IdCaracteristica == id);

            entity.Caracteristica = model.Caracteristica;
            entity.Valor = model.Valor;
            entity.Unidade = model.Unidade;
            entity.Ativo = model.Ativo;
        }

        await db.SaveChangesAsync();

    }
}
