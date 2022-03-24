import React, { useState } from "react";
import styles from "./styles.module.css";
import arrowIcon from "./arrow.svg";
import clsx from "clsx";

const FilesTree = ({ root, onContextMenu }) => {
  const itemsProps = {
    onContextMenu,
  };
  return (
    <div>
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

const Folder = ({ folder, children, itemsProps: { onContextMenu } }) => {
  const [isVisible, setVisible] = useState(true);

  return (
    <div
      className={styles.folder}
      onContextMenu={(e) => {
        e.folder = folder;
        onContextMenu(e);
      }}
    >
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
    >
      {file.name}
    </div>
  );
};
// FilesTree.Item = Item;
