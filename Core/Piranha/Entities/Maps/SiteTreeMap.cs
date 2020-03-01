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
using System.Web;

namespace Piranha.Entities.Maps
{
	/// <summary>
	/// Entity map for the site tree.
	/// </summary>
	public class SiteTreeMap : EntityTypeConfiguration<SiteTree>
	{
		public SiteTreeMap() {
			ToTable("sitetree");

			Property(s => s.Id).HasColumnName("sitetree_id");
			Property(s => s.NamespaceId).HasColumnName("sitetree_namespace_id");
			Property(s => s.InternalId).HasColumnName("sitetree_internal_id").IsRequired().HasMaxLength(32);
			Property(s => s.Name).HasColumnName("sitetree_name").IsRequired().HasMaxLength(64);
			Property(s => s.Description).HasColumnName("sitetree_description").HasMaxLength(255);
			Property(s => s.MetaTitle).HasColumnName("sitetree_meta_title").HasMaxLength(128);
			Property(s => s.MetaDescription).HasColumnName("sitetree_meta_description").HasMaxLength(255);
			Property(s => s.HostNames).HasColumnName("sitetree_hostnames");
			Property(s => s.Created).HasColumnName("sitetree_created");
			Property(s => s.Updated).HasColumnName("sitetree_updated");
			Property(s => s.CreatedById).HasColumnName("sitetree_created_by");
			Property(s => s.UpdatedById).HasColumnName("sitetree_updated_by");

			HasRequired(s => s.CreatedBy);
			HasRequired(s => s.UpdatedBy);
		}
	}
}