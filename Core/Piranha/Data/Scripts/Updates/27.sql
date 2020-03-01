--
-- Database version 27 Update script
--
-- 2013-03-03
--
-- Extends the site tree functionality with meta data.

ALTER TABLE [sitetree] ADD [sitetree_meta_title] NVARCHAR(128) NULL;
ALTER TABLE [sitetree] ADD [sitetree_meta_description] NVARCHAR(255) NULL;
ALTER TABLE [pagetemplate] ADD [pagetemplate_site_template] BIT NOT NULL default(0);

-- Copy site meta to default site.
UPDATE sitetree
SET
	sitetree_meta_title = (SELECT sysparam_value FROM sysparam WHERE sysparam_name = 'SITE_TITLE'),
	sitetree_meta_description = (SELECT sysparam_value FROM sysparam WHERE sysparam_name = 'SITE_DESCRIPTION')
WHERE
	sitetree_internal_id = 'DEFAULT_SITE' ;

-- Now delete the old sysparams
DELETE FROM sysparam WHERE sysparam_name = 'SITE_TITLE';
DELETE FROM sysparam WHERE sysparam_name = 'SITE_DESCRIPTION';

-- Add site template and page for the default site
INSERT INTO pagetemplate (pagetemplate_id, pagetemplate_name, pagetemplate_site_template,
	pagetemplate_created, pagetemplate_updated, pagetemplate_created_by, pagetemplate_updated_by)
VALUES ('C2F87B2B-F585-4696-8A2B-3C9DF882701E', 'C2F87B2B-F585-4696-8A2B-3C9DF882701E', 1,
	 GETDATE(), GETDATE(), 'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
INSERT INTO permalink (permalink_id, permalink_namespace_id, permalink_type, permalink_name, permalink_created,
	permalink_updated, permalink_created_by, permalink_updated_by)
VALUES ('2E168001-D113-4216-ACC5-03C61C2D0C21', '8FF4A4B4-9B6C-4176-AAA2-DB031D75AC03', 'SITE', 'C2F87B2B-F585-4696-8A2B-3C9DF882701E', 
	GETDATE(), GETDATE(), 'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
INSERT INTO page (page_id, page_sitetree_id, page_draft, page_template_id, page_permalink_id, page_parent_id, page_seqno, page_title, 
	page_created, page_updated, page_published, page_last_published, page_created_by, page_updated_by)
VALUES ('94823A5C-1E29-4BDB-84E4-9B5F636CDDB5', 'C2F87B2B-F585-4696-8A2B-3C9DF882701E', 1, 'C2F87B2B-F585-4696-8A2B-3C9DF882701E', '2E168001-D113-4216-ACC5-03C61C2D0C21', 
	'C2F87B2B-F585-4696-8A2B-3C9DF882701E', 1, 'C2F87B2B-F585-4696-8A2B-3C9DF882701E', GETDATE(), GETDATE(), GETDATE(), GETDATE(),
	'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
INSERT INTO page (page_id, page_sitetree_id, page_draft, page_template_id, page_permalink_id, page_parent_id, page_seqno, page_title, 
	page_created, page_updated, page_published, page_last_published, page_created_by, page_updated_by)
VALUES ('94823A5C-1E29-4BDB-84E4-9B5F636CDDB5', 'C2F87B2B-F585-4696-8A2B-3C9DF882701E', 0, 'C2F87B2B-F585-4696-8A2B-3C9DF882701E', '2E168001-D113-4216-ACC5-03C61C2D0C21', 
	'C2F87B2B-F585-4696-8A2B-3C9DF882701E', 1, 'C2F87B2B-F585-4696-8A2B-3C9DF882701E', GETDATE(), GETDATE(), GETDATE(), GETDATE(),
	'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');

