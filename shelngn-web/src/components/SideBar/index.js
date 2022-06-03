import React from "react";
import useWorkspaceId from "../../WorkspacePage/hooks/useWorkspaceId";
import homeIcon from "../../assets/home.svg";
import gearsIcon from "../../assets/gears.svg";
import wrenchIcon from "../../assets/wrench.svg";
import styles from "./styles.module.css";
import Hint from "../Hint";
import { NavLink } from "react-router-dom";
import UrlTo from "../../UrlTo";
import clsx from "clsx";
import LanguagePicker from "./LanguagePicker";

const SideBar = ({ className }) => {
  const workspaceId = useWorkspaceId();

  return (
    <nav className={clsx(styles["side-bar"], className)}>
      <div className={styles["nav-buttons"]}>
        <Button iconSrc={homeIcon} text="Home" link={UrlTo.home()} />
        {workspaceId && (
          <>
            <Button iconSrc={wrenchIcon} text="Workspace" link={UrlTo.workspace(workspaceId)} />
            <Button iconSrc={gearsIcon} text="Options" link={UrlTo.options(workspaceId)} />
          </>
        )}
      </div>
      <LanguagePicker />
    </nav>
  );
};
export default SideBar;

const Button = ({ link, iconSrc, text }) => (
  <NavLink to={link} className={({ isActive }) => clsx(styles.link, isActive && styles.active)}>
    <Hint className={styles.button} renderContent={() => <span>{text}</span>} orientation="right" arrowDistance={14}>
      <img src={iconSrc} alt={text} />
    </Hint>
  </NavLink>
);
