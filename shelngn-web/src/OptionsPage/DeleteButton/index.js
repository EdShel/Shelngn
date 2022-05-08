import React from 'react'
import styles from './styles.module.css';

const DeleteButton = ({ onPress }) => {
  return (
    <div className={styles.container} onClick={onPress}>delete</div>
  )
}

export default DeleteButton