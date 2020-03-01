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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Piranha.Legacy.Services.DataContracts
{
	[DataContract()]
	public class Content
	{
		[DataMember()]
		public Guid Id { get; set; }
		[DataMember()]
		public Guid ParentId { get; set; }
		[DataMember()]
		public string Filename { get; set; }
		[DataMember()]
		public string Name { get; set; }
		[DataMember()]
		public string DisplayName { get; set; }
		[DataMember()]
		public string Description { get; set; }
		[DataMember()]
		public string Type { get; set; }
		[DataMember()]
		public int Size { get; set; }
		[DataMember()]
		public int? Width { get; set; }
		[DataMember()]
		public int? Height { get; set; }
		[DataMember()]
		public string ThumbnailUrl { get; set; }
		[DataMember()]
		public string ContentUrl { get; set; }
		[DataMember()]
		public List<Category> Categories { get; set; }
		[DataMember()]
		public string Created { get; set; }
		[DataMember()]
		public string Updated { get; set; }

		public Content() {
			Categories = new List<Category>();
		}
	}
}
