import theme from "@styles/Tema.module.scss";

export default function Logo() {
  return (
    // Caminho absoluto de proposito: relativo quebra em rota aninhada como
    // /order/9, onde resolveria para /order/CircleERP2.png.
    <img src="/CircleERP2.png" alt="CircleERP" className={theme.logoCircleERP} />
  );
}
