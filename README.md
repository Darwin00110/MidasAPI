# FinTrackAI — API Financeira Bancária

API REST financeira desenvolvida com .NET 8 e ASP.NET Core,
aplicando regras de domínio bancário reais.

## 🛠️ Tecnologias

- .NET 8 / ASP.NET Core
- Entity Framework Core + MySQL
- JWT Authentication + Roles
- xUnit + Moq (testes unitários)
- Clean Architecture + DDD
- Docker

## 📋 Funcionalidades

- Cadastro e autenticação de usuários (User/Admin)
- Criação e gestão de contas bancárias
- Transferências PIX, TED e DOC com rollback transacional
- Testes unitários cobrindo CRUD, autenticação e segurança
- Implementação de rotas administrativas como (Bloquear usuario, Desbloquear usuario)

## 🏗️ Arquitetura

src/
├── Domain/        # Entidades e regras de negócio
├── Application/   # Use Cases e DTOs
├── Infrastructure/# Repositórios e DbContext
└── API/           # Controllers e configuração

## ▶️ Como rodar

```bash
git clone https://github.com/Darwin00110/FinTrackAI
cd FinTrackAI
docker-compose up
```

## 📬 Contato
isaquesantos001100@gmail.com