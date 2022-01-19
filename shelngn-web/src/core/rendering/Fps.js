let fps = 0;

let frameCount = 0;
let previousTime = 0;
let elapsedTime = 0;

const fpsObservers = [];

export const updateFps = () => {
  const now = new Date().getTime();

  frameCount++;
  elapsedTime += now - (previousTime || now);
  previousTime = now;

  if (elapsedTime >= 1000) {
    fps = frameCount;
    frameCount = 0;
    elapsedTime -= 1000;

    for (const listener of fpsObservers) {
      listener(fps);
    }
  }
};

export const getFps = () => fps;

export const addFpsChangeListener = (handlerFunction) => {
  fpsObservers.push(handlerFunction);
  return {
    remove: () => {
      const index = fpsObservers.indexOf(handlerFunction);
      if (index === -1) {
        throw new Error("Attempt to unsubscribe multiple times.");
      }
      fpsObservers.splice(index, 1);
    },
  };
};
