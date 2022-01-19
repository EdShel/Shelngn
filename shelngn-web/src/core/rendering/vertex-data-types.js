const glFloat = WebGLRenderingContext.FLOAT;
const glInteger = WebGLRenderingContext.INT;

export const FLOAT = { glType: glFloat, elementsCount: 1, sizeBytes: 4 };
export const FLOAT2 = { glType: glFloat, elementsCount: 2, sizeBytes: 8 };
export const FLOAT3 = { glType: glFloat, elementsCount: 3, sizeBytes: 12 };
export const FLOAT4 = { glType: glFloat, elementsCount: 4, sizeBytes: 16 };

export const INT32 = { glType: glInteger, elementsCount: 1, sizeBytes: 4 };

export const MATRIX4X4 = { glType: glFloat, elementsCount: 16, sizeBytes: 64 };

// export const SAMPLER2D = { glType: WebGLRenderingContext.glInteger, elementsCount: 1, sizeBytes: 4 };
