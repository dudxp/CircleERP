import { httpClient } from "@shared/api/httpClient";
import { toApiError } from "@shared/api/problemDetails";
import type {
  ChangeCurrencyInput,
  Currency,
  RegisterCurrencyInput,
} from "../model/currency";

const resource = "currencies";

/**
 * Unico modulo que conhece as rotas de moeda. Traduz falhas de rede e respostas
 * de erro em ApiError, para que os componentes nunca precisem inspecionar
 * status HTTP.
 */
export const currencyApi = {
  async list(signal?: AbortSignal): Promise<Currency[]> {
    try {
      const { data } = await httpClient.get<Currency[]>(resource, { signal });
      return data;
    } catch (error) {
      throw toApiError(error);
    }
  },

  async register(input: RegisterCurrencyInput): Promise<number> {
    try {
      const { data } = await httpClient.post<number>(resource, input);
      return data;
    } catch (error) {
      throw toApiError(error);
    }
  },

  async change(id: number, input: ChangeCurrencyInput): Promise<void> {
    try {
      await httpClient.put(`${resource}/${id}`, input);
    } catch (error) {
      throw toApiError(error);
    }
  },

  async remove(id: number): Promise<void> {
    try {
      await httpClient.delete(`${resource}/${id}`);
    } catch (error) {
      throw toApiError(error);
    }
  },
};
