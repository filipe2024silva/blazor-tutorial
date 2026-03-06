namespace BlazorTarefas.Shared.Data;

public static class TarefaDados
{
  public static List<Tarefa> ObterTarefas() => new()
  {
     new Tarefa { Descricao = "Fazer curso", Concluida = false, DataCriacao = DateTime.Parse("2025-01-25") },
     new Tarefa { Descricao = "Estudar .NET 9", Concluida = true, DataCriacao = DateTime.Parse("2025-02-20") },
     new Tarefa { Descricao = "Ler documentação Blazor", Concluida = false, DataCriacao = DateTime.Parse("2025-03-04") },
     new Tarefa { Descricao = "Praticar consultas LINQ", Concluida = true, DataCriacao = DateTime.Parse("2025-04-09") },
     new Tarefa { Descricao = "Criar API com EF Core", Concluida = false, DataCriacao = DateTime.Parse("2025-05-12") },
     new Tarefa { Descricao = "Fazer testes unitários", Concluida = true, DataCriacao = DateTime.Parse("2025-06-01") },
     new Tarefa { Descricao = "Publicar no GitHub", Concluida = false, DataCriacao = DateTime.Parse("2025-06-15") }
   };
}
