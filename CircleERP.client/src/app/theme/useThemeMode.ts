import { useContext } from "react";
import { ThemeModeContext, type ThemeModeContextValue } from "./ThemeModeContext";

/**
 * Tema escolhido e como troca-lo.
 *
 */
export function useThemeMode(): ThemeModeContextValue {
  const context = useContext(ThemeModeContext);

  if (!context) {
    throw new Error("useThemeMode precisa estar dentro de ThemeModeProvider.");
  }

  return context;
}
