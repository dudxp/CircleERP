import {
  Button,
  MenuItem,
  Stack,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import AddLocationAltIcon from "@mui/icons-material/AddLocationAlt";
import { useState } from "react";
import { AddressDialog, useAddresses } from "@features/addresses";
import {
  customerTypeLabel,
  documentLength,
  type CustomerFormValues,
  type CustomerType,
} from "../model/customer";

interface Props {
  values: CustomerFormValues;
  onChange: (values: CustomerFormValues) => void;
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  onCancelEdit: () => void;
  isEditing: boolean;
  isBusy: boolean;
}

/**
 * Formulario de cliente, com o endereco escolhido entre os ja cadastrados.
 *
 * O botao ao lado do seletor abre o cadastro de endereco em popup e vincula o
 * recem-criado, para que ninguem precise sair da tela no meio do preenchimento
 * e perder o que ja digitou.
 */
export default function CustomerForm({
  values,
  onChange,
  onSubmit,
  onCancelEdit,
  isEditing,
  isBusy,
}: Props) {
  const { addresses, reload: reloadAddresses } = useAddresses();
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const set = <K extends keyof CustomerFormValues>(
    field: K,
    value: CustomerFormValues[K]
  ) => onChange({ ...values, [field]: value });

  const handleAddressCreated = (addressId: number) => {
    // A lista local precisa conhecer o endereco novo para conseguir exibi-lo
    // como selecionado.
    reloadAddresses();
    set("addressId", addressId);
  };

  return (
    <form onSubmit={onSubmit}>
      <Stack spacing={2}>
        <Stack direction="row" spacing={2}>
          <TextField
            label="Nome / Razão social"
            value={values.name}
            onChange={(event) => set("name", event.target.value)}
            slotProps={{ htmlInput: { maxLength: 120 } }}
            sx={{ flexGrow: 1 }}
            disabled={isBusy}
            required
          />

          <TextField
            select
            label="Tipo"
            value={values.type}
            onChange={(event) => set("type", event.target.value as CustomerType)}
            sx={{ width: 200 }}
            disabled={isBusy}
            required
          >
            {(Object.keys(customerTypeLabel) as CustomerType[]).map((type) => (
              <MenuItem key={type} value={type}>
                {customerTypeLabel[type]}
              </MenuItem>
            ))}
          </TextField>

          <TextField
            label={values.type === "Individual" ? "CPF" : "CNPJ"}
            value={values.document}
            onChange={(event) => set("document", event.target.value)}
            // Espaco para a pontuacao, que o servidor descarta na gravacao.
            slotProps={{ htmlInput: { maxLength: 18 } }}
            helperText={`${documentLength[values.type]} dígitos`}
            sx={{ width: 220 }}
            disabled={isBusy}
            required
          />
        </Stack>

        <Stack direction="row" spacing={1} alignItems="flex-start">
          <TextField
            select
            label="Endereço"
            value={values.addressId ?? ""}
            onChange={(event) =>
              set("addressId", event.target.value === "" ? null : Number(event.target.value))
            }
            sx={{ flexGrow: 1 }}
            disabled={isBusy}
            helperText="Opcional"
          >
            <MenuItem value="">
              <Typography color="text.secondary">Sem endereço</Typography>
            </MenuItem>
            {addresses.map((address) => (
              <MenuItem key={address.id} value={address.id}>
                {address.singleLine}
              </MenuItem>
            ))}
          </TextField>

          <Tooltip title="Cadastrar um endereço novo sem sair desta tela">
            <span>
              <Button
                type="button"
                variant="outlined"
                startIcon={<AddLocationAltIcon />}
                onClick={() => setIsDialogOpen(true)}
                disabled={isBusy}
                sx={{ height: "56px", whiteSpace: "nowrap" }}
              >
                Novo endereço
              </Button>
            </span>
          </Tooltip>
        </Stack>

        <Stack direction="row" spacing={2} justifyContent="flex-end">
          {isEditing && (
            <Button type="button" variant="outlined" onClick={onCancelEdit}>
              Cancelar
            </Button>
          )}
          <Button type="submit" variant="contained" disabled={isBusy}>
            {isEditing ? "Atualizar" : "Cadastrar"}
          </Button>
        </Stack>
      </Stack>

      <AddressDialog
        open={isDialogOpen}
        onClose={() => setIsDialogOpen(false)}
        onCreated={handleAddressCreated}
      />
    </form>
  );
}
