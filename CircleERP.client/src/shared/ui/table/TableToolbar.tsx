import { IconButton, Toolbar, Tooltip, Typography, alpha } from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";

interface Props {
  title: string;
  selectedCount: number;
  /** Quando ausente, a acao de excluir selecionados nem aparece. */
  onDeleteSelected?: () => void;
}

/**
 * Barra de titulo da tabela. O botao de exclusao so e renderizado quando ha um
 * handler: botao visivel que nao faz nada e pior do que botao nenhum.
 */
export default function TableToolbar({ title, selectedCount, onDeleteSelected }: Props) {
  const hasSelection = selectedCount > 0;

  return (
    <Toolbar
      sx={{
        pl: { sm: 2 },
        pr: { xs: 1, sm: 1 },
        ...(hasSelection && {
          bgcolor: (theme) =>
            alpha(theme.palette.primary.main, theme.palette.action.activatedOpacity),
        }),
      }}
    >
      <Typography
        sx={{ flex: "1 1 100%" }}
        variant={hasSelection ? "subtitle1" : "h6"}
        component="div"
      >
        {hasSelection ? `${selectedCount} selecionada(s)` : title}
      </Typography>

      {hasSelection && onDeleteSelected && (
        <Tooltip title="Excluir selecionadas">
          <IconButton aria-label="Excluir selecionadas" onClick={onDeleteSelected}>
            <DeleteIcon />
          </IconButton>
        </Tooltip>
      )}
    </Toolbar>
  );
}
