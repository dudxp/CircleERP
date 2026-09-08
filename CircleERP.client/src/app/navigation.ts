import { Home, Paid, ShoppingCart } from "@mui/icons-material";
import type { SvgIconComponent } from "@mui/icons-material";

export const RoutesPath = {
  Home: "/",
  Currency: "/currency",
  Order: "/order",
} as const;

export interface NavigationItem {
  label: string;
  path: string;
  icon: SvgIconComponent;
}

/** Itens do menu lateral, na ordem em que aparecem. */
export const navigationItems: readonly NavigationItem[] = [
  { label: "Início", path: RoutesPath.Home, icon: Home },
  { label: "Moedas", path: RoutesPath.Currency, icon: Paid },
  { label: "Pedidos", path: RoutesPath.Order, icon: ShoppingCart },
];
