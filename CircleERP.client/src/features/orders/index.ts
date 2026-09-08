/**
 * Entrada publica da feature de pedidos.
 *
 * Outras partes do app importam daqui, e nunca de um caminho interno.
 */
export { useOrders } from "./hooks/useOrders";
export { orderStatusLabel } from "./model/order";
export type { OrderStatus, OrderSummary } from "./model/order";
