import React, { useEffect, useState } from "react";
import { getFileSource } from "../../../api";
import Scrollable from "../../../components/Scrollable";
import useWorkspaceId from "../../hooks/useWorkspaceId";
import styles from "./styles.module.css";

const ImagePreview = ({ currentFileId }) => {
  const workspaceId = useWorkspaceId();
  const [imageUrl, setImageUrl] = useState(null);

  useEffect(() => {
    let imageUrl = null;

    const fetchImage = async () => {
      const file = await getFileSource(workspaceId, currentFileId);
      console.log("file", file);
      imageUrl = URL.createObjectURL(file);
      setImageUrl(imageUrl);
    };
    fetchImage();

    return () => {
      if (imageUrl) {
        URL.revokeObjectURL(imageUrl);
      }
    };
  }, [currentFileId, workspaceId]);

  return (
    <Scrollable className={styles.scrollable}>
      <img src={imageUrl} alt="Preview" />
    </Scrollable>
  );
};

export default ImagePreview;
