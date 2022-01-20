import { canvas } from "./global";

const GameScreen = {
  getDimensions() {
    return {
      width: canvas.width,
      height: canvas.height,
    };
  },
};
export default GameScreen;
