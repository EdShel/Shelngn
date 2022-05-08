import React from "react";
import { useDispatch } from "react-redux";
import { useFormik } from "formik";
import InputField from "../../components/InputField";
import Button from "../../components/Button";
import Form from "../Form";
import { useNavigate } from "react-router-dom";
import { login, register } from "../reducer";
import UrlTo from "../../UrlTo";
import styles from "./styles.module.css";
import ButtonBack from "../../components/ButtonBack";
import ScreenLayout, { contentClassName } from "../../components/ScreenLayout";
import { useShowAlertNotification } from "../../InfoAlert";

const RegisterPage = () => {
  const emailField = "email";
  const passwordField = "password";
  const userNameField = "userName";

  const navigate = useNavigate();
  const dispatch = useDispatch();
  const {showError} = useShowAlertNotification();

  const formik = useFormik({
    initialValues: {
      [emailField]: "",
      [passwordField]: "",
      [userNameField]: "",
    },
    onSubmit: async ({ email, password, userName }) => {
      try {
        await dispatch(register({ email, password, userName })).unwrap();
        await dispatch(login({ email, password })).unwrap();
        navigate(UrlTo.home());
      } catch(e){
        const text = e.response?.data.Message;
        showError(text || 'An error occurred')
      }
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
        <InputField
          labelText="User name"
          name={userNameField}
          value={formik.values[userNameField]}
          onChange={formik.handleChange}
        />
        <div className={styles.buttons}>
          <ButtonBack text="Back" type="cancel" />
          <Button text="Create account" onPress={formik.handleSubmit} disabled={formik.isSubmitting} />
        </div>
      </Form>
    </ScreenLayout>
  );
};

export default RegisterPage;
