using f10.pulsar.mes;
using f10.pulsar.mes.data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

public class EquipamentosService
{
    private readonly IDbContextFactory<MySqlDataContext> _factory;
    private readonly Utils _utils;
    public EquipamentosService(IDbContextFactory<MySqlDataContext> factory, Utils utils)
    {
        _factory = factory;
        _utils = utils;
    }

    /// <summary>
    /// Devolve tuplo de equipamentos ativas/desativas
    /// </summary>
    /// <returns></returns>
    public async Task<(List<Equipamento>, List<Equipamento>)> GetAllAsync()
    {
        try
        {
            await using var db = await _factory.CreateDbContextAsync();

            var all = await db.Equipamentos
                            .AsNoTracking()
                            .OrderBy(x => x.Nome)
                            .ToListAsync();

            var ativas = all.Where(x => x.Active).ToList();
            var inativas = all.Where(x => !x.Active).ToList();

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
    public async Task<Equipamento?> GetByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Equipamentos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Save create ou update do objeto
    /// </summary>
    /// <param name="model"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task SaveAsync(Equipamento model, int id)
    {
        await using var db = await _factory.CreateDbContextAsync();

        if (id == 0)
        {
            db.Equipamentos.Add(new Equipamento
            {
                Nome = model.Nome,
                Designacao = model.Designacao,
                Localizacao = model.Localizacao,
                Serial = model.Serial,
                IP = model.IP,
                Porta = model.Porta,
                Active = true
            });

            _utils.UserLog($"Adicionou o equipamento '{model.Nome}'", "MES");
        }
        else
        {
            var entity = await db.Equipamentos.FirstAsync(x => x.Id == id);

            entity.Nome = model.Nome;
            entity.Designacao = model.Designacao;
            entity.Localizacao = model.Localizacao;
            entity.Serial = model.Serial;
            entity.IP = model.IP;
            entity.Porta = model.Porta;
            entity.Active = model.Active;

            _utils.UserLog($"Alterou o equipamento '{model.Nome}'", "MES");
        }

        await db.SaveChangesAsync();
    }

}
