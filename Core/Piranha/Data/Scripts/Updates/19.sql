--
-- Database version 19 Update script
--
-- 2012-12-13
--
-- Adds last login time to the sysuser.

ALTER TABLE [sysuser] add [sysuser_last_login] DATETIME NULL;
ALTER TABLE [sysuser] add [sysuser_prev_login] DATETIME NULL;
