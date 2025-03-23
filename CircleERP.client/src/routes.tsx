// import Footer from "@components/Footer";
// import Menu from "@components/Menu";
// import NotFound from "@components/NotFound";
// import PaginaPadrao from "@components/PaginaPadrao";
// import Cardapio from "@pages/Cardapio";
// import Inicio from "@pages/Inicio";
// import Prato from "@pages/Prato";
// import Sobre from "@pages/Sobre";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import { RoutesPath } from "./shared/mainConfig";

export default function AppRouter(){
  return (  
    <main className='container'>
      <Router
        future={{
          v7_startTransition: true,
        }}
      >
        <Routes>
        </Routes>
      </Router>
    </main>
  );
}