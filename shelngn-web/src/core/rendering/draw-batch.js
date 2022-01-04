import { assertParamNotFalse } from "../errors";
import { gl, canvas } from "./global";
import Shader from "./shader";

let began = false;
let queuedVerticesBuffer;
let queuedVerticesCount;

/** @type Shader */
let currentShader;

/** @type Shader */
let previousShader;

export const begin = () => {
  if (began) {
    throw new Error("Already began batching.");
  }

  began = true;
  queuedVerticesBuffer = [];
  queuedVerticesCount = 0;
};

export const flush = () => {
  const vertexBuffer = gl.createBuffer();
  gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
  gl.bufferData(
    gl.ARRAY_BUFFER,
    new Float32Array(queuedVerticesBuffer),
    gl.STATIC_DRAW
  );
  if (previousShader !== currentShader) {
    currentShader.init();
  }
  currentShader.bindShader();
  
  gl.bindBuffer(gl.ARRAY_BUFFER, null);

  gl.drawArrays(gl.POINTS, 0, queuedVerticesCount);

  previousShader = currentShader;
  began = false;
};

export const setShader = (newShader) => {
  if (!newShader) {
    throw new Error("newShader parameter is invalid");
  }
  if (newShader === currentShader) {
    return;
  }
  if (!began) {
    currentShader = newShader;
    return;
  }
  flush();
  currentShader = newShader;
  begin();
};

export const addElement = (vertices) => {
  queuedVerticesBuffer = [];
  queuedVerticesCount = 0;
  assertParamNotFalse("vertices", vertices);
  if (!currentShader) {
    throw new Error("Specify shader first.");
  }

  for (const vertexData of vertices) {
    currentShader.setVertexInArray(
      queuedVerticesBuffer,
      queuedVerticesCount,
      vertexData
    );
    queuedVerticesCount++;
  }
};
