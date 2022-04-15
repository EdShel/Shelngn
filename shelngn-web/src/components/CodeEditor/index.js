import React, { useState } from "react";
import Editor from "react-simple-code-editor";
import { highlight, languages } from "prismjs/components/prism-core";
import "prismjs/components/prism-clike";
import "prismjs/components/prism-javascript";
import "prismjs/themes/prism-dark.min.css";

const test = `
const CodeEditor = () => {
    return <Editor highlight={(a) => a} />;
  };
  
  export default CodeEditor;
  
`;

const CodeEditor = () => {
  const [src, setSrc] = useState(test);
  return <Editor highlight={(code) => highlight(code, languages.js)} padding={10} value={src} onValueChange={setSrc} />;
};

export default CodeEditor;
