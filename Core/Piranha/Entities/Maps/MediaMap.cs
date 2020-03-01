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
	/// Entity map for media.
	/// </summary>
	internal class MediaMap : EntityTypeConfiguration<Media>
	{
		public MediaMap() {
			ToTable("content");

			HasKey(m => new { m.Id, m.IsDraft });
			Property(m => m.Id).HasColumnName("content_id");
			Property(m => m.IsDraft).HasColumnName("content_draft");
			Property(m => m.ParentId).HasColumnName("content_parent_id");
			Property(m => m.PermalinkId).HasColumnName("content_permalink_id");
			Property(m => m.Filename).HasColumnName("content_filename").HasMaxLength(128);
			Property(m => m.OriginalUrl).HasColumnName("content_url").HasMaxLength(255);
			Property(m => m.LastSynced).HasColumnName("content_synced");
			Property(m => m.ContentType).HasColumnName("content_type").HasMaxLength(255);
			Property(m => m.Size).HasColumnName("content_size");
			Property(m => m.IsImage).HasColumnName("content_image");
			Property(m => m.IsFolder).HasColumnName("content_folder");
			Property(m => m.Width).HasColumnName("content_width");
			Property(m => m.Height).HasColumnName("content_height");
			Property(m => m.Name).HasColumnName("content_name").HasMaxLength(128);
			Property(m => m.AltText).HasColumnName("content_alt").HasMaxLength(128);
			Property(m => m.Description).HasColumnName("content_description").HasMaxLength(255);
			Property(m => m.Created).HasColumnName("content_created");
			Property(m => m.Updated).HasColumnName("content_updated");
			Property(m => m.Published).HasColumnName("content_published");
			Property(m => m.LastPublished).HasColumnName("content_last_published");
			Property(m => m.CreatedById).HasColumnName("content_created_by");
			Property(m => m.UpdatedById).HasColumnName("content_updated_by");

			HasRequired(m => m.CreatedBy);
			HasRequired(m => m.UpdatedBy);
			HasMany(m => m.Extensions).WithRequired().HasForeignKey(e => new { e.ParentId, e.IsDraft });
			HasMany(m => m.Comments).WithRequired().HasForeignKey(c => new { c.ParentId, c.ParentIsDraft });
		}
	}
}
