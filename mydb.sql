-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `mydb` DEFAULT CHARACTER SET utf8 ;
USE `mydb` ;

-- -----------------------------------------------------
-- Table `mydb`.`favoriler`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`favoriler` (
  `idfavoriler`  NOT NULL,
  `fav_etiket` VARCHAR(45) NULL DEFAULT NULL,
  `fav_latt` VARCHAR(45) NULL DEFAULT NULL,
  `fav_long` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`idfavoriler`))
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `mydb`.`konumlar`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`konumlar` (
  `id`  NOT NULL,
  `etiket` VARCHAR(45) NULL DEFAULT NULL,
  `latitute` VARCHAR(50) NULL DEFAULT NULL,
  `longtitute` VARCHAR(50) NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 26
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `mydb`.`rota`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`rota` (
  `idrota`  NOT NULL,
  `sira` VARCHAR(45) NULL DEFAULT NULL,
  `baslangic` VARCHAR(45) NULL DEFAULT NULL,
  `bitis` VARCHAR(45) NULL DEFAULT NULL,
  `km` VARCHAR(45) NULL DEFAULT NULL,
  `dk` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`idrota`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;