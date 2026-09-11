import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  IconButton,
  MenuItem,
  Paper,
  Snackbar,
  Stack,
  Switch,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import { useState } from "react";
import { useNotice } from "@shared/hooks/useNotice";
import { formatMoney } from "@shared/lib/format";
import { useCurrencies } from "@features/currency";
import { useProducts } from "../hooks/useProducts";
import {
  emptyProductForm,
  toFormValues,
  unitAbbreviation,
  unitLabel,
  type Product,
  type ProductFormValues,
  type UnitOfMeasure,
} from "../model/product";

/**
 * Aba de cadastro de produtos.
 *
 * Nao ha exclusao: produto ja vendido se inativa -- as linhas de pedido que o
 * venderam precisam seguir legiveis. Inativo some do seletor de itens.
 */
export default function ProductsPage() {
  const { products, isLoading, loadError, register, change, setStatus } = useProducts();
  const { currencies, isLoading: isLoadingCurrencies } = useCurrencies();
  const { notice, dismiss, run, isBusy } = useNotice();

  const [values, setValues] = useState<ProductFormValues>(emptyProductForm);
  const [editing, setEditing] = useState<Product | undefined>();

  const set = <K extends keyof ProductFormValues>(field: K, value: ProductFormValues[K]) =>
    setValues((current) => ({ ...current, [field]: value }));

  const startEditing = (product: Product) => {
    setEditing(product);
    setValues(toFormValues(product));
  };

  const cancelEditing = () => {
    setEditing(undefined);
    setValues(emptyProductForm);
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const succeeded = editing
      ? await run(() => change(editing.id, values), "Produto atualizado.")
      : await run(() => register(values), "Produto cadastrado.");

    if (succeeded) cancelEditing();
  };

  const hasCurrencies = currencies.length > 0;

  return (
    <Box>
      <Typography variant="h5" sx={{ mb: 2 }}>
        Produtos
      </Typography>

      <Paper sx={{ p: 2, mb: 3 }}>
        <form onSubmit={handleSubmit}>
          <Stack direction="row" spacing={2} alignItems="flex-start">
            <TextField
              label="Código"
              value={values.sku}
              onChange={(event) => set("sku", event.target.value)}
              slotProps={{ htmlInput: { maxLength: 30 } }}
              helperText="Sem espaços"
              sx={{ width: 180 }}
              disabled={isBusy}
              required
            />
            <TextField
              label="Nome"
              value={values.name}
              onChange={(event) => set("name", event.target.value)}
              slotProps={{ htmlInput: { maxLength: 150 } }}
              sx={{ flexGrow: 1 }}
              disabled={isBusy}
              required
            />
            <TextField
              select
              label="Unidade"
              value={values.unit}
              onChange={(event) => set("unit", event.target.value as UnitOfMeasure)}
              sx={{ width: 200 }}
              disabled={isBusy}
              required
            >
              {(Object.keys(unitLabel) as UnitOfMeasure[]).map((unit) => (
                <MenuItem key={unit} value={unit}>
                  {unitLabel[unit]}
                </MenuItem>
              ))}
            </TextField>
            <TextField
              label="Preço"
              type="number"
              value={values.price}
              onChange={(event) => set("price", event.target.value)}
              slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
              sx={{ width: 150 }}
              disabled={isBusy}
              required
            />
            <TextField
              select
              label="Moeda"
              value={values.currencyCode}
              onChange={(event) => set("currencyCode", event.target.value)}
              sx={{ width: 130 }}
              disabled={isBusy || isLoadingCurrencies || !hasCurrencies}
              helperText={!isLoadingCurrencies && !hasCurrencies ? "Cadastre uma moeda" : " "}
              required
            >
              {currencies.map((currency) => (
                <MenuItem key={currency.id} value={currency.code}>
                  {currency.code}
                </MenuItem>
              ))}
            </TextField>
          </Stack>

          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ mt: 2 }}>
            {editing && (
              <Button type="button" variant="outlined" onClick={cancelEditing}>
                Cancelar
              </Button>
            )}
            <Button type="submit" variant="contained" disabled={isBusy || !hasCurrencies}>
              {editing ? "Atualizar" : "Cadastrar"}
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
                <TableCell>Código</TableCell>
                <TableCell>Nome</TableCell>
                <TableCell align="center">Un.</TableCell>
                <TableCell align="right">Preço</TableCell>
                <TableCell align="center">Ativo</TableCell>
                <TableCell align="center">Editar</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {products.map((product) => (
                <TableRow key={product.id} hover selected={editing?.id === product.id}>
                  <TableCell>{product.sku}</TableCell>
                  <TableCell>{product.name}</TableCell>
                  <TableCell align="center">
                    <Chip size="small" label={unitAbbreviation[product.unit]} />
                  </TableCell>
                  <TableCell align="right">
                    {formatMoney(product.price, product.currencyCode)}
                  </TableCell>
                  <TableCell align="center">
                    <Tooltip
                      title={
                        product.isActive
                          ? "Inativar: deixa de aparecer em novos itens de pedido"
                          : "Reativar"
                      }
                    >
                      <span>
                        <Switch
                          checked={product.isActive}
                          disabled={isBusy}
                          onChange={() =>
                            void run(
                              () => setStatus(product.id, !product.isActive),
                              product.isActive ? "Produto inativado." : "Produto reativado."
                            )
                          }
                          slotProps={{ input: { "aria-label": `Ativo: ${product.name}` } }}
                        />
                      </span>
                    </Tooltip>
                  </TableCell>
                  <TableCell align="center">
                    <IconButton
                      aria-label={`Editar ${product.name}`}
                      onClick={() => startEditing(product)}
                    >
                      <EditIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}

              {!products.length && (
                <TableRow>
                  <TableCell colSpan={6} align="center" sx={{ py: 4 }}>
                    Nenhum produto cadastrado.
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
