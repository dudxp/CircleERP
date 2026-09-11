import { httpClient } from "@shared/api/httpClient";
import { toApiError } from "@shared/api/problemDetails";
import {
  toPayload,
  type Address,
  type AddressFormValues,
  type ZipCodeLookup,
} from "../model/address";

const resource = "addresses";

async function request<T>(operation: () => Promise<T>): Promise<T> {
  try {
    return await operation();
  } catch (error) {
    throw toApiError(error);
  }
}

export const addressApi = {
  list: (signal?: AbortSignal) =>
    request(async () => {
      const { data } = await httpClient.get<Address[]>(resource, { signal });
      return data;
    }),

  /**
   * Consulta o endereco de um CEP. A API fala com o servico externo -- o front
   * continua conhecendo apenas a nossa API.
   */
  lookupZipCode: (zipCode: string, signal?: AbortSignal) =>
    request(async () => {
      const { data } = await httpClient.get<ZipCodeLookup>(
        `${resource}/lookup/${encodeURIComponent(zipCode)}`,
        { signal }
      );
      return data;
    }),

  register: (values: AddressFormValues) =>
    request(async () => {
      const { data } = await httpClient.post<number>(resource, toPayload(values));
      return data;
    }),

  change: (id: number, values: AddressFormValues) =>
    request(async () => {
      await httpClient.put(`${resource}/${id}`, toPayload(values));
    }),

  remove: (id: number) =>
    request(async () => {
      await httpClient.delete(`${resource}/${id}`);
    }),
};
