import { useCallback, useEffect, useState } from "react";
import { currencyApi } from "../api/currencyApi";
import { toApiError } from "@shared/api/problemDetails";
import type {
  ChangeCurrencyInput,
  Currency,
  RegisterCurrencyInput,
} from "../model/currency";

interface UseCurrenciesResult {
  currencies: Currency[];
  isLoading: boolean;
  loadError: string | null;
  reload: () => void;
  register: (input: RegisterCurrencyInput) => Promise<void>;
  change: (id: number, input: ChangeCurrencyInput) => Promise<void>;
  remove: (id: number) => Promise<void>;
}

/**
 * Fonte unica da lista de moedas.
 *
 * Antes a mesma lista era buscada em dois lugares -- a pagina e a tabela cada
 * uma com seu proprio efeito -- e as duas escreviam no mesmo estado. Aqui a
 * busca acontece uma vez, e as operacoes de escrita recarregam a lista a partir
 * do servidor em vez de tentar reproduzir localmente o que o backend fez.
 */
export function useCurrencies(): UseCurrenciesResult {
  const [currencies, setCurrencies] = useState<Currency[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [reloadToken, setReloadToken] = useState(0);

  const reload = useCallback(() => setReloadToken((token) => token + 1), []);

  useEffect(() => {
    const controller = new AbortController();

    setIsLoading(true);
    setLoadError(null);

    currencyApi
      .list(controller.signal)
      .then(setCurrencies)
      .catch((error) => {
        if (controller.signal.aborted) return;
        setLoadError(toApiError(error).message);
      })
      .finally(() => {
        if (!controller.signal.aborted) setIsLoading(false);
      });

    return () => controller.abort();
  }, [reloadToken]);

  const register = useCallback(
    async (input: RegisterCurrencyInput) => {
      await currencyApi.register(input);
      reload();
    },
    [reload]
  );

  const change = useCallback(
    async (id: number, input: ChangeCurrencyInput) => {
      await currencyApi.change(id, input);
      reload();
    },
    [reload]
  );

  const remove = useCallback(
    async (id: number) => {
      await currencyApi.remove(id);
      reload();
    },
    [reload]
  );

  return { currencies, isLoading, loadError, reload, register, change, remove };
}
