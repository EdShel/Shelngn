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
import { FLOAT, FLOAT2, FLOAT4 } from "./core/rendering/vertex-data-types";
import {
  addElement,
  begin,
  flush,
  setShader,
} from "./core/rendering/draw-batch";

function App() {
  const canvasRef = useRef();

  useEffect(() => {
    initializeRenderer(canvasRef.current);
    console.log("init");

    // const imageTexture = new Image();
    // imageTexture.src = require('./logo192.png').default;
    // imageTexture.onload = () => {
    //   instanceCreate({
    //     x: 0,
    //     y: 0,
    //     w: 80,
    //     h: 80,
    //     draw() {

    //     }
    //   })
    // }

    // return () => {
    //   unitializeRenderer();
    // }
    const vsSource = `
    
      attribute vec2 a_position;
      attribute vec4 a_color;

      varying vec4 v_color;

      void main() {
        gl_Position = vec4(a_position, 0, 1.0);
        v_color = a_color;
        gl_PointSize = 30.0;
      }
    `;
    const psSource = `
      uniform mediump float u_tint;

      varying mediump vec4 v_color;

      void main() {
        gl_FragColor = v_color * u_tint;
      }
    `;

    // const vs = gl.createShader(gl.VERTEX_SHADER);
    // gl.shaderSource(vs, vsSource);
    // gl.compileShader(vs);
    // console.log(gl.getShaderInfoLog(vs));
    // const ps = gl.createShader(gl.FRAGMENT_SHADER);
    // gl.shaderSource(ps, psSource);
    // gl.compileShader(ps);
    // console.log(gl.getShaderInfoLog(ps));
    // const program = gl.createProgram();
    // gl.attachShader(program, vs);
    // gl.attachShader(program, ps);
    // gl.linkProgram(program);
    // gl.useProgram(program);
    // const vs_a_position = gl.getAttribLocation(program, 'a_position');
    // const vs_a_color = gl.getAttribLocation(program, 'a_color');
    // const vs_u_tint = gl.getUniformLocation(program, 'u_tint');

    // const vertices = [
    //   0, 0, /**/ 1, 0, 0, 1,
    //   1, 0, /**/ 0, 1, 0, 1,
    //   0, 1, /**/ 0, 0, 0, 1,
    //   1, 1, /**/ 1, 0, 0, 1,
    // ];
    // const vertexBuffer = gl.createBuffer();
    // gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
    // gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW);
    // gl.bindBuffer(gl.ARRAY_BUFFER, null);

    // const indices = [
    //   0, 1, 2,
    //   1, 3, 2,
    // ];
    // const indexBuffer = gl.createBuffer();
    // gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, indexBuffer);
    // gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(indices), gl.STATIC_DRAW);
    // gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, null);

    // gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
    // gl.vertexAttribPointer(vs_a_position, 2, gl.FLOAT, false, 24, 0);
    // gl.vertexAttribPointer(vs_a_color, 4, gl.FLOAT, false, 24, 8);
    // gl.enableVertexAttribArray(vs_a_position);
    // gl.enableVertexAttribArray(vs_a_color);

    // gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, indexBuffer);
    // gl.enableVertexAttribArray(gl.ELEMENT_ARRAY_BUFFER);

    // gl.uniform1f(vs_u_tint, 1);

    // gl.clearColor(1, 1, 0, 1);
    // gl.clear(gl.COLOR_BUFFER_BIT);
    // gl.viewport(0, 0, canvas.width, canvas.height);

    // gl.drawElements(gl.TRIANGLES, indices.length, gl.UNSIGNED_SHORT, 0);

    const triangleShader = Shader.compile(vsSource, psSource, {
      attributes: {
        a_position: FLOAT2,
        a_color: FLOAT4,
      },
      uniforms: {
        u_tint: FLOAT,
      },
    });

    let running = true;
    const render = () => {
      const x = Math.abs(-Math.sin(new Date().getTime() / 1000));
      triangleShader.setUniform("u_tint", 1);
      renderBegin();
      clearCanvas([0.2, 0.2, 0.2, 1]);
      setShader(triangleShader);
      begin();

      addElement([
        {
          a_position: [-1, x],
          a_color: [x, 0, 0, 1],
        },
        {
          a_position: [1, 0],
          a_color: [0, 1, 0, 1],
        },
        {
          a_position: [0, 1],
          a_color: [0, 0, 1, 1],
        },
        // {
        //   a_position: [1, 1],
        //   a_color: [1, 0, 0, 1],
        // },
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
