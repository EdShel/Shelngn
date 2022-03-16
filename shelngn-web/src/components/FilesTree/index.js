import React, { useState } from "react";
import styles from "./styles.module.css";
import arrowIcon from './arrow.svg';
import clsx from "clsx";

const FilesTree = ({ root }) => {
  return <div>{renderDirectory(root)}</div>;
};

const renderDirectory = (directory) => {
  return (
    <Folder key={directory.name} name={directory.name}>
      {directory.directories.map(renderDirectory)}
      {directory.files.map((f) => (
        <Item key={f.name} name={f.name} />
      ))}
    </Folder>
  );
};

export default FilesTree;

const Folder = ({ name, children }) => {
  const [isVisible, setVisible] = useState(true);

  return (
    <div className={styles.folder}>
      <div className={clsx(styles.item)} onClick={() => setVisible(!isVisible)}>
        <img src={arrowIcon} className={clsx(styles['arrow-icon'], !isVisible && styles.collapsed)} alt="Arrow" /> {name}
      </div>
      <div className={styles['folder-children']} style={{ display: !isVisible && "none" }}>{children}</div>
    </div>
  );
};
// FilesTree.Folder = Folder;

const Item = ({ name }) => {
  return <div>{name}</div>;
};
// FilesTree.Item = Item;
