/** Unidade em que o produto e vendido. */
export type UnitOfMeasure = "Unit" | "Kilogram" | "Box" | "Liter" | "Meter";

export const unitLabel: Record<UnitOfMeasure, string> = {
  Unit: "UN — unidade",
  Kilogram: "KG — quilograma",
  Box: "CX — caixa",
  Liter: "L — litro",
  Meter: "M — metro",
};

/** Abreviacao para tabelas, onde o rotulo completo nao cabe. */
export const unitAbbreviation: Record<UnitOfMeasure, string> = {
  Unit: "UN",
  Kilogram: "KG",
  Box: "CX",
  Liter: "L",
  Meter: "M",
};

export interface Product {
  id: number;
  sku: string;
  name: string;
  price: number;
  /** Moeda do preco: um preco sem moeda nao significa nada. */
  currencyCode: string;
  unit: UnitOfMeasure;
  isActive: boolean;
}

export interface ProductFormValues {
  sku: string;
  name: string;
  price: string;
  currencyCode: string;
  unit: UnitOfMeasure;
}

export const emptyProductForm: ProductFormValues = {
  sku: "",
  name: "",
  price: "0",
  currencyCode: "",
  unit: "Unit",
};

export function toFormValues(product: Product): ProductFormValues {
  return {
    sku: product.sku,
    name: product.name,
    price: String(product.price),
    currencyCode: product.currencyCode,
    unit: product.unit,
  };
}

export function toPayload(values: ProductFormValues) {
  return { ...values, price: Number(values.price) };
}
