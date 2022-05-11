import { mat4, vec2, vec3 } from "gl-matrix";
import { addElement, setShader, setTexture } from "./draw-batch";
import { gl } from "./global";
import Shader from "./shader";
import Texture from "./texture"; // eslint-disable-line no-unused-vars
import { FLOAT, FLOAT2, INT32, MATRIX4X4 } from "./vertex-data-types";
import { degToRad } from "../util/math";

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
      gl_FragColor = col;
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

  gl.enable(gl.BLEND);
  gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);
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
  texture(texture, x, y) {
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
  stretchedTexture(texture, x, y, width, height, rotation, origin) {
    initializeIfNeed();
    setShader(drawingShader);
    setTexture(texture);
    const rotationMatrix = mat4.create();
    let coords = [
      [x, y],
      [x + width, y],
      [x + width, y + height],
      [x, y + height],
    ];
    if (rotation) {
      let originX = 0;
      let originY = 0;
      if (origin === "center") {
        originX = width / 2;
        originY = height / 2;
      } else if (Array.isArray(origin)) {
        [originX = 0, originY = 0] = origin;
      }
      const originVec = vec3.fromValues(originX, originY, 0);
      mat4.translate(rotationMatrix, rotationMatrix, originVec);
      mat4.rotateZ(rotationMatrix, rotationMatrix, degToRad(rotation));
      vec3.negate(originVec, originVec);
      mat4.translate(rotationMatrix, rotationMatrix, originVec);
      
      let vec = vec2.create();
      coords = coords.map(([x, y]) => {
        vec[0] = x;
        vec[1] = y;
        vec2.transformMat4(vec, vec, rotationMatrix);
        return [vec[0], vec[1]];
      });
    }
    addElement(
      [
        {
          a_position: coords[0],
          a_textureCoord: [0, 0],
        },
        {
          a_position: coords[1],
          a_textureCoord: [1, 0],
        },
        {
          a_position: coords[2],
          a_textureCoord: [1, 1],
        },
        {
          a_position: coords[3],
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
    if (camera2D.rotation) {
      mat4.rotateZ(projectionMatrix, projectionMatrix, degToRad(camera2D.rotation));
    }
    if (camera2D.zoom !== 1) {
      mat4.scale(projectionMatrix, projectionMatrix, [camera2D.zoom, camera2D.zoom, camera2D.zoom]);
    }
    drawingShader.setUniform("u_transformation", projectionMatrix);
  },
};
export default Draw;
