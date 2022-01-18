import React, { useEffect, useRef } from "react";
import {
  clearCanvas,
  initializeRenderer,
  renderBegin,
  unitializeRenderer,
} from "./core/rendering";
import "./App.css";
import Shader from "./core/rendering/shader";
import { FLOAT, FLOAT2, INT32 } from "./core/rendering/vertex-data-types";
import {
  begin,
  flush,
} from "./core/rendering/draw-batch";
import Texture from "./core/rendering/texture";
import Draw from "./core/rendering/Draw";

function App() {
  const canvasRef = useRef();

  useEffect(() => {
    if (!canvasRef.current) {
      return;
    }
    initializeRenderer(canvasRef.current);


    const texture = Texture.loadLazy("./logo192.png");

    let running = true;
    const render = () => {
      const x = Math.abs(-Math.sin(new Date().getTime() / 1000)) / 2;
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
      console.log("ded");
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
      </div>
    </div>
  );
}

export default App;
