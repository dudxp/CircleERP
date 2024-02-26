import {
  Box,
  Checkbox,
  FormControlLabel,
  Switch,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TablePagination,
  TableRow,
} from "@mui/material";
import Paper from "@mui/material/Paper";
import axios from "axios";
import * as React from "react";

import EnhancedTableHead from "components/Table/EnhancedTableHead";
import { ICurrency, Order } from "shared";
import EnhancedTableToolbar from "components/Table/EnhancedTableToolbar";
import { getComparator, stableSort } from "components/Table/TableFunctions";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";

const headCells = [
  {
    id: "id",
    numeric: true,
    disablePadding: true,
    label: "Id interno",
  },
  {
    id: "code",
    numeric: false,
    disablePadding: false,
    label: "Código da moeda",
  },
  {
    id: "description",
    numeric: false,
    disablePadding: false,
    label: "Descrição",
  },
  {
    id: "rating",
    numeric: true,
    disablePadding: false,
    label: "Taca de câmbio",
  },
  {
    id: "edit",
    numeric: false,
    disablePadding: true,
    label: "Editar",
  },
  {
    id: "delete",
    numeric: false,
    disablePadding: true,
    label: "Deletar",
  }
];

//Ver como modificar
interface Props {
  currency: ICurrency[],
  setCurrency: React.Dispatch<React.SetStateAction<ICurrency[]>>,
  deleteCurrency(id: number): void
}

export default function ListCurrency(props: Props) {
  const {currency, setCurrency} = props;

  const [order, setOrder] = React.useState<Order>("asc");
  const [orderBy, setOrderBy] = React.useState<keyof ICurrency>("id");
  const [selected, setSelected] = React.useState<number[]>([]);
  const [page, setPage] = React.useState(0);
  const [dense, setDense] = React.useState(false);
  const [rowsPerPage, setRowsPerPage] = React.useState(5);

  // const [currency, setCurrency] = React.useState<Data[]>([])

  React.useEffect(() => {
    axios.get("http://localhost:8000/api/v2/currency/")
      .then((response) => {
        setCurrency(response.data);
      })
      .catch((response) =>{
        console.log(response.message)
      })  
  },[])

  function handleRequestSort (
    event: React.MouseEvent<unknown>,
    property: keyof ICurrency
  ) {
    const isAsc = orderBy === property && order === "asc";
    setOrder(isAsc ? "desc" : "asc");
    setOrderBy(property);
  };

  const handleSelectAllClick = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.checked) {
      const newSelected = currency.map((moeda) => moeda.id);
      setSelected(newSelected);
      return;
    }
    setSelected([]);
  };

  const handleClick = (event: React.MouseEvent<unknown>, id: number) => {
    const selectedIndex = selected.indexOf(id);
    let newSelected: number[] = [];

    if (selectedIndex === -1) {
      newSelected = newSelected.concat(selected, id);
    } else {
      newSelected = selected.slice(0, selectedIndex).concat(selected.slice(selectedIndex + 1));
    }

    setSelected(newSelected);
  };

  const handleChangePage = (event: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const handleChangeDense = (event: React.ChangeEvent<HTMLInputElement>) => {
    setDense(event.target.checked);
  };

  const isSelected = (id: number) => selected.indexOf(id) !== -1;

  // Avoid a layout jump when reaching the last page with empty rows.
  const emptyRows =
    page > 0 ? Math.max(0, (1 + page) * rowsPerPage - currency.length) : 0;

  const visibleRows = React.useMemo(
    () =>
      stableSort<ICurrency>(currency, getComparator<keyof ICurrency>(order, orderBy)).slice(
        page * rowsPerPage,
        page * rowsPerPage + rowsPerPage
      ),
    [order, orderBy, page, rowsPerPage, currency]
  );

  return (
    <Box sx={{ width: "100%" }}>
      <Paper sx={{ width: "100%", mb: 2 }}>
        <EnhancedTableToolbar selected={selected} />
        <TableContainer>
          <Table
            sx={{ minWidth: 750 }}
            aria-labelledby="tableTitle"
            size={dense ? "small" : "medium"}
          >
            <EnhancedTableHead
              numSelected={selected.length}
              order={order}
              orderBy={orderBy}
              onSelectAllClick={handleSelectAllClick}
              onRequestSort={handleRequestSort}
              rowCount={currency.length}
              headCells={headCells}
              checkBoxAriaLabel="Marcar todas as comidas"
            />
            <TableBody>
              {visibleRows.map((moeda, index) => {
                const isItemSelected = isSelected(moeda.id);
                const labelId = `enhanced-table-checkbox-${index}`;

                return (
                  <TableRow
                    hover
                    aria-checked={isItemSelected}
                    tabIndex={-1}
                    key={moeda.id}
                    selected={isItemSelected}
                    sx={{ cursor: "pointer" }}
                  >
                    <TableCell padding="checkbox">
                      <Checkbox
                        role="checkbox"
                        color="primary"
                        checked={isItemSelected}
                        inputProps={{
                          "aria-labelledby": labelId,
                        }}
                        onClick={(event) => handleClick(event, moeda.id)}
                      />
                    </TableCell>
                    <TableCell align="right" padding="none">{moeda.id}</TableCell>
                    <TableCell
                      component="th"
                      id={labelId}
                      scope="row"
                      align="left"
                    >
                      {moeda.code}
                    </TableCell>
                    <TableCell align="left">{moeda.description}</TableCell>
                    <TableCell align="right">{moeda.rating}</TableCell>
                    <TableCell align="left"
                      
                    
                    ><EditIcon/></TableCell>
                    <TableCell align="left">
                      <DeleteIcon/>
                    </TableCell>
                  </TableRow>
                );
              })}
              {emptyRows > 0 && (
                <TableRow
                  style={{
                    height: (dense ? 33 : 53) * emptyRows,
                  }}
                >
                  <TableCell colSpan={6} />
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
        <TablePagination
          rowsPerPageOptions={[5, 10, 25]}
          component="div"
          count={currency.length}
          rowsPerPage={rowsPerPage}
          page={page}
          onPageChange={handleChangePage}
          onRowsPerPageChange={handleChangeRowsPerPage}
        />
      </Paper>
      <FormControlLabel
        control={<Switch checked={dense} onChange={handleChangeDense} />}
        label="Dense padding"
      />
    </Box>
  );
}
