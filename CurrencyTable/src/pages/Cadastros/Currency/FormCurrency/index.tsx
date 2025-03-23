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
  registerCurrency(code: string, description: string, rating: number): void,
  updateCurrency(id: number, code: string, description: string, rating: number): void,
}

export default function FormCurrency(props: Props) {
  const {currencyUpdate, setCurrencyUpdate, registerCurrency, updateCurrency} = props;

  const [code, setCode] = useState("");
  const [description, setDescription] = useState("");
  const [rating, setRating] = useState(0);

  useEffect(() => {
    if (currencyUpdate) {
      setCode(currencyUpdate.code);
      setDescription(currencyUpdate.description);
      setRating(currencyUpdate.rating);
    }
  },[currencyUpdate]);

  const limparCampos = () => {
    setCode("");
    setDescription("");
    setRating(0);
  }

  const submitMoeda = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (currencyUpdate) {
      updateCurrency(currencyUpdate.id, code, description, rating);
      setCurrencyUpdate(undefined);
    } else {
      registerCurrency(code, description, rating);
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
          required
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
            value={rating}
            onChange={(event) => setRating(Number(event.target.value))}
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
