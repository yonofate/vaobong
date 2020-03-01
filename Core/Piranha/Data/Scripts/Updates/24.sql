--
-- Database version 24 Update script
--
-- 2013-02-04
--
-- Support for multiple site trees

CREATE TABLE [sitetree] (
	[sitetree_id] UNIQUEIDENTIFIER NOT NULL,
	[sitetree_namespace_id] UNIQUEIDENTIFIER NOT NULL,
	[sitetree_internal_id] NVARCHAR(32) NOT NULL,
	[sitetree_name] NVARCHAR(64) NOT NULL,
	[sitetree_description] NVARCHAR(255) NULL,
	[sitetree_created] DATETIME NOT NULL,
	[sitetree_updated] DATETIME NOT NULL,
	[sitetree_created_by] UNIQUEIDENTIFIER NOT NULL,
	[sitetree_updated_by] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT pk_sitetree_id PRIMARY KEY ([sitetree_id]),
	CONSTRAINT fk_sitetree_namespace_id FOREIGN KEY ([sitetree_namespace_id]) REFERENCES [namespace] ([namespace_id]),
	CONSTRAINT fk_sitetree_created_by FOREIGN KEY ([sitetree_created_by]) REFERENCES [sysuser] ([sysuser_id]),
	CONSTRAINT fk_sitetree_updated_by FOREIGN KEY ([sitetree_updated_by]) REFERENCES [sysuser] ([sysuser_id])
);

INSERT INTO [sitetree] ([sitetree_id], [sitetree_namespace_id], [sitetree_internal_id], [sitetree_name], [sitetree_description], [sitetree_created],
	[sitetree_updated], [sitetree_created_by], [sitetree_updated_by])
VALUES ('C2F87B2B-F585-4696-8A2B-3C9DF882701E', '8FF4A4B4-9B6C-4176-AAA2-DB031D75AC03', 'DEFAULT_SITE', 'Default site', 'This is the default site tree.',
	GETDATE(), GETDATE(), 'CA19D4E7-92F0-42F6-926A-68413BBDAFBC', 'CA19D4E7-92F0-42F6-926A-68413BBDAFBC');

ALTER TABLE [page] ADD [page_sitetree_id] UNIQUEIDENTIFIER NULL;
ALTER TABLE [page] ADD [page_original_id] UNIQUEIDENTIFIER NULL;

UPDATE [page] SET [page_sitetree_id] = 'C2F87B2B-F585-4696-8A2B-3C9DF882701E';

ALTER TABLE [page] ALTER COLUMN [page_sitetree_id] UNIQUEIDENTIFIER NOT NULL;