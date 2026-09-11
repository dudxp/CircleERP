import {
  Alert,
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
} from "@mui/material";
import { useState } from "react";
import { ApiError, toApiError } from "@shared/api/problemDetails";
import { addressApi } from "../api/addressApi";
import { emptyAddressForm, type AddressFormValues } from "../model/address";
import AddressFormFields from "./AddressFormFields";

interface Props {
  open: boolean;
  onClose: () => void;
  /** Recebe o id do endereco recem-cadastrado, para ja vincula-lo. */
  onCreated: (addressId: number) => void;
}

/**
 * Cadastro de endereco em popup, para usar de dentro da tela de cliente sem
 * perder o que ja foi digitado ali.
 *
 * Fala direto com a API em vez de usar o hook de listagem: quem abriu o popup
 * quer o id do endereco novo para vincular, e nao a lista inteira recarregada.
 */
export default function AddressDialog({ open, onClose, onCreated }: Props) {
  const [values, setValues] = useState<AddressFormValues>(emptyAddressForm);
  const [error, setError] = useState<string | null>(null);
  const [duplicateId, setDuplicateId] = useState<number | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  const close = () => {
    setValues(emptyAddressForm);
    setError(null);
    setDuplicateId(null);
    onClose();
  };

  const linkExisting = () => {
    if (duplicateId === null) return;

    onCreated(duplicateId);
    close();
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    setIsSaving(true);
    setError(null);
    setDuplicateId(null);

    try {
      const addressId = await addressApi.register(values);
      onCreated(addressId);
      close();
    } catch (caught) {
      const apiError = caught instanceof ApiError ? caught : toApiError(caught);

      setError(apiError.message);

      // A API manda o id do endereco ja cadastrado; em vez de so recusar,
      // oferecemos vincular aquele.
      setDuplicateId(apiError.numberExtension("existingAddressId"));
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <Dialog open={open} onClose={close} maxWidth="md" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>Novo endereço</DialogTitle>

        <DialogContent>
          {error && (
            <Alert
              severity={duplicateId === null ? "error" : "warning"}
              sx={{ mb: 2 }}
              action={
                duplicateId === null ? undefined : (
                  <Button color="inherit" size="small" onClick={linkExisting}>
                    Vincular o existente
                  </Button>
                )
              }
            >
              {error}
            </Alert>
          )}

          {/* pt compensa o padding do DialogContent, que corta o label
              flutuante da primeira linha de campos. */}
          <Box sx={{ pt: 1 }}>
            <AddressFormFields
              values={values}
              onChange={setValues}
              disabled={isSaving}
            />
          </Box>
        </DialogContent>

        <DialogActions>
          <Button type="button" onClick={close} disabled={isSaving}>
            Cancelar
          </Button>
          <Button type="submit" variant="contained" disabled={isSaving}>
            Cadastrar e vincular
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
}
