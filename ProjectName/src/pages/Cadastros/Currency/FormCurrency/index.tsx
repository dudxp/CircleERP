// import { bootstrap } from 'shared';
import {
  Box,
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

const TextFieldStyled = styled(TextField)(() => ({
  margin: "10px",
}));

// interface Props {
//   currency: ICurrency
//   setCurrency: ICurrency
// }

export default function FormCurrency() {
  const [codigo, setCodigo] = useState("");
  const [descricao, setDescricao] = useState("");
  const [taxa, setTaxa] = useState(0);

  const cadastrarMoeda = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    axios.put("http://localhost:8000/api/v2/currency/",
      {
        codigo: codigo,
        descricao: descricao,
        currency: taxa  
      }
    )
      .then((response) => {
        // setRestaurantes([...restaurantes, response.data]);
        alert("Restaurante registrado com sucesso");
      })
    console.log(codigo, descricao,taxa);
  };

  return (
    <form onSubmit={(event) => cadastrarMoeda(event)}>
      <Stack direction="row" spacing={2}>
        <TextFieldStyled
          id="outlined-basic"
          label="Código moeda"
          variant="outlined"
          value={codigo}
          onChange={(event) => setCodigo(event.target.value)}
          required
        />
        <TextFieldStyled
          id="outlined-basic"
          label="Descrição moeda"
          sx={{
            width: "100%"
          }}
          variant="outlined"
          value={descricao}
          onChange={(event) => setDescricao(event.target.value)}
          required
        />
        <FormControl variant="standard">
          <InputLabel htmlFor="taxa-cambio">Taxa de câmbio</InputLabel>
          <Input
            id="taxa-cambio"
            placeholder="Taxa de câmbio"
            // variant="outlined"
            type="number"
            value={taxa}
            onChange={(event) => setTaxa(Number(event.target.value))}
            required
          />
        </FormControl>
      </Stack>
      <div>

        <Button
          type="submit"
          variant="contained"
          sx={{
            height: "40px",
            width: "100px",
            alignSelf: "",
            margin: "10px",
          }}
          >
          Cadastrar
        </Button>
      </div>
    </form>
  );
}
