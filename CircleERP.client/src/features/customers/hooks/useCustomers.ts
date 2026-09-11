import { useCallback, useEffect, useState } from "react";
import { toApiError } from "@shared/api/problemDetails";
import { customerApi } from "../api/customerApi";
import type { Customer, CustomerFormValues } from "../model/customer";

interface UseCustomersResult {
  customers: Customer[];
  isLoading: boolean;
  loadError: string | null;
  reload: () => void;
  register: (values: CustomerFormValues) => Promise<number>;
  change: (id: number, values: CustomerFormValues) => Promise<void>;
  setStatus: (id: number, active: boolean) => Promise<void>;
}

/** Fonte unica da lista de clientes. */
export function useCustomers(): UseCustomersResult {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [reloadToken, setReloadToken] = useState(0);

  const reload = useCallback(() => setReloadToken((token) => token + 1), []);

  useEffect(() => {
    const controller = new AbortController();

    setIsLoading(true);
    setLoadError(null);

    customerApi
      .list(controller.signal)
      .then(setCustomers)
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
    async (values: CustomerFormValues) => {
      const id = await customerApi.register(values);
      reload();
      return id;
    },
    [reload]
  );

  const change = useCallback(
    async (id: number, values: CustomerFormValues) => {
      await customerApi.change(id, values);
      reload();
    },
    [reload]
  );

  const setStatus = useCallback(
    async (id: number, active: boolean) => {
      await customerApi.setStatus(id, active);
      reload();
    },
    [reload]
  );

  return { customers, isLoading, loadError, reload, register, change, setStatus };
}
