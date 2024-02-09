import {
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from "@mui/material";
import InboxIcon from "@mui/icons-material/MoveToInbox";
import MailIcon from "@mui/icons-material/Mail";
import { Link } from "react-router-dom";
import React from "react";

interface Props {
  name: string;
  path: string;
  index: number;
}

export default function ItemList(props: Props) {
  const { name, path, index } = props;
  return (
    <React.Fragment>
      <ListItem key={index} disablePadding>
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
            {index % 2 === 0 ? <InboxIcon /> : <MailIcon />}
          </ListItemIcon>
          <ListItemText primary={name} />
        </ListItemButton>
      </ListItem>
    </React.Fragment>
  );
}
