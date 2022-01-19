import { glMatrix, mat4 } from "gl-matrix";
import { addElement, setShader, setTexture } from "./draw-batch";
import Shader from "./shader";
import Texture from "./texture"; // eslint-disable-line no-unused-vars
import { FLOAT, FLOAT2, INT32, MATRIX4X4 } from "./vertex-data-types";

/** @type Shader */
let drawingShader = null;

const initialize = () => {
  const vsSource = `
    attribute vec2 a_position;
    attribute vec2 a_textureCoord;

    uniform mat4 u_transformation;

    varying vec2 v_textureCoord;

    void main() {
      gl_Position = u_transformation * vec4(a_position, 0, 1.0);
      v_textureCoord = a_textureCoord;
    }
  `;
  const psSource = `
    precision mediump float;

    varying vec2 v_textureCoord;

    uniform sampler2D u_sampler;

    void main() {
      vec4 col = texture2D(u_sampler, v_textureCoord);
      gl_FragColor = vec4(col.a, col.a, col.a, 1.0);
    }
  `;

  drawingShader = Shader.compile(vsSource, psSource, {
    attributes: {
      a_position: FLOAT2,
      a_textureCoord: FLOAT2,
    },
    uniforms: {
      u_tint: FLOAT,
      u_sampler: INT32,
      u_transformation: MATRIX4X4,
    },
  });

  console.log(drawingShader);
};

const initializeIfNeed = () => {
  if (!drawingShader) {
    initialize();
  }
};

const Draw = {
  initialize,
  /**
   * Draws the texture with 1x1 scale.
   *
   * @param {Texture} texture
   * @param {Number} x
   * @param {Number} y
   */
  drawTexture(texture, x, y) {
    initializeIfNeed();
    setShader(drawingShader);
    setTexture(texture);
    addElement(
      [
        {
          a_position: [x, y],
          a_textureCoord: [0, 0],
        },
        {
          a_position: [x + texture.width, y],
          a_textureCoord: [1, 0],
        },
        {
          a_position: [x + texture.width, y + texture.height],
          a_textureCoord: [1, 1],
        },
        {
          a_position: [x, y + texture.height],
          a_textureCoord: [0, 1],
        },
      ],
      [0, 1, 2, 2, 3, 0]
    );
  },
  /**
   *
   * @param {Camera2D} camera2D
   */
  applyCamera(camera2D) {
    initializeIfNeed();
    setShader(drawingShader);
    const { x, y, width, height } = camera2D;
    const projectionMatrix = mat4.create();
    mat4.ortho(projectionMatrix, x, x + width, y + height, y, 0, 10);
    drawingShader.setUniform("u_transformation", projectionMatrix);
  },
};
export default Draw;
