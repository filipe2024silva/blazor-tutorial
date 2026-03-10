namespace Tarefas.Entities;

public class Tarefa
{
    public Guid ID { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Concluida { get; set; }
    public DateTime DataCriacao { get; set; }
}
