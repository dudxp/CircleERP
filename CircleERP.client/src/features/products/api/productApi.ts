import { httpClient } from "@shared/api/httpClient";
import { toApiError } from "@shared/api/problemDetails";
import { toPayload, type Product, type ProductFormValues } from "../model/product";

const resource = "products";

async function request<T>(operation: () => Promise<T>): Promise<T> {
  try {
    return await operation();
  } catch (error) {
    throw toApiError(error);
  }
}

export const productApi = {
  list: (signal?: AbortSignal) =>
    request(async () => {
      const { data } = await httpClient.get<Product[]>(resource, { signal });
      return data;
    }),

  register: (values: ProductFormValues) =>
    request(async () => {
      const { data } = await httpClient.post<number>(resource, toPayload(values));
      return data;
    }),

  change: (id: number, values: ProductFormValues) =>
    request(async () => {
      await httpClient.put(`${resource}/${id}`, toPayload(values));
    }),

  /** Nao ha exclusao: produto ja vendido se inativa. */
  setStatus: (id: number, active: boolean) =>
    request(async () => {
      await httpClient.post(`${resource}/${id}/${active ? "activate" : "deactivate"}`);
    }),
};
