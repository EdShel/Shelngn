import { useRef, useState } from "react";

const useDragArea = ({ onDropped } = {}) => {
  const [isDraggingOver, setDraggingOver] = useState(false);
  const dragCountRef = useRef(0);

  return {
    dragAreaProps: {
      onDragEnter: (ev) => {
        ev.preventDefault();
        ev.stopPropagation();
        if (dragCountRef.current++ === 0) {
          setDraggingOver(true);
        }
      },
      onDragOver: (ev) => {
        ev.preventDefault();
        ev.stopPropagation();

        const allowed = ev.dataTransfer.effectAllowed;
        console.log(allowed)
        ev.dataTransfer.dropEffect = ('move' === allowed || 'linkMove' === allowed) ? 'move' : 'copy';

        // ev.dataTransfer.dropEffect = "move";
      },
      onDragLeave: (ev) => {
        ev.preventDefault();
        ev.stopPropagation();
        if (--dragCountRef.current === 0) {
          setDraggingOver(false);
        }
      },
      onDrop: (ev) => {
        console.log('DROP')
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
