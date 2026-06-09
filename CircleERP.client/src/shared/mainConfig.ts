import { Home, Paid } from "@mui/icons-material";
import axios from "axios";
// import OrderIcon from "../assets/Icons/order.svg";

export enum RoutesPath {
  Home = "/",
  Currency = "/currency",
  Forbidden = "/forbidden",
  Order = "/order",
}

export const itemsList = [
  {
    name: "Inicio",
    path: "/",
    buttonIcon: Home,
  },
  {
    name: "Currency",
    path: "/currency",
    buttonIcon: Paid,
  },
  {
    name: "Order",
    path: "/order",
    buttonIcon: 'OrderIcon',
  }
];

export interface ICurrency {
  id: number;
  code: string;
  description: string;
  rating: number;
}

export type Order = "asc" | "desc";

export const axiosV2 = axios.create({
  
  baseURL: `${__BACKEND_HOST__}:${__BACKEND_PORT__}/api/`
});
