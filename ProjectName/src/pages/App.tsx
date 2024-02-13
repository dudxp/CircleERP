import "es6-shim";
import { BrowserRouter as Router } from "react-router-dom";
import Dashboard from "components/Dashboard";

export default function App () {
  return (
    <Router>
      <Dashboard/> 
    </Router>
  );
};
