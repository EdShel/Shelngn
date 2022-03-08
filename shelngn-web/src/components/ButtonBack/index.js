import React from "react";
import { useNavigate } from "react-router-dom";
import Button from "../Button";

const ButtonBack = (props) => {
  const navigate = useNavigate();
  return <Button onPress={(e) => e.preventDefault() || navigate(-1)} {...props} />;
};

export default ButtonBack;
