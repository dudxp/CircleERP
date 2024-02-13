import Box from "@mui/material/Box";
import Drawer from "@mui/material/Drawer";
import CssBaseline from "@mui/material/CssBaseline";
import Toolbar from "@mui/material/Toolbar";
import Divider from "@mui/material/Divider";
import { Route,  Routes } from "react-router-dom";
import { RoutesPath, itemsList } from "shared";
import Currency from "pages/Cadastros/Currency";
import ItemList from "./ItemList";
import { Container, List, createTheme, styled } from "@mui/material";
import { ThemeProvider } from "@emotion/react";
import Temas from "styles/Tema.module.scss";

const DashboardDrawer = styled(Drawer)(
  () => ({
    "& .MuiDrawer-paper": {
      width: 150,
      position: "relative",
      whiteSpace: "nowrap",
      boxSizing: "border-box",
    },
}));


const defaultTheme = createTheme();

export default function Dashboard() {
 
  return (
    <ThemeProvider theme={defaultTheme}>
      <Box sx={{ display: "flex", flexDirection: "row" }}>
        <CssBaseline />
        <DashboardDrawer variant="permanent" >
          <Toolbar
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "flex-end",
              px: [1],
            }}
          >
            <img
              src="icons/cyncly-logo.png"
              alt="Logo da cyncly"
              className={Temas.logoCyncly}
            />
          </Toolbar>
          <Divider />
          <List component="nav">
            {itemsList.map((route, index) => {
              return <ItemList {...route} key={index}/>
            })}
          </List>
        </DashboardDrawer>

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
            <Routes>
              <Route element={<Currency />} path={RoutesPath.Currency} />
            </Routes>
          </Container>
        </Box>
      </Box>
    </ThemeProvider>
  );
}
