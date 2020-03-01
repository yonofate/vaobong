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
	/// Entity map for the permission.
	/// </summary>
	internal class PermissionMap : EntityTypeConfiguration<Permission>
	{
		public PermissionMap() {
			ToTable("sysaccess");

			Property(p => p.Id).HasColumnName("sysaccess_id");
			Property(p => p.GroupId).HasColumnName("sysaccess_group_id");
			Property(p => p.Name).HasColumnName("sysaccess_function").IsRequired().HasMaxLength(64);
			Property(p => p.Description).HasColumnName("sysaccess_description").HasMaxLength(255);
			Property(p => p.IsLocked).HasColumnName("sysaccess_locked");
			Property(p => p.Created).HasColumnName("sysaccess_created");
			Property(p => p.Updated).HasColumnName("sysaccess_updated");
			Property(p => p.CreatedById).HasColumnName("sysaccess_created_by");
			Property(p => p.UpdatedById).HasColumnName("sysaccess_updated_by");

			HasRequired(p => p.Group);
			HasOptional(p => p.CreatedBy);
			HasOptional(p => p.UpdatedBy);
		}
	}
}
