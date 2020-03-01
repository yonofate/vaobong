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

namespace Piranha.Entities
{
	/// <summary>
	/// Entity for handling uploads from the client application.
	/// </summary>
	[Serializable]
	public class Upload : StandardEntity<Upload>
	{
		#region Properties
		/// <summary>
		/// Gets/sets the id of the entity this upload belongs to.
		/// </summary>
		public Guid ParentId { get; set; }

		/// <summary>
		/// Gets/sets the filename.
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Gets/sets the content type.
		/// </summary>
		public string Type { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the available comments.
		/// </summary>
		public IList<Comment> Comments { get; set; }
		#endregion

		/// <summary>
		/// Deletes the current upload.
		/// </summary>
		/// <param name="db">The db context</param>
		public override void OnDelete(DataContext db) {
			// Delete the main file
			Application.Current.MediaProvider.Delete(Id, IO.MediaType.Upload);
			DeleteCache();
		}

		/// <summary>
		/// Deletes all cached versions of the media file.
		/// </summary>
		public void DeleteCache() {
			Application.Current.MediaCacheProvider.Delete(Id, IO.MediaType.Upload);
		}
	}
}
