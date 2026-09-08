import { Button, Stack, TextField, styled } from "@mui/material";
import { useEffect, useState } from "react";
import type { Currency } from "../model/currency";
import style from "./CurrencyForm.module.scss";

const Field = styled(TextField)(() => ({ margin: "10px" }));

interface Props {
  /** Quando presente, o formulario esta editando essa moeda. */
  editing?: Currency;
  onCancelEdit: () => void;
  onSubmit: (values: {
    code: string;
    description: string;
    rate: number;
    symbol: string | null;
  }) => void;
  isSubmitting: boolean;
}

export default function CurrencyForm({
  editing,
  onCancelEdit,
  onSubmit,
  isSubmitting,
}: Props) {
  const [code, setCode] = useState("");
  const [description, setDescription] = useState("");
  const [rate, setRate] = useState("0");
  const [symbol, setSymbol] = useState("");

  useEffect(() => {
    setCode(editing?.code ?? "");
    setDescription(editing?.description ?? "");
    setRate(String(editing?.rate ?? 0));
    setSymbol(editing?.symbol ?? "");
  }, [editing]);

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    onSubmit({
      code,
      description,
      rate: Number(rate),
      // String vazia e ausencia de simbolo, nao um simbolo vazio.
      symbol: symbol.trim() || null,
    });
  };

  return (
    <form onSubmit={handleSubmit}>
      <Stack direction="row" spacing={2}>
        <Field
          label="Código"
          variant="outlined"
          value={code}
          onChange={(event) => setCode(event.target.value)}
          slotProps={{ htmlInput: { maxLength: 3 } }}
          helperText="3 letras (ISO 4217)"
          // O codigo identifica a moeda: alterar seria cadastrar outra.
          disabled={Boolean(editing)}
          required
        />
        <Field
          label="Símbolo"
          variant="outlined"
          value={symbol}
          onChange={(event) => setSymbol(event.target.value)}
          slotProps={{ htmlInput: { maxLength: 5 } }}
          helperText="Opcional (ex.: R$)"
        />
        <Field
          label="Descrição"
          variant="outlined"
          sx={{ width: "100%" }}
          value={description}
          onChange={(event) => setDescription(event.target.value)}
          slotProps={{ htmlInput: { maxLength: 100 } }}
          required
        />
        <Field
          label="Taxa de câmbio"
          variant="outlined"
          type="number"
          value={rate}
          onChange={(event) => setRate(event.target.value)}
          slotProps={{ htmlInput: { step: "0.000001", min: "0" } }}
          required
        />
      </Stack>

      <div className={style.actions}>
        <Button
          type="submit"
          variant="contained"
          disabled={isSubmitting}
          sx={{ height: "40px", margin: "10px" }}
        >
          {editing ? "Atualizar" : "Cadastrar"}
        </Button>

        {editing && (
          <Button
            type="button"
            variant="outlined"
            onClick={onCancelEdit}
            sx={{ height: "40px", margin: "10px" }}
          >
            Cancelar
          </Button>
        )}
      </div>
    </form>
  );
}
