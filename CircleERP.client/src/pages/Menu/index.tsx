import { Link } from "react-router-dom";
import style from "./Menu.module.scss";

export default function Menu () {
  const rotas = [{
    label: "Início",
    to: "/"
  }, {
    label: "Cadastrar moeda",
    to: "/currency"
  }, {
    label: "Sobre",
    to: "/sobre"
  }];
  return (
    <nav className={style.menu}>
      {/* <Logo/> */}
      <ul className={style.menu__list}>
        {
          rotas.map((rota, index) => (
            <li key={index} className={style.menu__link}>
              <Link to={rota.to}>
                {rota.label}
              </Link>
            </li>
          ))
        }
      </ul>
    </nav>
  );
}