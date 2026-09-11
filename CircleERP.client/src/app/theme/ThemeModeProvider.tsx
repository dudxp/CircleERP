import { CssBaseline, ThemeProvider } from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import { ThemeModeContext } from "./ThemeModeContext";
import { buildTheme, type ThemeId } from "./themes";

const storageKey = "circleerp.theme";

/**
 * Le a escolha salva.
 *
 * Em janela anonima ou com armazenamento bloqueado, o acesso lanca -- e um tema
 * nao e motivo para a aplicacao nao abrir.
 */
function readStoredTheme(): ThemeId {
  try {
    return (localStorage.getItem(storageKey) as ThemeId | null) ?? "system";
  } catch {
    return "system";
  }
}

function storeTheme(id: ThemeId): void {
  try {
    localStorage.setItem(storageKey, id);
  } catch {
    // Preferencia nao persistida vale menos do que a tela quebrada.
  }
}

/**
 * Fornece o tema escolhido para a aplicacao inteira.
 *
 * A escolha fica no navegador de quem usa, e nao no servidor: e preferencia de
 * exibicao daquele dispositivo, nao dado do negocio.
 */
export function ThemeModeProvider({ children }: { children: React.ReactNode }) {
  const [themeId, setThemeIdState] = useState<ThemeId>(readStoredTheme);
  const [prefersDark, setPrefersDark] = useState(
    () => window.matchMedia?.("(prefers-color-scheme: dark)").matches ?? false
  );

  // Com "Sistema" escolhido, mudar o tema do SO precisa refletir na hora, sem
  // recarregar a pagina.
  useEffect(() => {
    const query = window.matchMedia?.("(prefers-color-scheme: dark)");

    if (!query) return;

    const handleChange = (event: MediaQueryListEvent) => setPrefersDark(event.matches);

    query.addEventListener("change", handleChange);

    return () => query.removeEventListener("change", handleChange);
  }, []);

  const setThemeId = (id: ThemeId) => {
    setThemeIdState(id);
    storeTheme(id);
  };

  const theme = useMemo(() => buildTheme(themeId, prefersDark), [themeId, prefersDark]);

  const value = useMemo(() => ({ themeId, setThemeId }), [themeId]);

  return (
    <ThemeModeContext.Provider value={value}>
      <ThemeProvider theme={theme}>
        {/* enableColorScheme pinta tambem a barra de rolagem e os controles
            nativos, que de outro modo ficam claros num tema escuro. */}
        <CssBaseline enableColorScheme />
        {children}
      </ThemeProvider>
    </ThemeModeContext.Provider>
  );
}
