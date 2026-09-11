import { MenuItem, Stack, TextField } from "@mui/material";
import { brazilianStates, type AddressFormValues } from "../model/address";

interface Props {
  values: AddressFormValues;
  onChange: (values: AddressFormValues) => void;
  disabled?: boolean;
}

/**
 * Campos de um endereco, sem formulario nem botoes em volta.
 *
 * Existe separado justamente para servir aos dois lugares que cadastram
 * endereco: a aba propria e o popup dentro da tela de cliente. Uma copia em
 * cada lugar viraria duas telas divergindo com o tempo.
 */
export default function AddressFormFields({ values, onChange, disabled }: Props) {
  const set = (field: keyof AddressFormValues) => (value: string) =>
    onChange({ ...values, [field]: value });

  return (
    <Stack spacing={2}>
      <Stack direction="row" spacing={2}>
        <TextField
          label="CEP"
          value={values.zipCode}
          onChange={(event) => set("zipCode")(event.target.value)}
          slotProps={{ htmlInput: { maxLength: 9 } }}
          helperText="8 dígitos"
          sx={{ width: 160 }}
          disabled={disabled}
          required
        />
        <TextField
          label="Logradouro"
          value={values.street}
          onChange={(event) => set("street")(event.target.value)}
          slotProps={{ htmlInput: { maxLength: 150 } }}
          sx={{ flexGrow: 1 }}
          disabled={disabled}
          required
        />
        <TextField
          label="Número"
          value={values.number}
          onChange={(event) => set("number")(event.target.value)}
          slotProps={{ htmlInput: { maxLength: 20 } }}
          helperText="Aceita S/N"
          sx={{ width: 140 }}
          disabled={disabled}
          required
        />
      </Stack>

      <Stack direction="row" spacing={2}>
        <TextField
          label="Complemento"
          value={values.complement}
          onChange={(event) => set("complement")(event.target.value)}
          slotProps={{ htmlInput: { maxLength: 60 } }}
          helperText="Opcional"
          sx={{ width: 220 }}
          disabled={disabled}
        />
        <TextField
          label="Bairro"
          value={values.district}
          onChange={(event) => set("district")(event.target.value)}
          slotProps={{ htmlInput: { maxLength: 80 } }}
          sx={{ flexGrow: 1 }}
          disabled={disabled}
          required
        />
        <TextField
          label="Cidade"
          value={values.city}
          onChange={(event) => set("city")(event.target.value)}
          slotProps={{ htmlInput: { maxLength: 80 } }}
          sx={{ flexGrow: 1 }}
          disabled={disabled}
          required
        />
        <TextField
          select
          label="UF"
          value={values.state}
          onChange={(event) => set("state")(event.target.value)}
          sx={{ width: 110 }}
          disabled={disabled}
          required
        >
          {brazilianStates.map((state) => (
            <MenuItem key={state} value={state}>
              {state}
            </MenuItem>
          ))}
        </TextField>
      </Stack>
    </Stack>
  );
}
