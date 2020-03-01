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
	/// Entity map for the category.
	/// </summary>
	public class CategoryMap : EntityTypeConfiguration<Category>
	{
		public CategoryMap() {
			ToTable("category");

			Property(c => c.Id).HasColumnName("category_id");
			Property(c => c.ParentId).HasColumnName("category_parent_id");
			Property(c => c.PermalinkId).HasColumnName("category_permalink_id");
			Property(c => c.Name).HasColumnName("category_name").IsRequired().HasMaxLength(64);
			Property(c => c.Description).HasColumnName("category_description").HasMaxLength(255);
			Property(c => c.Created).HasColumnName("category_created");
			Property(c => c.Updated).HasColumnName("category_updated");
			Property(c => c.CreatedById).HasColumnName("category_created_by");
			Property(c => c.UpdatedById).HasColumnName("category_updated_by");

			HasOptional(c => c.Parent);
			HasRequired(c => c.Permalink);
			HasMany(c => c.Extensions).WithRequired().HasForeignKey(e => e.ParentId);
			HasRequired(c => c.CreatedBy);
			HasRequired(c => c.UpdatedBy);
		}
	}
}
