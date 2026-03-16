using f10.pulsar.mes.DTOs;
using f10.pulsar.sv.data;

public class PermissionsService
{
    public List<PermissoesAccessDTO> GetAll()
    {
        return new()
        {
            new() { Id = "1",  Name = "Administrador" },
            new() { Id = "5",  Name = "Gestor" },
            new() { Id = "10", Name = "Supervisor" },
            new() { Id = "20", Name = "Operador" }
        };
    }

    public List<PermissoesAccessDTO> GetForUser(string? nivel)
    {
        var all = GetAll();

        if (!string.IsNullOrWhiteSpace(nivel))
        {
            var match = all.FirstOrDefault(x => x.Id == nivel);
            if (match != null)
                match.IsChecked  = true;
        }

        return all;
    }

    public void SelectPermission(List<PermissoesAccessDTO> permissions, string selectedId, Utilizadores user)
    {
        foreach (var p in permissions)
            p.IsChecked = false;

        var selected = permissions.First(x => x.Id == selectedId);
        selected.IsChecked = true;

        user.Nivel = selectedId;
    }
}


