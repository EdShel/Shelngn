/** @type {HTMLCanvasElement} */
let canvas = null;

/** @type {WebGLRenderingContext} */
let gl = null;

export { canvas, gl };
export const initialize = (renderingCanvas) => {
  canvas = renderingCanvas;
  gl = canvas.getContext("webgl");
  if (!gl) {
    throw new Error("WebGL not supported.");
  }
};
