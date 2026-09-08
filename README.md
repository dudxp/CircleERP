# CircleERP

## IMPORTANTE

No primeiro momento esse projeto só possui um módulo de moedas

## Descrição

O **CircleERP** é uma aplicação de ERP (sistema integrado de gestão) híbrida que combina back-end em C# (.NET) e front-end em React + TypeScript.  
O objetivo é demonstrar funcionalidades de controle comercial, integrações, e interface de usuário web interativa.

Este projeto foi desenvolvido como parte de estudos pessoais e exercícios avançados, com foco em boas práticas de arquitetura, modularização e integração de camadas.

---

## Funcionalidades Principais

- Cadastro, consulta e atualização de entidades típicas de ERP (clientes, produtos, pedidos, etc).  
- Integrações via APIs REST entre front-end e back-end.  
- Interface SPA (Single Page Application) moderna com React + TypeScript.  
- Testes automatizados (caso haja testes) para garantir qualidade do back-end.  
- Organização modular: separação de camadas (client, service model, testes, etc.).

---

## Tecnologias Utilizadas

| Camada        | Tecnologia / Ferramenta                 |
|----------------|------------------------------------------|
| Back-End        | C#, .NET 9, ASP.NET Core, APIs REST     |
| Front-End       | React, TypeScript, Webpack              |
| Banco de Dados  | MySql |
| Testes          | (JUnit, xUnit, MSTest, ou ferramenta que usou) |
| Ferramentas      | Git, Azure DevOps ou GitHub Actions, VSCode / IDE, Postman |

---

## Estrutura do Projeto

```
CircleERP.Domain          agregados, value objects, eventos (zero dependencias)
CircleERP.Application     casos de uso (MediatR), contratos de porta
CircleERP.Infrastructure  EF Core, MySQL, repositorios
CircleERP                 host da API: controllers, DI, middleware
CircleERP.Domain.Tests    testes de dominio
CircleERP.client          SPA em React + TypeScript (Vite)
```

Ver [ARCHITECTURE.md](ARCHITECTURE.md) para a regra de dependencia entre as camadas.

---

## Como rodar

Requisitos: .NET 10 SDK, Node 22 e um MySQL acessivel.

Configure a string de conexao (a variavel de ambiente tem prioridade):

```bash
dotnet user-secrets set "ConnectionStrings:CircleERP" "<sua-string>" --project CircleERP
```

Prepare o banco (o mesmo comando serve para banco novo e para o existente --
ver [docs/database-baseline.md](docs/database-baseline.md)):

```bash
dotnet tool restore && dotnet ef database update --project CircleERP.Infrastructure --startup-project CircleERP.Infrastructure
```

Backend (https://localhost:5001, com Scalar em `/scalar`):

```bash
dotnet run --project CircleERP
```

Frontend (http://localhost:54783):

```bash
npm install --prefix CircleERP.client && npm run dev --prefix CircleERP.client
```
