import FormCurrency from "./FormCurrency";
import style from "./Currency.module.scss";
import ListCurrency from "./ListCurrency";
import { useState } from "react";
import Draggable from "react-draggable";
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
    <Draggable
        // axis="x"
        // handle=".handle"
        // defaultPosition={{x: 0, y: 0}}
        // position={null}
        // grid={[25, 25]}
        // scale={1}
        // onStart={this.handleStart}
        // onDrag={this.handleDrag}
        // onStop={this.handleStop}>
    
    >
      <div className={style.Currency}>
        {/* <NavCurrency/> */}
        <FormCurrency currency={currency} setCurrency={setCurrency}/>
        <ListCurrency currency={currency} setCurrency={setCurrency} deleteCurrency={deleteCurrency}/>
      </div>
    </Draggable>
  );
}