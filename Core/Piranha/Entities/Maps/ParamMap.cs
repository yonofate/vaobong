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
	/// Entity map for the param
	/// </summary>
	internal class ParamMap : EntityTypeConfiguration<Param>
	{
		public ParamMap() {
			ToTable("sysparam");

			Property(p => p.Id).HasColumnName("sysparam_id");
			Property(p => p.Name).HasColumnName("sysparam_name").IsRequired().HasMaxLength(64);
			Property(p => p.Value).HasColumnName("sysparam_value").HasMaxLength(128);
			Property(p => p.Description).HasColumnName("sysparam_description").HasMaxLength(255);
			Property(p => p.IsLocked).HasColumnName("sysparam_locked");
			Property(p => p.Created).HasColumnName("sysparam_created");
			Property(p => p.Updated).HasColumnName("sysparam_updated");
			Property(p => p.CreatedById).HasColumnName("sysparam_created_by");
			Property(p => p.UpdatedById).HasColumnName("sysparam_updated_by");

			HasOptional(p => p.CreatedBy);
			HasOptional(p => p.UpdatedBy);
		}
	}
}
