import React from "react";
import { useTranslation } from "react-i18next";
import Button from "../../components/Button";

const PublishButton = ({ isPublished, isLoading, onTogglePublish }) => {
  const { t } = useTranslation();

  return (
    <Button
      text={isPublished ? t("options.publish") : t("options.unpublish")}
      onPress={onTogglePublish}
      disabled={isLoading}
    />
  );
};

export default PublishButton;
