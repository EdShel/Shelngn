import React, { useEffect, useRef, useState } from "react";
import clsx from "clsx";
import styles from "./styles.module.css";

const Hint = ({ className, children, renderContent }) => {
  return (
    <div className={clsx(styles.hint, className)}>
      {children}
      <Tooltip>{renderContent()}</Tooltip>
    </div>
  );
};

const TOOLTIP_PADDING_X = 8;

const Tooltip = ({ children }) => {
  const elRef = useRef(null);
  const [offset, setOffset] = useState(null);

  useEffect(() => {
    const { x, width } = elRef.current.getBoundingClientRect();
    let offsetX = 0;
    if (x < TOOLTIP_PADDING_X) {
      offsetX = TOOLTIP_PADDING_X - x;
    } else if (x + width > window.innerWidth - TOOLTIP_PADDING_X) {
      offsetX = window.innerWidth - TOOLTIP_PADDING_X - (x + width);
    }
    setOffset({ x: offsetX });
  }, []);
  return (
    <div ref={elRef} className={styles.tooltip} style={offset && { left: offset.x }}>
      {children}
    </div>
  );
};

export default Hint;
