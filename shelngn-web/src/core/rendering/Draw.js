import { addElement, setShader, setTexture } from "./draw-batch";
import Shader from "./shader";
import Texture from "./texture"; // eslint-disable-line no-unused-vars
import { FLOAT, FLOAT2, INT32 } from "./vertex-data-types";

/** @type Shader */
let drawingShader = null;

const initialize = () => {
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

    uniform sampler2D u_sampler;

    void main() {
      gl_FragColor = texture2D(u_sampler, v_textureCoord);
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
    },
  });
};

const initializeIfNeed = () => {
  if (!drawingShader) {
    initialize();
  }
};

const Draw = {
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
          a_position: [x + texture.width / 256, y],
          a_textureCoord: [1, 0],
        },
        {
          a_position: [x + texture.width / 256, y + texture.height / 256],
          a_textureCoord: [1, 1],
        },
        {
          a_position: [x, y + texture.height / 256],
          a_textureCoord: [0, 1],
        },
      ],
      [0, 1, 2, 2, 3, 0]
    );
  },
};
export default Draw;
