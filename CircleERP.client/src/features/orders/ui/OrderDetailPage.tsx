import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  IconButton,
  Paper,
  Snackbar,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableFooter,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import DeleteIcon from "@mui/icons-material/Delete";
import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { formatDateTime, formatMoney } from "@shared/lib/format";
import { RoutesPath } from "@app/navigation";
import { useOrder } from "../hooks/useOrder";
import { useNotice } from "../hooks/useNotice";
import { isEditable, orderStatusLabel, type OrderStatus } from "../model/order";

const statusColor: Record<OrderStatus, "default" | "success" | "error"> = {
  Draft: "default",
  Placed: "success",
  Cancelled: "error",
};

/**
 * Detalhe do pedido: itens, total e as acoes do ciclo.
 *
 * A tela desabilita o que o dominio recusaria, mas nao e ela quem decide -- se
 * a regra mudar no backend, o pior que acontece aqui e um botao habilitado a
 * mais, e a API responde 400. A alternativa seria reimplementar as invariantes
 * em TypeScript e mante-las em sincronia na mao.
 */
export default function OrderDetailPage() {
  const { orderId } = useParams();
  const navigate = useNavigate();

  const {
    order,
    isLoading,
    loadError,
    addItem,
    changeItemQuantity,
    removeItem,
    place,
    cancel,
  } = useOrder(Number(orderId));

  const { notice, dismiss, run, isBusy } = useNotice();

  const [description, setDescription] = useState("");
  const [quantity, setQuantity] = useState("1");
  const [unitPrice, setUnitPrice] = useState("0");

  const handleAddItem = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const succeeded = await run(
      () =>
        addItem({
          description,
          quantity: Number(quantity),
          unitPrice: Number(unitPrice),
        }),
      "Item adicionado."
    );

    if (succeeded) {
      setDescription("");
      setQuantity("1");
      setUnitPrice("0");
    }
  };

  if (isLoading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (loadError || !order) {
    return (
      <Box>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(RoutesPath.Order)}>
          Voltar
        </Button>
        <Alert severity="error" sx={{ mt: 2 }}>
          {loadError ?? "Pedido não encontrado."}
        </Alert>
      </Box>
    );
  }

  const editable = isEditable(order);

  return (
    <Box>
      <Stack direction="row" spacing={2} alignItems="center" sx={{ mb: 2 }}>
        <IconButton aria-label="Voltar" onClick={() => navigate(RoutesPath.Order)}>
          <ArrowBackIcon />
        </IconButton>
        <Typography variant="h5">Pedido {order.id}</Typography>
        <Chip
          size="small"
          label={orderStatusLabel[order.status]}
          color={statusColor[order.status]}
        />
      </Stack>

      <Paper sx={{ p: 2, mb: 3 }}>
        <Stack direction="row" spacing={4} flexWrap="wrap">
          <Info label="Cliente" value={order.customer} />
          <Info label="Moeda" value={order.currency} />
          <Info label="Aberto em" value={formatDateTime(order.createdOnUtc)} />
          {order.placedOnUtc && (
            <Info label="Confirmado em" value={formatDateTime(order.placedOnUtc)} />
          )}
        </Stack>
      </Paper>

      {editable && (
        <Paper sx={{ p: 2, mb: 3 }}>
          <form onSubmit={handleAddItem}>
            <Stack direction="row" spacing={2} alignItems="flex-start">
              <TextField
                label="Descrição do item"
                value={description}
                onChange={(event) => setDescription(event.target.value)}
                slotProps={{ htmlInput: { maxLength: 200 } }}
                sx={{ flexGrow: 1 }}
                required
              />
              <TextField
                label="Quantidade"
                type="number"
                value={quantity}
                onChange={(event) => setQuantity(event.target.value)}
                slotProps={{ htmlInput: { min: 1, step: 1 } }}
                sx={{ width: 140 }}
                required
              />
              <TextField
                label={`Preço unitário (${order.currency})`}
                type="number"
                value={unitPrice}
                onChange={(event) => setUnitPrice(event.target.value)}
                slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                sx={{ width: 200 }}
                required
              />
              <Button
                type="submit"
                variant="contained"
                disabled={isBusy}
                sx={{ height: "56px" }}
              >
                Adicionar
              </Button>
            </Stack>
          </form>
        </Paper>
      )}

      <Paper sx={{ mb: 3 }}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Descrição</TableCell>
              <TableCell align="right">Quantidade</TableCell>
              <TableCell align="right">Preço unitário</TableCell>
              <TableCell align="right">Total da linha</TableCell>
              {editable && <TableCell align="center">Remover</TableCell>}
            </TableRow>
          </TableHead>
          <TableBody>
            {order.items.map((item) => (
              <TableRow key={item.id} hover>
                <TableCell>{item.description}</TableCell>
                <TableCell align="right">
                  {editable ? (
                    <TextField
                      type="number"
                      size="small"
                      defaultValue={item.quantity}
                      slotProps={{ htmlInput: { min: 1, step: 1 } }}
                      sx={{ width: 90 }}
                      // Confirma no blur: alterar a cada tecla dispararia uma
                      // requisicao por digito.
                      onBlur={(event) => {
                        const value = Number(event.target.value);
                        if (value !== item.quantity) {
                          void run(
                            () => changeItemQuantity(item.id, value),
                            "Quantidade atualizada."
                          );
                        }
                      }}
                    />
                  ) : (
                    item.quantity
                  )}
                </TableCell>
                <TableCell align="right">
                  {formatMoney(item.unitPrice, order.currency)}
                </TableCell>
                <TableCell align="right">
                  {formatMoney(item.lineTotal, order.currency)}
                </TableCell>
                {editable && (
                  <TableCell align="center">
                    <IconButton
                      color="error"
                      aria-label={`Remover ${item.description}`}
                      disabled={isBusy}
                      onClick={() =>
                        void run(() => removeItem(item.id), "Item removido.")
                      }
                    >
                      <DeleteIcon />
                    </IconButton>
                  </TableCell>
                )}
              </TableRow>
            ))}

            {!order.items.length && (
              <TableRow>
                <TableCell colSpan={editable ? 5 : 4} align="center" sx={{ py: 4 }}>
                  Nenhum item. Um pedido precisa de pelo menos um para ser confirmado.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
          <TableFooter>
            <TableRow>
              <TableCell colSpan={3} align="right">
                <Typography variant="subtitle1">Total</Typography>
              </TableCell>
              <TableCell align="right">
                <Typography variant="subtitle1">
                  {formatMoney(order.total, order.currency)}
                </Typography>
              </TableCell>
              {editable && <TableCell />}
            </TableRow>
          </TableFooter>
        </Table>
      </Paper>

      <Stack direction="row" spacing={2} justifyContent="flex-end">
        {order.status !== "Cancelled" && (
          <Button
            variant="outlined"
            color="error"
            disabled={isBusy}
            onClick={() => {
              if (window.confirm(`Cancelar o pedido ${order.id}?`)) {
                void run(cancel, "Pedido cancelado.");
              }
            }}
          >
            Cancelar pedido
          </Button>
        )}

        {editable && (
          <Button
            variant="contained"
            disabled={isBusy || order.items.length === 0}
            onClick={() => void run(place, "Pedido confirmado.")}
          >
            Confirmar pedido
          </Button>
        )}
      </Stack>

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

function Info({ label, value }: { label: string; value: string }) {
  return (
    <Box>
      <Typography variant="caption" color="text.secondary">
        {label}
      </Typography>
      <Typography variant="body1">{value}</Typography>
    </Box>
  );
}
