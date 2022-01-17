import React, { useEffect, useRef } from "react";
import { instanceCreate } from "./core/ecs";
import {
  clearCanvas,
  initializeRenderer,
  renderBegin,
  unitializeRenderer,
} from "./core/rendering";
import "./App.css";
import Shader from "./core/rendering/shader";
import { FLOAT, FLOAT2, FLOAT4, INT32, SAMPLER2D } from "./core/rendering/vertex-data-types";
import {
  addElement,
  begin,
  flush,
  setShader,
  setTexture,
} from "./core/rendering/draw-batch";
import Texture from "./core/rendering/texture";

function App() {
  const canvasRef = useRef();

  useEffect(() => {
    initializeRenderer(canvasRef.current);
  
    const vsSource = `
      attribute vec2 a_position;
      attribute vec2 a_textureCoord;

      varying vec2 v_textureCoord;

      void main() {
        gl_Position = vec4(a_position, 0, 1.0);
        v_textureCoord = a_textureCoord;
      }
    `;
    const psSource = `
      varying mediump vec2 v_textureCoord;

      uniform mediump float u_tint;
      uniform sampler2D u_sampler;


      void main() {
        gl_FragColor = texture2D(u_sampler, v_textureCoord) * u_tint;
      }
    `;

    const triangleShader = Shader.compile(vsSource, psSource, {
      attributes: {
        a_position: FLOAT2,
        a_textureCoord: FLOAT2,
      },
      uniforms: {
        u_tint: FLOAT,
        u_sampler: INT32
      },
    });

    const texture = Texture.loadLazy('./logo192.png');
    
    let running = true;
    const render = () => {
      const x = Math.abs(-Math.sin(new Date().getTime() / 1000));
      triangleShader.setUniform("u_tint", 1);
      triangleShader.setUniform("u_sampler", texture);
      renderBegin();
      clearCanvas([0.2, 0.2, 0.2, 1]);
      setShader(triangleShader);
      setTexture(texture);
      begin();

      addElement([
        {
          a_position: [-1, x],
          a_textureCoord: [0, 0],
        },
        {
          a_position: [1, 0],
          a_textureCoord: [0, 1],
        },
        {
          a_position: [0, 1],
          a_textureCoord: [1, 1],
        },
      ]);

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
      <canvas
        ref={canvasRef}
        style={{ width: "100vh", height: "100vh" }}
      ></canvas>
    </div>
  );
}

export default App;
