import { useState } from "react";

let idCounter = 1;

const useUniquedId = () => {
  const [id] = useState(() => `c${idCounter++}`);
  return id;
};
export default useUniquedId;
