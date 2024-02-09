import { Theme, ThemeProvider, ThemeProviderProps } from "@emotion/react";
// import {  } from '@mui/material/styles/createTheme';

export enum RoutesPath {
  Home = '/',
  Currency = '/currency',
  Forbidden = '/forbidden',
}

export const itemsList = [
  { 
    name: "Inicio", 
    path: '/' 
  },
  { 
    name: "Moeda", 
    path: '/currency' 
  }
];

const bootstrap = require('bootstrap');


export type Order = "asc" | "desc";

