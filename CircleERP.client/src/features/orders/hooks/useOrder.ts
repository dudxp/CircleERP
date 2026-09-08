import { useCallback, useEffect, useState } from "react";
import { toApiError } from "@shared/api/problemDetails";
import { orderApi } from "../api/orderApi";
import type { AddOrderItemInput, Order } from "../model/order";

interface UseOrderResult {
  order: Order | null;
  isLoading: boolean;
  loadError: string | null;
  addItem: (input: AddOrderItemInput) => Promise<void>;
  changeItemQuantity: (itemId: number, quantity: number) => Promise<void>;
  removeItem: (itemId: number) => Promise<void>;
  place: () => Promise<void>;
  cancel: () => Promise<void>;
}

/**
 * Carrega um pedido e expoe as operacoes do seu ciclo.
 *
 * Toda operacao recarrega o pedido a partir do servidor em vez de ajustar o
 * estado local: o total e a situacao sao decididos pelo dominio, e reproduzir
 * esse calculo aqui seria manter duas versoes da mesma regra.
 */
export function useOrder(orderId: number): UseOrderResult {
  const [order, setOrder] = useState<Order | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [reloadToken, setReloadToken] = useState(0);

  useEffect(() => {
    const controller = new AbortController();

    setIsLoading(true);
    setLoadError(null);

    orderApi
      .getById(orderId, controller.signal)
      .then(setOrder)
      .catch((error) => {
        if (controller.signal.aborted) return;
        setLoadError(toApiError(error).message);
      })
      .finally(() => {
        if (!controller.signal.aborted) setIsLoading(false);
      });

    return () => controller.abort();
  }, [orderId, reloadToken]);

  const reload = useCallback(() => setReloadToken((token) => token + 1), []);

  const addItem = useCallback(
    async (input: AddOrderItemInput) => {
      await orderApi.addItem(orderId, input);
      reload();
    },
    [orderId, reload]
  );

  const changeItemQuantity = useCallback(
    async (itemId: number, quantity: number) => {
      await orderApi.changeItemQuantity(orderId, itemId, quantity);
      reload();
    },
    [orderId, reload]
  );

  const removeItem = useCallback(
    async (itemId: number) => {
      await orderApi.removeItem(orderId, itemId);
      reload();
    },
    [orderId, reload]
  );

  const place = useCallback(async () => {
    await orderApi.place(orderId);
    reload();
  }, [orderId, reload]);

  const cancel = useCallback(async () => {
    await orderApi.cancel(orderId);
    reload();
  }, [orderId, reload]);

  return {
    order,
    isLoading,
    loadError,
    addItem,
    changeItemQuantity,
    removeItem,
    place,
    cancel,
  };
}
