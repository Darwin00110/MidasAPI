# Midas API

> API bancária REST desenvolvida em C# com ASP.NET Core, simulando operações reais de um sistema financeiro com foco em arquitetura limpa, segurança e testabilidade.

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

---

## Sobre o projeto

O Midas é uma API bancária que simula como um sistema financeiro funciona por trás da tela. Separa clientes e administradores, gerencia contas, processa transações PIX e aplica regras de negócio reais.

O objetivo não é apenas ter rotas funcionando — é demonstrar como um sistema bancário pode ser estruturado de forma séria, com foco em manutenção, testes, organização por camadas e segurança.

---

## Funcionalidades

**Usuários e autenticação**
- Cadastro com CPF, e-mail, telefone e data de nascimento
- Autenticação via JWT com claims de ID, role e status
- Separação de perfis — USER e ADMIN
- Bloqueio e desbloqueio de usuários e contas

**Contas bancárias**
- Criação automática de conta ao cadastrar usuário
- Consulta de saldo e dados cadastrais
- Chave PIX vinculada à conta

**Transações**
- Transferência PIX entre contas
- Depósito na própria conta
- Saque com validação de CPF
- Extrato completo de transações enviadas e recebidas
- Consulta de transação por ID

**Segurança**
- Autenticação JWT
- Hash de senha com BCrypt
- Rate Limiting — 100 requisições por minuto
- Políticas de autorização por status da conta

---

## Tecnologias

| Tecnologia | Uso |
|---|---|
| C# / ASP.NET Core 8 | Framework principal |
| Entity Framework Core | ORM e migrations |
| MySQL | Banco de dados |
| JWT | Autenticação |
| BCrypt | Hash de senhas |
| Docker | Containerização |
| xUnit + Moq | Testes unitários |
| Swagger | Documentação |

---

## Arquitetura

O projeto foi organizado com **Clean Architecture**, **DDD**, **Repository Pattern** e **Inversão de Dependência**
```
.MidasAPI/
├── src/
│   ├── Domain/          → entidades, enums, interfaces e exceções
│   ├── Application/     → casos de uso e DTOs
│   └── Infra/           → controllers, repositories, services, DbContext
|
├── docker-compose.yml
└── Program.cs
MidasAPI.Tests/      → testes unitários

```
**Fluxo de dependência:**
Infra → Application → Domain

O Domain não conhece MySQL, EF Core ou HTTP.
O Application usa interfaces para orquestrar os casos de uso.
O Infra implementa os detalhes concretos.

---

## Como executar

### Com Docker (recomendado)

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/MidasAPI.git
cd MidasAPI

# Crie o arquivo .env na raiz
cp .env.example .env

# Suba os containers
docker compose up --build
```

A API estará disponível em `http://localhost:5065/swagger`

### Localmente

```bash
# Garanta que o MySQL esteja rodando
# Configure o .env com suas credenciais
dotnet run --project MidasAPI/MidasAPI.csproj
```

### Testes

```bash
dotnet test
```

---

## Variáveis de ambiente

Crie um arquivo `.env` na raiz do projeto:

```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Server=localhost;Port=3306;Database=MidasAPI;User=root;Password=sua_senha
Jwt__Secret=sua_chave_secreta_aqui
Jwt__Issuer=MidasAPI
Jwt__Audience=MidasAPI-Client
MYSQL_ROOT_PASSWORD=sua_senha
MYSQL_DATABASE=MidasDb
```

---

## Endpoints principais

| Método | Rota | Descrição |
|---|---|---|
| POST | /user/register | Cadastro de usuário |
| POST | /user/login | Login e geração de token |
| GET | /accounts/data | Dados da conta autenticada |
| POST | /transacao/transferir | Transferência PIX |
| POST | /transacao/depositar | Depósito |
| POST | /transacao/sacar | Saque |
| GET | /transacao/extrato | Extrato completo |
| GET | /transacao/extrato/{id} | Transação por ID |

---

## Decisões técnicas

**Clean Architecture** — as regras de negócio ficam protegidas de mudanças no banco ou na interface HTTP. Fica mais fácil trocar MySQL, ajustar controllers ou alterar serviços sem quebrar o domínio.

**DDD** — trata o sistema como um conjunto de regras e comportamentos, não apenas tabelas. User, Accounts e Transacao são entidades centrais com validações próximas das regras do negócio.

**Repository Pattern** — isola o acesso aos dados. O Application chama contratos, o Infra conversa com o DbContext. A regra de negócio não sabe como a persistência funciona por baixo.

**TDD** — testes escritos junto com a lógica, cobrindo cenários de usuários, contas, transações e admin com xUnit e Moq.

---

## Autor

Desenvolvido por **Isaque Santos**

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Isaque%20Santos-blue)](https://linkedin.com/in/seu-perfil)
[![Email](https://img.shields.io/badge/Email-isaquesantos001100%40gmail.com-red)](mailto:isaquesantos001100@gmail.com)