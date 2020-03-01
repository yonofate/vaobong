--
-- Database version 31 Update script
--
-- 2013-06-25
--
-- Adds support for programmatically defined page & post types

ALTER TABLE [pagetemplate] ADD [pagetemplate_type] NVARCHAR(255) NULL;
ALTER TABLE [posttemplate] ADD [posttemplate_type] NVARCHAR(255) NULL;
