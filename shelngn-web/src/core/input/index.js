export function inputProvider() {
  let keyboardState = {};
  let newKeys = {};

  const handleKeyUp = (e) => {
    newKeys[e.key] = "up";
  };
  const handleKeyPress = (e) => {
    newKeys[e.key] = "press";
  };
  const handleKeyDown = (e) => {
    newKeys[e.key] = "down";
  };

  const handleUpdate = () => {
    const allKeys = [...Object.keys({ ...keyboardState, ...newKeys })];
    for (const key of allKeys) {
      switch (keyboardState[key]) {
        case undefined:
          keyboardState[key] = "down";
          break;
        case "down":
        case "press":
          if (newKeys[key] === "up") {
            keyboardState[key] = "up";
          } else {
            keyboardState[key] = "press";
          }
          break;
        case "up":
          if (newKeys[key] === "down" || newKeys[key] === "press") {
            keyboardState[key] = "down";
          } else {
            delete keyboardState[key];
          }
          break;
        default:
          throw new Error(`Invalid state '${keyboardState[key]}' for key '${key}'.`);
      }
    }
    newKeys = {};
    return { ...keyboardState };
  };

  window.addEventListener("keyup", handleKeyUp);
  window.addEventListener("keypress", handleKeyPress);
  window.addEventListener("keydown", handleKeyDown);

  return {
    update: handleUpdate,
    cleanup: () => {
      window.removeEventListener("keyup", handleKeyUp);
      window.removeEventListener("keypress", handleKeyPress);
      window.removeEventListener("keydown", handleKeyDown);
    },
  };
}
