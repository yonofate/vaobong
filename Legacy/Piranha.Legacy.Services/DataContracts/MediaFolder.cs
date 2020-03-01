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
using System.Runtime.Serialization;

namespace Piranha.Legacy.Services.DataContracts
{
	[DataContract]
	public class MediaFolder
	{
		/// <summary>
		/// Gets/sets the unique id.
		/// </summary>
		[DataMember]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the folder name.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the availble child folders.
		/// </summary>
		[DataMember]
		public IList<MediaFolder> Folders { get; set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MediaFolder() {
			Folders = new List<MediaFolder>();
		}
	}
}