import {
  Box,
  Checkbox,
  TableCell,
  TableHead,
  TableRow,
  TableSortLabel,
  type TableCellProps,
} from "@mui/material";
import { visuallyHidden } from "@mui/utils";
import { SortDirection } from "@shared/types/sorting";

export type CellAlignment = TableCellProps["align"];

interface HeadCellBase {
  label: string;
  disablePadding: boolean;
  align: CellAlignment;
}

/** Coluna ligada a um campo da entidade: pode ser ordenada. */
export interface SortableHeadCell<T> extends HeadCellBase {
  kind: "sortable";
  id: keyof T;
}

/**
 * Coluna de acao (editar, deletar, ...). Nao corresponde a nenhum campo,
 * portanto nao e ordenavel -- o tipo impede declara-la como se fosse.
 */
export interface ActionHeadCell extends HeadCellBase {
  kind: "action";
  id: string;
}

export type HeadCell<T> = SortableHeadCell<T> | ActionHeadCell;

interface Props<T> {
  headCells: readonly HeadCell<T>[];
  order: SortDirection;
  orderBy: keyof T;
  onRequestSort: (property: keyof T) => void;
  numSelected: number;
  rowCount: number;
  onSelectAllClick: (event: React.ChangeEvent<HTMLInputElement>) => void;
  selectAllAriaLabel: string;
}

export default function SortableTableHead<T>(props: Props<T>) {
  const {
    headCells,
    order,
    orderBy,
    onRequestSort,
    numSelected,
    rowCount,
    onSelectAllClick,
    selectAllAriaLabel,
  } = props;

  return (
    <TableHead>
      <TableRow>
        <TableCell padding="checkbox">
          <Checkbox
            color="primary"
            indeterminate={numSelected > 0 && numSelected < rowCount}
            checked={rowCount > 0 && numSelected === rowCount}
            onChange={onSelectAllClick}
            slotProps={{ input: { "aria-label": selectAllAriaLabel } }}
          />
        </TableCell>

        {headCells.map((headCell) => {
          const isSorted = headCell.kind === "sortable" && orderBy === headCell.id;

          return (
            <TableCell
              key={String(headCell.id)}
              align={headCell.align}
              padding={headCell.disablePadding ? "none" : "normal"}
              sortDirection={isSorted ? order : false}
            >
              {headCell.kind === "action" ? (
                headCell.label
              ) : (
                <TableSortLabel
                  active={isSorted}
                  direction={isSorted ? order : "asc"}
                  onClick={() => onRequestSort(headCell.id)}
                >
                  {headCell.label}
                  {isSorted ? (
                    <Box component="span" sx={visuallyHidden}>
                      {order === "desc" ? "ordenado decrescente" : "ordenado crescente"}
                    </Box>
                  ) : null}
                </TableSortLabel>
              )}
            </TableCell>
          );
        })}
      </TableRow>
    </TableHead>
  );
}
