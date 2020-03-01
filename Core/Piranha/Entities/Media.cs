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
using System.Text;

namespace Piranha.Entities
{
	/// <summary>
	/// The media entity.
	/// </summary>
	[Serializable]
	public class Media : StandardEntity<Media>
	{
		#region Properties
		/// <summary>
		/// Gets/sets whether this is a draft or not.
		/// </summary>
		internal bool IsDraft { get; set; }

		/// <summary>
		/// Gets/sets the optional parent id.
		/// </summary>
		public Guid? ParentId { get; set; }

		/// <summary>
		/// Gets/sets the permalink id.
		/// </summary>
		public Guid? PermalinkId { get; set; }

		/// <summary>
		/// Gets/sets the filename.
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Gets/sets the original url the media object was 
		/// fetched from.
		/// </summary>
		public string OriginalUrl { get; set; }

		/// <summary>
		/// Gets/sets the last time the media object was synced 
		/// from its external url.
		/// </summary>
		public DateTime? LastSynced { get; set; }

		/// <summary>
		/// Gets/sets the content type.
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Gets/sets the size,
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// Gets/sets whether this is an image or not.
		/// </summary>
		public bool IsImage { get; set; }

		/// <summary>
		/// Gets/sets whether this is a folder or not.
		/// </summary>
		public bool IsFolder { get; set; }

		/// <summary>
		/// Gets/sets the width of the media object in case it is an image.
		/// </summary>
		public int? Width { get; set; }

		/// <summary>
		/// Gets/sets the height of the media object in case it is an image.
		/// </summary>
		public int? Height { get; set; }

		/// <summary>
		/// Gets/sets the media name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the optional alternative text.
		/// </summary>
		public string AltText { get; set; }

		/// <summary>
		/// Gets/sets the optional description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the date the post was initially published.
		/// </summary>
		public DateTime? Published { get; set; }

		/// <summary>
		/// Gets/sets the date the post was last updated.
		/// </summary>
		public DateTime? LastPublished { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		public Permalink Permalink { get; set; }

		/// <summary>
		/// Gets/sets the currently available extensions.
		/// </summary>
		public IList<Extension> Extensions { get; set; }

		/// <summary>
		/// Gets/sets the currently available comments.
		/// </summary>
		public IList<Comment> Comments { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Media() {
			Extensions = new List<Extension>();
			Comments = new List<Comment>();
		}
	}
}
