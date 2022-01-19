import React, { useEffect, useRef, useState } from "react";
import {
  clearCanvas,
  initializeRenderer,
  renderBegin,
  unitializeRenderer,
} from "./core/rendering";
import "./App.css";
import { begin, flush } from "./core/rendering/draw-batch";
import Texture from "./core/rendering/texture";
import Draw from "./core/rendering/Draw";
import Camera2D from "./core/rendering/Camera2D";
import { addFpsChangeListener, updateFps } from "./core/rendering/Fps";

function App() {
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

      const x = Math.abs(-Math.sin(new Date().getTime() / 1000)) * 512;

      Draw.applyCamera(new Camera2D(0, 0, 512, 512));
      renderBegin();
      clearCanvas([0.2, 0.2, 0.2, 1]);
      begin();

      Draw.drawTexture(texture, x, 0);
      Draw.drawTexture(texture, 0, x);
      Draw.drawTexture(texture, 0, -x);
      Draw.drawTexture(texture, -x, 0);
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
    <div className="App">
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
}

export default App;
