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
	public class Deleted
	{
		[DataMember()]
		public List<DeletedItem> Pages { get; set; }
		[DataMember()]
		public List<DeletedItem> Posts { get; set; }
		[DataMember()]
		public List<DeletedItem> Content { get; set; }
		[DataMember()]
		public List<DeletedItem> Categories { get; set; }
		[DataMember()]
		public List<DeletedItem> PageTemplates { get; set; }
		[DataMember()]
		public List<DeletedItem> PostTemplates { get; set; }
		[DataMember()]
		public List<DeletedItem> MediaFolders { get; set; }
	}

	[DataContract()]
	public class DeletedItem
	{
		[DataMember()]
		public Guid Id { get; set; }
		[DataMember()]
		public string Deleted { get; set; }
	}
}
