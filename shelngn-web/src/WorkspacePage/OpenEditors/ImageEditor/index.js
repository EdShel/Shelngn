import React, { useEffect, useState } from "react";
import { getFileSource } from "../../../api";
import useWorkspaceId from "../../hooks/useWorkspaceId";

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
    <div>
      <img src={imageUrl} alt="Preview" />
    </div>
  );
};

export default ImagePreview;
