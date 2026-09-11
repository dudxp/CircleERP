import { createTheme, type Theme, type ThemeOptions } from "@mui/material";

/**
 * Identificador do tema escolhido.
 *
 * `system` nao e uma paleta: e a instrucao de seguir o sistema operacional, e
 * resolve para `light` ou `dark` em tempo de execucao.
 */
export type ThemeId =
  | "system"
  | "light"
  | "dark"
  | "ocean"
  | "sepia"
  | "highContrast";

export interface ThemeChoice {
  id: ThemeId;
  label: string;
  description: string;
  /** Duas cores para a amostra no seletor. */
  swatch: [string, string];
}

/**
 * Opcoes oferecidas no seletor, na ordem em que aparecem.
 */
export const themeChoices: readonly ThemeChoice[] = [
  {
    id: "system",
    label: "Sistema",
    description: "Acompanha o claro/escuro do sistema operacional",
    swatch: ["#fafafa", "#121212"],
  },
  {
    id: "light",
    label: "Claro",
    description: "Padrão, fundo branco",
    swatch: ["#ffffff", "#1976d2"],
  },
  {
    id: "dark",
    label: "Escuro",
    description: "Fundo escuro neutro",
    swatch: ["#121212", "#90caf9"],
  },
  {
    id: "ocean",
    label: "Oceano",
    description: "Escuro em tons de azul",
    swatch: ["#0b1f2a", "#4dd0e1"],
  },
  {
    id: "sepia",
    label: "Sépia",
    description: "Claro e quente, menos brilho",
    swatch: ["#f4ecd8", "#8d6e37"],
  },
  {
    id: "highContrast",
    label: "Alto contraste",
    description: "Preto e amarelo, para máxima legibilidade",
    swatch: ["#000000", "#ffd600"],
  },
];

/**
 * Ajustes comuns a todos os temas.
 *
 * Ficam aqui, e nao repetidos em cada paleta, para que trocar o arredondamento
 * ou a fonte seja uma edicao so.
 */
const shared: ThemeOptions = {
  shape: { borderRadius: 8 },
  components: {
    MuiPaper: {
      styleOverrides: {
        // Sem isto o MUI clareia o Paper por elevacao nos temas escuros, e as
        // paletas customizadas perdem a cor.
        root: { backgroundImage: "none" },
      },
    },
  },
};

const palettes: Record<Exclude<ThemeId, "system">, ThemeOptions> = {
  light: {
    palette: {
      mode: "light",
      primary: { main: "#1976d2" },
      background: { default: "#f5f5f5", paper: "#ffffff" },
    },
  },

  dark: {
    palette: {
      mode: "dark",
      primary: { main: "#90caf9" },
      background: { default: "#121212", paper: "#1e1e1e" },
    },
  },

  ocean: {
    palette: {
      mode: "dark",
      primary: { main: "#4dd0e1" },
      secondary: { main: "#80cbc4" },
      background: { default: "#0b1f2a", paper: "#12303d" },
      text: { primary: "#e0f7fa", secondary: "#9fc4cf" },
    },
  },

  sepia: {
    palette: {
      mode: "light",
      primary: { main: "#8d6e37" },
      secondary: { main: "#a1887f" },
      background: { default: "#f4ecd8", paper: "#fbf6ea" },
      text: { primary: "#3e2f1c", secondary: "#6d5b43" },
    },
  },

  highContrast: {
    palette: {
      mode: "dark",
      primary: { main: "#ffd600" },
      secondary: { main: "#00e5ff" },
      background: { default: "#000000", paper: "#0a0a0a" },
      text: { primary: "#ffffff", secondary: "#e0e0e0" },
      divider: "#ffffff",
    },
  },
};

/**
 * Monta o tema do MUI para a escolha feita.
 *
 * @param prefersDark resultado do `prefers-color-scheme`, usado apenas quando a
 * escolha e `system`.
 */
export function buildTheme(id: ThemeId, prefersDark: boolean): Theme {
  const resolved = id === "system" ? (prefersDark ? "dark" : "light") : id;

  return createTheme({ ...shared, ...palettes[resolved] });
}
