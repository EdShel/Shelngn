import clsx from 'clsx';
import React from 'react'
import styles from './styles.module.css';

const getUserNameAbbrev = (userName) => userName.match(/\w\w/)?.[0].toUpperCase() || "";

const UserPreview = ({ userName, className }) => {
  return (
    <div className={clsx(styles.user, className)}>
    <span className={styles["user-name"]}>{getUserNameAbbrev(userName)}</span>
  </div>
  )
}

export default UserPreview