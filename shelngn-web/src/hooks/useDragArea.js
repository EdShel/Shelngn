import { useRef, useState } from "react";

const useDragArea = ({ onDropped } = {}) => {
  const [isDraggingOver, setDraggingOver] = useState(false);
  const dragCountRef = useRef(0);

  return {
    dragAreaProps: {
      onDragEnter: (ev) => {
        console.log('Herer')
        ev.preventDefault();
        ev.stopPropagation();
        if (dragCountRef.current++ === 0) {
          setDraggingOver(true);
        }
      },
      onDragOver: (ev) => {
        ev.preventDefault();
        ev.stopPropagation();

        console.log('ev.dataTransfer.items[0].type', ev.dataTransfer.items[0].type)
        
        if (
          ev.dataTransfer.items &&
          /image|application\/x-folderid|application\/x-fileid/.test(ev.dataTransfer.items[0].type)
        ) {
          const allowed = ev.dataTransfer.effectAllowed;
          ev.dataTransfer.dropEffect = "move" === allowed || "linkMove" === allowed ? "move" : "copy";
        } else {
          ev.dataTransfer.dropEffect = "none";
        }
      },
      onDragLeave: (ev) => {
        ev.preventDefault();
        ev.stopPropagation();
        if (--dragCountRef.current === 0) {
          setDraggingOver(false);
        }
      },
      onDrop: (ev) => {
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
