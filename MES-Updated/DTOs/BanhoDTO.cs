using f10.pulsar.mes.Enums;
using Org.BouncyCastle.Crypto.Utilities;
using static f10.pulsar.mes.Pages.Linha1.L1Banhos;

namespace f10.pulsar.mes.DTOs
{
    /// <summary>
    /// Agrupa o que é pretendido mostrar num banho
    /// </summary>
    //public class BanhoDTO
    //{
    //   public int Id { get; set; }
    //   public string Name { get; set; }
    //   public bool IsActive { get; set; }
    //   public int TinaId { get; set; }
    //   public int ReceitaId { get; set; }
    //   public DateTime DateCreated { get; set; }
    //   public DateTime DateFinished { get; set; }
    //   public decimal TemperaturaAtual { get; set; }
    //   public decimal PhAtual { get; set; }
    //   public decimal AmperesAtuais { get; set; }
    //   public int NumeroCargas { get; set; }
    //   public string UltimoEvento { get; set; }
    //   public decimal EstadoCalculado { get; set; }
    //   public string CorEstado { get; set; }

    //}

    public class BanhoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TinaNum { get; set;}
        public bool IsActive { get; set; }
        public EstadoBanhoMES Estado { get; set; }
        public string CorEstado { get; set; } = string.Empty;

        public decimal TemperaturaAtual { get; set; }
        public decimal PhAtual { get; set; }
        public decimal AmperesAtuais { get; set; }

        public int NumeroCargas { get; set; }
        public string UltimoEvento { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }
        public DateTime? DateFinished { get; set; }
    }
}
