/** Endereco como a API o devolve. */
export interface Address {
  id: number;
  /** Somente digitos. */
  zipCode: string;
  /** Com pontuacao, para exibicao. */
  formattedZipCode: string;
  street: string;
  number: string;
  complement: string | null;
  district: string;
  city: string;
  state: string;
  /** Endereco montado em uma linha, vindo pronto do servidor. */
  singleLine: string;
}

/** Campos do formulario. Cadastro e alteracao usam os mesmos. */
export interface AddressFormValues {
  zipCode: string;
  street: string;
  number: string;
  complement: string;
  district: string;
  city: string;
  state: string;
}

export const emptyAddressForm: AddressFormValues = {
  zipCode: "",
  street: "",
  number: "",
  complement: "",
  district: "",
  city: "",
  state: "",
};

export function toFormValues(address: Address): AddressFormValues {
  return {
    zipCode: address.formattedZipCode,
    street: address.street,
    number: address.number,
    complement: address.complement ?? "",
    district: address.district,
    city: address.city,
    state: address.state,
  };
}

/** String vazia e ausencia de complemento, nao um complemento vazio. */
export function toPayload(values: AddressFormValues) {
  return { ...values, complement: values.complement.trim() || null };
}

/** As 27 unidades federativas, para o seletor. */
export const brazilianStates = [
  "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
  "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
  "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO",
] as const;
