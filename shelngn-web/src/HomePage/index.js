import React from "react";
import { Link } from "react-router-dom";
import UrlTo from "../UrlTo";

const HomePage = () => {
  return (
    <div>
      <Link to={UrlTo.home()}>Home</Link>
      <Link to={UrlTo.login()}>Login</Link>
      <Link to={UrlTo.register()}>Register</Link>
    </div>
  );
};

export default HomePage;
