import React from "react";
import { useFormik } from "formik";
import InputField from "../../components/InputField";
import Button from "../../components/Button";
import Form from "../Form";
import { useNavigate } from "react-router-dom";
import UrlTo from "../../UrlTo";
import styles from "./styles.module.css";
import ButtonBack from "../../components/ButtonBack";
import ScreenLayout, { contentClassName } from "../../components/ScreenLayout";
import { postLogin } from "../../api";

const LoginPage = () => {
  const emailField = "email";
  const passwordField = "password";

  const navigate = useNavigate();

  const formik = useFormik({
    initialValues: {
      [emailField]: "",
      [passwordField]: "",
    },
    onSubmit: async ({ email, password }) => {
      await postLogin({ email, password });
      navigate(UrlTo.home());
    },
  });

  return (
    <ScreenLayout>
      <Form className={contentClassName} title="Sign up">
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
    </ScreenLayout>
  );
};

export default LoginPage;
