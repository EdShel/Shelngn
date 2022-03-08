import React from "react";
import { useDispatch } from "react-redux";
import { useFormik } from "formik";
import InputField from "../../components/InputField";
import SideBar from "../../components/SideBar";
import ScreenContainer from "../../components/ScreenContainer";
import Button from "../../components/Button";
import Form from "../Form";
import { useNavigate } from "react-router-dom";
import { login } from "../reducer";
import UrlTo from "../../UrlTo";
import styles from "./styles.module.css";
import ButtonBack from "../../components/ButtonBack";

const LoginPage = () => {
  const emailField = "email";
  const passwordField = "password";

  const navigate = useNavigate();
  const dispatch = useDispatch();

  const formik = useFormik({
    initialValues: {
      [emailField]: "",
      [passwordField]: "",
    },
    onSubmit: async ({ email, password }) => {
      await dispatch(login({ email, password })).unwrap();
      navigate(UrlTo.home());
    },
  });

  return (
    <ScreenContainer>
      <SideBar />
      <Form title="Sign up">
        <InputField
          labelText="Email"
          name={emailField}
          value={formik.values[emailField]}
          onChange={formik.handleChange}
          required
        />
        <InputField
          labelText="Password"
          name={passwordField}
          value={formik.values[passwordField]}
          onChange={formik.handleChange}
          type="password"
          required
        />
        <div className={styles.buttons}>
          <ButtonBack text="Back" type="cancel" />
          <Button text="Login" onPress={formik.handleSubmit} disabled={formik.isSubmitting} />
        </div>
      </Form>
    </ScreenContainer>
  );
};

export default LoginPage;
