import React from "react";
import pixelArtImage from "../assets/pixelArt.webp";
import styles from "./styles.module.css";

const TipOfTheDay = () => {
  return (
    <section className={styles.container} style={{ backgroundImage: `url(${pixelArtImage})` }}>
      <h2>Tip of the day</h2>
      <p>
        Did you know that you can quickly make and publish screenshots for your game? Just press <b>F12</b> while
        playing it.
      </p>
    </section>
  );
};

export default TipOfTheDay;
