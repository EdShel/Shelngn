const useDraggable = (dataType, dataValue) => {
  return {
    draggableProps: {
      onDragStart: (ev) => {
        ev.dataTransfer.dropEffect = "move";
        ev.dataTransfer.setData(dataType, dataValue);
      },
      draggable: true,
    },
  };
};
export default useDraggable;
