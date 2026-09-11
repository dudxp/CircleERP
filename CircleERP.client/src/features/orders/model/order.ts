/** Situacao do pedido, como a API a devolve. */
export type OrderStatus = "Draft" | "Placed" | "Cancelled";

/** Pedido na listagem: sem as linhas. */
export interface OrderSummary {
  id: number;
  customerId: number;
  /** Nome resolvido pela API a partir do cadastro. */
  customer: string;
  currency: string;
  status: OrderStatus;
  total: number;
  itemCount: number;
  createdOnUtc: string;
}

export interface OrderItem {
  id: number;
  productId: number;
  /** Nome do produto no momento da venda, nao o nome atual. */
  description: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

/** Pedido completo, com as linhas. */
export interface Order {
  id: number;
  customerId: number;
  /** Nome resolvido pela API a partir do cadastro. */
  customer: string;
  currency: string;
  status: OrderStatus;
  total: number;
  createdOnUtc: string;
  placedOnUtc: string | null;
  items: OrderItem[];
}

export interface OpenOrderInput {
  customerId: number;
  currencyCode: string;
}

export interface AddOrderItemInput {
  productId: number;
  quantity: number;
  /** Negociavel: o cadastro sugere, o vendedor decide. */
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
