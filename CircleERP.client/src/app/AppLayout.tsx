import { Box, Container, Divider, Drawer, List, Toolbar, styled } from "@mui/material";
import Logo from "@shared/ui/Logo";
import NavItem from "./NavItem";
import ThemeModeSelector from "./theme/ThemeModeSelector";
import { navigationItems } from "./navigation";

const SideDrawer = styled(Drawer)(() => ({
  "& .MuiDrawer-paper": {
    width: 150,
    position: "relative",
    whiteSpace: "nowrap",
    boxSizing: "border-box",
  },
}));

/**
 * Moldura da aplicacao: menu lateral e area de conteudo. Nao conhece as rotas
 * em si -- recebe a pagina ja resolvida como filho.
 */
export default function AppLayout({ children }: { children: React.ReactNode }) {
  return (
    <Box sx={{ display: "flex", flexDirection: "row" }}>
      <SideDrawer variant="permanent">
        <Toolbar
          sx={{ display: "flex", alignItems: "center", justifyContent: "flex-end", px: [1] }}
        >
          <Logo />
        </Toolbar>
        <Divider />
        <List component="nav">
          {navigationItems.map((item) => (
            <NavItem key={item.path} {...item} />
          ))}
        </List>

        <Divider sx={{ mt: "auto" }} />
        <Box sx={{ display: "flex", justifyContent: "center", py: 1 }}>
          <ThemeModeSelector />
        </Box>
      </SideDrawer>

      <Box
        component="main"
        sx={{
          // background.default vem da paleta escolhida; fixar em grey[100]
          // ou grey[900] ignoraria os temas customizados.
          backgroundColor: "background.default",
          flexGrow: 1,
          height: "100vh",
          overflow: "auto",
        }}
      >
        <Toolbar />
        <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
          {children}
        </Container>
      </Box>
    </Box>
  );
}
