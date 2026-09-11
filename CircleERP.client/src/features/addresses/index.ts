/**
 * Entrada publica da feature de enderecos.
 *
 * O dialog e exportado porque a tela de cliente cadastra endereco sem sair
 * dela -- e o unico ponto em que outra feature encosta na de enderecos.
 */
export { useAddresses } from "./hooks/useAddresses";
export { default as AddressDialog } from "./ui/AddressDialog";
export type { Address } from "./model/address";
