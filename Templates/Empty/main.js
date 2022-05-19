import { Draw, GameScreen, Input, Random, Vector, Alert } from "Shelngn";

let asteroids;
let player = { pos: [0, 0] };
let gameStop;
let invinsible;

export function begin() {
  invinsible = true;
  gameStop = false;

  const d = GameScreen.getDimensions();
  asteroids = Array(20)
    .fill(0)
    .map(() => ({
      pos: [Random.range(-d.width / 2, d.width / 2), Random.range(-d.height / 2, d.height / 2)],
      speed: [Random.range(-10, 10), Random.range(-10, 10)],
      size: Random.range(40, 100),
    }));

  setTimeout(() => (invinsible = false), 3000);
}

export function draw() {
  const d = GameScreen.getDimensions();
  Draw.stretchedTexture("stars.jpg", -d.width / 2, -d.height / 2, d.width, d.height, 0, "center");

  !gameStop && Vector.move(player.pos, Input.wasd(10));
  Draw.stretchedTexture("ufo.png", player.pos[0], player.pos[1], 140, 140);

  asteroids.forEach((a) => {
    !gameStop && Vector.move(a.pos, a.speed);
    const {
      pos: [x, y],
      size,
    } = a;
    if (x < -d.width / 2 || x > d.width / 2) a.speed[0] *= -1;
    if (y < -d.height / 2 || y > d.height / 2) a.speed[1] *= -1;

    if (!gameStop && !invinsible && Vector.distance(player.pos, a.pos) < size / 2) {
      Alert.error("You lost", { autoClose: true });
      gameStop = true;
      setTimeout(begin, 5000);
    }

    const scaleFactor = invinsible ? 0.3 : 1;
    Draw.stretchedTexture("asteroid.png", x, y, size * scaleFactor, size * scaleFactor, 0, "center");
  });
}
