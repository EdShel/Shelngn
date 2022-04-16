import React, { useState } from "react";
import Editor from "react-simple-code-editor";
import Scrollable from "../../components/Scrollable";
import { highlight, languages } from "prismjs/components/prism-core";
import "prismjs/components/prism-clike";
import "prismjs/components/prism-javascript";
import "prismjs/themes/prism-dark.min.css";
import styles from "./styles.module.css";
import clsx from "clsx";

const test = `  const CodeEditor = () => {
    return <Editor highlight={(a) => a} />;
  };
  
  export default CodeEditor;
  
`;

const hightlightWithLineNumbers = (input, language) =>
  highlight(input, language)
    .split("\n")
    .map((line, i) => `<span class='${styles["line-number"]}'>${i + 1}</span>${line}`)
    .join("\n");

const CodeEditor = ({ className }) => {
  const [src, setSrc] = useState(test);
  return (
    <Scrollable className={clsx(styles.container, className)}>
      <Editor
        className={styles.editor}
        highlight={(code) => hightlightWithLineNumbers(code, languages.js)}
        padding={10}
        value={src}
        onValueChange={setSrc}
      />
    </Scrollable>
  );
};

export default CodeEditor;
