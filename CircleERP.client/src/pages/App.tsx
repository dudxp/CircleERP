import "es6-shim";
import { BrowserRouter as Router } from "react-router-dom";
import Dashboard from "@components/Dashboard";

export default function App () {
  return (
    <Router
      future={{
        v7_startTransition: true,
        v7_relativeSplatPath: true
      }}>
      <Dashboard/>  
    </Router>
  );
};
