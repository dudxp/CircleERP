import {
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  SvgIcon,
} from "@mui/material";
import { Link } from "react-router-dom";
import React from "react";

interface Props {
  name: string;
  path: string;
  buttonIcon: typeof SvgIcon;
}

export default function ItemList(props: Props) {
  const { name, path, buttonIcon: ButtonIcon } = props;
  return (
    <React.Fragment>
      <ListItem disablePadding>
        <ListItemButton
          component={Link}
          to={path}
          sx={{
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <ListItemIcon sx={{ display: "contents" }}>
            <ButtonIcon/>
          </ListItemIcon>
          <ListItemText primary={name} />
        </ListItemButton>
      </ListItem>
    </React.Fragment>
  );
}
