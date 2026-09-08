import { useCallback, useEffect, useState } from "react";
import { toApiError } from "@shared/api/problemDetails";
import { orderApi } from "../api/orderApi";
import type { OpenOrderInput, OrderSummary } from "../model/order";

interface UseOrdersResult {
  orders: OrderSummary[];
  isLoading: boolean;
  loadError: string | null;
  open: (input: OpenOrderInput) => Promise<number>;
}

/** Fonte unica da listagem de pedidos. */
export function useOrders(): UseOrdersResult {
  const [orders, setOrders] = useState<OrderSummary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [reloadToken, setReloadToken] = useState(0);

  useEffect(() => {
    const controller = new AbortController();

    setIsLoading(true);
    setLoadError(null);

    orderApi
      .list(controller.signal)
      .then(setOrders)
      .catch((error) => {
        if (controller.signal.aborted) return;
        setLoadError(toApiError(error).message);
      })
      .finally(() => {
        if (!controller.signal.aborted) setIsLoading(false);
      });

    return () => controller.abort();
  }, [reloadToken]);

  const open = useCallback(async (input: OpenOrderInput) => {
    const id = await orderApi.open(input);
    setReloadToken((token) => token + 1);
    return id;
  }, []);

  return { orders, isLoading, loadError, open };
}
