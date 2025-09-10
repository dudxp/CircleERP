import FormCurrency from "./FormCurrency";
import style from "./Currency.module.scss";
import ListCurrency from "./ListCurrency";
import { useState } from "react";
import { ICurrency, axiosV2 } from "@shared/mainConfig";
import { AxiosRequestConfig } from "axios";

export default function Currency() {
  const [currency, setCurrency] = useState<ICurrency[]>([]);
  const [currencyUpdate, setCurrencyUpdate] = useState<ICurrency | undefined>();

  const filterCurrency = (options: AxiosRequestConfig = {}) => {
    axiosV2
      .get("currency/", options)
      .then((response) => {
        setCurrency(response.data);
      })
      .catch((response) => {
        console.log(response.data);
      });
  }

  const deleteCurrency = (id: number) => {
    const question = window.confirm("Deseja realmente deletar essa moeda?");
    if (question) {
      axiosV2
        .delete(`currency/delete-by-id/${id}/`)
        .then(() => {
          alert("Moeda deletada com sucesso!");
          setCurrency(currency.filter((currency) => currency.id !== id));
        })
        .catch((resposta) => {
          console.log(resposta.data);
        });
    }
  }

  const registerCurrency = (
    code: string,
    description: string,
    rating: number
  ) => {
    const existeMoeda = currency.find((currency) => currency.code === code);

    if (existeMoeda) {
      alert("Currency já cadastrada");
      return;
    }

    axiosV2
      .post("currency/", {
        code: code,
        description: description,
        rating: rating,
      })
      .then((response) => {
        console.log(response.data);

        if (response.data)
        {
          const newCurrency: ICurrency = {
            id: response.data,
            code,
            description,
            rating
          };
          setCurrency([...currency, newCurrency]);
        }

        alert("Moeda registrada com sucesso");
      })
      .catch((response) => {
        console.log(response.message);
      });
  };

  const updateCurrency = (
    id: number,
    code: string,
    description: string,
    rating: number
  ) => {
    axiosV2
      .put(`currency/${id}/`, {
        code: code,
        description: description,
        rating: rating,
      })
      .then(() => {
        const newCurrency = currency.map((currency) => {
          if (currency.id === id) {
            currency.code = code;
            currency.description = description;
            currency.rating = rating;
          }
          return currency;
        });

        setCurrency(newCurrency);
        alert("Moeda atualizada com sucesso");
      })
      .catch((resposta) => {
        console.log(resposta.data);
      });
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
