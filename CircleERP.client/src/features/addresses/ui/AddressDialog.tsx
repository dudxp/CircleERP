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
import { toApiError } from "@shared/api/problemDetails";
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
  const [isSaving, setIsSaving] = useState(false);

  const close = () => {
    setValues(emptyAddressForm);
    setError(null);
    onClose();
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    setIsSaving(true);
    setError(null);

    try {
      const addressId = await addressApi.register(values);
      onCreated(addressId);
      close();
    } catch (caught) {
      setError(toApiError(caught).message);
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
            <Alert severity="error" sx={{ mb: 2 }}>
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
