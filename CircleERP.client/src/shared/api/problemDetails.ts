import { isAxiosError } from "axios";

/**
 * Formato de erro devolvido pela API (RFC 7807).
 */
interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
  /**
   * Campos extras que a API anexa para tornar o erro acionavel -- por exemplo
   * `existingAddressId` num conflito de endereco duplicado.
   */
  [key: string]: unknown;
}

/**
 * Erro de API ja traduzido para uma mensagem exibivel.
 */
export class ApiError extends Error {
  constructor(
    message: string,
    readonly status?: number,
    /** Extensoes do ProblemDetails, quando a API anexou alguma. */
    readonly extensions: Record<string, unknown> = {}
  ) {
    super(message);
    this.name = "ApiError";
  }

  /** Le uma extensao numerica, quando presente e utilizavel. */
  numberExtension(key: string): number | null {
    const value = this.extensions[key];

    return typeof value === "number" ? value : null;
  }
}

/**
 * Converte qualquer falha de requisicao em um {@link ApiError}, preferindo o
 * `detail` do ProblemDetails -- e ali que a API explica o que houve.
 */
export function toApiError(error: unknown): ApiError {
  if (error instanceof ApiError) {
    return error;
  }

  if (isAxiosError<ProblemDetails>(error)) {
    // Sem resposta: a requisicao nem chegou ao servidor (API fora do ar,
    // certificado recusado, CORS). "Network Error" nao ajuda ninguem.
    if (!error.response) {
      return new ApiError(
        "Nao foi possivel falar com o servidor. Verifique se a API esta no ar."
      );
    }

    const problem = error.response.data;
    const message = problem?.detail ?? problem?.title ?? error.message;

    return new ApiError(message, error.response.status, problem ?? {});
  }

  return new ApiError("Erro inesperado ao falar com o servidor.");
}
