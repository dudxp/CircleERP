import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  MenuItem,
  Paper,
  Snackbar,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { formatDateTime, formatMoney } from "@shared/lib/format";
import { useCurrencies } from "@features/currency";
import { useCustomers } from "@features/customers";
import { useOrders } from "../hooks/useOrders";
import { useNotice } from "@shared/hooks/useNotice";
import { orderDetailPath } from "@app/navigation";
import { orderStatusLabel, type OrderStatus } from "../model/order";

const statusColor: Record<OrderStatus, "default" | "success" | "error"> = {
  Draft: "default",
  Placed: "success",
  Cancelled: "error",
};

/**
 * Lista de pedidos e abertura de um novo.
 *
 * O pedido nasce vazio: os itens sao adicionados na tela de detalhe. Isso
 * espelha o dominio, onde um pedido em rascunho sem itens e um estado valido --
 * o que nao se pode e confirmar um assim.
 */
export default function OrdersPage() {
  const navigate = useNavigate();
  // Uma unica chamada: dois usos do hook seriam duas buscas da mesma lista.
  const { orders, isLoading, loadError, open } = useOrders();
  const { currencies, isLoading: isLoadingCurrencies } = useCurrencies();
  const { customers, isLoading: isLoadingCustomers } = useCustomers();
  const { notice, dismiss, run, isBusy } = useNotice();

  const [customerId, setCustomerId] = useState<number | "">("");
  const [currencyCode, setCurrencyCode] = useState("");

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    let openedId: number | null = null;

    const succeeded = await run(async () => {
      openedId = await open({ customerId: Number(customerId), currencyCode });
    }, "Pedido aberto. Adicione os itens.");

    if (succeeded && openedId !== null) {
      navigate(orderDetailPath(openedId));
    }
  };

  const hasCurrencies = currencies.length > 0;

  // Cliente inativo nao recebe pedido novo -- a API recusaria com 409, entao a
  // tela nem o oferece. Quem decide continua sendo o servidor.
  const activeCustomers = customers.filter((customer) => customer.isActive);
  const hasCustomers = activeCustomers.length > 0;
  const canOpen = hasCurrencies && hasCustomers;

  return (
    <Box>
      <Typography variant="h5" sx={{ mb: 2 }}>
        Pedidos
      </Typography>

      <Paper sx={{ p: 2, mb: 3 }}>
        <form onSubmit={handleSubmit}>
          <Stack direction="row" spacing={2} alignItems="flex-start">
            <TextField
              select
              label="Cliente"
              value={customerId}
              onChange={(event) => setCustomerId(Number(event.target.value))}
              sx={{ flexGrow: 1 }}
              disabled={isLoadingCustomers || !hasCustomers}
              helperText={
                !isLoadingCustomers && !hasCustomers
                  ? "Cadastre um cliente ativo primeiro"
                  : " "
              }
              required
            >
              {activeCustomers.map((customer) => (
                <MenuItem key={customer.id} value={customer.id}>
                  {customer.name} — {customer.formattedDocument}
                </MenuItem>
              ))}
            </TextField>

            <TextField
              select
              label="Moeda"
              value={currencyCode}
              onChange={(event) => setCurrencyCode(event.target.value)}
              sx={{ minWidth: 180 }}
              disabled={isLoadingCurrencies || !hasCurrencies}
              helperText={
                !isLoadingCurrencies && !hasCurrencies
                  ? "Cadastre uma moeda primeiro"
                  : " "
              }
              required
            >
              {currencies.map((currency) => (
                <MenuItem key={currency.id} value={currency.code}>
                  {currency.symbol
                    ? `${currency.code} (${currency.symbol})`
                    : currency.code}
                </MenuItem>
              ))}
            </TextField>

            <Button
              type="submit"
              variant="contained"
              disabled={isBusy || !canOpen}
              sx={{ height: "56px" }}
            >
              Abrir pedido
            </Button>
          </Stack>
        </form>
      </Paper>

      {loadError ? (
        <Alert severity="error">{loadError}</Alert>
      ) : isLoading ? (
        <Box sx={{ display: "flex", justifyContent: "center", p: 4 }}>
          <CircularProgress />
        </Box>
      ) : (
        <Paper>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell align="right">Nº</TableCell>
                <TableCell>Cliente</TableCell>
                <TableCell>Situação</TableCell>
                <TableCell align="right">Itens</TableCell>
                <TableCell align="right">Total</TableCell>
                <TableCell>Aberto em</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {orders.map((order) => (
                <TableRow
                  key={order.id}
                  hover
                  sx={{ cursor: "pointer" }}
                  onClick={() => navigate(orderDetailPath(order.id))}
                >
                  <TableCell align="right">{order.id}</TableCell>
                  <TableCell>{order.customer}</TableCell>
                  <TableCell>
                    <Chip
                      size="small"
                      label={orderStatusLabel[order.status]}
                      color={statusColor[order.status]}
                    />
                  </TableCell>
                  <TableCell align="right">{order.itemCount}</TableCell>
                  <TableCell align="right">
                    {formatMoney(order.total, order.currency)}
                  </TableCell>
                  <TableCell>{formatDateTime(order.createdOnUtc)}</TableCell>
                </TableRow>
              ))}

              {!orders.length && (
                <TableRow>
                  <TableCell colSpan={6} align="center" sx={{ py: 4 }}>
                    Nenhum pedido cadastrado.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </Paper>
      )}

      <Snackbar
        open={notice !== null}
        autoHideDuration={5000}
        onClose={dismiss}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert severity={notice?.severity} onClose={dismiss}>
          {notice?.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}
