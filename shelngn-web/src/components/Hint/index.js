import React, { useLayoutEffect, useRef, useState } from "react";
import clsx from "clsx";
import styles from "./styles.module.css";

const Hint = ({ className, children, renderContent, orientation = "bottom", arrowDistance = 10 }) => {
  return (
    <div className={clsx(styles.hint, className)} style={{ "--arrow-distance": `${arrowDistance}px` }}>
      {children}
      <Tooltip orientation={orientation}>{renderContent()}</Tooltip>
    </div>
  );
};

const TOOLTIP_PADDING_X = 8;

const Tooltip = ({ children, orientation }) => {
  const elRef = useRef(null);
  const [offset, setOffset] = useState(null);

  useLayoutEffect(() => {
    const { x, width } = elRef.current.getBoundingClientRect();
    
    if (orientation !== "right") {
      let offsetX = 0;
      if (x < TOOLTIP_PADDING_X) {
        offsetX = TOOLTIP_PADDING_X - x;
      } else if (x + width > window.innerWidth - TOOLTIP_PADDING_X) {
        offsetX = window.innerWidth - TOOLTIP_PADDING_X - (x + width);
      }
      setOffset({ x: offsetX });
    }
  }, [orientation]);

  return (
    <div className={clsx(styles["tooltip-container"], styles[orientation])}>
      <div ref={elRef} className={clsx(styles.tooltip, styles[orientation])} style={offset && { left: offset.x }}>
        {children}
      </div>
    </div>
  );
};

export default Hint;
