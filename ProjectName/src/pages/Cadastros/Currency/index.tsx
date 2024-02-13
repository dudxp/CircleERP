import FormCurrency from "./FormCurrency";
import style from "./Currency.module.scss";
import ListCurrency from "./ListCurrency";
import { useState } from "react";
import { ICurrency } from "shared";

export default function Currency () {
  const [ currency, setCurrency ] = useState<ICurrency[]>([]);


  return (
    <div className={style.Currency}>
      {/* <NavCurrency/> */}
      <FormCurrency currency={currency} setCurrency={setCurrency}/>
      <ListCurrency currency={currency} setCurrency={setCurrency}/>
    </div>
  );
}