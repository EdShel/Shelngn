import { assertParamNotFalse } from "../errors";
import { gl, canvas } from "./global";
import Shader from "./shader"; // eslint-disable-line no-unused-vars

let began = false;
let queuedVerticesBuffer;
let queuedVerticesCount;
let queuedIndicesBuffer;

/** @type Shader */
let currentShader;

/** @type Shader */
let previousShader;

let currentTexture;

export const begin = () => {
  if (began) {
    throw new Error("Already began batching.");
  }

  began = true;
  queuedVerticesBuffer = [];
  queuedVerticesCount = 0;
  queuedIndicesBuffer = [];
};

export const flush = () => {
  const vertexBuffer = gl.createBuffer(); // TODO: reuse from previous call
  gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
  gl.bufferData(
    gl.ARRAY_BUFFER,
    new Float32Array(queuedVerticesBuffer),
    gl.STATIC_DRAW
  );
  const indexBuffer = gl.createBuffer(); // TODO: reuse from previous call
  gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, indexBuffer);
  gl.bufferData(
    gl.ELEMENT_ARRAY_BUFFER,
    new Uint16Array(queuedIndicesBuffer),
    gl.STATIC_DRAW
  );
  
  if (previousShader !== currentShader) {
    currentShader.init();
  }
  currentShader.bindShader();

  gl.bindBuffer(gl.ARRAY_BUFFER, null);
  gl.drawElements(gl.TRIANGLES, queuedIndicesBuffer.length, gl.UNSIGNED_SHORT, 0);

  gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, null);
  previousShader = currentShader;
  began = false;
  gl.deleteBuffer(vertexBuffer);
  gl.deleteBuffer(indexBuffer);
};

export const setShader = (newShader) => {
  if (!newShader) {
    throw new Error("newShader parameter is invalid");
  }
  if (newShader === currentShader) {
    return;
  }
  if (!began || queuedIndicesBuffer.length === 0) {
    currentShader = newShader;
    return;
  }
  flush();
  currentShader = newShader;
  begin();
};

/**
 * Puts the texture into 0 slot (main texture to be drawn).
 * 
 * @param {Texture} newTexture 
 */
export const setTexture = (newTexture) => {
  assertParamNotFalse("newTexture", newTexture);
  if (newTexture === currentTexture) {
    return;
  }
  if (!began || queuedIndicesBuffer.length === 0) {
    currentTexture = newTexture;
    return;
  }
  flush();
  currentTexture = newTexture;
  begin();
};

export const addElement = (vertices, indices = null) => {
  assertParamNotFalse("vertices", vertices);
  if (!currentShader) {
    throw new Error("Specify shader first.");
  }
  const indicesWithinBuffer = indices
    ? indices.map((i) => i + queuedVerticesCount)
    : rangeFrom(queuedVerticesCount, vertices.length);

  for (const vertexData of vertices) {
    currentShader.setVertexInArray(
      queuedVerticesBuffer,
      queuedVerticesCount,
      vertexData
    );
    queuedVerticesCount++;
  }

  for (const index of indicesWithinBuffer) {
    queuedIndicesBuffer.push(index);
  }
};

const rangeFrom = (fromInclusive, count) =>
  Array.from(new Array(count), (x, i) => i + fromInclusive);
