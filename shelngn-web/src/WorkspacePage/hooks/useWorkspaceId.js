import { useParams } from "react-router-dom";

const useWorkspaceId = () => {
  return useParams().id;
};
export default useWorkspaceId;
