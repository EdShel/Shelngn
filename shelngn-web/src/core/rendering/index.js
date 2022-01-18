import { gl, canvas, initialize } from './global'


const notInitialized = () => { throw new Error('Not initialized yet.'); };

let stopObservingCanvasSize = notInitialized;
let matchCanvasRenderSize = notInitialized;

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
