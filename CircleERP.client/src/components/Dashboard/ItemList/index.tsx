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
  buttonIcon: typeof SvgIcon | string;
}

import OrderIcon from "@assets/Icons/order.svg";

export default function ItemList(props: Props) {
  const { name, path, buttonIcon: ButtonIcon } = props;

  console.log("AQUIII");
  console.log(OrderIcon);

  return (
    <React.Fragment>
      
      <img 
        src={OrderIcon} 
        alt="Order"
        style={{ width: "50px", height: "50px" }}
      />
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
