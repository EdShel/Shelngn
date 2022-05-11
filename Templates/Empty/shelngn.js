let callsRecords = [];

const handler = {
  get(target, prop) {
    return (...args) => {
      callsRecords.push([target.name, prop, ...args]);
    };
  },
};

export const Draw = new Proxy({ name: "Draw" }, handler);

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
};

export const __flushCallsRecorder = () => {
  postMessage(callsRecords);
  callsRecords = [];
};
