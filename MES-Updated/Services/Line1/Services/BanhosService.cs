using f10.pulsar.mes.DTOs;
using f10.pulsar.mes.Enums;
using f10.pulsar.sv.data;


public interface IBanhosService
{
    
    Task<List<BanhoDTO>> GetOverviewAsync();

    Task<Banho?> GetByIdAsync(int id);

    Task SaveBanhoAsync(Banho model);
}


public class BanhosService : IBanhosService
{
    private readonly IBanhoRepository _banhoRepo;
    private readonly IBanhoReceitaRepository _receitaRepo;

    public BanhosService(IBanhoRepository banhoRepo, IBanhoReceitaRepository receitaRepo)
    {
        _banhoRepo = banhoRepo;
        _receitaRepo = receitaRepo;
    }

  
  
    public async Task<List<BanhoDTO>> GetOverviewAsync()
    {
        var banhos = await _banhoRepo.GetAllAsync();
        var receitas = await _receitaRepo.GetAllAsync();

        return banhos.Select(b =>
        {
            var receita = receitas.FirstOrDefault(r => r.Id == b.BanhoReceitaId);
            return CriarDto(b, receita);
        }).ToList();
    }


    public Task<Banho?> GetByIdAsync(int id) => _banhoRepo.GetByIdAsync(id);

  
    public async Task SaveBanhoAsync(Banho model)
    {
        if (model.Id == 0)
        {
            model.IsActive = true;
            await _banhoRepo.AddAsync(model);
        }
        else
        {
            await _banhoRepo.UpdateAsync(model);
        }
    }


    private BanhoDTO CriarDto(Banho banho, BanhoReceita? receita)
    {
        var dto = new BanhoDTO
        {
            Id = banho.Id,
            Name = banho.Name,
            IsActive = banho.IsActive,
            TinaNum = banho.TinaNum,

            TemperaturaAtual = banho.TemperatureAtual,
            PhAtual = banho.PhAtual,
            AmperesAtuais = banho.AmperesAtual,

            NumeroCargas = banho.NumeroCargas,
            DateCreated = banho.DateCreated
        };

        AplicarEstadoMES(dto, receita);
        return dto;
    }

    private void AplicarEstadoMES(BanhoDTO dto, BanhoReceita? receita)
    {
        if (receita == null)
        {
            dto.Estado = EstadoBanhoMES.Hold;
            dto.CorEstado = "#6c757d";
            return;
        }

        //if (dto.TemperaturaAtual < receita.TemperaturaMin ||
        //    dto.TemperaturaAtual > receita.TemperaturaMax)
        //{
        //    dto.Estado = EstadoBanhoMES.Alarm;
        //    dto.CorEstado = "#dc3545";
        //    return;
        //}

        //if (dto.PhAtual < receita.PhMin ||
        //    dto.PhAtual > receita.PhMax)
        //{
        //    dto.Estado = EstadoBanhoMES.Alarm;
        //    dto.CorEstado = "#dc3545";
        //    return;
        //}

        dto.Estado = EstadoBanhoMES.Running;
        dto.CorEstado = "#28a745";
    }
}
