import { useCallback, useRef, useState } from "react";
import { ApiError, toApiError } from "@shared/api/problemDetails";
import { addressApi } from "../api/addressApi";
import type { AddressFormValues } from "../model/address";

interface UseZipCodeAutofillResult {
  isLooking: boolean;
  /** Mensagem para exibir abaixo do campo. Nunca bloqueia o formulario. */
  notice: string | null;
  lookup: (zipCode: string) => Promise<void>;
  clearNotice: () => void;
}

/** Quantidade de digitos que dispara a consulta automatica. */
const zipCodeDigits = 8;

/**
 * Preenche logradouro, bairro, cidade e UF a partir do CEP.
 *
 * A consulta e uma conveniencia, nunca um pre-requisito: se o CEP nao existir
 * ou o servico estiver fora, a mensagem aparece e os campos seguem editaveis.
 * Numero e complemento nunca sao tocados -- o CEP nao os conhece.
 */
export function useZipCodeAutofill(
  values: AddressFormValues,
  onChange: (values: AddressFormValues) => void
): UseZipCodeAutofillResult {
  const [isLooking, setIsLooking] = useState(false);
  const [notice, setNotice] = useState<string | null>(null);

  // Evita repetir a consulta do mesmo CEP a cada blur do campo.
  const lastLookedUp = useRef<string | null>(null);

  const lookup = useCallback(
    async (zipCode: string) => {
      const digits = zipCode.replace(/\D/g, "");

      if (digits.length !== zipCodeDigits || digits === lastLookedUp.current) {
        return;
      }

      lastLookedUp.current = digits;
      setIsLooking(true);
      setNotice(null);

      try {
        const found = await addressApi.lookupZipCode(digits);

        onChange({
          ...values,
          zipCode,
          street: found.street || values.street,
          district: found.district || values.district,
          city: found.city || values.city,
          state: found.state || values.state,
        });

        // Alguns CEPs cobrem uma cidade inteira e nao trazem logradouro.
        if (!found.street) {
          setNotice("CEP encontrado, mas sem logradouro. Complete os campos.");
        }
      } catch (error) {
        const apiError = error instanceof ApiError ? error : toApiError(error);

        setNotice(
          apiError.status === 404
            ? "CEP não encontrado. Preencha o endereço manualmente."
            : apiError.message
        );
      } finally {
        setIsLooking(false);
      }
    },
    [values, onChange]
  );

  return { isLooking, notice, lookup, clearNotice: () => setNotice(null) };
}
