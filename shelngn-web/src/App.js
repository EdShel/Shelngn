import React, { useEffect, useRef } from 'react';
import './App.css';

function createShader(gl, type, source) {
  var shader = gl.createShader(type);
  gl.shaderSource(shader, source);
  gl.compileShader(shader);
  var success = gl.getShaderParameter(shader, gl.COMPILE_STATUS);
  if (success) {
    return shader;
  }

  console.log(gl.getShaderInfoLog(shader));
  gl.deleteShader(shader);
}

function createProgram(gl, vertexShader, fragmentShader) {
  var program = gl.createProgram();
  gl.attachShader(program, vertexShader);
  gl.attachShader(program, fragmentShader);
  gl.linkProgram(program);
  var success = gl.getProgramParameter(program, gl.LINK_STATUS);
  if (success) {
    return program;
  }

  console.log(gl.getProgramInfoLog(program));
  gl.deleteProgram(program);
}


function observeCanvasSize(canvas) {
  const canvasToDisplaySizeMap = new Map([[canvas, [640, 480]]]);

  function onResize(entries) {
    for (const entry of entries) {
      let width;
      let height;
      let dpr = window.devicePixelRatio;
      if (entry.devicePixelContentBoxSize) {
        // NOTE: Only this path gives the correct answer
        // The other 2 paths are an imperfect fallback
        // for browsers that don't provide anyway to do this
        width = entry.devicePixelContentBoxSize[0].inlineSize;
        height = entry.devicePixelContentBoxSize[0].blockSize;
        dpr = 1; // it's already in width and height
      } else if (entry.contentBoxSize) {
        if (entry.contentBoxSize[0]) {
          width = entry.contentBoxSize[0].inlineSize;
          height = entry.contentBoxSize[0].blockSize;
        } else {
          // legacy
          width = entry.contentBoxSize.inlineSize;
          height = entry.contentBoxSize.blockSize;
        }
      } else {
        // legacy
        width = entry.contentRect.width;
        height = entry.contentRect.height;
      }
      const displayWidth = Math.round(width * dpr);
      const displayHeight = Math.round(height * dpr);
      canvasToDisplaySizeMap.set(entry.target, [displayWidth, displayHeight]);
    }
  }

  const resizeObserver = new ResizeObserver(onResize);
  resizeObserver.observe(canvas, { box: 'content-box' });

  function resizeCanvasToDisplaySize() {
    // Get the size the browser is displaying the canvas in device pixels.
    const [displayWidth, displayHeight] = canvasToDisplaySizeMap.get(canvas);

    // Check if the canvas is not the same size.
    const needResize = canvas.width !== displayWidth ||
      canvas.height !== displayHeight;

    if (needResize) {
      // Make the canvas the same size
      canvas.width = displayWidth;
      canvas.height = displayHeight;
    }

    return needResize;
  }

  return {
    cleanup: () => resizeObserver.unobserve(canvas),
    resizeCanvas: () => resizeCanvasToDisplaySize(),
  }
}


function App() {
  const canvasRef = useRef();

  const startGameDrawing = (textureImage) => {
    const canvas = canvasRef.current;
    const gl = canvas.getContext('webgl');
    if (!gl) {
      throw new Error('Web gl not supported.');
    }

    const vs = `
    attribute vec2 a_position;
   
    uniform vec2 u_resolution;

    attribute vec2 a_texCoord;

    varying vec2 v_texCoord;
 
    void main() {
      // convert the position from pixels to 0.0 to 1.0
      vec2 zeroToOne = a_position / u_resolution;
   
      // convert from 0->1 to 0->2
      vec2 zeroToTwo = zeroToOne * 2.0;
   
      // convert from 0->2 to -1->+1 (clip space)
      vec2 clipSpace = zeroToTwo - 1.0;
   
      gl_Position = vec4(clipSpace * vec2(1, -1), 0, 1);

      v_texCoord = a_texCoord;
    }
    `;
    const fs = `
    precision mediump float;

    uniform vec4 u_fillColor;
    uniform sampler2D u_texture;
    varying vec2 v_texCoord;
 
    void main() {
      gl_FragColor = texture2D(u_texture, v_texCoord) * u_fillColor;
    }
    `;

    const vertexShader = createShader(gl, gl.VERTEX_SHADER, vs);
    const fragmentShader = createShader(gl, gl.FRAGMENT_SHADER, fs);

    const program = createProgram(gl, vertexShader, fragmentShader);

    const resolutionUniformLocation = gl.getUniformLocation(program, "u_resolution");
    const positionAttributeLocation = gl.getAttribLocation(program, "a_position");
    const texCoordAttributeLocation = gl.getAttribLocation(program, "a_texCoord");

    const fillColorUniformLocation = gl.getUniformLocation(program, "u_fillColor");
    // const textureUniformLocation = gl.getUniformLocation(program, "u_texture");

    const positionBuffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
    const x = 0;
    const y = 0;
    const w = 100;
    const h = 100;
    const positions = [
      x, y,
      x + w, y,
      x + w, y + h,
      x, y,
      x + w, y + h,
      x, y + h,
    ];
    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(positions), gl.STATIC_DRAW);

    const texCoordBuffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, texCoordBuffer);
    const uvs = [
      0, 0,
      1, 0,
      1, 1,
      0, 0,
      1, 1,
      0, 1
    ];
    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(uvs), gl.STATIC_DRAW);

    const texture = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, texture);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, textureImage);

    // Below is rendering
    const { cleanup, resizeCanvas } = observeCanvasSize(canvas);

    function render() {
      resizeCanvas();
      gl.viewport(0, 0, gl.canvas.width, gl.canvas.height);

      gl.clearColor(0, 0, 0, 0);
      gl.clear(gl.COLOR_BUFFER_BIT);

      gl.useProgram(program);

      gl.uniform2f(resolutionUniformLocation, gl.canvas.width, gl.canvas.height);
      gl.uniform4f(fillColorUniformLocation, 1, 1, 1, 1);

      gl.enableVertexAttribArray(positionAttributeLocation);
      // Bind the position buffer.
      gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);

      // Tell the attribute how to get data out of positionBuffer (ARRAY_BUFFER)
      var size = 2;          // 2 components per iteration
      var type = gl.FLOAT;   // the data is 32bit floats
      var normalize = false; // don't normalize the data
      var stride = 0;        // 0 = move forward size * sizeof(type) each iteration to get the next position
      var offset = 0;        // start at the beginning of the buffer
      gl.vertexAttribPointer(
        positionAttributeLocation, size, type, normalize, stride, offset)

      gl.enableVertexAttribArray(texCoordAttributeLocation);
      gl.bindBuffer(gl.ARRAY_BUFFER, texCoordBuffer);
      gl.vertexAttribPointer(texCoordAttributeLocation, 2, gl.FLOAT, false, 0, 0);

      var primitiveType = gl.TRIANGLES;
      var offset = 0;
      var count = 6;
      gl.drawArrays(primitiveType, offset, count);

      console.log('call')
    }

    render();

    return () => {
      cleanup();
    }
  }

  useEffect(() => {
    const imageTexture = new Image();
    imageTexture.src = require('./logo192.png').default;
    imageTexture.onload = () => startGameDrawing(imageTexture);
  }, [])

  return (
    <div className="App">
      <canvas ref={canvasRef}></canvas>
    </div>
  );
}

export default App;
