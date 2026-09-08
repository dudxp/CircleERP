/**
 * Moeda como a API a devolve.
 */
export interface Currency {
  id: number;
  /** Codigo ISO 4217, sempre com tres letras maiusculas. */
  code: string;
  description: string;
  rate: number;
  /** Simbolo de exibicao ("R$"). Ausente quando nao se conhece um. */
  symbol: string | null;
}

/** Dados para cadastrar uma moeda. */
export interface RegisterCurrencyInput {
  code: string;
  description: string;
  rate: number;
  symbol: string | null;
}

/** Dados para alterar uma moeda. O codigo identifica a moeda e nao muda. */
export type ChangeCurrencyInput = Omit<RegisterCurrencyInput, "code">;
