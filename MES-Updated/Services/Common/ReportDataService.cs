using f10.pulsar.mes;
using Microsoft.EntityFrameworkCore;


public interface IReportDataService
{
    /// <summary>
    /// Devolve a informacação geral do roteiro a consultar.
    /// </summary>
    /// <param name="idTratamento"></param>
    /// <returns></returns>
    Task<(ReportCabecalho, List<ReportRoteiroProducao>, Dictionary<string, List<RetificadoresValues>>)> GetReportInfo(int idTratamento);

    /// <summary>
    /// Devolve a lista do roteiro.
    /// </summary>
    /// <param name="lote"></param>
    /// <returns></returns>
    Task<List<ReportRoteiroProducao>> GetRoteiroProducao(string lote);

    /// <summary>
    /// Devolve os valores do retificador. Constroi um dicionário com as tinas(processo) que trabalhou no tratamento e respetivos valores.
    /// </summary>
    /// <param name="lote"></param>
    /// <param name="rangeIni"></param>
    /// <param name="rangeFin"></param>
    /// <returns></returns>
    Task<Dictionary<string, List<RetificadoresValues>>> GetRetificadoresValues(string lote, DateTime rangeIni, DateTime rangeFin);

}


public class ReportDataService : IReportDataService
{

    private readonly DataLink_L1 _dl = new DataLink_L1();

    public async Task<(ReportCabecalho, List<ReportRoteiroProducao>, Dictionary<string, List<RetificadoresValues>>)> GetReportInfo(int idTratamento)
    {

        try
        {
            ReportCabecalho fillReportCabecalho = await (from inf in _dl.db.ReferenciaTrabalhos
                                                         join progs in _dl.db.PlcProgramas on inf.Campo1 equals progs.Num.ToString()
                                                         where inf.Id == idTratamento
                                                         select new ReportCabecalho
                                                         {
                                                             Lote = inf.Lote,
                                                             Tambor = "A definir...",
                                                             Operador = inf.Operador,
                                                             Notas = inf.Notas,
                                                             Tratamento = progs.Nome
                                                         }).FirstAsync();

            List<ReportRoteiroProducao> roteiro = await GetRoteiroProducao(fillReportCabecalho.Lote);

            var firstRecord = roteiro.First();
            var lastRecord = roteiro.Last();

            if (roteiro != null && roteiro.Any())
            {
                if (firstRecord.Tini.HasValue && lastRecord.Tfin.HasValue)
                {
                    TimeSpan duracao = lastRecord.Tfin.Value - firstRecord.Tini.Value;
                    fillReportCabecalho.TempTotal = duracao.ToString(@"hh\:mm\:ss");
                    fillReportCabecalho.DataInicio = firstRecord.Tini.Value.ToString("dd/MM/yyyy HH:mm:ss");
                    fillReportCabecalho.DataFim = lastRecord.Tfin.Value.ToString("dd/MM/yyyy HH:mm:ss"); 
                }

            }

            Dictionary<string, List<RetificadoresValues>> retificadoresValores = await GetRetificadoresValues(fillReportCabecalho.Lote, firstRecord.Tini.Value, lastRecord.Tfin.Value);

            return (fillReportCabecalho, roteiro, retificadoresValores);

        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error in GetReportCabecalhoData: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetReportCabecalhoData: {ex.Message}");
            throw;
        }

    }

    public async Task<List<ReportRoteiroProducao>> GetRoteiroProducao(string lote)
    {

        try
        {
            List<ReportRoteiroProducao> registos = await _dl.db.Registos
                .Where(rg => rg.Lote == lote)
                .OrderBy(rg => rg.TempoInicial)
                .Select(o => new ReportRoteiroProducao
                {
                    Tina = o.Tina ?? "",
                    Designacao = o.Posicao ?? "",
                    Tini = o.TempoInicial.GetValueOrDefault(),
                    Tfin = o.TempoFinal.GetValueOrDefault(),
                    Ttotal = o.TempoFinal != null ? (o.TempoFinal.GetValueOrDefault() - o.TempoInicial.GetValueOrDefault()).ToString(@"hh\:mm\:ss") : DateTime.MinValue.ToString(@"00:00:00"),
                    TempMin = o.TemperaturaMin != null ? o.TemperaturaMin : 0,
                    TempMax = o.TemperaturaMax != null ? o.TemperaturaMax : 0,
                    TempMed = o.TemperaturaMed != null ? o.TemperaturaMed : 0,
                    TempDes = o.TemperaturaDesejada != null ? o.TemperaturaDesejada : 0,
                    Ph = o.Ph != null ? o.Ph : 0,
                    Corrente = o.Corrente != null ? o.Corrente : 0,
                    Tensao = o.Tensao != null ? o.Tensao : 0,
                    Utilizador = o.Operador ?? "",
                    Estado = o.Estado ?? ""
                }).ToListAsync();

            return registos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRoteiroProducao: {ex.Message}");
            return new List<ReportRoteiroProducao>();
        }

    }

    public async Task<Dictionary<string, List<RetificadoresValues>>> GetRetificadoresValues(string lote, DateTime rangeIni, DateTime rangeFin)
    {
        try
        {
            //tinas que foram usadas em determindado lote
            var tinasDoLote = _dl.db.Registos.Where(r => r.Lote == lote).Select(r => r.Tina);

            //tinas usados no lote que contém retidicadores
            var tinasComRetificador = _dl.db.Tinas.Where(t => t.Rectificador == true && tinasDoLote.Contains(t.Tina1));

            //agrupa os valores dos retificadores registados entre datas e forma as listas para cada tina
            var groupedData = await _dl.db.Rectificadors
                                .Where(r => r.Data >= rangeIni && r.Data <= rangeFin)
                                .Join(tinasComRetificador,
                                    rect => rect.Tina,
                                    ti => ti.Num.ToString(),
                                    (rect, ti) => new RetificadoresValues
                                    {
                                        Tina = ti.Designacao,
                                        Corrente = rect.Corrente ?? 0m,
                                        Tensao = rect.Tensao ?? 0m,
                                        DataRegisto = rect.Data.HasValue ? rect.Data.Value.ToString("dd/MM/yyyy HH:mm:ss") : "N/A"
                                    })
                                .GroupBy(x => x.Tina).ToListAsync();

            //cria o dicionário dinamicamente consoante a quantidade de agrupamentos feitos
            var rectValues = groupedData.Select((item, index) => new
            {
                Key = $"dataSource{index + 1}",
                item
            }).ToDictionary(x => x.Key, x => x.item.ToList());


            return rectValues;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRetificadoresValues: {ex.Message}");
            throw;
        }
    }
}

#region DTO's

public class ReportCabecalho
{
    public string Lote { get; set; } = string.Empty;
    public string Tambor { get; set; } = string.Empty;
    public string Operador { get; set; } = string.Empty;
    public string Notas { get; set; } = string.Empty;
    public string Tratamento { get; set; } = string.Empty;
    public string DataInicio { get; set; } = string.Empty;
    public string DataFim { get; set; } = string.Empty;
    public string TempTotal { get; set; } = string.Empty;

}

public class ReportRoteiroProducao
{
    public string Tina { get; set; } = string.Empty;
    public string Designacao { get; set; } = string.Empty;
    public DateTime? Tini { get; set; } = DateTime.MinValue;
    public DateTime? Tfin { get; set; } = DateTime.MinValue;
    public string? Ttotal { get; set; } = string.Empty;
    public decimal? TempMin { get; set; } = 0.0m;
    public decimal? TempMed { get; set; } = 0.0m;
    public decimal? TempMax { get; set; } = 0.0m;
    public decimal? TempDes { get; set; } = 0.0m;
    public decimal? Ph { get; set; } = 0.0m;
    public decimal? Corrente { get; set; } = 0.0m;
    public decimal? Tensao { get; set; } = 0.0m;
    public string Utilizador { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;

}

public class RetificadoresValues
{
    public string Tina { get; set; } = string.Empty;
    public string DataRegisto { get; set; } = string.Empty;
    public decimal Tensao { get; set; } = 0.0m;
    public decimal Corrente { get; set; } = 0.0m;

}

#endregion