import "bootstrap/dist/css/bootstrap.css";
import "./index.css";

import React from "react";
import ReactDOM from "react-dom/client";
import App from "./app/App";

const container = document.getElementById("root");

if (!container) {
  throw new Error("Elemento #root nao encontrado no index.html.");
}

ReactDOM.createRoot(container).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
