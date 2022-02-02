import React, { useEffect, useRef, useState } from "react";
import {
  clearCanvas,
  initializeRenderer,
  renderBegin,
  unitializeRenderer,
} from "../core/rendering";
import { begin, flush } from "../core/rendering/draw-batch";
import Texture from "../core/rendering/texture";
import Draw from "../core/rendering/Draw";
import Camera2D from "../core/rendering/Camera2D";
import { addFpsChangeListener, updateFps } from "../core/rendering/Fps";
import GameScreen from "../core/rendering/GameScreen";
import styles from "./styles.module.css";

const ProjectPage = () => {
  const [fps, setFps] = useState(0);
  const canvasRef = useRef();

  useEffect(() => {
    const subscriber = addFpsChangeListener(setFps);
    return () => subscriber.remove();
  }, []);

  useEffect(() => {
    if (!canvasRef.current) {
      return;
    }
    initializeRenderer(canvasRef.current);

    const texture = Texture.loadLazy("./logo192.png");

    let running = true;
    const render = () => {
      updateFps();
      const screenDimensions = GameScreen.getDimensions();

      const camera = new Camera2D(
        -screenDimensions.width / 2,
        -screenDimensions.height / 2,
        screenDimensions.width,
        screenDimensions.height
      );
      camera.rotation = new Date().getTime() / 10;
      camera.zoom = Math.sin(new Date().getTime() / 500);

      Draw.applyCamera(camera);
      renderBegin();
      clearCanvas([0.2, 0.2, 0.2, 1]);
      begin();

      const x = Math.abs(-Math.sin(new Date().getTime() / 1000)) * 512;
      Draw.drawTexture(texture, x - texture.width / 2, -texture.height / 2);
      Draw.drawTexture(texture, -texture.width / 2, x - texture.height / 2);
      Draw.drawTexture(texture, -texture.width / 2, -x - texture.height / 2);
      Draw.drawTexture(texture, -x - texture.width / 2, -texture.height / 2);
      flush();

      if (running) {
        window.requestAnimationFrame(render);
      }
    };
    window.requestAnimationFrame(render);

    return () => {
      console.log("cleaning up canvas");
      running = false;
      unitializeRenderer();
    };
  }, []);

  return (
    <div className={styles.container}>
      <div style={{ position: "relative", width: "100%", height: "100%" }}>
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
    </div>
  );
};

export default ProjectPage;
