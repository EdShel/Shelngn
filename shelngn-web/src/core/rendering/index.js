import { gl, canvas, initialize } from './global'


const notInitialized = () => { throw new Error('Not initialized yet.'); };

let stopObservingCanvasSize = notInitialized;
let matchCanvasRenderSize = notInitialized;

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

    return {
        program,
        deleteProgram: () => { gl.deleteProgram(program); }
    };
}

function createProgramOfShadersSources(vertexShaderText, pixelShaderText) {
    const vertexShader = createShader(gl.VERTEX_SHADER, vertexShaderText);
    const pixelShader = createShader(gl.FRAGMENT_SHADER, pixelShaderText);
    return createProgram(gl, vertexShader, pixelShader);
}

function observeCanvasSize() {
    const canvasToDisplaySizeMap = new Map([[canvas, [canvas.width, canvas.height]]]);

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

function createSimpleTextureEffect() {
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

    const { program, deleteProgram } = createProgramOfShadersSources(vs, fs);

    const resolutionUniformLocation = gl.getUniformLocation(program, "u_resolution");
    const positionAttributeLocation = gl.getAttribLocation(program, "a_position");
    const texCoordAttributeLocation = gl.getAttribLocation(program, "a_texCoord");

    const fillColorUniformLocation = gl.getUniformLocation(program, "u_fillColor");

    return {
        program,
        deleteProgram,
        u_resolution: resolutionUniformLocation,
        a_position: positionAttributeLocation,
        a_texCoord: texCoordAttributeLocation,
        u_fillColor: fillColorUniformLocation,
    };
}

function createFloatBuffer(dataArray) {
    const buffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(dataArray), gl.STATIC_DRAW);
    return buffer;
}

function createTexture(textureImage) {
    const texture = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, texture);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, textureImage);
    return texture;
}

export function initializeRenderer(renderingCanvas) {
    initialize(renderingCanvas);

    const { cleanup, resizeCanvas } = observeCanvasSize(canvas);
    stopObservingCanvasSize = cleanup;
    matchCanvasRenderSize = resizeCanvas;
}

export function unitializeRenderer() {
    stopObservingCanvasSize();
}

export function renderBegin() {
    matchCanvasRenderSize();
    gl.viewport(0, 0, canvas.width, canvas.height);
}

export function clearCanvas(color) {
    const [r, g, b, a] = color || [1, 1, 1, 1];
    gl.clearColor(r, g, b, a);
    gl.clear(gl.COLOR_BUFFER_BIT);
}


// const startGameDrawing = (textureImage) => {
//     const x = 0;
//     const y = 0;
//     const w = 100;
//     const h = 100;
//     const positions = [
//         x, y,
//         x + w, y,
//         x + w, y + h,
//         x, y,
//         x + w, y + h,
//         x, y + h,
//     ];
//     const positionBuffer = createFloatBuffer(positions);

//     const uvs = [
//         0, 0,
//         1, 0,
//         1, 1,
//         0, 0,
//         1, 1,
//         0, 1
//     ];
//     const texCoordBuffer = createFloatBuffer(uvs);

//     const texture = createTexture(textureImage);

//     function render() {


//         gl.uniform2f(resolutionUniformLocation, gl.canvas.width, gl.canvas.height);
//         gl.uniform4f(fillColorUniformLocation, 1, 1, 1, 1);

//         gl.enableVertexAttribArray(positionAttributeLocation);
//         // Bind the position buffer.
//         gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);

//         // Tell the attribute how to get data out of positionBuffer (ARRAY_BUFFER)
//         var size = 2;          // 2 components per iteration
//         var type = gl.FLOAT;   // the data is 32bit floats
//         var normalize = false; // don't normalize the data
//         var stride = 0;        // 0 = move forward size * sizeof(type) each iteration to get the next position
//         var offset = 0;        // start at the beginning of the buffer
//         gl.vertexAttribPointer(
//             positionAttributeLocation, size, type, normalize, stride, offset)

//         gl.enableVertexAttribArray(texCoordAttributeLocation);
//         gl.bindBuffer(gl.ARRAY_BUFFER, texCoordBuffer);
//         gl.vertexAttribPointer(texCoordAttributeLocation, 2, gl.FLOAT, false, 0, 0);

//         var primitiveType = gl.TRIANGLES;
//         var offset = 0;
//         var count = 6;
//         gl.drawArrays(primitiveType, offset, count);

//         console.log('call')
//     }

//     render();

//     return () => {
//         cleanup();
//     }
// }