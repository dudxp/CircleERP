import { Outlet } from "react-router-dom";
import style from "./Home.module.scss";

export default function PaginaPadrao(){
  return (
    <>
      <header className={style.header}>
        <div className={style.header__text}>
          Cadastro de moeda
        </div>
      </header>
      <div>
        <Outlet />
      </div>
    </>
  );
}