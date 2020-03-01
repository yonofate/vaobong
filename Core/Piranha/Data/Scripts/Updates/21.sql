--
-- Database version 21 Update script
--
-- 2013-01-10
--
-- Adds support for locking user accounts.

ALTER TABLE [sysuser] ADD [sysuser_locked] BIT NOT NULL default(0);
ALTER TABLE [sysuser] ADD [sysuser_locked_until] DATETIME NULL;
