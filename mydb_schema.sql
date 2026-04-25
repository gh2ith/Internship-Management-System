-- MySQL Script edited to assign foreign keys at the end
-- and fix missing column references.

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

CREATE SCHEMA IF NOT EXISTS `mydb` DEFAULT CHARACTER SET utf8 ;
USE `mydb` ;

-- -----------------------------------------------------
-- Table `mydb`.`Company`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Company` (
  `company_id` INT NOT NULL,
  `company_name` VARCHAR(45) NULL,
  `contact_info` VARCHAR(45) NULL,
  `description` VARCHAR(45) NULL,
  `location` VARCHAR(45) NULL,
  PRIMARY KEY (`company_id`),
  UNIQUE INDEX `company_ID_UNIQUE` (`company_id` ASC) VISIBLE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `mydb`.`College`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`College` (
  `college_id` INT NOT NULL,
  `college_name` VARCHAR(45) NULL,
  `uni-name` VARCHAR(45) NULL,
  PRIMARY KEY (`college_id`),
  UNIQUE INDEX `college_id_UNIQUE` (`college_id` ASC) VISIBLE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `mydb`.`Supervisor`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Supervisor` (
  `supervisor_id` INT NOT NULL,
  `full_name` VARCHAR(45) NULL,
  `super_department` VARCHAR(45) NULL,
  `college_id` INT NULL,
  PRIMARY KEY (`supervisor_id`),
  UNIQUE INDEX `supervisor_id_UNIQUE` (`supervisor_id` ASC) VISIBLE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `mydb`.`Report`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Report` (
  `report_id` INT NOT NULL,
  `comp_id` INT NULL,
  `super_id` INT NULL,
  `task` VARCHAR(45) NULL,
  `tools` VARCHAR(45) NULL,
  `number_of_hours` INT NULL,
  PRIMARY KEY (`report_id`),
  UNIQUE INDEX `evaluation_id_UNIQUE` (`report_id` ASC) VISIBLE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `mydb`.`Student`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Student` (
  `student_id` INT NOT NULL,
  `super_id` INT NULL,
  `report_id` INT NULL,
  `college_id` INT NULL,
  `st_department` VARCHAR(45) NULL,
  `full_name` VARCHAR(45) NULL,
  `gpa` FLOAT NULL,
  `resume` INT NULL,
  PRIMARY KEY (`student_id`),
  UNIQUE INDEX `student_id_UNIQUE` (`student_id` ASC) VISIBLE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `mydb`.`internship_opportunity`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`internship_opportunity` (
  `internship_id` INT NOT NULL,
  `company_id` INT NULL,
  `super_id` INT NULL,
  `title` VARCHAR(45) NULL,
  `description` VARCHAR(45) NULL,
  `deadline` DATETIME NULL,
  `requirments` VARCHAR(45) NULL,
  `std_id` INT NULL,
  PRIMARY KEY (`internship_id`),
  UNIQUE INDEX `internship_id_UNIQUE` (`internship_id` ASC) VISIBLE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `mydb`.`Application`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Application` (
  `application_id` INT NOT NULL,
  `super_id` INT NULL,
  `internship_id` INT NULL,
  `std_id` INT NULL,
  `comp_id` INT NULL,
  `status` TINYINT NULL,
  PRIMARY KEY (`application_id`),
  UNIQUE INDEX `application_id_UNIQUE` (`application_id` ASC) VISIBLE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `mydb`.`User`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`User` (
  `user_id` INT NOT NULL,
  `college_id` INT NULL,
  `email` VARCHAR(45) NULL,
  `password` VARCHAR(45) NULL,
  `role` VARCHAR(45) NULL,
  PRIMARY KEY (`user_id`),
  UNIQUE INDEX `user_id_UNIQUE` (`user_id` ASC) VISIBLE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- ADDING FOREIGN KEYS AT THE END
-- -----------------------------------------------------

ALTER TABLE `mydb`.`Supervisor`
  ADD CONSTRAINT `fk_supervisor_college`
  FOREIGN KEY (`college_id`) REFERENCES `mydb`.`College` (`college_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `mydb`.`Report`
  ADD CONSTRAINT `fk_report_comp`
  FOREIGN KEY (`comp_id`) REFERENCES `mydb`.`Company` (`company_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_report_super`
  FOREIGN KEY (`super_id`) REFERENCES `mydb`.`Supervisor` (`supervisor_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `mydb`.`Student`
  ADD CONSTRAINT `fk_student_super`
  FOREIGN KEY (`super_id`) REFERENCES `mydb`.`Supervisor` (`supervisor_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_student_report`
  FOREIGN KEY (`report_id`) REFERENCES `mydb`.`Report` (`report_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_student_college`
  FOREIGN KEY (`college_id`) REFERENCES `mydb`.`College` (`college_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `mydb`.`internship_opportunity`
  ADD CONSTRAINT `fk_internship_comp`
  FOREIGN KEY (`company_id`) REFERENCES `mydb`.`Company` (`company_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_internship_super`
  FOREIGN KEY (`super_id`) REFERENCES `mydb`.`Supervisor` (`supervisor_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_internship_std`
  FOREIGN KEY (`std_id`) REFERENCES `mydb`.`Student` (`student_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `mydb`.`Application`
  ADD CONSTRAINT `fk_application_super`
  FOREIGN KEY (`super_id`) REFERENCES `mydb`.`Supervisor` (`supervisor_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_application_internship`
  FOREIGN KEY (`internship_id`) REFERENCES `mydb`.`internship_opportunity` (`internship_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_application_comp`
  FOREIGN KEY (`comp_id`) REFERENCES `mydb`.`Company` (`company_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_application_std`
  FOREIGN KEY (`std_id`) REFERENCES `mydb`.`Student` (`student_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `mydb`.`User`
  ADD CONSTRAINT `fk_user_college`
  FOREIGN KEY (`college_id`) REFERENCES `mydb`.`College` (`college_id`)
  ON DELETE NO ACTION ON UPDATE NO ACTION;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
