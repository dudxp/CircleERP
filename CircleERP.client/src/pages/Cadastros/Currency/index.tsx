import FormCurrency from "./FormCurrency";
import style from "./Currency.module.scss";
import ListCurrency from "./ListCurrency";
import { useState } from "react";
import { ICurrency, axiosV2 } from "@shared/mainConfig";
import { AxiosRequestConfig, isAxiosError } from "axios";

/**
 * Extrai a mensagem do ProblemDetails devolvido pela API. Antes o erro so ia
 * para o console e o usuario nao ficava sabendo do que se tratava.
 */
function describeError(error: unknown): string {
  if (isAxiosError(error)) {
    return error.response?.data?.detail ?? error.message;
  }
  return "Erro inesperado.";
}

export default function Currency() {
  const [currency, setCurrency] = useState<ICurrency[]>([]);
  const [currencyUpdate, setCurrencyUpdate] = useState<ICurrency | undefined>();

  const filterCurrency = (options: AxiosRequestConfig = {}) => {
    axiosV2
      .get<ICurrency[]>("currencies", options)
      .then((response) => setCurrency(response.data))
      .catch((error) => alert(describeError(error)));
  };

  const deleteCurrency = (id: number) => {
    if (!window.confirm("Deseja realmente deletar essa moeda?")) {
      return;
    }

    axiosV2
      .delete(`currencies/${id}`)
      .then(() => {
        setCurrency(currency.filter((item) => item.id !== id));
      })
      .catch((error) => alert(describeError(error)));
  };

  // A verificacao de codigo duplicado saiu daqui: quem decide e a API, que
  // responde 409. Duplicar a regra no cliente so cria duas versoes dela.
  const registerCurrency = (
    code: string,
    description: string,
    rate: number,
    symbol: string | null
  ) => {
    axiosV2
      .post<number>("currencies", { code, description, rate, symbol })
      .then((response) => {
        setCurrency([...currency, { id: response.data, code, description, rate, symbol }]);
      })
      .catch((error) => alert(describeError(error)));
  };

  const updateCurrency = (
    id: number,
    description: string,
    rate: number,
    symbol: string | null
  ) => {
    axiosV2
      .put(`currencies/${id}`, { description, rate, symbol })
      .then(() => {
        setCurrency(
          currency.map((item) =>
            item.id === id ? { ...item, description, rate, symbol } : item
          )
        );
      })
      .catch((error) => alert(describeError(error)));
  };

  return (
    <div className={style.Currency}>
      <FormCurrency
        setCurrencyUpdate={setCurrencyUpdate}
        currencyUpdate={currencyUpdate}
        registerCurrency={registerCurrency}
        updateCurrency={updateCurrency}
      />
      <ListCurrency
        currency={currency}
        setCurrency={setCurrency}
        deleteCurrency={deleteCurrency}
        setCurrencyUpdate={setCurrencyUpdate}
        filterCurrency={filterCurrency}
      />
    </div>
  );
}
