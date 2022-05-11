import { initializeRenderer } from "../rendering";

export function runner(canvas, textureUriResolver) {
    let running = true;

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
        const textureAbsoluteUrl = getBuiltResourceFile(workspaceId, textureUrl);
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
      };

      const bundleSourceCode = await getBuiltJsBundle(workspaceId);
      const bundleBlob = new Blob([bundleSourceCode]);
      const scriptUrl = URL.createObjectURL(bundleBlob);
      gameWebWorkerRef.current = new Worker(scriptUrl);
      gameWebWorkerRef.current.onerror = e => {
        showError(e.message);
      }
      gameWebWorkerRef.current.onmessage = ({ data }) => {
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
        if (!gameWebWorkerRef.current) {
          return;
        }

        updateFps();

        let stateIncr = {};

        const screenDimensions = GameScreen.getDimensions();
        if (!deepEqual(state.screenDimensions, screenDimensions)) {
          state.screenDimensions = screenDimensions;
          stateIncr = { screenDimensions };
        }

        gameWebWorkerRef.current.postMessage({ type: "draw", stateIncr });
      };
      window.requestAnimationFrame(render);
    };
    fetchGame();

    return () => {
      gameWebWorkerRef.current?.terminate();
      running = false;
      unitializeRenderer();
    };
}