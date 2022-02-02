import React from "react";
import { Provider } from "react-redux";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import HomePage from "./HomePage";
import LoginPage from "./auth/LoginPage";
import RegisterPage from "./auth/RegisterPage";
import store from "./redux/store";
import UrlTo from "./UrlTo";

const App = () => {
  return (
    <Provider store={store}>
      <BrowserRouter>
        <Routes>
          <Route path={UrlTo.home()} element={<HomePage />} />
          <Route path={UrlTo.login()} element={<LoginPage />} />
          <Route path={UrlTo.register()} element={<RegisterPage />} />
        </Routes>
      </BrowserRouter>
    </Provider>
  );
};

export default App;
