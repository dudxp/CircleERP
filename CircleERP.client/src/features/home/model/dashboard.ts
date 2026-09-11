import type { OrderStatus } from "@features/orders";

/** Números do painel, já agregados pela API. */
export interface OrderDashboard {
  totalOrders: number;
  byStatus: { status: OrderStatus; orderCount: number }[];
  byCustomer: { customerId: number; customer: string; orderCount: number }[];
  /** Data no formato ISO (yyyy-mm-dd). */
  byDate: { date: string; orderCount: number }[];
  /**
   * Valor por moeda, nunca somado num número único: um total que junta reais
   * com dólares não significa nada.
   */
  totalsByCurrency: { currency: string; total: number; orderCount: number }[];
}

/** Como agrupar os pedidos no gráfico de participação. */
export type GroupBy = "customer" | "status" | "currency";

export const groupByLabel: Record<GroupBy, string> = {
  customer: "Cliente",
  status: "Situação",
  currency: "Moeda",
};

export interface Slice {
  label: string;
  value: number;
}

/**
 * Percentual de uma fatia, arredondado para uma casa.
 *
 * As fatias podem não somar exatamente 100% por causa do arredondamento — é
 * esperado, e melhor do que forçar o fechamento distorcendo um dos valores.
 */
export function percentageOf(value: number, total: number): string {
  return total === 0 ? "0%" : `${((value / total) * 100).toFixed(1)}%`;
}
