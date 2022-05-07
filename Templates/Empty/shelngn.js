let callsRecords = [];

const handler = {
  get(target, prop) {
    return (...args) => {
      callsRecords.push([target.name, prop, ...args]);
    };
  },
};

export const Draw = new Proxy({ name: "Draw" }, handler);
export const Texture = new Proxy({ name: "Texture" }, handler);

export const flushCallsRecorder = () => {
  postMessage(callsRecords);
  callsRecords = [];
};
