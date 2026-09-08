/**
 * Formata um valor monetario com o codigo da moeda ("BRL 1.234,56").
 *
 * Usa o codigo, e nao o simbolo, porque o codigo acompanha o pedido -- e o
 * simbolo pertence ao cadastro de moedas, que pode nem ter um.
 */
export function formatMoney(amount: number, currencyCode: string): string {
  return `${currencyCode} ${amount.toLocaleString("pt-BR", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })}`;
}

export function formatDateTime(isoDate: string): string {
  return new Date(isoDate).toLocaleString("pt-BR");
}
