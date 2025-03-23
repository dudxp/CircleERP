import { Home, Paid } from "@mui/icons-material";
import axios from "axios";

export enum RoutesPath {
  Home = "/",
  Currency = "/currency",
  Forbidden = "/forbidden",
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
