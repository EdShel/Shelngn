import { useState } from "react";

const useDraggable = () => {
  return {
    draggableProps: {
      onDragStart: (ev) => {
        ev.dataTransfer.dropEffect = "move";
        ev.dataTransfer.setData("application/x-resource");
      },
    },
  };
};
export default useDraggable;
