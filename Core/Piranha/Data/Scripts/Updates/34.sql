--
-- Database version 34 Update script
--
-- 2015-11-26
--
-- Some extensions to page blocks.

ALTER TABLE [pagetemplate] ADD [pagetemplate_blocktypes] NTEXT NULL;
ALTER TABLE [pagetemplate] ADD [pagetemplate_subpages] BIT NOT NULL default(0);