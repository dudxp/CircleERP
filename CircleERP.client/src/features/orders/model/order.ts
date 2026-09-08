/** Situacao do pedido, como a API a devolve. */
export type OrderStatus = "Draft" | "Placed" | "Cancelled";

/** Pedido na listagem: sem as linhas. */
export interface OrderSummary {
  id: number;
  customer: string;
  currency: string;
  status: OrderStatus;
  total: number;
  itemCount: number;
  createdOnUtc: string;
}

export interface OrderItem {
  id: number;
  description: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

/** Pedido completo, com as linhas. */
export interface Order {
  id: number;
  customer: string;
  currency: string;
  status: OrderStatus;
  total: number;
  createdOnUtc: string;
  placedOnUtc: string | null;
  items: OrderItem[];
}

export interface OpenOrderInput {
  customer: string;
  currencyCode: string;
}

export interface AddOrderItemInput {
  description: string;
  quantity: number;
  unitPrice: number;
}

/** Rotulos em portugues para a situacao vinda da API. */
export const orderStatusLabel: Record<OrderStatus, string> = {
  Draft: "Rascunho",
  Placed: "Confirmado",
  Cancelled: "Cancelado",
};

/** Itens so podem ser alterados enquanto o pedido esta em rascunho. */
export function isEditable(order: { status: OrderStatus }): boolean {
  return order.status === "Draft";
}
