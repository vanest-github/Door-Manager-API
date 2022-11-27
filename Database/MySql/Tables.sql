CREATE DATABASE IF NOT EXISTS `door_manager`;

USE `door_manager`;

CREATE TABLE IF NOT EXISTS `door_manager`.`offices`
(
	`office_id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	`office_name` VARCHAR(50) NOT NULL,
	`latitude` DECIMAL(6,4) NULL,
	`longitude` DECIMAL(6,4) NULL,
    `country` VARCHAR(50) NOT NULL,
    `is_active` BOOL NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS `door_manager`.`roles`
(
	`role_id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    `role_name` VARCHAR(20) NOT NULL,
	`role_priority` INT NOT NULL DEFAULT 9999,
    `is_active` BOOL NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS `door_manager`.`door_types`
(
	`door_type_id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	`door_type` VARCHAR(50) NOT NULL,
	`is_active` BOOL NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS `door_manager`.`users`
(
	`user_id` BIGINT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    `first_name` CHAR(50) NOT NULL,
    `last_name` CHAR(50) NOT NULL,
    `is_active` BOOL NOT NULL DEFAULT TRUE,
    `last_access_time` DATETIME NULL,
    `last_access_type` DATETIME NULL
);

CREATE TABLE IF NOT EXISTS `door_manager`.`doors`
(
	`door_id` CHAR(36) NOT NULL DEFAULT (UUID()) PRIMARY KEY,
    `office_id` INT NOT NULL,
	`door_type_id` INT NOT NULL,
	`is_active` BOOL NOT NULL DEFAULT TRUE,
    `current_status` VARCHAR(10) NOT NULL,
    `manufacturer` VARCHAR(20),
    `lock_version` VARCHAR(20),
    `created_time` DATETIME NOT NULL,
	`modified_time` DATETIME NOT NULL,
	FOREIGN KEY (office_id) REFERENCES offices (office_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (door_type_id) REFERENCES door_types (door_type_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS `door_manager`.`user_office_roles`
(
	`user_office_role_id` BIGINT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    `user_id` BIGINT NOT NULL,
    `office_id` INT NOT NULL,
    `role_id` INT NOT NULL,
    `valid_from` DATETIME NOT NULL,
    `valid_to` DATETIME NOT NULL,
	`acting_for` BIGINT NULL,
	FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (office_id) REFERENCES offices (office_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (role_id) REFERENCES roles (role_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (acting_for) REFERENCES users (user_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS `door_manager`.`door_access_roles`
(
	`door_access_id` BIGINT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    `door_type_id` INT NOT NULL,
    `office_id` INT NOT NULL,
    `role_id` INT NOT NULL,
    `access_from` DATETIME NOT NULL,
    `access_to` DATETIME NOT NULL,
	FOREIGN KEY (door_type_id) REFERENCES door_types (door_type_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (office_id) REFERENCES offices (office_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (role_id) REFERENCES roles (role_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS `door_manager`.`activity_logs`
(
	`activity_id` BIGINT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    `user_id` BIGINT NOT NULL,
    `office_id` INT NOT NULL,
    `activity_time` DATETIME NOT NULL,
    `action` VARCHAR(20) NOT NULL,
    `description` VARCHAR(1000),
	FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (office_id) REFERENCES offices (office_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS `door_manager`.`rbac_role_features`
(
	`rbac_role_feature_id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	`role_id` INT NOT NULL,
    `feature_id` INT NOT NULL,
	`is_active` BOOL NOT NULL DEFAULT TRUE,
	FOREIGN KEY (role_id) REFERENCES roles (role_id) ON DELETE CASCADE ON UPDATE CASCADE
);
