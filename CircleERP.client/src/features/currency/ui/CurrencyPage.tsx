import { Alert, Snackbar } from "@mui/material";
import { useState } from "react";
import { toApiError } from "@shared/api/problemDetails";
import { useCurrencies } from "../hooks/useCurrencies";
import type { Currency } from "../model/currency";
import CurrencyForm from "./CurrencyForm";
import CurrencyTable from "./CurrencyTable";
import style from "./CurrencyPage.module.scss";

interface Notice {
  severity: "success" | "error";
  message: string;
}

/**
 * Orquestra o cadastro de moedas: liga o hook de dados aos dois componentes de
 * apresentacao e cuida do retorno ao usuario. Nao conhece HTTP nem regra de
 * negocio -- a duplicidade de codigo, por exemplo, e decidida pela API (409) e
 * chega aqui ja como mensagem.
 */
export default function CurrencyPage() {
  const { currencies, isLoading, loadError, register, change, remove } = useCurrencies();

  const [editing, setEditing] = useState<Currency | undefined>();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [notice, setNotice] = useState<Notice | null>(null);

  const run = async (action: () => Promise<void>, successMessage: string) => {
    setIsSubmitting(true);
    try {
      await action();
      setNotice({ severity: "success", message: successMessage });
      return true;
    } catch (error) {
      setNotice({ severity: "error", message: toApiError(error).message });
      return false;
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleSubmit = async (values: {
    code: string;
    description: string;
    rate: number;
    symbol: string | null;
  }) => {
    const { code, ...rest } = values;

    const succeeded = editing
      ? await run(() => change(editing.id, rest), "Moeda atualizada com sucesso.")
      : await run(() => register({ code, ...rest }), "Moeda cadastrada com sucesso.");

    if (succeeded) {
      setEditing(undefined);
    }
  };

  const handleDelete = async (currency: Currency) => {
    const confirmed = window.confirm(
      `Deseja realmente deletar a moeda ${currency.code}?`
    );

    if (!confirmed) return;

    await run(() => remove(currency.id), "Moeda deletada com sucesso.");

    if (editing?.id === currency.id) {
      setEditing(undefined);
    }
  };

  return (
    <div className={style.page}>
      <CurrencyForm
        editing={editing}
        onCancelEdit={() => setEditing(undefined)}
        onSubmit={handleSubmit}
        isSubmitting={isSubmitting}
      />

      <CurrencyTable
        currencies={currencies}
        isLoading={isLoading}
        loadError={loadError}
        onEdit={setEditing}
        onDelete={handleDelete}
      />

      <Snackbar
        open={notice !== null}
        autoHideDuration={5000}
        onClose={() => setNotice(null)}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert severity={notice?.severity} onClose={() => setNotice(null)}>
          {notice?.message}
        </Alert>
      </Snackbar>
    </div>
  );
}
