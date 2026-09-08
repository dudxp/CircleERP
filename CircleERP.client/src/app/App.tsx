import { ThemeProvider, createTheme } from "@mui/material";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import HomePage from "@features/home/ui/HomePage";
import CurrencyPage from "@features/currency/ui/CurrencyPage";
import OrdersPage from "@features/orders/ui/OrdersPage";
import OrderDetailPage from "@features/orders/ui/OrderDetailPage";
import AppLayout from "./AppLayout";
import { RoutesPath } from "./navigation";

const theme = createTheme();

export default function App() {
  return (
    <ThemeProvider theme={theme}>
      <BrowserRouter>
        <AppLayout>
          <Routes>
            <Route path={RoutesPath.Home} element={<HomePage />} />
            <Route path={RoutesPath.Currency} element={<CurrencyPage />} />
            <Route path={RoutesPath.Order} element={<OrdersPage />} />
            <Route path={RoutesPath.OrderDetail} element={<OrderDetailPage />} />
          </Routes>
        </AppLayout>
      </BrowserRouter>
    </ThemeProvider>
  );
}
