--
-- Database version 33 Update script
--
-- 2015-11-25
--
-- Adds support for page blocks.

ALTER TABLE [pagetemplate] ADD [pagetemplate_is_block] BIT NOT NULL default(0);