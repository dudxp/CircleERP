import { createContext } from "react";
import type { ThemeId } from "./themes";

export interface ThemeModeContextValue {
  themeId: ThemeId;
  setThemeId: (id: ThemeId) => void;
}

/**
 * Vive em arquivo proprio porque exportar um contexto do mesmo arquivo que
 * exporta um componente desliga o fast refresh do Vite.
 */
export const ThemeModeContext = createContext<ThemeModeContextValue | null>(null);
