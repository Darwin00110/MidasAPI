# MidasAPI

MidasAPI é uma API bancária pensada para simular como um banco funciona por trás da tela. Ela separa clientes e administradores, organiza contas, processa transações e aplica regras de negócio de verdade.

Se você não é da área técnica, pense nela como um laboratório para entender como um sistema bancário sério é montado por dentro.

O objetivo do projeto é mostrar como um sistema bancário pode ser estruturado de forma séria, com foco em manutenção, testes, organização por camadas e segurança.

## O que este projeto resolve

- Cadastro e autenticação de usuários e administradores.
- Gestão de contas bancárias por cliente.
- Transações financeiras com regras de negócio reais.
- Bloqueio e desbloqueio de usuários e contas.
- Consulta de saldo, dados cadastrais e extrato.
- Segurança com JWT e separação por perfis.

## Como executar

### Link do Deploy
```

```
### Com Docker

```bash
docker compose up --build
```

### Localmente

1. Crie um arquivo `.env` na raiz do projeto com as variáveis de ambiente.
2. Garanta que o MySQL esteja rodando.
3. Execute a API:

```bash
dotnet run --project MidasAPI/MidasAPI.csproj
```

## Variáveis de ambiente

O projeto lê as configurações sensíveis por `.env`, para evitar subir credenciais para o repositório.

Exemplo de estrutura:

```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Server=localhost;Port=3306;Database=MidasAPI;User=Client;Password=********
Jwt__Secret=your-jwt-secret
Jwt__Issuer=MidasAPI
Jwt__Audience=MidasAPI-Client
MYSQL_ROOT_PASSWORD=********
MYSQL_DATABASE=MidasDb
```

## Arquitetura

O projeto foi organizado com Clean Architecture, DDD, Repository Pattern e Inversão de Dependência.

### Camadas

- `Domain`: entidades, enums, exceções e contratos centrais do negócio.
- `Application`: casos de uso e DTOs que orquestram as regras da aplicação.
- `Infra`: implementação de persistência, serviços externos, controllers e integração com o banco.

### Fluxo de dependência

As dependências apontam para dentro:

- O `Domain` não conhece MySQL, EF Core ou HTTP.
- O `Application` usa interfaces para executar os casos de uso.
- O `Infra` implementa os detalhes concretos, como banco, JWT, hash e controllers.

Isso evita acoplamento excessivo e torna o projeto mais fácil de testar, manter e evoluir.

## Por que Clean Architecture

Usei Clean Architecture porque este projeto não quer ser apenas uma API com rotas. A ideia é simular um sistema bancário que continue organizado mesmo quando crescer.

Na prática, isso traz alguns benefícios:

- As regras de negócio ficam protegidas de mudanças no banco ou na interface HTTP.
- Fica mais fácil trocar MySQL, ajustar controllers ou alterar serviços sem quebrar o domínio.
- Os casos de uso ficam explícitos e mais próximos do que realmente acontece no banco.
- Os testes ficam mais simples porque a lógica central depende de contratos, não de implementações concretas.

## Por que DDD

O projeto usa DDD para dar forma ao domínio bancário. Isso ajuda a tratar o sistema como um conjunto de regras e comportamentos, e não só como tabelas no banco.

Aqui isso aparece principalmente em:

- `User` e `Accounts` como entidades centrais.
- `Transacao` como modelagem do processo financeiro.
- Exceções de domínio e validações próximas das regras do negócio.
- Separação clara entre perfis de usuário e administrador.

## Por que Repository Pattern

O Repository Pattern foi usado para isolar o acesso aos dados.

Em vez de espalhar consultas pelo sistema inteiro:

- O `Application` chama contratos de repositório.
- O `Infra` conversa com o `DbContext`.
- A regra de negócio não precisa saber como a persistência funciona por baixo.

Isso melhora a leitura do código e facilita futuras mudanças de infraestrutura.

## Por que EF Core

O Entity Framework Core foi escolhido porque ele encaixa bem nesse tipo de projeto bancário em ASP.NET Core.

Os principais motivos são:

- Integração natural com ASP.NET Core 8.
- Mapeamento das entidades para MySQL.
- Migrations para controlar evolução do schema.
- LINQ para consultas legíveis.
- Suporte a async/await e tracking de entidades.
- Boa combinação com Repository Pattern e Clean Architecture.

No projeto, o EF Core fica concentrado na camada de infraestrutura, o que evita que o domínio fique preso a detalhes do banco.

## Estrutura do projeto

```text
MidasAPI/
|-- Program.cs
|-- appsettings.json
|-- appsettings.Development.json
|-- Migrations/
`-- src/
    |-- Domain/
    |-- Application/
    `-- Infra/
```

## Tecnologias

- .NET 8 / ASP.NET Core
- Entity Framework Core
- MySQL
- JWT Authentication
- Docker
- xUnit
- Moq
- Swashbuckle / Swagger

## Testes

O projeto possui uma suíte de testes em `MidasAPI.Tests`, cobrindo cenários de usuários, contas, transações e admin.

Para executar:

```bash
dotnet test
```

## Observações

- O Swagger está habilitado em ambiente de desenvolvimento.
- O container usa MySQL com migrations aplicadas no startup.
- As credenciais sensíveis devem ficar apenas no `.env`.

## Contato

isaquesantos001100@gmail.com
