import FormCurrency from "./FormCurrency";
import style from "./Currency.module.scss";
import ListCurrency from "./ListCurrency";
import { useState } from "react";
import { ICurrency } from "shared";
import axios from "axios";

export default function Currency () {
  const [ currency, setCurrency ] = useState<ICurrency[]>([]);

  function deleteCurrency(id: number){
    axios.delete(`http://localhost:8000/api/v2/currency/${id}/`)
      .then((resposta) => {
        alert("Currency deletada")
      })
      .catch((resposta) => {
        console.log(resposta.data)
      })
  }

  return (
    <div className={style.Currency}>
      {/* <NavCurrency/> */}
      <FormCurrency currency={currency} setCurrency={setCurrency}/>
      <ListCurrency currency={currency} setCurrency={setCurrency} deleteCurrency={deleteCurrency}/>
    </div>
  );
}