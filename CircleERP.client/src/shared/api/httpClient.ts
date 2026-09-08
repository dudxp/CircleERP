import axios from "axios";

/**
 * Cliente HTTP da aplicacao. Nenhum componente conhece axios: as features
 * falam com a API atraves dos modulos em `features/<nome>/api`.
 */
export const httpClient = axios.create({
  baseURL: __API_BASE_URL__,
  headers: { "Content-Type": "application/json" },
});
