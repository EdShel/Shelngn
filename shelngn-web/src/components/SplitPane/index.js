import clsx from "clsx";
import React, { createRef, useEffect, useState } from "react";
import styles from "./styles.module.css";

const MIN_WIDTH = 0;

const SplitPane = ({ className, left, right }) => {
  const [separatorXPosition, setSeparatorXPosition] = useState();
  const [dragging, setDragging] = useState(false);
  const [leftWidth, setLeftWidth] = useState(undefined);
  const splitPaneRef = createRef();

  const onTouchStart = (e) => {
    setSeparatorXPosition(e.touches[0].clientX);
    setDragging(true);
  };

  const onMove = (clientX) => {
    if (dragging && leftWidth !== undefined && separatorXPosition) {
      const newLeftWidth = leftWidth + clientX - separatorXPosition;
      setSeparatorXPosition(clientX);

      if (newLeftWidth < MIN_WIDTH) {
        setLeftWidth(MIN_WIDTH);
        return;
      }

      if (splitPaneRef.current) {
        const splitPaneWidth = splitPaneRef.current.clientWidth;

        if (newLeftWidth > splitPaneWidth - MIN_WIDTH) {
          setLeftWidth(splitPaneWidth - MIN_WIDTH);
          return;
        }
      }

      setLeftWidth(newLeftWidth);
    }
  };

  const onMouseMove = (e) => {
    if (dragging) e.preventDefault();
    onMove(e.clientX);
  };

  const onTouchMove = (e) => {
    onMove(e.touches[0].clientX);
  };

  const onMouseUp = () => {
    setSeparatorXPosition(undefined);
    setDragging(false);
  };

  const onMouseDown = (e) => {
    setSeparatorXPosition(e.clientX);
    setDragging(true);
  };

  useEffect(() => {
    document.addEventListener("mousemove", onMouseMove);
    document.addEventListener("touchmove", onTouchMove);
    document.addEventListener("mouseup", onMouseUp);

    return () => {
      document.removeEventListener("mousemove", onMouseMove);
      document.removeEventListener("touchmove", onTouchMove);
      document.removeEventListener("mouseup", onMouseUp);
    };
  });

  return (
    <div ref={splitPaneRef} className={clsx(styles.container, className)}>
      <LeftPanel leftWidth={leftWidth} setLeftWidth={setLeftWidth}>
        {left}
      </LeftPanel>
      <div
        className={styles["divider-hitbox"]}
        onMouseDown={onMouseDown}
        onTouchStart={onTouchStart}
        onTouchEnd={onMouseUp}
      >
        <div className={styles.divider} />
      </div>
      <div className={styles.right}>{right}</div>
    </div>
  );
};

export default SplitPane;

const LeftPanel = ({ children, leftWidth, setLeftWidth }) => {
  const leftRef = createRef();

  useEffect(() => {
    if (leftRef.current) {
      if (leftWidth === undefined) {
        // setLeftWidth(leftRef.current?.clientWidth); Maybe use this
        setLeftWidth(240);
        return;
      }

      leftRef.current.style.width = `${leftWidth}px`;
    }
  }, [leftRef, leftWidth, setLeftWidth]);

  return (
    <div ref={leftRef} className={styles.left}>
      {children}
    </div>
  );
};
