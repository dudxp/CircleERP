# Workflows

| Workflow | Quando roda | O que faz |
|---|---|---|
| `build.yml` | push em qualquer branch e em todo pull request | Job `backend`: restore, build (Release) e `dotnet test` da solucao inteira em .NET 10. Job `client`: `npm ci`, `npm run lint` e `npm run build` do front em Node 22. |

Os dois jobs rodam em paralelo e sao independentes.

## Notas

- O SDK do CI precisa acompanhar o `TargetFramework` definido em `Directory.Build.props` (hoje `net10.0`).
- O front usa `npm ci`, entao o `package-lock.json` precisa estar commitado e em sincronia com o `package.json`.
