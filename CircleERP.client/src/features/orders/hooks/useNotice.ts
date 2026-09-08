import { useCallback, useState } from "react";
import { toApiError } from "@shared/api/problemDetails";

export interface Notice {
  severity: "success" | "error";
  message: string;
}

/**
 * Executa uma operacao de escrita e traduz o resultado em mensagem para o
 * usuario. Evita repetir o mesmo try/catch/setState em cada acao da tela.
 */
export function useNotice() {
  const [notice, setNotice] = useState<Notice | null>(null);
  const [isBusy, setIsBusy] = useState(false);

  const run = useCallback(
    async (action: () => Promise<unknown>, successMessage: string): Promise<boolean> => {
      setIsBusy(true);
      try {
        await action();
        setNotice({ severity: "success", message: successMessage });
        return true;
      } catch (error) {
        setNotice({ severity: "error", message: toApiError(error).message });
        return false;
      } finally {
        setIsBusy(false);
      }
    },
    []
  );

  return { notice, dismiss: () => setNotice(null), run, isBusy };
}
