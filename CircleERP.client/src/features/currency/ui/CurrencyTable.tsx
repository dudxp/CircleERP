import {
  Alert,
  Box,
  Button,
  Checkbox,
  CircularProgress,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TablePagination,
  TableRow,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import { useMemo, useState } from "react";
import SortableTableHead, {
  type HeadCell,
} from "@shared/ui/table/SortableTableHead";
import TableToolbar from "@shared/ui/table/TableToolbar";
import { getComparator, stableSort } from "@shared/ui/table/sorting";
import { SortDirection } from "@shared/types/sorting";
import type { Currency } from "../model/currency";

const headCells: readonly HeadCell<Currency>[] = [
  { kind: "sortable", id: "id", label: "Id interno", disablePadding: true, align: "right" },
  { kind: "sortable", id: "code", label: "Código", disablePadding: false, align: "left" },
  { kind: "sortable", id: "symbol", label: "Símbolo", disablePadding: false, align: "left" },
  { kind: "sortable", id: "description", label: "Descrição", disablePadding: false, align: "left" },
  { kind: "sortable", id: "rate", label: "Taxa de câmbio", disablePadding: false, align: "right" },
  { kind: "action", id: "edit", label: "Editar", disablePadding: true, align: "center" },
  { kind: "action", id: "delete", label: "Deletar", disablePadding: true, align: "center" },
];

interface Props {
  currencies: Currency[];
  isLoading: boolean;
  loadError: string | null;
  onEdit: (currency: Currency) => void;
  onDelete: (currency: Currency) => void;
}

/**
 * Apresentacao pura: recebe a lista pronta e nao busca nada. Antes este
 * componente fazia a propria requisicao, criando uma segunda fonte de verdade
 * para os mesmos dados.
 */
export default function CurrencyTable({
  currencies,
  isLoading,
  loadError,
  onEdit,
  onDelete,
}: Props) {
  const [order, setOrder] = useState<SortDirection>("asc");
  const [orderBy, setOrderBy] = useState<keyof Currency>("id");
  const [selected, setSelected] = useState<number[]>([]);
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(5);

  const handleRequestSort = (property: keyof Currency) => {
    const isAsc = orderBy === property && order === "asc";
    setOrder(isAsc ? "desc" : "asc");
    setOrderBy(property);
  };

  const handleSelectAll = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSelected(event.target.checked ? currencies.map((currency) => currency.id) : []);
  };

  const toggleSelection = (id: number) => {
    setSelected((current) =>
      current.includes(id)
        ? current.filter((selectedId) => selectedId !== id)
        : [...current, id]
    );
  };

  const visibleRows = useMemo(
    () =>
      stableSort(currencies, getComparator<Currency>(order, orderBy)).slice(
        page * rowsPerPage,
        page * rowsPerPage + rowsPerPage
      ),
    [currencies, order, orderBy, page, rowsPerPage]
  );

  if (loadError) {
    return <Alert severity="error">{loadError}</Alert>;
  }

  return (
    <Box sx={{ width: "100%" }}>
      <Paper sx={{ width: "100%", mb: 2 }}>
        <TableToolbar title="Moedas" selectedCount={selected.length} />

        {isLoading ? (
          <Box sx={{ display: "flex", justifyContent: "center", p: 4 }}>
            <CircularProgress />
          </Box>
        ) : (
          <TableContainer>
            <Table sx={{ minWidth: 750 }} aria-label="Moedas cadastradas">
              <SortableTableHead
                headCells={headCells}
                order={order}
                orderBy={orderBy}
                onRequestSort={handleRequestSort}
                numSelected={selected.length}
                rowCount={currencies.length}
                onSelectAllClick={handleSelectAll}
                selectAllAriaLabel="Marcar todas"
              />
              <TableBody>
                {visibleRows.map((currency, index) => {
                  const isSelected = selected.includes(currency.id);
                  const labelId = `currency-row-${index}`;

                  return (
                    <TableRow hover key={currency.id} selected={isSelected}>
                      <TableCell padding="checkbox">
                        <Checkbox
                          color="primary"
                          checked={isSelected}
                          slotProps={{ input: { "aria-labelledby": labelId } }}
                          onChange={() => toggleSelection(currency.id)}
                        />
                      </TableCell>
                      <TableCell align="right" padding="none">{currency.id}</TableCell>
                      <TableCell component="th" id={labelId} scope="row">{currency.code}</TableCell>
                      <TableCell align="left">{currency.symbol ?? "—"}</TableCell>
                      <TableCell align="left">{currency.description}</TableCell>
                      <TableCell align="right">{currency.rate}</TableCell>
                      <TableCell align="center">
                        <Button aria-label={`Editar ${currency.code}`} onClick={() => onEdit(currency)}>
                          <EditIcon />
                        </Button>
                      </TableCell>
                      <TableCell align="center">
                        <Button
                          color="error"
                          aria-label={`Deletar ${currency.code}`}
                          onClick={() => onDelete(currency)}
                        >
                          <DeleteIcon />
                        </Button>
                      </TableCell>
                    </TableRow>
                  );
                })}

                {!currencies.length && (
                  <TableRow>
                    <TableCell colSpan={headCells.length + 1} align="center" sx={{ py: 4 }}>
                      Nenhuma moeda cadastrada.
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
        )}

        <TablePagination
          rowsPerPageOptions={[5, 10, 25]}
          component="div"
          count={currencies.length}
          rowsPerPage={rowsPerPage}
          page={page}
          onPageChange={(_, newPage) => setPage(newPage)}
          onRowsPerPageChange={(event) => {
            setRowsPerPage(Number(event.target.value));
            setPage(0);
          }}
          labelRowsPerPage="Linhas por página"
        />
      </Paper>
    </Box>
  );
}
