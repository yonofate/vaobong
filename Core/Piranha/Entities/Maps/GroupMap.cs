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
	/// Entity map for the group.
	/// </summary>
	internal class GroupMap : EntityTypeConfiguration<Group>
	{
		public GroupMap() {
			ToTable("sysgroup");

			Property(g => g.Id).HasColumnName("sysgroup_id");
			Property(g => g.ParentId).HasColumnName("sysgroup_parent_id");
			Property(g => g.Name).HasColumnName("sysgroup_name").IsRequired().HasMaxLength(64);
			Property(g => g.Description).HasColumnName("sysgroup_description");
			Property(g => g.Created).HasColumnName("sysgroup_created");
			Property(g => g.Updated).HasColumnName("sysgroup_updated");
			Property(g => g.CreatedById).HasColumnName("sysgroup_created_by");
			Property(g => g.UpdatedById).HasColumnName("sysgroup_updated_by");

			HasOptional(g => g.Parent);
			HasOptional(g => g.CreatedBy);
			HasOptional(g => g.UpdatedBy);
			HasMany(g => g.Users).WithOptional(u => u.Group);
			HasMany(g => g.Permissions).WithRequired(p => p.Group);
			HasMany(g => g.Extensions).WithRequired().HasForeignKey(e => e.ParentId);
		}
	}
}
