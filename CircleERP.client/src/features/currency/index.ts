/**
 * Entrada publica da feature de moedas.
 *
 * Outras features importam daqui, e nunca de um caminho interno: assim o que
 * esta exposto e uma decisao explicita, e reorganizar as pastas de dentro nao
 * quebra ninguem.
 */
export { useCurrencies } from "./hooks/useCurrencies";
export type { Currency } from "./model/currency";
