import {
  Alert,
  Box,
  Chip,
  CircularProgress,
  IconButton,
  Paper,
  Snackbar,
  Switch,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Tooltip,
  Typography,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import { useState } from "react";
import { useNotice } from "@shared/hooks/useNotice";
import { useCustomers } from "../hooks/useCustomers";
import {
  customerTypeLabel,
  emptyCustomerForm,
  toFormValues,
  type Customer,
  type CustomerFormValues,
} from "../model/customer";
import CustomerForm from "./CustomerForm";

/**
 * Aba de cadastro de clientes.
 *
 * Nao ha exclusao: cliente com pedidos nao se apaga, se inativa -- o historico
 * precisa continuar legivel. Inativo some do seletor de novos pedidos.
 */
export default function CustomersPage() {
  const { customers, isLoading, loadError, register, change, setStatus } = useCustomers();
  const { notice, dismiss, run, isBusy } = useNotice();

  const [values, setValues] = useState<CustomerFormValues>(emptyCustomerForm);
  const [editing, setEditing] = useState<Customer | undefined>();

  const startEditing = (customer: Customer) => {
    setEditing(customer);
    setValues(toFormValues(customer));
  };

  const cancelEditing = () => {
    setEditing(undefined);
    setValues(emptyCustomerForm);
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const succeeded = editing
      ? await run(() => change(editing.id, values), "Cliente atualizado.")
      : await run(() => register(values), "Cliente cadastrado.");

    if (succeeded) cancelEditing();
  };

  const handleToggleStatus = (customer: Customer) =>
    run(
      () => setStatus(customer.id, !customer.isActive),
      customer.isActive ? "Cliente inativado." : "Cliente reativado."
    );

  return (
    <Box>
      <Typography variant="h5" sx={{ mb: 2 }}>
        Clientes
      </Typography>

      <Paper sx={{ p: 2, mb: 3 }}>
        <CustomerForm
          values={values}
          onChange={setValues}
          onSubmit={handleSubmit}
          onCancelEdit={cancelEditing}
          isEditing={Boolean(editing)}
          isBusy={isBusy}
        />
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
                <TableCell align="right">Id</TableCell>
                <TableCell>Nome</TableCell>
                <TableCell>Tipo</TableCell>
                <TableCell>Documento</TableCell>
                <TableCell>Endereço</TableCell>
                <TableCell align="center">Ativo</TableCell>
                <TableCell align="center">Editar</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {customers.map((customer) => (
                <TableRow key={customer.id} hover selected={editing?.id === customer.id}>
                  <TableCell align="right">{customer.id}</TableCell>
                  <TableCell>{customer.name}</TableCell>
                  <TableCell>
                    <Chip size="small" label={customerTypeLabel[customer.type]} />
                  </TableCell>
                  <TableCell>{customer.formattedDocument}</TableCell>
                  <TableCell>
                    {customer.address?.singleLine ?? (
                      <Typography variant="body2" color="text.secondary">
                        —
                      </Typography>
                    )}
                  </TableCell>
                  <TableCell align="center">
                    <Tooltip
                      title={
                        customer.isActive
                          ? "Inativar: deixa de aparecer em novos pedidos"
                          : "Reativar"
                      }
                    >
                      <span>
                        <Switch
                          checked={customer.isActive}
                          disabled={isBusy}
                          onChange={() => void handleToggleStatus(customer)}
                          slotProps={{
                            input: { "aria-label": `Ativo: ${customer.name}` },
                          }}
                        />
                      </span>
                    </Tooltip>
                  </TableCell>
                  <TableCell align="center">
                    <IconButton
                      aria-label={`Editar ${customer.name}`}
                      onClick={() => startEditing(customer)}
                    >
                      <EditIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}

              {!customers.length && (
                <TableRow>
                  <TableCell colSpan={7} align="center" sx={{ py: 4 }}>
                    Nenhum cliente cadastrado.
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
