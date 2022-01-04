import { gl } from "./global";
import { assertParamNotFalse } from "../errors";
import { FLOAT, FLOAT2, FLOAT3, FLOAT4 } from "./vertex-data-types";

export default class Shader {
  /** @type WebGLProgram */
  glProgram = undefined;
  attributes = undefined;
  uniforms = undefined;

  constructor(glProgram, data) {
    assertParamNotFalse("glParam", glProgram);
    assertParamNotFalse("data", data);
    assertParamNotFalse("data.attributes", data.attributes);

    this.glProgram = glProgram;
    this.attributes = [];

    const { attributes, uniforms } = data;
    const vertexSize = Object.values(attributes).reduce(
      (sum, { sizeBytes }) => sum + sizeBytes,
      0
    );
    let attributesBytesOffset = 0;
    for (const attributeName in attributes) {
      const attributeType = attributes[attributeName];

      const attribLocation = gl.getAttribLocation(
        this.glProgram,
        attributeName
      );
      const floatCount = vertexSize / 4;
      const floatOffset = attributesBytesOffset / 4;
      const offsetToElement = attributesBytesOffset;
      this.attributes.push({
        name: attributeName,
        enable: () => gl.enableVertexAttribArray(attribLocation),
        setPointer: () =>
          gl.vertexAttribPointer(
            attribLocation,
            attributeType.elementsCount,
            attributeType.glType,
            false,
            vertexSize,
            offsetToElement
          ),
        setArrayValue(vertexArray, vertexIndex, attribValue) {
          if (typeof attribValue === "number") {
            vertexArray[vertexIndex * floatCount + floatOffset] = attribValue;
            return;
          }
          for (let i = 0; i < attributeType.elementsCount; i++) {
            vertexArray[vertexIndex * floatCount + floatOffset + i] =
              attribValue[i];
          }
        },
      });
      attributesBytesOffset += attributeType.sizeBytes;
    }

    this.uniforms = {};
    if (uniforms) {
      for (const uniformName in uniforms) {
        const uniformDataType = uniforms[uniformName];
        const uniformLocation = gl.getUniformLocation(
          this.glProgram,
          uniformName
        );
        const uniformSetFunction = getUniformSetterFunction(uniformDataType);
        this.uniforms[uniformName] = {
          setValue(val) {
            uniformSetFunction(uniformLocation, val);
          },
        };
      }
    }
  }

  init() {
    for (const attrib of this.attributes) {
      // attrib.setPointer();
      attrib.enable();
    }
  }

  bindShader() {
    for (const attrib of this.attributes) {
      attrib.setPointer();
      // attrib.enable();
    }

    gl.useProgram(this.glProgram);
  }

  setUniform(uniformName, value) {
    const uniform = this.uniforms[uniformName];
    if (!uniform) {
      throw new Error(
        `Uniform '${uniformName}' is not found. List of uniforms is: [ ${Object.keys(
          this.uniforms
        )} ].`
      );
    }
    uniform.setValue(value);
  }

  setVertexInArray(vertexArray, vertexIndex, vertexData) {
    const attributesGiven = Object.keys(vertexData);
    const attributesRequired = this.attributes.map((a) => a.name);
    const missingAttributes = attributesRequired.filter(
      (a) => !attributesGiven.includes(a)
    );

    const redundandAttributes = attributesGiven.filter(
      (a) => !attributesRequired.includes(a)
    );
    if (redundandAttributes.length > 0) {
      throw new Error(
        `Redundand attributes set provided for vertex: [ ${redundandAttributes} ]. Expected [ ${attributesRequired} ].`
      );
    }
    if (missingAttributes.length > 0) {
      throw new Error(
        `Not complete vertex provided for shader. Missing attributes: [ ${missingAttributes} ].`
      );
    }

    for (const attrib of this.attributes) {
      attrib.setArrayValue(vertexArray, vertexIndex, vertexData[attrib.name]);
    }
  }

  static compile(
    vertexShaderSource,
    pixelShaderSource,
    shaderAttributes,
    shaderUniforms
  ) {
    const vertexShader = createShader(gl.VERTEX_SHADER, vertexShaderSource);
    const pixelShader = createShader(gl.FRAGMENT_SHADER, pixelShaderSource);
    const program = createProgram(vertexShader, pixelShader);
    return new Shader(program, shaderAttributes, shaderUniforms);

    function createShader(type, source) {
      const shader = gl.createShader(type);
      gl.shaderSource(shader, source);
      gl.compileShader(shader);
      const success = gl.getShaderParameter(shader, gl.COMPILE_STATUS);
      if (!success) {
        console.log(gl.getShaderInfoLog(shader));
        gl.deleteShader(shader);
        throw new Error(gl.getShaderInfoLog(shader));
      }
      return shader;
    }

    function createProgram(vertexShader, fragmentShader) {
      var program = gl.createProgram();
      gl.attachShader(program, vertexShader);
      gl.attachShader(program, fragmentShader);
      gl.linkProgram(program);
      var success = gl.getProgramParameter(program, gl.LINK_STATUS);
      if (!success) {
        console.log(gl.getProgramInfoLog(program));
        gl.deleteProgram(program);
      }

      return program;
    }
  }
}

const getUniformSetterFunction = (uniformType) => {
  switch (uniformType) {
    case FLOAT:
      return (l, v) => gl.uniform1f(l, v);
    case FLOAT2:
      return (l, [x, y]) => gl.uniform2f(l, x, y);
    case FLOAT3:
      return (l, [x, y, z]) => gl.uniform3f(l, x, y, z);
    case FLOAT4:
      return (l, [x, y, z, w]) => gl.uniform4f(l, x, y, z, w);
    default:
      throw new Error("Unknown uniform data type: " + uniformType);
  }
};
