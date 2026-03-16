using f10.pulsar.mes.DTOs;
using f10.pulsar.mes.Enums;
using f10.pulsar.sv.data;
using System;

public interface IBanhoReceitaService
{
    Task<List<BanhoReceita>> GetAllAsync();
    Task<BanhoReceita?> GetByIdAsync(int id);

    Task CreateAsync(BanhoReceita model);
    Task UpdateAsync(BanhoReceita model);

    Task ActivateAsync(int receitaId);
    Task DeactivateAsync(int receitaId);

    Task<List<BanhoReceitaParametro>> GetByReceitaIdAsync(int receitaId);
    Task CreateAsync(BanhoReceitaParametro model);
    Task UpdateAsync(BanhoReceitaParametro model);
}

public class BanhoReceitaService : IBanhoReceitaService
{
    private readonly IBanhoReceitaRepository _repo;
    private readonly IBanhoReceitaParametroRepository _repoReceitaParametro;

    public BanhoReceitaService(IBanhoReceitaRepository repo, IBanhoReceitaParametroRepository repoReceitaParametro)
    {
        _repo = repo;
        _repoReceitaParametro = repoReceitaParametro;
    }

    public Task<List<BanhoReceita>> GetAllAsync() =>
        _repo.GetAllAsync();

    public Task<BanhoReceita?> GetByIdAsync(int id) =>
        _repo.GetByIdAsync(id);

    public async Task CreateAsync(BanhoReceita model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
            throw new InvalidOperationException("Nome da receita é obrigatório.");

        // Regra MES: receita nasce inativa
        model.IsActive = false;

        await _repo.AddAsync(model);
    }

    public async Task UpdateAsync(BanhoReceita model)
    {
        if (model.Id <= 0)
            throw new InvalidOperationException("Receita inválida.");

        var current = await _repo.GetByIdAsync(model.Id)
            ?? throw new InvalidOperationException("Receita não encontrada.");

        // Regra MES: receita ativa não deve ser alterada estruturalmente
        if (current.IsActive)
            throw new InvalidOperationException(
                "Não é permitido alterar uma receita ativa.");

        current.Name = model.Name;

        await _repo.UpdateAsync(current);
    }

    public async Task ActivateAsync(int receitaId)
    {
        var receita = await _repo.GetByIdAsync(receitaId)
            ?? throw new InvalidOperationException("Receita não encontrada.");

        if (receita.IsActive)
            return;

        receita.IsActive = true;
        await _repo.UpdateAsync(receita);
    }

    public async Task DeactivateAsync(int receitaId)
    {
        var receita = await _repo.GetByIdAsync(receitaId)
            ?? throw new InvalidOperationException("Receita não encontrada.");

        if (!receita.IsActive)
            return;

        receita.IsActive = false;
        await _repo.UpdateAsync(receita);
    }

    public Task<List<BanhoReceitaParametro>> GetByReceitaIdAsync(int receitaId) =>
        _repoReceitaParametro.GetByReceitaIdAsync(receitaId);

    public async Task CreateAsync(BanhoReceitaParametro model)
    {
        await Validate(model);

        await _repoReceitaParametro.AddAsync(model);
    }

    public async Task UpdateAsync(BanhoReceitaParametro model)
    {
        if (model.Id <= 0)
            throw new InvalidOperationException("Parâmetro inválido.");

        await Validate(model);

        await _repoReceitaParametro.UpdateAsync(model);
    }

    private async Task Validate(BanhoReceitaParametro model)
    {
        if (string.IsNullOrWhiteSpace(model.ParameterName))
            throw new InvalidOperationException("Nome do parâmetro é obrigatório.");

        if (model.Min >= model.Max)
            throw new InvalidOperationException("Min deve ser inferior a Max.");

        if (model.WarningMin.HasValue && model.WarningMin < model.Min)
            throw new InvalidOperationException("WarningMin não pode ser inferior ao Min.");

        if (model.WarningMax.HasValue && model.WarningMax > model.Max)
            throw new InvalidOperationException("WarningMax não pode ser superior ao Max.");

        // Regra MES importante:
        var receita = await _repo.GetByIdAsync(model.BanhoReceitaId)
            ?? throw new InvalidOperationException("Receita não encontrada.");

        if (!receita.IsActive)
            throw new InvalidOperationException(
                "Não é permitido alterar parâmetros de uma receita inativa.");
    }
}