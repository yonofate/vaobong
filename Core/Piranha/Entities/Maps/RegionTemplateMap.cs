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
	/// Entity map for the region template.
	/// </summary>
	public class RegionTemplateMap : EntityTypeConfiguration<RegionTemplate>
	{
		public RegionTemplateMap() {
			ToTable("regiontemplate");

			Property(r => r.Id).HasColumnName("regiontemplate_id");
			Property(r => r.TemplateId).HasColumnName("regiontemplate_template_id");
			Property(r => r.InternalId).HasColumnName("regiontemplate_internal_id").IsRequired().HasMaxLength(32);
			Property(r => r.Seqno).HasColumnName("regiontemplate_seqno");
			Property(r => r.Name).HasColumnName("regiontemplate_name").IsRequired().HasMaxLength(64);
			Property(r => r.Description).HasColumnName("regiontemplate_description").HasMaxLength(255);
			Property(r => r.Type).HasColumnName("regiontemplate_type").IsRequired().HasMaxLength(255);
			Property(r => r.Created).HasColumnName("regiontemplate_created");
			Property(r => r.Updated).HasColumnName("regiontemplate_updated");
			Property(r => r.CreatedById).HasColumnName("regiontemplate_created_by");
			Property(r => r.UpdatedById).HasColumnName("regiontemplate_updated_by");

			HasRequired(r => r.CreatedBy);
			HasRequired(r => r.UpdatedBy);
		}
	}
}