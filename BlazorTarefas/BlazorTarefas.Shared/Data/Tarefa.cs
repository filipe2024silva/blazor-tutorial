namespace BlazorTarefas.Shared.Data;

public class Tarefa
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public string? Descricao { get; set; }
    public bool Concluida { get; set; }
    public DateTime DataCriacao { get; set; }
}
