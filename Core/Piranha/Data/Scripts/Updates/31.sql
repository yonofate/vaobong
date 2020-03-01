--
-- Database version 31 Update script
--
-- 2013-06-10
--
-- Adds permalink support for post types and media

ALTER TABLE [posttemplate] ADD [posttemplate_permalink_id] UNIQUEIDENTIFIER NULL;
ALTER TABLE [content] ADD [content_permalink_id] UNIQUEIDENTIFIER NULL;

INSERT INTO [namespace] ([namespace_id], [namespace_internal_id], [namespace_name], [namespace_description], [namespace_created], [namespace_updated],
	[namespace_created_by], [namespace_updated_by])
	VALUES ('C8342FB4-D38E-4EAF-BBC1-4EF3BDD7500C', 'ARCHIVE', 'Archive namespace', 'This is the archive namespace for all post types.',
		GETDATE(), GETDATE(), 'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
INSERT INTO [namespace] ([namespace_id], [namespace_internal_id], [namespace_name], [namespace_description], [namespace_created], [namespace_updated],
	[namespace_created_by], [namespace_updated_by])
	VALUES ('368249B1-7F9C-4974-B9E3-A55D068DD9B6', 'MEDIA', 'Media namespace', 'This is the media namespace for all images & documents.',
		GETDATE(), GETDATE(), 'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
