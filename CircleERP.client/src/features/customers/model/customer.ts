import type { Address } from "@features/addresses";

/** Pessoa fisica ou juridica. Define se o documento e CPF ou CNPJ. */
export type CustomerType = "Individual" | "Company";

export const customerTypeLabel: Record<CustomerType, string> = {
  Individual: "Pessoa física",
  Company: "Pessoa jurídica",
};

/** Quantos digitos o documento tem, por tipo. */
export const documentLength: Record<CustomerType, number> = {
  Individual: 11,
  Company: 14,
};

export interface Customer {
  id: number;
  name: string;
  /** Somente digitos. */
  document: string;
  /** Com pontuacao, para exibicao. */
  formattedDocument: string;
  type: CustomerType;
  isActive: boolean;
  addressId: number | null;
  /** Endereco ja resolvido pela API, quando ha um vinculado. */
  address: Address | null;
}

export interface CustomerFormValues {
  name: string;
  type: CustomerType;
  document: string;
  addressId: number | null;
}

export const emptyCustomerForm: CustomerFormValues = {
  name: "",
  type: "Individual",
  document: "",
  addressId: null,
};

export function toFormValues(customer: Customer): CustomerFormValues {
  return {
    name: customer.name,
    type: customer.type,
    document: customer.formattedDocument,
    addressId: customer.addressId,
  };
}
