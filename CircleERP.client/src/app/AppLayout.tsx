import {
  Box,
  Container,
  CssBaseline,
  Divider,
  Drawer,
  List,
  Toolbar,
  styled,
} from "@mui/material";
import Logo from "@shared/ui/Logo";
import NavItem from "./NavItem";
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
      <CssBaseline />

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
      </SideDrawer>

      <Box
        component="main"
        sx={{
          backgroundColor: (theme) =>
            theme.palette.mode === "light"
              ? theme.palette.grey[100]
              : theme.palette.grey[900],
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
