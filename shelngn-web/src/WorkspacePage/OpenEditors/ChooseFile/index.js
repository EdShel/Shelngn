import React from "react";
import styles from "./styles.module.css";

const ChooseFile = () => {
  return (
    <div className={styles.container}>
        <h2>No file is chosen</h2>
      <ol>
        <li>
          Choose a <b>*.js</b> file to start writing code
        </li>
        <li>Drag and drop <b>resource files</b> into the project tree</li>
        <li>Press the <b>arrow</b> icon to play your game</li>
      </ol>
    </div>
  );
};

export default ChooseFile;
