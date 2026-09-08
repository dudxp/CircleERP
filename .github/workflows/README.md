# Workflows

| Workflow | Quando roda | O que faz |
|---|---|---|
| `build.yml` | push em qualquer branch e em todo pull request | Job `backend`: restore, build (Release) e `dotnet test` da solucao inteira em .NET 10. Job `client`: `npm ci`, `npm run lint` e `npm run build` do front em Node 22. |

Os dois jobs rodam em paralelo e sao independentes.

## Notas

- O SDK do CI precisa acompanhar o `TargetFramework` definido em `Directory.Build.props` (hoje `net10.0`).
- O front usa `npm ci`, entao o `package-lock.json` precisa estar commitado e em sincronia com o `package.json`.

## Por que nao ha servico de banco

O job `backend` roda sem container de MySQL de proposito. Os testes de
integracao sobem a API com `WebApplicationFactory` e trocam o `DbContext` por um
SQLite em memoria -- nenhuma conexao com MySQL acontece.

Se um dia forem escritos testes que precisem de MySQL de verdade (para cobrir o
que o SQLite nao cobre: tipo de coluna, colacao, comportamento do Pomelo), eles
devem ficar em um projeto separado, com o servico declarado no job e a
`MYSQL_CONNECTION_STRING` exportada no `env` do step. Seria um complemento aos
testes atuais, nao um substituto.

Para conferir que os testes nao dependem do ambiente da maquina:

```bash
env -u MYSQL_CONNECTION_STRING dotnet test CircleERP.sln
```
