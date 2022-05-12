import React from "react";
import Button from "../../components/Button";

const PublishButton = ({ isPublished, isLoading, onTogglePublish }) => {
  return <Button text={isPublished ? "Unpublish" : "Publish"} onPress={onTogglePublish} disabled={isLoading} />;
};

export default PublishButton;
