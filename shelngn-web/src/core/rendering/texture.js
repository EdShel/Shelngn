import { assertParamNotFalse } from "../errors";
import { gl } from "./global";

let loadedTextures = [];
let loadingTextures = [];

export default class Texture {
  constructor(path, glTexture, width, height) {
    this.path = path;
    this.glTexture = glTexture;
    /** @type Number */
    this.width = width;
    /** @type Number */
    this.height = height;
  }

  static loadLazy(textureUrl) {
    assertParamNotFalse("textureUrl", textureUrl);

    const alreadyLoadedTexture = loadedTextures.find((t) => t.path === textureUrl);
    if (alreadyLoadedTexture) {
      return alreadyLoadedTexture;
    }

    const loadingTexture = loadingTextures.find((t) => t.path === textureUrl);
    if (loadingTexture) {
      return loadingTexture;
    }

    const texture = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, texture);

    const level = 0;
    const internalFormat = gl.RGBA;
    const width = 1;
    const height = 1;
    const border = 0;
    const srcFormat = gl.RGBA;
    const srcType = gl.UNSIGNED_BYTE;
    const pixel = new Uint8Array([0, 0, 255, 255]);
    gl.texImage2D(gl.TEXTURE_2D, level, internalFormat, width, height, border, srcFormat, srcType, pixel);

    const textureEntry = new Texture(textureUrl, texture, width, height);
    loadingTextures.push(textureEntry);

    const image = new Image();
    image.onload = function () {
      textureEntry.width = image.width;
      textureEntry.height = image.height;

      gl.bindTexture(gl.TEXTURE_2D, texture);
      gl.texImage2D(gl.TEXTURE_2D, level, internalFormat, srcFormat, srcType, image);
      if (!isPowerOf2(image.width) || !isPowerOf2(image.height)) {
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
      }

      loadingTextures = loadingTextures.filter((t) => t !== textureEntry);
      loadedTextures.push(textureEntry);
    };
    image.onerror = function (e) {
      throw new Error(`Error loading the image '${textureUrl}' for texture: ${JSON.stringify(e)}`);
    };
    image.src = textureUrl;

    return textureEntry;
  }
}

function isPowerOf2(value) {
  return (value & (value - 1)) === 0;
}
