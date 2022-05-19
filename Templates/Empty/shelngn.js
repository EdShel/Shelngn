let callsRecords = [];

const handler = {
  get(target, prop) {
    return (...args) => {
      callsRecords.push([target.name, prop, ...args]);
    };
  },
};

export const Draw = new Proxy({ name: "Draw" }, handler);
export const Alert = new Proxy({ name: "Alert" }, handler);

//
let state = {
  screenDimensions: { width: 0, height: 0 },
  input: {},
};
export const __mergeState = (stateIncr) => {
  state = {
    ...state,
    ...stateIncr,
  };
};
//

export const GameScreen = {
  getDimensions() {
    return state.screenDimensions;
  },
};

export const Input = {
  up: (key) => state.input[key] === "up",
  down: (key) => state.input[key] === "down",
  press: (key) => state.input[key] === "press",
  wasd: (speed = 1) => {
    const w = Input.press("w");
    const s = Input.press("s");
    const a = Input.press("a");
    const d = Input.press("d");
    return [(d - a) * speed, (s - w) * speed];
  },
};

export const Random = {
  range: (min, max) => Math.random() * (max - min) + min,
  rangeInt: (min, max) => Math.floor(Math.random() * (Math.floor(max) - Math.ceil(min))) + Math.ceil(min),
};

export const Vector = {
  move: (pos, speed) => {
    if (!Array.isArray(pos)) throw new Error(`pos should be an array, but given ${typeof pos}.`);
    if (!Array.isArray(speed)) throw new Error(`speed should be an array, but given ${typeof pos}.`);

    pos[0] += speed[0];
    pos[1] += speed[1];
  },
  distance: (a, b) => {
    return Math.sqrt((a[0] - b[0]) ** 2 + (a[1] - b[1]) ** 2);
  },
};

export const __flushCallsRecorder = () => {
  postMessage(callsRecords);
  callsRecords = [];
};
