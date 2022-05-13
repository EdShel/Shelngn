import { useEffect } from "react";

export default function useScreenshot({ onScreenshot, canvas }) {
  useEffect(() => {
    if (!canvas || !onScreenshot) {
      return () => {};
    }

    const handleTakeScreenshot = (e) => {
      if (e.key !== "F12" || e.shiftKey || e.ctrlKey) {
        return;
      }
      e.preventDefault();
      e.stopPropagation();

      canvas.toBlob((canvasBlob) => {
        onScreenshot(canvasBlob);
      }, "image/png");
    };

    document.addEventListener("keydown", handleTakeScreenshot);

    return () => {
      document.removeEventListener("keydown", handleTakeScreenshot);
    };
  }, [canvas, onScreenshot]);
}
