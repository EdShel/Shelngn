class Camera2D {
  rotation = 0;
  zoom = 1;

  constructor(x, y, width, height) {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;
  }
}

export default Camera2D;
