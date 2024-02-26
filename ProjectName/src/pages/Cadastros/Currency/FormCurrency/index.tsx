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
import axios from "axios";
import { useState } from "react";
import { ICurrency } from "shared";
import style from "./FormCurrency.module.scss";

const TextFieldStyled = styled(TextField)(() => ({
  margin: "10px",
}));

interface Props {
  currency: ICurrency[],
  setCurrency: React.Dispatch<React.SetStateAction<ICurrency[]>>
}

export default function FormCurrency(props: Props) {
  const {currency, setCurrency} = props;

  const [code, setCode] = useState("");
  const [description, setDescription] = useState("");
  const [rating, setRating] = useState(0);

  const cadastrarMoeda = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    axios.post("http://localhost:8000/api/v2/currency/",
      {
        code: code,
        description: description,
        rating: rating  
      }
    )
      .then((response) => {
        if (currency) {
          setCurrency([...currency, response.data]);
        }
        else setCurrency(response.data);
        alert("Moeda registrada com sucesso");
      })
      .catch((response) =>{
        console.log(response.message)
      })
  };

  return (
    <form onSubmit={(event) => cadastrarMoeda(event)}>
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
            left: "0"
          }}
          >
          Cadastrar
        </Button>
      </div>
    </form>
  );
}
