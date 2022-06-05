import React from "react";
import { useTranslation } from "react-i18next";
import styles from "./styles.module.css";

const ChooseFile = () => {
  const { t } = useTranslation();

  return (
    <div className={styles.container}>
      <h2>{t("workspace.noFileChosen")}</h2>
      <ol>
        <li dangerouslySetInnerHTML={{ __html: t("workspace.chooseJsFile") }} />
        <li dangerouslySetInnerHTML={{ __html: t("workspace.dragNDrop") }} />
        <li dangerouslySetInnerHTML={{ __html: t("workspace.pressArrow") }} />
      </ol>
    </div>
  );
};

export default ChooseFile;
