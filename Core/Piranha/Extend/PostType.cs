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

namespace Piranha.Extend
{
	/// <summary>
	/// Base class for easily defining a post type. 
	/// </summary>
	public abstract class PostType : IPostType
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		public virtual string Name { get; protected set; }

		/// <summary>
		/// Gets the optional description.
		/// </summary>
		public virtual string Description { get; protected set; }

		/// <summary>
		/// Gets the html preview.
		/// </summary>
		public virtual string Preview { get; protected set; }

		/// <summary>
		/// Gets the controller/viewtemplate depending on the current
		/// application is using WebPages or Mvc.
		/// </summary>
		public virtual string Controller { get; protected set; }

		/// <summary>
		/// Gets if pages of the current type should be able to 
		/// override the controller.
		/// </summary>
		public virtual bool ShowController { get; protected set; }

		/// <summary>
		/// Gets the view. This is only relevant for Mvc applications.
		/// </summary>
		public virtual string View { get; protected set; }

		/// <summary>
		/// Gets if pages of the current type should be able to 
		/// override the controller.
		/// </summary>
		public virtual bool ShowView { get; protected set; }

		/// <summary>
		/// Gets if posts of this type should be included in the site RSS feed.
		/// </summary>
		public virtual bool AllowRss { get; protected set; }

		/// <summary>
		/// Gets the defíned properties.
		/// </summary>
		public virtual IList<string> Properties { get; protected set; }

		public PostType() {
			Properties = new List<string>();

			Preview = "<table class=\"template\"><tr><td></td></tr></table>";
		}
	}
}