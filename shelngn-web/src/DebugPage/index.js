import React, { useEffect, useRef, useState } from "react";
import { clearCanvas, initializeRenderer, renderBegin, unitializeRenderer } from "../core/rendering";
import { begin, flush } from "../core/rendering/draw-batch";
import Texture from "../core/rendering/texture";
import Draw from "../core/rendering/Draw";
import Camera2D from "../core/rendering/Camera2D";
import GameScreen from "../core/rendering/GameScreen";
import useWorkspaceId from "../WorkspacePage/hooks/useWorkspaceId";
import { addFpsChangeListener, updateFps } from "../core/rendering/Fps";
import { getBuiltJsBundle, getBuiltResourceFile } from "../api";
import styles from "./styles.module.css";
import { deepEqual } from "./util";
import { useShowAlertNotification } from "../InfoAlert";

const DebugPage = () => {
  const workspaceId = useWorkspaceId();
  const [fps, setFps] = useState(0);
  const gameWebWorkerRef = useRef(null);
  const canvasRef = useRef();
  const { showError } = useShowAlertNotification();

  useEffect(() => {
    let running = true;

    const fetchGame = async () => {
      if (!canvasRef.current) {
        return;
      }
      initializeRenderer(canvasRef.current);
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
  }, []);

  useEffect(() => {
    const subscriber = addFpsChangeListener(setFps);
    return () => subscriber.remove();
  }, []);

  // useEffect(() => {
  //   if (!canvasRef.current) {
  //     return;
  //   }
  //   initializeRenderer(canvasRef.current);

  //   const texture = Texture.loadLazy("/logo192.png");

  //   let running = true;
  //   const render = () => {
  //     updateFps();
  //     const screenDimensions = GameScreen.getDimensions();

  //     const camera = new Camera2D(
  //       -screenDimensions.width / 2,
  //       -screenDimensions.height / 2,
  //       screenDimensions.width,
  //       screenDimensions.height
  //     );
  //     camera.rotation = new Date().getTime() / 10;
  //     camera.zoom = Math.sin(new Date().getTime() / 500);

  //     Draw.applyCamera(camera);
  //     renderBegin();
  //     clearCanvas([0.2, 0.2, 0.2, 1]);
  //     begin();

  //     const x = Math.abs(-Math.sin(new Date().getTime() / 1000)) * 512;
  //     Draw.drawTexture(texture, x - texture.width / 2, -texture.height / 2);
  //     Draw.drawTexture(texture, -texture.width / 2, x - texture.height / 2);
  //     Draw.drawTexture(texture, -texture.width / 2, -x - texture.height / 2);
  //     Draw.drawTexture(texture, -x - texture.width / 2, -texture.height / 2);
  //     flush();

  //     if (running) {
  //       window.requestAnimationFrame(render);
  //     }
  //   };
  //   window.requestAnimationFrame(render);

  //   return () => {
  //     console.log("cleaning up canvas");
  //     running = false;
  //     unitializeRenderer();
  //   };
  // }, []);

  return (
    <div className={styles.container}>
      <canvas
        ref={canvasRef}
        style={{
          position: "absolute",
          display: "block",
          width: "100%",
          height: "100%",
        }}
      />
      <div style={{ position: "absolute", left: 10, top: 10 }}>{fps} FPS</div>
    </div>
  );
};

export default DebugPage;
