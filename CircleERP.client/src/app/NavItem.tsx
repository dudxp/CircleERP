import { ListItem, ListItemButton, ListItemIcon, ListItemText } from "@mui/material";
import { Link, useLocation } from "react-router-dom";
import type { NavigationItem } from "./navigation";

export default function NavItem({ label, path, icon: Icon }: NavigationItem) {
  const { pathname } = useLocation();

  return (
    <ListItem disablePadding>
      <ListItemButton
        component={Link}
        to={path}
        selected={pathname === path}
        sx={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
        }}
      >
        <ListItemIcon sx={{ display: "contents" }}>
          <Icon />
        </ListItemIcon>
        <ListItemText primary={label} />
      </ListItemButton>
    </ListItem>
  );
}
