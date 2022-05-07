import { flushCallsRecorder } from "./shelngn";
import { draw } from "./main";

onmessage = function (e) {
  console.log("e", e);

  draw();

  flushCallsRecorder();
};
