import { __flushCallsRecorder, __mergeState } from "Shelngn";
import { draw } from "./main";

onmessage = function ({ data: { type, stateIncr } }) {
  if (type === "draw") {
    __mergeState(stateIncr);

    draw();
    __flushCallsRecorder();
  }
};
