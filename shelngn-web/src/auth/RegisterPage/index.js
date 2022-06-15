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
import { useShowAlertNotification } from "../../InfoAlert";
import { postLogin, postRegister } from "../../api";
import { useTranslation } from "react-i18next";

const RegisterPage = () => {
  const emailField = "email";
  const passwordField = "password";
  const userNameField = "userName";

  const navigate = useNavigate();
  const { showError } = useShowAlertNotification();
  const {t} = useTranslation();

  const formik = useFormik({
    initialValues: {
      [emailField]: "",
      [passwordField]: "",
      [userNameField]: "",
    },
    onSubmit: async ({ email, password, userName }) => {
      try {
        await postRegister({ email, password, userName });
        await postLogin({ email, password });
        navigate(UrlTo.home());
      } catch (e) {
        const text = e.response?.data.Message;
        showError(text || "An error occurred");
      }
    },
  });

  return (
    <ScreenLayout>
      <Form className={contentClassName} title={t('auth.signUp')}>
        <InputField
          labelText={t('auth.email')}
          name={emailField}
          value={formik.values[emailField]}
          onChange={formik.handleChange}
          required
        />
        <InputField
          labelText={t('auth.password')}
          name={passwordField}
          value={formik.values[passwordField]}
          onChange={formik.handleChange}
          type="password"
          required
        />
        <InputField
          labelText={t('auth.userName')}
          name={userNameField}
          value={formik.values[userNameField]}
          onChange={formik.handleChange}
        />
        <div className={styles.buttons}>
          <ButtonBack text={t('auth.back')} type="cancel" />
          <Button text={t('auth.createAccount')} onPress={formik.handleSubmit} disabled={formik.isSubmitting} />
        </div>
      </Form>
    </ScreenLayout>
  );
};

export default RegisterPage;
