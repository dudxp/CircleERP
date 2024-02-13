import InboxIcon from "@mui/icons-material/MoveToInbox";
import MailIcon from "@mui/icons-material/Mail";
import { CurrencyBitcoin, Home, Paid } from "@mui/icons-material";

export enum RoutesPath {
  Home = '/',
  Currency = '/currency',
  Forbidden = '/forbidden',
}

export const itemsList = [
  { 
    name: "Inicio", 
    path: '/',
    buttonIcon: Home
  },
  { 
    name: "Moeda", 
    path: '/currency',
    buttonIcon: Paid
  }
];

export interface ICurrency {
  id: number,
  code: string,
  description: string,
  rating: number
}

export type Order = "asc" | "desc";

