import Temas from "@styles/Tema.module.scss";

export const Logo = () => {
  return (
    <img src="svg/svg-icons/coin.svg"
         alt="Logo moeda" 
         className={Temas.logoCyncly} 
    />
  );
};
