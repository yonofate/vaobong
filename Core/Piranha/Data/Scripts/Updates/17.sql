--
-- Database version 17 Update script
--
-- 2012-12-06
--
-- Adds the comment table to the database.

CREATE TABLE [comment] (
	[comment_id] UNIQUEIDENTIFIER NOT NULL,
	[comment_parent_id] UNIQUEIDENTIFIER NOT NULL,
	[comment_approved] BIT,
	[comment_title] NVARCHAR(64) NULL,
	[comment_body] NTEXT NOT NULL,
	[comment_author_name] NVARCHAR(128) NULL,
	[comment_author_email] NVARCHAR(128) NULL,
	[comment_created] DATETIME NOT NULL,
	[comment_updated] DATETIME NOT NULL,
	[comment_created_by] UNIQUEIDENTIFIER NULL,
	[comment_updated_by] UNIQUEIDENTIFIER NULL,
	CONSTRAINT pk_comment_id PRIMARY KEY ([comment_id]),
	CONSTRAINT fk_comment_created_by FOREIGN KEY ([comment_created_by]) REFERENCES [sysuser] ([sysuser_id]),
	CONSTRAINT fk_comment_updated_by FOREIGN KEY ([comment_updated_by]) REFERENCES [sysuser] ([sysuser_id])
);
