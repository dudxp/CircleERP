// import { bootstrap } from 'shared';
import {
  Button,
  FormControl,
  Input,
  InputLabel,
  Stack,
  TextField,
  styled,
} from "@mui/material";
import { useEffect, useState } from "react";
import { ICurrency } from "@shared/mainConfig";
import style from "./FormCurrency.module.scss";

const TextFieldStyled = styled(TextField)(() => ({
  margin: "10px",
}));

interface Props {
  currencyUpdate?: ICurrency | undefined,
  setCurrencyUpdate: React.Dispatch<React.SetStateAction<ICurrency | undefined>>,
  registerCurrency(code: string, description: string, rate: number, symbol: string | null): void,
  updateCurrency(id: number, description: string, rate: number, symbol: string | null): void,
}

export default function FormCurrency(props: Props) {
  const {currencyUpdate, setCurrencyUpdate, registerCurrency, updateCurrency} = props;

  const [code, setCode] = useState("");
  const [description, setDescription] = useState("");
  const [rate, setRate] = useState(0);
  const [symbol, setSymbol] = useState("");

  useEffect(() => {
    if (currencyUpdate) {
      setCode(currencyUpdate.code);
      setDescription(currencyUpdate.description);
      setRate(currencyUpdate.rate);
      setSymbol(currencyUpdate.symbol ?? "");
    }
  },[currencyUpdate]);

  const limparCampos = () => {
    setCode("");
    setDescription("");
    setRate(0);
    setSymbol("");
  }

  const submitMoeda = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (currencyUpdate) {
      updateCurrency(currencyUpdate.id, description, rate, symbol.trim() || null);
      setCurrencyUpdate(undefined);
    } else {
      registerCurrency(code, description, rate, symbol.trim() || null);
    }
    limparCampos();
  };

  return (
    <form onSubmit={(event) => submitMoeda(event)}>
      <Stack direction="row" spacing={2}>
        <TextFieldStyled
          id="outlined-basic"
          label="Código moeda"
          variant="outlined"
          value={code}
          onChange={(event) => setCode(event.target.value)}
          slotProps={{ htmlInput: { maxLength: 3 } }}
          helperText="3 letras (ISO 4217)"
          disabled={Boolean(currencyUpdate)}
          required
        />
        <TextFieldStyled
          id="currency-symbol"
          label="Símbolo"
          variant="outlined"
          value={symbol}
          onChange={(event) => setSymbol(event.target.value)}
          slotProps={{ htmlInput: { maxLength: 5 } }}
          helperText="Opcional (ex.: R$)"
        />
        <TextFieldStyled
          id="outlined-basic"
          label="Descrição moeda"
          sx={{
            width: "100%"
          }}
          variant="outlined"
          value={description}
          onChange={(event) => setDescription(event.target.value)}
          required
        />
        <FormControl variant="standard">
          <InputLabel htmlFor="rating-cambio">Taxa de câmbio</InputLabel>
          <Input
            id="rating-cambio"
            placeholder="Taxa de câmbio"
            type="number"
            value={rate}
            onChange={(event) => setRate(Number(event.target.value))}
            required
          />
        </FormControl>
      </Stack>
      <div className={style.container}>
        <Button
          type="submit"
          variant="contained"
          sx={{
            height: "40px",
            width: "100px",
            margin: "10px",
            float: "right",
          }}
          >
          { currencyUpdate ? "Atualizar" : "Cadastrar" }
        </Button>
      </div>
    </form>
  );
}
