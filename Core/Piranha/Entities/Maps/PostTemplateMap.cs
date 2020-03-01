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
	/// Entity map for the post template.
	/// </summary>
	internal class PostTemplateMap : EntityTypeConfiguration<PostTemplate>
	{
		public PostTemplateMap() {
			ToTable("posttemplate");

			Property(t => t.Id).HasColumnName("posttemplate_id");
			Property(t => t.PermalinkId).HasColumnName("posttemplate_permalink_id");
			Property(t => t.Name).HasColumnName("posttemplate_name").IsRequired().HasMaxLength(64);
			Property(t => t.Description).HasColumnName("posttemplate_description").HasMaxLength(255);
			Property(t => t.Preview).HasColumnName("posttemplate_preview");
			Property(t => t.PropertiesJson).HasColumnName("posttemplate_properties");
			Property(t => t.ViewTemplate).HasColumnName("posttemplate_controller").HasMaxLength(128);
			Property(t => t.ShowViewTemplate).HasColumnName("posttemplate_controller_show");
			Property(t => t.ViewArchiveTemplate).HasColumnName("posttemplate_archive_controller").HasMaxLength(128);
			Property(t => t.ShowViewArchiveTemplate).HasColumnName("posttemplate_archive_controller_show");
			Property(t => t.AllowRss).HasColumnName("posttemplate_rss");
			Property(t => t.Type).HasColumnName("posttemplate_type").HasMaxLength(255);
			Property(t => t.Created).HasColumnName("posttemplate_created");
			Property(t => t.Updated).HasColumnName("posttemplate_updated");
			Property(t => t.CreatedById).HasColumnName("posttemplate_created_by");
			Property(t => t.UpdatedById).HasColumnName("posttemplate_updated_by");

			HasRequired(t => t.CreatedBy);
			HasRequired(t => t.UpdatedBy);
		}
	}
}
