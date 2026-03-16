using f10.pulsar.mes.data;
using Microsoft.EntityFrameworkCore;

public class Linha1Service
{
    private readonly MySqlDataContext _dbLinha1;

    public Linha1Service(MySqlDataContext dbLinha1)
    {
        _dbLinha1 = dbLinha1;
    }

    public async Task<bool> CanConnectAsync()
    {
        return await _dbLinha1.Database.CanConnectAsync();
    }
}
