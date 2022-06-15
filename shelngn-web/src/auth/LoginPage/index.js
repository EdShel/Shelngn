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
import { useTranslation } from "react-i18next";

const LoginPage = () => {
  const emailField = "email";
  const passwordField = "password";

  const navigate = useNavigate();
  const { t } = useTranslation();

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
      <Form className={contentClassName} title={t("auth.signIn")}>
        <InputField
          labelText={t("auth.email")}
          name={emailField}
          value={formik.values[emailField]}
          onChange={formik.handleChange}
          required
        />
        <InputField
          labelText={t("auth.password")}
          name={passwordField}
          value={formik.values[passwordField]}
          onChange={formik.handleChange}
          type="password"
          required
        />
        <div className={styles.buttons}>
          <ButtonBack text={t("auth.back")} type="cancel" />
          <Button text={t("auth.login")} onPress={formik.handleSubmit} disabled={formik.isSubmitting} />
        </div>
      </Form>
    </ScreenLayout>
  );
};

export default LoginPage;
