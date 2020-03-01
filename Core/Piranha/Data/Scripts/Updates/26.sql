--
-- Database version 26 Update script
--
-- 2013-02-20
--
-- Adds the permission for site tree management.

INSERT INTO sysaccess (sysaccess_id, sysaccess_group_id, sysaccess_function, sysaccess_description, sysaccess_locked,
	sysaccess_created, sysaccess_updated, sysaccess_created_by, sysaccess_updated_by)
VALUES ('f71ca1b9-1276-4c3e-a090-5fba6c4980ce', '8940b41a-e3a9-44f3-b564-bfd281416141', 'ADMIN_SITETREE', 
	'Access to administrate site trees.', 1, GETDATE(), GETDATE(), 
	'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');

INSERT INTO sysparam (sysparam_id, sysparam_name, sysparam_value, sysparam_description, sysparam_locked,
	sysparam_created, sysparam_updated, sysparam_created_by, sysparam_updated_by)
VALUES ('5a7f257e-c3e3-4b7b-bf11-02d374409489', 'CACHE_SERVER_EXPIRES', '0', 'Sets how many minutes entities should be cached server side. A value of 0 indicates no expiration', 1,
	GETDATE(), GETDATE(), 'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
