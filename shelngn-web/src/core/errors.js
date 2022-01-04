export const assertParamNotFalse = (paramName, actualValue) => {
    if (!actualValue) {
        throw new Error(`Parameter '${paramName}' is not defined (has the value ${actualValue})`);
    }
};
