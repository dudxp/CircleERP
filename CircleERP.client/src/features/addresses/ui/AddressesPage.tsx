import {
  Alert,
  Box,
  Button,
  CircularProgress,
  IconButton,
  Paper,
  Snackbar,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import { useState } from "react";
import { useNotice } from "@shared/hooks/useNotice";
import { useAddresses } from "../hooks/useAddresses";
import {
  emptyAddressForm,
  toFormValues,
  type Address,
  type AddressFormValues,
} from "../model/address";
import AddressFormFields from "./AddressFormFields";

/**
 * Aba de cadastro de enderecos.
 *
 * O endereco tem tela propria porque e um cadastro independente: pode existir
 * antes de qualquer cliente, e a tela de cliente apenas vincula um que ja
 * exista (ou cadastra pelo popup).
 */
export default function AddressesPage() {
  const { addresses, isLoading, loadError, register, change, remove } = useAddresses();
  const { notice, dismiss, run, isBusy } = useNotice();

  const [values, setValues] = useState<AddressFormValues>(emptyAddressForm);
  const [editing, setEditing] = useState<Address | undefined>();

  const startEditing = (address: Address) => {
    setEditing(address);
    setValues(toFormValues(address));
  };

  const cancelEditing = () => {
    setEditing(undefined);
    setValues(emptyAddressForm);
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const succeeded = editing
      ? await run(() => change(editing.id, values), "Endereço atualizado.")
      : await run(() => register(values), "Endereço cadastrado.");

    if (succeeded) cancelEditing();
  };

  const handleRemove = async (address: Address) => {
    if (!window.confirm(`Excluir o endereço "${address.singleLine}"?`)) return;

    await run(() => remove(address.id), "Endereço excluído.");

    if (editing?.id === address.id) cancelEditing();
  };

  return (
    <Box>
      <Typography variant="h5" sx={{ mb: 2 }}>
        Endereços
      </Typography>

      <Paper sx={{ p: 2, mb: 3 }}>
        <form onSubmit={handleSubmit}>
          <AddressFormFields values={values} onChange={setValues} disabled={isBusy} />

          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ mt: 2 }}>
            {editing && (
              <Button type="button" variant="outlined" onClick={cancelEditing}>
                Cancelar
              </Button>
            )}
            <Button type="submit" variant="contained" disabled={isBusy}>
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
                <TableCell align="right">Id</TableCell>
                <TableCell>CEP</TableCell>
                <TableCell>Endereço</TableCell>
                <TableCell align="center">Editar</TableCell>
                <TableCell align="center">Excluir</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {addresses.map((address) => (
                <TableRow key={address.id} hover selected={editing?.id === address.id}>
                  <TableCell align="right">{address.id}</TableCell>
                  <TableCell>{address.formattedZipCode}</TableCell>
                  <TableCell>{address.singleLine}</TableCell>
                  <TableCell align="center">
                    <IconButton
                      aria-label={`Editar endereço ${address.id}`}
                      onClick={() => startEditing(address)}
                    >
                      <EditIcon />
                    </IconButton>
                  </TableCell>
                  <TableCell align="center">
                    <IconButton
                      color="error"
                      aria-label={`Excluir endereço ${address.id}`}
                      disabled={isBusy}
                      onClick={() => handleRemove(address)}
                    >
                      <DeleteIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}

              {!addresses.length && (
                <TableRow>
                  <TableCell colSpan={5} align="center" sx={{ py: 4 }}>
                    Nenhum endereço cadastrado.
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
