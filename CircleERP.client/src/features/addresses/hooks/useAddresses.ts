import { useCallback, useEffect, useState } from "react";
import { toApiError } from "@shared/api/problemDetails";
import { addressApi } from "../api/addressApi";
import type { Address, AddressFormValues } from "../model/address";

interface UseAddressesResult {
  addresses: Address[];
  isLoading: boolean;
  loadError: string | null;
  reload: () => void;
  register: (values: AddressFormValues) => Promise<number>;
  change: (id: number, values: AddressFormValues) => Promise<void>;
  remove: (id: number) => Promise<void>;
}

/** Fonte unica da lista de enderecos. */
export function useAddresses(): UseAddressesResult {
  const [addresses, setAddresses] = useState<Address[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [reloadToken, setReloadToken] = useState(0);

  const reload = useCallback(() => setReloadToken((token) => token + 1), []);

  useEffect(() => {
    const controller = new AbortController();

    setIsLoading(true);
    setLoadError(null);

    addressApi
      .list(controller.signal)
      .then(setAddresses)
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
    async (values: AddressFormValues) => {
      const id = await addressApi.register(values);
      reload();
      return id;
    },
    [reload]
  );

  const change = useCallback(
    async (id: number, values: AddressFormValues) => {
      await addressApi.change(id, values);
      reload();
    },
    [reload]
  );

  const remove = useCallback(
    async (id: number) => {
      await addressApi.remove(id);
      reload();
    },
    [reload]
  );

  return { addresses, isLoading, loadError, reload, register, change, remove };
}
