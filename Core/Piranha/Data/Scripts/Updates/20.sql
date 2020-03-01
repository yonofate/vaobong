--
-- Database version 20 Update script
--
-- 2012-12-19
--
-- Adds status support and abuse report to comments

ALTER TABLE [comment] ADD [comment_status] INT NOT NULL default(0);
ALTER TABLE [comment] ADD [comment_reported_count] INT NOT NULL default(0);
ALTER TABLE [comment] DROP COLUMN [comment_approved];

INSERT INTO sysaccess (sysaccess_id, sysaccess_group_id, sysaccess_function, sysaccess_description, sysaccess_locked,
	sysaccess_created, sysaccess_updated, sysaccess_created_by, sysaccess_updated_by)
VALUES ('f65bd7dd-6dfe-45b7-87e3-20a11e1f8d55', '8940b41a-e3a9-44f3-b564-bfd281416141', 'ADMIN_COMMENT', 
	'Access to administrate comments.', 1, GETDATE(), GETDATE(), 
	'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
