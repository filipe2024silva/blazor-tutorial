# MES – Arquitetura Blazor Server 

Refatoração assente nos seguintes principios:

- evitar duplicação de lógica
- separar responsabilidades
- permitir escalar para múltiplas linhas
- manter páginas simples e previsíveis

---

##  Princípios base

- **Páginas NÃO conhecem a base de dados**
- **Serviços conhecem os dados e regras**
- **DbContext só existe nos repositórios e/ou serviços dependendo do tamanho da class**
- **Serviços são organizados por CONTEXTO (MES / Linha / Comum)**
- **EF Core é usado diretamente nos respositórios e/ou serviços**

---

##  Estrutura do projeto

```text
/MES
├─ Pages/
│   ├─ MES/
│   │   └─ Caracteristicas.razor
│   └─ Line/
│       └─ Artigos.razor
│
├─ Components/
│   └─ Shared/
│
├─ Services/
│   ├─ MES/
│   │   ├─ CaracteristicasService.cs
│   │   └─ AuthService.cs
│   │
│   ├─ Line/
│   │   ├─ ArtigosService.cs
│   │   └─ TratamentoService.cs
│   │
│   └─ Common/
│       └─ ReportService.cs
│
└─ Base/
    └─ BaseMesComponent.cs   (hipótese futura – 06/02/2026)

``` 

##  *“onde meto este código?”*

- **UI / estado / eventos** → Page
- **Regras / queries / cálculos** → Service
- **Acesso a dados** → Repositórios


## Nomenculatoras a seguir

A ideia é standarizar para que todos os intervenientes tenham a mesma abordagem de maneira a simplificar quem mexe e quem possa vir a mexer.

| Tipo | Convenção | Exemplo correto | Exemplo incorreto |
|---|---|---|---|
| Classe | `PascalCase` | `CaracteristicasService` | `caracteristicasService` |
| Interface | `IPascalCase` | `ILinhaDashboardService` | `LinhaDashboardServiceInterface` |
| Método | `PascalCase` | `LoadCaracteristicas()` | `load_caracteristicas()` |
| Método assíncrono | `PascalCaseAsync` | `GetAllAsync()` | `GetAll()` |
| Campo privado | `_camelCase` | `_dbContext` | `dbContext` |
| Variável local | `camelCase` | `totalCargas` | `TotalCargas` |
| Propriedade | `PascalCase` | `Caracteristica` | `caracteristica` |
| DTO | `PascalCaseDto` | `LinhaDashboardDto` | `LinhaDashboardData` |
| Serviço | `DomínioService` | `AuthService` | `AuthHelper` |
| DbSet (EF Core) | `PluralPascalCase` | `Caracteristicas` | `Caracteristica` |
| Boolean | `is / has / can` | `isUpdating` | `updating` |
| Namespace | `PascalCase` | `MES.Services.MES` | `mes.services` |