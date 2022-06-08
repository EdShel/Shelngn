import hexToRgb from "../util/hexToRgb";

let colorsMemory = {};

function getOrCreate(colorText, valueGenerator) {
  if (colorsMemory[colorText]) {
    return colorsMemory[colorText];
  }
  colorsMemory[colorText] = valueGenerator();
  return colorsMemory[colorText];
}

const defaultColor = [1, 1, 1, 1];

export default function getColor(colorText) {
  if (!colorText) {
    return getOrCreate(colorText, () => defaultColor);
  }
  if (colorText[0] === "#") {
    return getOrCreate(colorText, () => {
      const rgb = hexToRgb(colorText);
      if (!rgb) {
        return defaultColor;
      }
      const { r, g, b } = rgb;
      return [r / 255, g / 255, b / 255, 1];
    });
  }
  return getOrCreate(colorText, () => defaultColor);
}
