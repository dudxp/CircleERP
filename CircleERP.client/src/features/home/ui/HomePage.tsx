import {
  Alert,
  Box,
  Card,
  CardActionArea,
  CardContent,
  CircularProgress,
  Paper,
  Stack,
  Typography,
} from "@mui/material";
import PaidIcon from "@mui/icons-material/Paid";
import PeopleIcon from "@mui/icons-material/People";
import LocationOnIcon from "@mui/icons-material/LocationOn";
import Inventory2Icon from "@mui/icons-material/Inventory2";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import { useNavigate } from "react-router-dom";
import { RoutesPath } from "@app/navigation";
import { useCurrencies } from "@features/currency";
import { useOrders } from "@features/orders";
import { useCustomers } from "@features/customers";
import { useAddresses } from "@features/addresses";
import { useProducts } from "@features/products";
import { useDashboard } from "../hooks/useDashboard";
import OrdersCharts from "./OrdersCharts";

export default function HomePage() {
  const navigate = useNavigate();

  const {
    currencies,
    isLoading: isLoadingCurrencies,
    loadError: currenciesError,
  } = useCurrencies();

  const { orders, isLoading: isLoadingOrders, loadError: ordersError } = useOrders();

  const {
    customers,
    isLoading: isLoadingCustomers,
    loadError: customersError,
  } = useCustomers();

  const {
    addresses,
    isLoading: isLoadingAddresses,
    loadError: addressesError,
  } = useAddresses();

  const {
    products,
    isLoading: isLoadingProducts,
    loadError: productsError,
  } = useProducts();

  const { dashboard, isLoading: isLoadingDashboard, loadError: dashboardError } = useDashboard();

  const isLoading =
    isLoadingCurrencies ||
    isLoadingOrders ||
    isLoadingCustomers ||
    isLoadingAddresses ||
    isLoadingProducts ||
    isLoadingDashboard;

  const loadError =
    currenciesError ?? ordersError ?? customersError ?? addressesError ??
    productsError ?? dashboardError;

  const activeCustomers = customers.filter((customer) => customer.isActive).length;
  const activeProducts = products.filter((product) => product.isActive).length;
  const drafts = orders.filter((order) => order.status === "Draft").length;
  const placed = orders.filter((order) => order.status === "Placed").length;

  return (
    <Box>
      <Typography variant="h4" sx={{ mb: 1 }}>
        CircleERP
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Visão geral do sistema.
      </Typography>

      {loadError && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {loadError}
        </Alert>
      )}

      {isLoading ? (
        <Box sx={{ display: "flex", justifyContent: "center", p: 4 }}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          <Stack direction="row" spacing={2} sx={{ mb: 3 }} flexWrap="wrap" useFlexGap>
            <Stat label="Clientes ativos" value={activeCustomers} />
            <Stat label="Produtos ativos" value={activeProducts} />
            <Stat label="Moedas" value={currencies.length} />
            <Stat label="Endereços" value={addresses.length} />
            <Stat label="Pedidos" value={orders.length} />
            <Stat label="Em rascunho" value={drafts} />
            <Stat label="Confirmados" value={placed} />
          </Stack>

          {dashboard && <OrdersCharts dashboard={dashboard} />}

        </>
      )}

      <Stack direction="row" spacing={2} flexWrap="wrap" useFlexGap>
        <Shortcut
          icon={<PeopleIcon fontSize="large" />}
          title="Clientes"
          description="Cadastre os clientes e vincule o endereço de cada um."
          onClick={() => navigate(RoutesPath.Customer)}
        />
        <Shortcut
          icon={<Inventory2Icon fontSize="large" />}
          title="Produtos"
          description="Mantenha o catálogo e os preços usados nos pedidos."
          onClick={() => navigate(RoutesPath.Product)}
        />
        <Shortcut
          icon={<LocationOnIcon fontSize="large" />}
          title="Endereços"
          description="Mantenha os endereços usados pelos clientes."
          onClick={() => navigate(RoutesPath.Address)}
        />
        <Shortcut
          icon={<PaidIcon fontSize="large" />}
          title="Moedas"
          description="Cadastre as moedas e as taxas de câmbio usadas nos pedidos."
          onClick={() => navigate(RoutesPath.Currency)}
        />
        <Shortcut
          icon={<ShoppingCartIcon fontSize="large" />}
          title="Pedidos"
          description="Abra um pedido, adicione os itens e confirme."
          onClick={() => navigate(RoutesPath.Order)}
        />
      </Stack>
    </Box>
  );
}

function Stat({ label, value }: { label: string; value: number }) {
  return (
    <Paper sx={{ p: 2, minWidth: 170, flexGrow: 1 }}>
      <Typography variant="caption" color="text.secondary">
        {label}
      </Typography>
      <Typography variant="h4">{value}</Typography>
    </Paper>
  );
}

function Shortcut({
  icon,
  title,
  description,
  onClick,
}: {
  icon: React.ReactNode;
  title: string;
  description: string;
  onClick: () => void;
}) {
  return (
    <Card sx={{ minWidth: 260, flexGrow: 1 }}>
      <CardActionArea onClick={onClick}>
        <CardContent>
          <Stack direction="row" spacing={2} alignItems="center">
            <Box sx={{ color: "primary.main", display: "flex" }}>{icon}</Box>
            <Box>
              <Typography variant="h6">{title}</Typography>
              <Typography variant="body2" color="text.secondary">
                {description}
              </Typography>
            </Box>
          </Stack>
        </CardContent>
      </CardActionArea>
    </Card>
  );
}
