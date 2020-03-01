--
-- Database version 29 Update script
--
-- 2013-03-13
--
-- Adds draft capabilities to images & documents

ALTER TABLE [content] ADD [content_draft] BIT NOT NULL default(1);
ALTER TABLE [content] ADD [content_published] DATETIME NULL;
ALTER TABLE [content] ADD [content_last_published] DATETIME NULL;
ALTER TABLE [content] DROP CONSTRAINT [pk_content_id];
ALTER TABLE [content] ADD CONSTRAINT [pk_content_id] PRIMARY KEY ([content_id],[content_draft]);

INSERT INTO [content] (
	[content_id],[content_draft],[content_parent_id],[content_filename],[content_url],[content_synced],[content_name],
	[content_type],[content_size],[content_image],[content_folder],[content_width],[content_height],[content_alt],[content_description],
	[content_created],[content_updated],[content_created_by],[content_updated_by],[content_published],[content_last_published])
SELECT
	[content_id],0,[content_parent_id],[content_filename],[content_url],[content_synced],[content_name],
	[content_type],[content_size],[content_image],[content_folder],[content_width],[content_height],[content_alt],[content_description],
	[content_created],[content_updated],[content_created_by],[content_updated_by],[content_created],[content_updated]
FROM [content];

UPDATE [content] SET [content_published] = [content_updated], [content_last_published] = [content_updated] WHERE [content_published] IS NULL AND [content_last_published] IS NULL;

ALTER TABLE [page] ADD [page_last_modified] DATETIME NULL;
ALTER TABLE [post] ADD [post_last_modified] DATETIME NULL;

UPDATE [page] SET [page_last_modified] = [page_last_published];
UPDATE [post] SET [post_last_modified] = [post_last_published];
