import React from "react";
import { Provider } from "react-redux";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import store from "./store";
import { InfoAlertProvider } from "./InfoAlert";
import HomePage from "./HomePage";
import LoginPage from "./auth/LoginPage";
import RegisterPage from "./auth/RegisterPage";
import WorkspacePage from "./WorkspacePage";
import UrlTo from "./UrlTo";

const App = () => {
  return (
    <Provider store={store}>
      <InfoAlertProvider>
        <BrowserRouter>
          <Routes>
            <Route path={UrlTo.home()} element={<HomePage />} />
            <Route path={UrlTo.login()} element={<LoginPage />} />
            <Route path={UrlTo.register()} element={<RegisterPage />} />
            <Route path={UrlTo.workspace(":id")} element={<WorkspacePage />} />
          </Routes>
        </BrowserRouter>
      </InfoAlertProvider>
    </Provider>
  );
};

export default App;
