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
	/// Entity map for the property.
	/// </summary>
	internal class PropertyMap : EntityTypeConfiguration<Property>
	{
		public PropertyMap() {
			ToTable("property");

			HasKey(p => new { p.Id, p.IsDraft });
			Property(p => p.Id).HasColumnName("property_id");
			Property(p => p.IsDraft).HasColumnName("property_draft");
			Property(p => p.ParentId).HasColumnName("property_parent_id");
			Property(p => p.Name).HasColumnName("property_name").IsRequired().HasMaxLength(64);
			Property(p => p.Value).HasColumnName("property_value");
			Property(p => p.Created).HasColumnName("property_created");
			Property(p => p.Updated).HasColumnName("property_updated");
			Property(p => p.CreatedById).HasColumnName("property_created_by");
			Property(p => p.UpdatedById).HasColumnName("property_updated_by");

			HasRequired(p => p.CreatedBy);
			HasRequired(p => p.UpdatedBy);
		}
	}
}
