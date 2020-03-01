--
-- Database version 23 Update script
--
-- 2013-01-15
--
-- Adds API-key to the sysuser.

ALTER TABLE [sysuser] ADD [sysuser_apikey] UNIQUEIDENTIFIER NULL;

UPDATE [sysuser] SET [sysuser_apikey] = 'FE868D4F-797C-4E60-B876-64E6FC2424AA' where [sysuser_id] = 'ca19d4e7-92f0-42f6-926a-68413bbdafbc';

INSERT INTO sysparam (sysparam_id, sysparam_name, sysparam_value, sysparam_description, sysparam_locked,
	sysparam_created, sysparam_updated, sysparam_created_by, sysparam_updated_by)
VALUES ('4C694949-DEE0-465E-AB08-253927BDCBD8', 'SITE_PRIVATE_KEY', SUBSTRING(REPLACE(CAST(NEWID() AS NVARCHAR(38)), '-', ''), 1, 16), 'The private key used for public key encryption.', 1, 
	GETDATE(), GETDATE(), 'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
