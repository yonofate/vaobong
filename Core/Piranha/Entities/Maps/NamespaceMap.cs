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
	/// Entity map for the namespace.
	/// </summary>
	public class NamespaceMap : EntityTypeConfiguration<Namespace>
	{
		public NamespaceMap() {
			ToTable("namespace");

			Property(n => n.Id).HasColumnName("namespace_id");
			Property(n => n.InternalId).HasColumnName("namespace_internal_id").IsRequired().HasMaxLength(32);
			Property(n => n.Name).HasColumnName("namespace_name").IsRequired().HasMaxLength(64);
			Property(n => n.Description).HasColumnName("namespace_description").HasMaxLength(255);
			Property(n => n.Created).HasColumnName("namespace_created");
			Property(n => n.Updated).HasColumnName("namespace_updated");
			Property(n => n.CreatedById).HasColumnName("namespace_created_by");
			Property(n => n.UpdatedById).HasColumnName("namespace_updated_by");

			HasMany(n => n.Permalinks).WithRequired(p => p.Namespace);
			HasRequired(n => n.CreatedBy);
			HasRequired(n => n.UpdatedBy);
		}
	}
}
