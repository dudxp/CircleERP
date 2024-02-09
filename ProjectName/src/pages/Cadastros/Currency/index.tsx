// import { Dashboard } from "layouts/DashboardTS/side-nav-item";
import FormCurrency from "./FormCurrency";
import style from "./Currency.module.scss";
import axios from "axios";
import ListCurrency from "./ListCurrency";

export default function Currency () {

  // axios.get("https:localhost:5001/")
  //   .then((resposta)=>{

  //   });

  return (
    <div className={style.Currency}>
      {/* <NavCurrency/> */}
      <FormCurrency/>
      <ListCurrency/>
    </div>
  );
}