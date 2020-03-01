--
-- Database version 18 Update script
--
-- 2012-12-06
--
-- Adds the parent draft column to the comment table to enable EF linking.

ALTER TABLE [comment] add [comment_parent_draft] BIT NOT NULL default(0);
