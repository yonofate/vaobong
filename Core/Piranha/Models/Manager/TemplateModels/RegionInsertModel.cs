﻿/*
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
using System.Linq;
using System.Text;

namespace Piranha.Models.Manager.TemplateModels
{
	/// <summary>
	/// Model for inserting a new region template.
	/// </summary>
	public class RegionInsertModel
	{
		/// <summary>
		/// The id of the template.
		/// </summary>
		public Guid TemplateId { get; set; }

		/// <summary>
		/// The name of the region template.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The internal id of the region template.
		/// </summary>
		public string InternalId { get; set; }

		/// <summary>
		/// The type of the region template.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// The sequence number of the region template.
		/// </summary>
		public int Seqno { get; set; }
	}
}
