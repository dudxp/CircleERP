import { httpClient } from "@shared/api/httpClient";
import { toApiError } from "@shared/api/problemDetails";
import { toPayload, type Address, type AddressFormValues } from "../model/address";

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
