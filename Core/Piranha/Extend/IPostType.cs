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

namespace Piranha.Extend
{
	/// <summary>
	/// Interface for defining a post type programmatically.
	/// </summary>
	public interface IPostType : IType { 
		/// <summary>
		/// Gets if posts of this type should be included in the site RSS feed.
		/// </summary>
		bool AllowRss { get; }
	}
}
