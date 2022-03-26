import React, { useCallback, useState } from "react";
import arrowIcon from "./arrow.svg";
import useDragArea from "../../hooks/useDragArea";
import clsx from "clsx";
import styles from "./styles.module.css";

const FilesTree = ({ root, onContextMenu, onDrop }) => {
  const itemsProps = {
    onContextMenu,
    onDrop,
  };
  return (
    <div className={styles["files-tree"]}>
      {root.directories.map((dir) => renderDirectory(dir, itemsProps))}
      {root.files.map((f) => (
        <Item key={f.name} file={f} itemsProps={itemsProps} />
      ))}
    </div>
  );
};

const renderDirectory = (directory, itemsProps) => {
  return (
    <Folder key={directory.name} folder={directory} itemsProps={itemsProps}>
      {directory.directories.map((dir) => renderDirectory(dir, itemsProps))}
      {directory.files.map((f) => (
        <Item key={f.name} file={f} itemsProps={itemsProps} />
      ))}
    </Folder>
  );
};

export default FilesTree;

const Folder = ({ folder, children, itemsProps: { onContextMenu, onDrop } }) => {
  const [isVisible, setVisible] = useState(true);
  const handleDrop = useCallback((e) => onDrop?.(e, folder), [onDrop, folder]);
  const { dragAreaProps, isDraggingOver } = useDragArea({ onDropped: handleDrop });

  return (
    <div className={clsx(styles.folder, isDraggingOver && styles["folder-drag"])} {...dragAreaProps}>
      <div className={styles.item} onClick={() => setVisible(!isVisible)}>
        <img src={arrowIcon} className={clsx(styles["arrow-icon"], !isVisible && styles.collapsed)} alt="Arrow" />{" "}
        {folder.name}
      </div>
      <div className={styles["folder-children"]} style={{ display: !isVisible && "none" }}>
        {children}
      </div>
    </div>
  );
};
// FilesTree.Folder = Folder;

const Item = ({ file, itemsProps: { onContextMenu } }) => {
  return (
    <div
      className={styles.item}
      onContextMenu={(e) => {
        e.file = file;
        onContextMenu(e);
      }}
      draggable
      onDragStart={(e) => {
        e.dataTransfer.dropEffect = "move";
        e.dataTransfer.setData("application/my-app", "blyaha");
        e.dataTransfer.setData("text/html", e.target.outerHTML);
      }}
    >
      {file.name}
    </div>
  );
};
// FilesTree.Item = Item;
