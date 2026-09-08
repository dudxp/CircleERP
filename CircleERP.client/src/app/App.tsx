import { ThemeProvider, createTheme } from "@mui/material";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import CurrencyPage from "@features/currency/ui/CurrencyPage";
import OrderPage from "@features/orders/ui/OrderPage";
import AppLayout from "./AppLayout";
import { RoutesPath } from "./navigation";

const theme = createTheme();

export default function App() {
  return (
    <ThemeProvider theme={theme}>
      <BrowserRouter>
        <AppLayout>
          <Routes>
            <Route path={RoutesPath.Home} element={<CurrencyPage />} />
            <Route path={RoutesPath.Currency} element={<CurrencyPage />} />
            <Route path={RoutesPath.Order} element={<OrderPage />} />
          </Routes>
        </AppLayout>
      </BrowserRouter>
    </ThemeProvider>
  );
}
