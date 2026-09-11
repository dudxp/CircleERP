import { httpClient } from "@shared/api/httpClient";
import { toApiError } from "@shared/api/problemDetails";
import type { OrderDashboard } from "../model/dashboard";

export const dashboardApi = {
  /** A agregacao acontece no banco; aqui so chegam os numeros prontos. */
  read: async (signal?: AbortSignal): Promise<OrderDashboard> => {
    try {
      const { data } = await httpClient.get<OrderDashboard>("orders/dashboard", { signal });
      return data;
    } catch (error) {
      throw toApiError(error);
    }
  },
};
