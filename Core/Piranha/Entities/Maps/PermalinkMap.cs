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
	/// Entity map for the permalink.
	/// </summary>
	internal class PermalinkMap : EntityTypeConfiguration<Permalink>
	{
		public PermalinkMap() {
			ToTable("permalink");

			Property(p => p.Id).HasColumnName("permalink_id");
			Property(p => p.NamespaceId).HasColumnName("permalink_namespace_id");
			Property(p => p.Type).HasColumnName("permalink_type").IsRequired().HasMaxLength(16);
			Property(p => p.Name).HasColumnName("permalink_name").IsRequired().HasMaxLength(128);
			Property(p => p.Created).HasColumnName("permalink_created");
			Property(p => p.Updated).HasColumnName("permalink_updated");
			Property(p => p.CreatedById).HasColumnName("permalink_created_by");
			Property(p => p.UpdatedById).HasColumnName("permalink_updated_by");

			HasRequired(p => p.Namespace);
			HasRequired(p => p.CreatedBy);
			HasRequired(p => p.UpdatedBy);
		}
	}
}
