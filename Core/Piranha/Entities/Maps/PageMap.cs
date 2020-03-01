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
	/// Entity map for the page.
	/// </summary>
	internal class PageMap : EntityTypeConfiguration<Page>
	{
		public PageMap() {
			ToTable("page");

			HasKey(p => new { p.Id, p.IsDraft });
			Property(p => p.Id).HasColumnName("page_id");
			Property(p => p.IsDraft).HasColumnName("page_draft");
			Property(p => p.SiteTreeId).HasColumnName("page_sitetree_id");
			Property(p => p.OriginalId).HasColumnName("page_original_id");
			Property(p => p.TemplateId).HasColumnName("page_template_id");
			Property(p => p.GroupId).HasColumnName("page_group_id");
			Property(p => p.DisabledGroupsJson).HasColumnName("page_group_disable_id");
			Property(p => p.ParentId).HasColumnName("page_parent_id");
			Property(p => p.PermalinkId).HasColumnName("page_permalink_id");
			Property(p => p.Seqno).HasColumnName("page_seqno");
			Property(p => p.Title).HasColumnName("page_title").IsRequired().HasMaxLength(128);
			Property(p => p.NavigationTitle).HasColumnName("page_navigation_title").HasMaxLength(128);
			Property(p => p.IsHidden).HasColumnName("page_is_hidden");
			Property(p => p.Keywords).HasColumnName("page_keywords").HasMaxLength(128);
			Property(p => p.Description).HasColumnName("page_description").HasMaxLength(255);
			Property(p => p.AttachmentsJson).HasColumnName("page_attachments");
			Property(p => p.ViewTemplate).HasColumnName("page_controller").HasMaxLength(128);
			Property(p => p.ViewRedirect).HasColumnName("page_redirect").HasMaxLength(128);
			Property(p => p.Created).HasColumnName("page_created");
			Property(p => p.Updated).HasColumnName("page_updated");
			Property(p => p.Published).HasColumnName("page_published");
			Property(p => p.LastPublished).HasColumnName("page_last_published");
			Property(p => p.LastModified).HasColumnName("page_last_modified");
			Property(p => p.CreatedById).HasColumnName("page_created_by");
			Property(p => p.UpdatedById).HasColumnName("page_updated_by");

			HasRequired(p => p.SiteTree);
			HasRequired(p => p.Template);
			HasRequired(p => p.Permalink);
			HasMany(p => p.Regions).WithRequired(r => r.Page);
			HasMany(p => p.Properties).WithRequired().HasForeignKey(pr => new { pr.ParentId, pr.IsDraft });
			HasMany(p => p.Extensions).WithRequired().HasForeignKey(e => new { e.ParentId, e.IsDraft });
			HasMany(p => p.Comments).WithRequired().HasForeignKey(c => new { c.ParentId, c.ParentIsDraft });
			HasRequired(p => p.CreatedBy);
			HasRequired(p => p.UpdatedBy);

			Ignore(p => p.Attachments);
			Ignore(p => p.DisabledGroups);
		}
	}
}
