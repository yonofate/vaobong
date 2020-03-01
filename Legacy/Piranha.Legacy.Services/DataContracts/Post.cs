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
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Piranha.Legacy.Services.DataContracts
{
	[DataContract()]
	public class Post
	{
		[DataMember()]
		public Guid Id { get; set; }
		[DataMember()]
		public string TemplateName { get; set; }
		[DataMember()]
		public string Title { get; set; }
		[DataMember()]
		public string Permalink { get; set; }
		[DataMember()]
		public string Excerpt { get; set; }
		[DataMember()]
		public string Body { get; set; }
		[DataMember()]
		public List<Category> Categories { get; set; }
		[DataMember()]
		public List<Property> Properties { get; set; }
		[DataMember()]
		public List<Attachment> Attachments { get; set; }
		[DataMember()]
		public List<Extension> Extensions { get; set; }
		[DataMember()]
		public ExpandoObject ExpandedExtensions { get; set; }
		[DataMember()]
		public string Created { get; set; }
		[DataMember()]
		public string Updated { get; set; }
		[DataMember()]
		public string Published { get; set; }
		[DataMember()]
		public string LastPublished { get; set; }

		public Post() {
			Categories = new List<Category>();
			Properties = new List<Property>();
			Attachments = new List<Attachment>();
			Extensions = new List<Extension>();
			ExpandedExtensions = new ExpandoObject();
		}
	}
}