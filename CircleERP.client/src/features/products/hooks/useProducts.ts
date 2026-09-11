import { useCallback, useEffect, useState } from "react";
import { toApiError } from "@shared/api/problemDetails";
import { productApi } from "../api/productApi";
import type { Product, ProductFormValues } from "../model/product";

interface UseProductsResult {
  products: Product[];
  isLoading: boolean;
  loadError: string | null;
  register: (values: ProductFormValues) => Promise<number>;
  change: (id: number, values: ProductFormValues) => Promise<void>;
  setStatus: (id: number, active: boolean) => Promise<void>;
}

/** Fonte unica da lista de produtos. */
export function useProducts(): UseProductsResult {
  const [products, setProducts] = useState<Product[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [reloadToken, setReloadToken] = useState(0);

  const reload = useCallback(() => setReloadToken((token) => token + 1), []);

  useEffect(() => {
    const controller = new AbortController();

    setIsLoading(true);
    setLoadError(null);

    productApi
      .list(controller.signal)
      .then(setProducts)
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
    async (values: ProductFormValues) => {
      const id = await productApi.register(values);
      reload();
      return id;
    },
    [reload]
  );

  const change = useCallback(
    async (id: number, values: ProductFormValues) => {
      await productApi.change(id, values);
      reload();
    },
    [reload]
  );

  const setStatus = useCallback(
    async (id: number, active: boolean) => {
      await productApi.setStatus(id, active);
      reload();
    },
    [reload]
  );

  return { products, isLoading, loadError, register, change, setStatus };
}
