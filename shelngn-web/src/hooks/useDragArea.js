import { useRef, useState } from "react";

const useDragArea = ({ onDropped, name } = {}) => {
  const [isDraggingOver, setDraggingOver] = useState(false);
  const dragCountRef = useRef(0);

  return {
    dragAreaProps: {
      onDragEnter: (ev) => {
        console.log('Enter')
        ev.preventDefault();
        ev.stopPropagation();
        if (dragCountRef.current++ === 0) {
          setDraggingOver(true);
        }
      },
      onDragOver: (ev) => {
        ev.preventDefault();
        console.log('Over')
        ev.stopPropagation();
        ev.dataTransfer.dropEffect = "move";
      },
      onDragLeave: (ev) => {
        console.log('Leave')
        ev.preventDefault();
        ev.stopPropagation();
        if (--dragCountRef.current === 0) {
          setDraggingOver(false);
        }
      },
      onDrop: (ev) => {
        console.log('Drop')
        ev.preventDefault();
        ev.stopPropagation();
        setDraggingOver(false);
        dragCountRef.current = 0;
        onDropped?.(ev);
      },
    },
    isDraggingOver,
  };
};

export default useDragArea;
