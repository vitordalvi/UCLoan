# UCLoan

Aplicação web para **gerenciamento de empréstimos** de equipamentos, construída com **ASP.NET Core MVC**, **Entity Framework Core** e **ASP.NET Core Identity** (usuários + roles)

## Tecnologias

- **.NET 9** (`net9.0`)
- **ASP.NET Core MVC**
- **Razor Pages** (UI padrão do Identity)
- **Entity Framework Core**
- **ASP.NET Core Identity** (com `ApplicationUser` e `IdentityRole`)
- **SQL Server** (padrão do projeto)
- Front-end: Bootstrap (via `bootstrap.sass`) e compilação com `BuildWebCompiler`

## Arquitetura (pastas principais)

Dentro de `UCLoan/`:

- `Controllers/` — controllers da aplicação (MVC)
- `Views/` — views (Razor)
- `Models/` — entidades/modelos
- `ViewModels/` — modelos para telas/formulários
- `Data/` — `DbContext`, migrations, seed, etc.
- `Repositories/` — repositórios (acesso a dados)
- `Services/` — regras de negócio / serviços
- `Areas/` — áreas (ex.: Identity)
- `wwwroot/` — arquivos estáticos (css/js/imagens)

## Pré-requisitos

- **.NET SDK 9**
- **SQL Server**

## Configuração do Banco de Dados

A connection string padrão está em `UCLoan/appsettings.json`:

- `DefaultConnection`: `Server=(localdb)\mssqllocaldb;Database=UCLoanDB;Trusted_Connection=True;MultipleActiveResultSets=true`

## Como rodar o projeto

Na raiz do repositório (onde está a solution `UCLoan.sln`):

```bash
dotnet restore
dotnet build
dotnet run --project UCLoan/UCLoan.csproj
```

## Autenticação e Roles

O projeto usa **ASP.NET Core Identity** com:

- `ApplicationUser`
- `IdentityRole`

## Refatoração do Projeto

Você pode acompanhar como foi o início da refatoração do projeto. Basta mudar para branch "refactor/".

- Essa refatoração foi iniciada para dividir o sistema em dois projetos principais, o front-end e o back-end
  rodando simultaneamente com comunicação via APIs. O desenvolvimento da versão refatorada foi pausado, mas você pode
  explorar o repositório e até comentar comigo o que pode melhorar! :)
  
