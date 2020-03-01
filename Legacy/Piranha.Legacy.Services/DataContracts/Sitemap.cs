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
	public class Sitemap
	{
		[DataMember()]
		public Guid Id { get; set; }
		[DataMember()]
		public string Title { get; set; }
		[DataMember()]
		public string Permalink { get; set; }
		[DataMember()]
		public bool IsHidden { get; set; }
		[DataMember()]
		public bool HasChildren { get; set; }
		[DataMember()]
		public List<Sitemap> ChildNodes { get; set; }
		[DataMember()]
		public string LastPublished { get; set; }
	}
}
