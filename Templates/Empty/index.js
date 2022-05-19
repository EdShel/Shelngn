import { __flushCallsRecorder, __mergeState } from "Shelngn";
import { begin, draw } from './main';

onmessage = function ({ data: { type, stateIncr } }) {
  __mergeState(stateIncr);
  
  if (type === "init") {
    begin?.();
  }
  if (type === "draw") {
    draw?.();
    __flushCallsRecorder();
  }
};
