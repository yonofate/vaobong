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
	/// Entity map for the extension.
	/// </summary>
	internal class ExtensionMap : EntityTypeConfiguration<Extension>
	{
		public ExtensionMap() {
			ToTable("extension");

			Property(e => e.Id).HasColumnName("extension_id");
			Property(e => e.ParentId).HasColumnName("extension_parent_id");
			Property(e => e.IsDraft).HasColumnName("extension_draft");
			Property(e => e.InternalBody).HasColumnName("extension_body");
			Property(e => e.Type).HasColumnName("extension_type").IsRequired().HasMaxLength(255);
			Property(e => e.Created).HasColumnName("extension_created");
			Property(e => e.Updated).HasColumnName("extension_updated");
			Property(e => e.CreatedById).HasColumnName("extension_created_by");
			Property(e => e.UpdatedById).HasColumnName("extension_updated_by");

			Ignore(e => e.Body);

			HasRequired(e => e.CreatedBy);
			HasRequired(e => e.UpdatedBy);
		}
	}
}
