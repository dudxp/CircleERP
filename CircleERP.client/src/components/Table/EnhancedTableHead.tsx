import {
	Box,
	Checkbox,
	TableCell,
	TableHead,
	TableRow,
	TableSortLabel,
} from "@mui/material";
import { visuallyHidden } from "@mui/utils";
import type { TableCellProps } from "@mui/material";
import { Order } from "@shared/mainConfig";

export type CellAlignment = TableCellProps["align"];

interface HeadCellBase {
	label: string;
	disablePadding: boolean;
	align: CellAlignment;
}

/**
 * Coluna ligada a um campo da entidade: pode ser ordenada.
 */
export interface SortableHeadCell<T> extends HeadCellBase {
	kind: "sortable";
	id: keyof T;
}

/**
 * Coluna de acao (editar, deletar, ...): nao corresponde a nenhum campo,
 * portanto nao e ordenavel.
 */
export interface ActionHeadCell extends HeadCellBase {
	kind: "action";
	id: string;
}

export type HeadCell<T> = SortableHeadCell<T> | ActionHeadCell;

interface Props<T> {
	numSelected: number;
	onRequestSort: (event: React.MouseEvent<unknown>, property: keyof T) => void;
	onSelectAllClick: (event: React.ChangeEvent<HTMLInputElement>) => void;
	order: Order;
	orderBy: keyof T;
	rowCount: number;
	headCells: readonly HeadCell<T>[];
	checkBoxAriaLabel: string;
}

export default function EnhancedTableHead<T>(props: Props<T>) {
	const {
		onSelectAllClick,
		order,
		orderBy,
		numSelected,
		rowCount,
		onRequestSort,
		headCells,
		checkBoxAriaLabel,
	} = props;

	const createSortHandler =
		(property: keyof T) => (event: React.MouseEvent<unknown>) => {
			onRequestSort(event, property);
		};

	return (
		<TableHead>
			<TableRow>
				<TableCell padding="checkbox">
					<Checkbox
						color="primary"
						indeterminate={numSelected > 0 && numSelected < rowCount}
						checked={rowCount > 0 && numSelected === rowCount}
						onChange={onSelectAllClick}
						slotProps={{
							input: {
								"aria-label": checkBoxAriaLabel,
							},
						}}
					/>
				</TableCell>
				{headCells.map((headCell) => {
					const isSorted =
						headCell.kind === "sortable" && orderBy === headCell.id;

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
									onClick={createSortHandler(headCell.id)}
								>
									{headCell.label}
									{isSorted ? (
										<Box component="span" sx={visuallyHidden}>
											{order === "desc" ? "sorted descending" : "sorted ascending"}
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
