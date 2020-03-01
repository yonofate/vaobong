--
-- Database version 28 Update script
--
-- 2013-03-09
--
-- Adds permissions for publishing and deleting content.

INSERT INTO sysaccess (sysaccess_id, sysaccess_group_id, sysaccess_function, sysaccess_description, sysaccess_locked,
	sysaccess_created, sysaccess_updated, sysaccess_created_by, sysaccess_updated_by)
VALUES ('da291f10-5bb6-44a5-ae20-1c9932c870e9', '8940b41a-e3a9-44f3-b564-bfd281416141', 
	'ADMIN_PAGE_PUBLISH', 'Access to publish, depublish and delete pages.', 1, GETDATE(), GETDATE(), 
	'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
INSERT INTO sysaccess (sysaccess_id, sysaccess_group_id, sysaccess_function, sysaccess_description, sysaccess_locked,
	sysaccess_created, sysaccess_updated, sysaccess_created_by, sysaccess_updated_by)
VALUES ('1bb90c7d-f3dd-43fe-aff5-985368d316e6', '8940b41a-e3a9-44f3-b564-bfd281416141', 
	'ADMIN_POST_PUBLISH', 'Access to publish, depublish and delete posts.', 1, GETDATE(), GETDATE(), 
	'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
INSERT INTO sysaccess (sysaccess_id, sysaccess_group_id, sysaccess_function, sysaccess_description, sysaccess_locked,
	sysaccess_created, sysaccess_updated, sysaccess_created_by, sysaccess_updated_by)
VALUES ('222119de-a510-427f-92ff-3d357bdf0c2c', '8940b41a-e3a9-44f3-b564-bfd281416141', 
	'ADMIN_CONTENT_PUBLISH', 'Access to publish, depublish and delete images & documents.', 1, GETDATE(), GETDATE(), 
	'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');

UPDATE sysaccess SET sysaccess_description = 'Access to add and update pages.' WHERE sysaccess_id = '36fbc1ad-4e17-4767-9fdc-af92802e5ebb';
UPDATE sysaccess SET sysaccess_description = 'Access to add and update posts.' WHERE sysaccess_id = 'c8b44826-d3e6-4add-b241-8ce95429a17e';
UPDATE sysaccess SET sysaccess_description = 'Access to add and update images & documents.' WHERE sysaccess_id = 'e08ae820-d438-4a38-b6e1-ac3aca3cf933';
