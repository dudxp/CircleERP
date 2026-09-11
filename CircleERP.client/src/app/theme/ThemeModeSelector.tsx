import {
  Box,
  IconButton,
  ListItemIcon,
  ListItemText,
  Menu,
  MenuItem,
  Tooltip,
  Typography,
} from "@mui/material";
import CheckIcon from "@mui/icons-material/Check";
import PaletteIcon from "@mui/icons-material/Palette";
import { useState } from "react";
import { themeChoices, type ThemeId } from "./themes";
import { useThemeMode } from "./useThemeMode";

/**
 * Amostra das duas cores principais do tema, para a escolha ser visual em vez
 * de adivinhada pelo nome.
 */
function Swatch({ colors }: { colors: readonly [string, string] }) {
  return (
    <Box
      sx={{
        width: 28,
        height: 18,
        borderRadius: 0.5,
        overflow: "hidden",
        display: "flex",
        border: "1px solid",
        borderColor: "divider",
        flexShrink: 0,
      }}
    >
      {colors.map((color) => (
        <Box key={color} sx={{ flex: 1, backgroundColor: color }} />
      ))}
    </Box>
  );
}

export default function ThemeModeSelector() {
  const { themeId, setThemeId } = useThemeMode();
  const [anchor, setAnchor] = useState<HTMLElement | null>(null);

  const current = themeChoices.find((choice) => choice.id === themeId);

  const choose = (id: ThemeId) => {
    setThemeId(id);
    setAnchor(null);
  };

  return (
    <>
      <Tooltip title={`Tema: ${current?.label ?? "Sistema"}`}>
        <IconButton
          aria-label="Escolher tema"
          aria-haspopup="menu"
          onClick={(event) => setAnchor(event.currentTarget)}
        >
          <PaletteIcon />
        </IconButton>
      </Tooltip>

      <Menu open={Boolean(anchor)} anchorEl={anchor} onClose={() => setAnchor(null)}>
        {themeChoices.map((choice) => (
          <MenuItem
            key={choice.id}
            selected={choice.id === themeId}
            onClick={() => choose(choice.id)}
            sx={{ gap: 1.5 }}
          >
            <Swatch colors={choice.swatch} />

            <ListItemText
              primary={choice.label}
              secondary={choice.description}
              slotProps={{ secondary: { variant: "caption" } }}
            />

            <ListItemIcon sx={{ minWidth: "auto", ml: 2 }}>
              {choice.id === themeId ? (
                <CheckIcon fontSize="small" />
              ) : (
                <Typography component="span" sx={{ width: 20 }} />
              )}
            </ListItemIcon>
          </MenuItem>
        ))}
      </Menu>
    </>
  );
}
