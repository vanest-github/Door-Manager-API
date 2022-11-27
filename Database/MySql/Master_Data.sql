INSERT INTO `door_manager`.`offices` (office_name, country) VALUES ('Main Office', 'Netherlands');
SET @OfficeId = LAST_INSERT_ID();

INSERT INTO `door_manager`.`roles` (role_name, role_priority) VALUES ('Super User', 1);
INSERT INTO `door_manager`.`roles` (role_name, role_priority) VALUES ('Director', 10);
INSERT INTO `door_manager`.`roles` (role_name, role_priority) VALUES ('Senior Manager', 20);
INSERT INTO `door_manager`.`roles` (role_name, role_priority) VALUES ('Manager', 30);
INSERT INTO `door_manager`.`roles` (role_name, role_priority) VALUES ('Team Lead', 40);
INSERT INTO `door_manager`.`roles` (role_name, role_priority) VALUES ('Employee', 50);
INSERT INTO `door_manager`.`roles` (role_name, role_priority) VALUES ('Visitor', 60);

INSERT INTO `door_manager`.`door_types` (door_type) VALUES ('MainEntrance');
INSERT INTO `door_manager`.`door_types` (door_type) VALUES ('StorageRoomEntrance');
INSERT INTO `door_manager`.`door_types` (door_type) VALUES ('ServerRoomEntrance');
INSERT INTO `door_manager`.`door_types` (door_type) VALUES ('SecurityLocker');
INSERT INTO `door_manager`.`door_types` (door_type) VALUES ('ParkingEntrance');

SET @SuperUserRoleId = (SELECT role_id FROM `door_manager`.`roles` WHERE role_name = 'Super User');
INSERT INTO `door_manager`.`users` (first_name, last_name) VALUES ('Super', 'User');
INSERT INTO `door_manager`.`user_office_roles` (user_id, office_id, role_id, valid_from, valid_to)
VALUES (LAST_INSERT_ID(), @OfficeId, @SuperUserRoleId, UTC_TIMESTAMP(), DATE_ADD(UTC_TIMESTAMP(), INTERVAL 1 YEAR));

SET @SeniorMgrRoleId = (SELECT role_id FROM `door_manager`.`roles` WHERE role_name = 'Senior Manager');
INSERT INTO `door_manager`.`users` (first_name, last_name) VALUES ('Senior', 'Manager');
INSERT INTO `door_manager`.`user_office_roles` (user_id, office_id, role_id, valid_from, valid_to)
VALUES (LAST_INSERT_ID(), @OfficeId, @SeniorMgrRoleId, UTC_TIMESTAMP(), DATE_ADD(UTC_TIMESTAMP(), INTERVAL 1 YEAR));

SET @LeadRoleId = (SELECT role_id FROM `door_manager`.`roles` WHERE role_name = 'Team Lead');
INSERT INTO `door_manager`.`users` (first_name, last_name) VALUES ('Lead', 'Employee');
INSERT INTO `door_manager`.`user_office_roles` (user_id, office_id, role_id, valid_from, valid_to)
VALUES (LAST_INSERT_ID(), @OfficeId, @LeadRoleId, UTC_TIMESTAMP(), DATE_ADD(UTC_TIMESTAMP(), INTERVAL 1 YEAR));

SET @EmployeeRoleId = (SELECT role_id FROM `door_manager`.`roles` WHERE role_name = 'Employee');
INSERT INTO `door_manager`.`users` (first_name, last_name) VALUES ('Test', 'Employee');
INSERT INTO `door_manager`.`user_office_roles` (user_id, office_id, role_id, valid_from, valid_to)
VALUES (LAST_INSERT_ID(), @OfficeId, @EmployeeRoleId, UTC_TIMESTAMP(), DATE_ADD(UTC_TIMESTAMP(), INTERVAL 1 YEAR));

INSERT INTO `door_manager`.`doors` (office_id, door_type_id, current_status, created_time, modified_time)
SELECT @OfficeId, door_type_id, 'Closed', UTC_TIMESTAMP(), UTC_TIMESTAMP() FROM `door_manager`.`door_types`;

SET @MainTypeId = (SELECT door_type_id FROM `door_manager`.`door_types` WHERE door_type = 'MainEntrance');
INSERT INTO `door_manager`.`door_access_roles` (door_type_id, office_id, role_id, access_from, access_to)
SELECT @MainTypeId, @OfficeId, role_id, UTC_TIMESTAMP(), DATE_ADD(UTC_TIMESTAMP(), INTERVAL 5 YEAR) FROM `door_manager`.`roles`;

SET @ParkingTypeId = (SELECT door_type_id FROM `door_manager`.`door_types` WHERE door_type = 'ParkingEntrance');
INSERT INTO `door_manager`.`door_access_roles` (door_type_id, office_id, role_id, access_from, access_to)
SELECT @ParkingTypeId, @OfficeId, role_id, UTC_TIMESTAMP(), DATE_ADD(UTC_TIMESTAMP(), INTERVAL 5 YEAR) FROM `door_manager`.`roles`;

SET @ServerTypeId = (SELECT door_type_id FROM `door_manager`.`door_types` WHERE door_type = 'ServerRoomEntrance');
INSERT INTO `door_manager`.`door_access_roles` (door_type_id, office_id, role_id, access_from, access_to)
SELECT @ServerTypeId, @OfficeId, role_id, UTC_TIMESTAMP(), DATE_ADD(UTC_TIMESTAMP(), INTERVAL 5 YEAR) FROM `door_manager`.`roles` WHERE role_priority <= 40;

SET @StorageTypeId = (SELECT door_type_id FROM `door_manager`.`door_types` WHERE door_type = 'StorageRoomEntrance');
INSERT INTO `door_manager`.`door_access_roles` (door_type_id, office_id, role_id, access_from, access_to)
SELECT @StorageTypeId, @OfficeId, role_id, UTC_TIMESTAMP(), DATE_ADD(UTC_TIMESTAMP(), INTERVAL 5 YEAR) FROM `door_manager`.`roles` WHERE role_priority <= 20;

SET @LockerTypeId = (SELECT door_type_id FROM `door_manager`.`door_types` WHERE door_type = 'SecurityLocker');
INSERT INTO `door_manager`.`door_access_roles` (door_type_id, office_id, role_id, access_from, access_to)
SELECT @LockerTypeId, @OfficeId, role_id, UTC_TIMESTAMP(), DATE_ADD(UTC_TIMESTAMP(), INTERVAL 5 YEAR) FROM `door_manager`.`roles` WHERE role_priority <= 10;

SET @ActivityLogFeatureId = 1;
INSERT INTO `door_manager`.`rbac_role_features` (role_id, feature_id)
SELECT role_id, @ActivityLogFeatureId FROM `door_manager`.`roles` WHERE role_priority <= 20;