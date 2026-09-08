import { httpClient } from "@shared/api/httpClient";
import { toApiError } from "@shared/api/problemDetails";
import type {
  AddOrderItemInput,
  OpenOrderInput,
  Order,
  OrderSummary,
} from "../model/order";

const resource = "orders";

async function request<T>(operation: () => Promise<T>): Promise<T> {
  try {
    return await operation();
  } catch (error) {
    throw toApiError(error);
  }
}

/** Unico modulo que conhece as rotas de pedido. */
export const orderApi = {
  list: (signal?: AbortSignal) =>
    request(async () => {
      const { data } = await httpClient.get<OrderSummary[]>(resource, { signal });
      return data;
    }),

  getById: (id: number, signal?: AbortSignal) =>
    request(async () => {
      const { data } = await httpClient.get<Order>(`${resource}/${id}`, { signal });
      return data;
    }),

  open: (input: OpenOrderInput) =>
    request(async () => {
      const { data } = await httpClient.post<number>(resource, input);
      return data;
    }),

  addItem: (orderId: number, input: AddOrderItemInput) =>
    request(async () => {
      const { data } = await httpClient.post<number>(`${resource}/${orderId}/items`, input);
      return data;
    }),

  changeItemQuantity: (orderId: number, itemId: number, quantity: number) =>
    request(async () => {
      await httpClient.put(`${resource}/${orderId}/items/${itemId}`, { quantity });
    }),

  removeItem: (orderId: number, itemId: number) =>
    request(async () => {
      await httpClient.delete(`${resource}/${orderId}/items/${itemId}`);
    }),

  /** Confirma o pedido. Acao, nao alteracao de campo -- por isso e um POST. */
  place: (orderId: number) =>
    request(async () => {
      await httpClient.post(`${resource}/${orderId}/place`);
    }),

  cancel: (orderId: number) =>
    request(async () => {
      await httpClient.post(`${resource}/${orderId}/cancel`);
    }),
};
