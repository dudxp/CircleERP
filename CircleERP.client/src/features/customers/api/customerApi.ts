import { httpClient } from "@shared/api/httpClient";
import { toApiError } from "@shared/api/problemDetails";
import type { Customer, CustomerFormValues } from "../model/customer";

const resource = "customers";

async function request<T>(operation: () => Promise<T>): Promise<T> {
  try {
    return await operation();
  } catch (error) {
    throw toApiError(error);
  }
}

export const customerApi = {
  list: (signal?: AbortSignal) =>
    request(async () => {
      const { data } = await httpClient.get<Customer[]>(resource, { signal });
      return data;
    }),

  register: (values: CustomerFormValues) =>
    request(async () => {
      const { data } = await httpClient.post<number>(resource, values);
      return data;
    }),

  change: (id: number, values: CustomerFormValues) =>
    request(async () => {
      await httpClient.put(`${resource}/${id}`, values);
    }),

  /** Nao ha exclusao: cliente com historico se inativa. */
  setStatus: (id: number, active: boolean) =>
    request(async () => {
      await httpClient.post(`${resource}/${id}/${active ? "activate" : "deactivate"}`);
    }),
};
