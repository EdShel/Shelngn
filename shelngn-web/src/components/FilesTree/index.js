import React, { useCallback, useState } from "react";
import arrowIcon from "./arrow.svg";
import useDragArea from "../../hooks/useDragArea";
import clsx from "clsx";
import styles from "./styles.module.css";
import useDraggable from "../../hooks/useDraggable";

const FilesTree = ({ root, onContextMenu, onDrop, onOpenFile, className }) => {
  const handleDrop = useCallback((e) => onDrop?.(e, root), [onDrop, root]);
  const { dragAreaProps, isDraggingOver } = useDragArea({ onDropped: handleDrop });
  const itemsProps = {
    onContextMenu,
    onDrop,
    onOpenFile,
  };
  return (
    <div
      className={clsx(styles["files-tree"], className, isDraggingOver && styles["folder-drag"])}
      {...dragAreaProps}
      onContextMenu={(e) => {
        e.folder = root;
        onContextMenu(e);
      }}
    >
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
  const { draggableProps } = useDraggable("application/x-folderid", folder.id);

  return (
    <div
      className={clsx(styles.folder, isDraggingOver && styles["folder-drag"])}
      {...dragAreaProps}
      onContextMenu={(e) => {
        e.folder = folder;
        onContextMenu(e);
      }}
    >
      <div className={styles.item} onClick={() => setVisible(!isVisible)} {...draggableProps}>
        <img src={arrowIcon} className={clsx(styles["arrow-icon"], !isVisible && styles.collapsed)} alt="Arrow" />{" "}
        {folder.name}
      </div>
      <div className={styles["folder-children"]} style={{ display: !isVisible && "none" }}>
        {children}
      </div>
    </div>
  );
};

const Item = ({ file, itemsProps: { onContextMenu, onOpenFile } }) => {
  const { draggableProps } = useDraggable("application/x-fileid", file.id);

  return (
    <div
      className={styles.item}
      onClick={() => onOpenFile(file)}
      onContextMenu={(e) => {
        e.file = file;
        onContextMenu(e);
      }}
      {...draggableProps}
    >
      {file.name}
    </div>
  );
};
