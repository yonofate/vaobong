--
-- Database version 22 Update script
--
-- 2013-01-11
--
-- Adds support for meda-file syncing. See issue #83.

ALTER TABLE [content] ADD [content_url] NVARCHAR(255) NULL;
ALTER TABLE [content] ADD [content_synced] DATETIME NULL;
