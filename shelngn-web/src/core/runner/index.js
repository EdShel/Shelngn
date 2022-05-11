import { inputProvider } from "../input";
import { clearCanvas, initializeRenderer, renderBegin, unitializeRenderer } from "../rendering";
import Camera2D from "../rendering/Camera2D";
import Draw from "../rendering/Draw";
import { begin, flush } from "../rendering/draw-batch";
import { updateFps } from "../rendering/Fps";
import GameScreen from "../rendering/GameScreen";
import Texture from "../rendering/texture";
import { deepEqual } from "../util/deepEqual";

export function runner(canvas, { loadBundle, textureUriResolver, onError }) {
  let running = true;
  let gameWebWorker = null;

  const input = inputProvider();

  const fetchGame = async () => {
    if (!canvas) {
      return;
    }
    initializeRenderer(canvas);
    let loadedTextures = {};

    const getTexture = (textureUrl) => {
      if (loadedTextures[textureUrl]) {
        return loadedTextures[textureUrl];
      }
      const textureAbsoluteUrl = textureUriResolver(textureUrl);
      return (loadedTextures[textureUrl] = Texture.loadLazy(textureAbsoluteUrl));
    };

    const proxyObjects = {
      Draw: {
        texture(textureUrl, x, y) {
          const txt = getTexture(textureUrl);
          Draw.texture(txt, x, y);
        },
        stretchedTexture(textureUrl, x, y, width, height, rotation, origin) {
          const txt = getTexture(textureUrl);
          Draw.stretchedTexture(txt, x, y, width, height, rotation, origin);
        },
      },
    };

    let state = {
      screenDimensions: { width: 0, height: 0 },
      input: {},
    };

    const bundleSourceCode = await loadBundle();
    const bundleBlob = new Blob([bundleSourceCode]);
    const scriptUrl = URL.createObjectURL(bundleBlob);
    gameWebWorker = new Worker(scriptUrl);
    gameWebWorker.onerror = (e) => {
      onError(e.message);
    };
    gameWebWorker.onmessage = ({ data }) => {
      const camera = new Camera2D(
        -state.screenDimensions.width / 2,
        -state.screenDimensions.height / 2,
        state.screenDimensions.width,
        state.screenDimensions.height
      );
      Draw.applyCamera(camera);

      renderBegin();
      clearCanvas([0.2, 0.2, 0.2, 1]);
      begin();

      for (const drawCall of data) {
        const [objectName, functionName, ...args] = drawCall;
        proxyObjects[objectName][functionName](...args);
      }
      flush();
      if (running) {
        window.requestAnimationFrame(render);
      }
    };
    URL.revokeObjectURL(scriptUrl);

    const render = () => {
      if (!gameWebWorker) {
        return;
      }

      updateFps();

      let stateIncr = {};

      const screenDimensions = GameScreen.getDimensions();
      if (!deepEqual(state.screenDimensions, screenDimensions)) {
        state.screenDimensions = screenDimensions;
        stateIncr = { ...stateIncr, screenDimensions };
      }

      const newInputState = input.update();
      if (!deepEqual(state.input, newInputState)) {
        state.input = newInputState;
        stateIncr = { ...stateIncr, input: newInputState };
        console.log('SENDING', newInputState)
      }

      gameWebWorker.postMessage({ type: "draw", stateIncr });
    };
    window.requestAnimationFrame(render);
  };
  fetchGame();

  return {
    cleanup: () => {
      gameWebWorker?.terminate();
      input.cleanup();
      running = false;
      unitializeRenderer();
    },
  };
}
