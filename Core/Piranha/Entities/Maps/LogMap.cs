/*
 * Copyright (c) 2011-2015 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Piranha.Entities.Maps
{
	/// <summary>
	/// Entity map for the system log.
	/// </summary>
	internal class LogMap : EntityTypeConfiguration<Log>
	{
		public LogMap() {
			ToTable("syslog");

			Property(l => l.Id).HasColumnName("syslog_id");
			Property(l => l.ParentId).HasColumnName("syslog_parent_id");
			Property(l => l.ParentType).HasColumnName("syslog_parent_type").IsRequired().HasMaxLength(128);
			Property(l => l.Action).HasColumnName("syslog_action").IsRequired().HasMaxLength(64);
			Property(l => l.Created).HasColumnName("syslog_created");
			Property(l => l.Updated).HasColumnName("syslog_updated");
			Property(l => l.CreatedById).HasColumnName("syslog_created_by");
			Property(l => l.UpdatedById).HasColumnName("syslog_updated_by");

			HasRequired(l => l.CreatedBy);
			HasRequired(l => l.UpdatedBy);
		}
	}
}
