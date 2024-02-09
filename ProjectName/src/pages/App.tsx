import "es6-shim";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import { RoutesPath } from "../shared";
import Menu from "pages/Menu";
import Home from "pages/Home";
import Currency from "pages/Cadastros/Currency";
import Dashboard from "components/Dashboard";

export default function App () {
  return (
    
    <Router>
      <Dashboard/> 
    </Router>
  );
};
