import React from "react";
import { Provider } from "react-redux";
import Helmet from "react-helmet";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import store from "./store";
import { InfoAlertProvider } from "./InfoAlert";
import HomePage from "./HomePage";
import LoginPage from "./auth/LoginPage";
import RegisterPage from "./auth/RegisterPage";
import WorkspacePage from "./WorkspacePage";
import DebugPage from "./DebugPage";
import OptionsPage from "./OptionsPage";
import UrlTo from "./UrlTo";
import { AuthenticationProvider } from "./hooks/useAuth";
import PlayPage from "./PlayPage";
import "./i18n";

const App = () => {
  return (
    <Provider store={store}>
      <InfoAlertProvider>
        <BrowserRouter>
          <AuthenticationProvider>
            <Helmet>
              <title>Shelngn</title>
            </Helmet>
            <Routes>
              <Route path={UrlTo.home()} element={<HomePage />} />
              <Route path={UrlTo.login()} element={<LoginPage />} />
              <Route path={UrlTo.register()} element={<RegisterPage />} />
              <Route path={UrlTo.workspace(":id")} element={<WorkspacePage />} />
              <Route path={UrlTo.debug(":id")} element={<DebugPage />} />
              <Route path={UrlTo.options(":id")} element={<OptionsPage />} />
              <Route path={UrlTo.play(":id")} element={<PlayPage />} />
            </Routes>
          </AuthenticationProvider>
        </BrowserRouter>
      </InfoAlertProvider>
    </Provider>
  );
};

export default App;
