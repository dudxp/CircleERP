import {
  Alert,
  Box,
  Card,
  CardActionArea,
  CardContent,
  CircularProgress,
  Divider,
  Paper,
  Stack,
  Typography,
} from "@mui/material";
import PaidIcon from "@mui/icons-material/Paid";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import { useNavigate } from "react-router-dom";
import { formatMoney } from "@shared/lib/format";
import { RoutesPath } from "@app/navigation";
import { useCurrencies } from "@features/currency";
import { useOrders, type OrderSummary } from "@features/orders";

/**
 * Total confirmado, agrupado por moeda.
 *
 * Deliberadamente nao devolve um numero unico: somar pedidos em moedas
 * diferentes produziria um total sem significado. E a mesma regra que o
 * `Money` do dominio impoe no backend.
 */
function placedTotalsByCurrency(orders: OrderSummary[]): [string, number][] {
  const totals = new Map<string, number>();

  for (const order of orders) {
    if (order.status !== "Placed") continue;

    totals.set(order.currency, (totals.get(order.currency) ?? 0) + order.total);
  }

  return [...totals.entries()].sort(([a], [b]) => a.localeCompare(b));
}

export default function HomePage() {
  const navigate = useNavigate();

  const {
    currencies,
    isLoading: isLoadingCurrencies,
    loadError: currenciesError,
  } = useCurrencies();

  const { orders, isLoading: isLoadingOrders, loadError: ordersError } = useOrders();

  const isLoading = isLoadingCurrencies || isLoadingOrders;
  const loadError = currenciesError ?? ordersError;

  const drafts = orders.filter((order) => order.status === "Draft").length;
  const placed = orders.filter((order) => order.status === "Placed").length;
  const totals = placedTotalsByCurrency(orders);

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
            <Stat label="Moedas cadastradas" value={currencies.length} />
            <Stat label="Pedidos" value={orders.length} />
            <Stat label="Em rascunho" value={drafts} />
            <Stat label="Confirmados" value={placed} />
          </Stack>

          <Paper sx={{ p: 2, mb: 3 }}>
            <Typography variant="subtitle1" sx={{ mb: 1 }}>
              Total confirmado por moeda
            </Typography>
            <Divider sx={{ mb: 2 }} />

            {totals.length ? (
              <Stack spacing={1}>
                {totals.map(([currency, total]) => (
                  <Stack
                    key={currency}
                    direction="row"
                    justifyContent="space-between"
                    sx={{ maxWidth: 320 }}
                  >
                    <Typography variant="body2" color="text.secondary">
                      {currency}
                    </Typography>
                    <Typography variant="body2">{formatMoney(total, currency)}</Typography>
                  </Stack>
                ))}
              </Stack>
            ) : (
              <Typography variant="body2" color="text.secondary">
                Nenhum pedido confirmado ainda.
              </Typography>
            )}
          </Paper>
        </>
      )}

      <Stack direction="row" spacing={2} flexWrap="wrap" useFlexGap>
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
