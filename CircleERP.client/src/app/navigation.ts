import {
  Home,
  Inventory2,
  LocationOn,
  Paid,
  People,
  ShoppingCart,
} from "@mui/icons-material";
import type { SvgIconComponent } from "@mui/icons-material";

export const RoutesPath = {
  Home: "/",
  Currency: "/currency",
  Customer: "/customer",
  Product: "/product",
  Address: "/address",
  Order: "/order",
  OrderDetail: "/order/:orderId",
} as const;

/** Monta a rota do detalhe de um pedido, para nao espalhar template de URL. */
export const orderDetailPath = (orderId: number) => `/order/${orderId}`;

export interface NavigationItem {
  label: string;
  path: string;
  icon: SvgIconComponent;
}

/** Itens do menu lateral, na ordem em que aparecem. */
export const navigationItems: readonly NavigationItem[] = [
  { label: "Início", path: RoutesPath.Home, icon: Home },
  { label: "Moedas", path: RoutesPath.Currency, icon: Paid },
  { label: "Clientes", path: RoutesPath.Customer, icon: People },
  { label: "Produtos", path: RoutesPath.Product, icon: Inventory2 },
  { label: "Endereços", path: RoutesPath.Address, icon: LocationOn },
  { label: "Pedidos", path: RoutesPath.Order, icon: ShoppingCart },
];
