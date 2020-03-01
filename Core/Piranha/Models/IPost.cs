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
using System.Web;

namespace Piranha.Models
{
	/// <summary>
	/// Client API for the post.
	/// </summary>
	public interface IPost
	{
		/// <summary>
		/// Gets the id.
		/// </summary>
		Guid Id { get; }

		/// <summary>
		/// Gets the title.
		/// </summary>
		string Title { get; }

		/// <summary>
		/// Gets the permalink.
		/// </summary>
		string Permalink { get; }

		/// <summary>
		/// Gets the excerpt.
		/// </summary>
		string Excerpt { get; }

		/// <summary>
		/// Gets the body.
		/// </summary>
		HtmlString Body { get; }

		/// <summary>
		/// Gets the meta keywords.
		/// </summary>
		string Keywords { get; }

		/// <summary>
		/// Gets the meta description.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the template name.
		/// </summary>
		string TemplateName { get; }

		/// <summary>
		/// Gets the created date.
		/// </summary>
		DateTime Created { get; }

		/// <summary>
		/// Gets the updated date.
		/// </summary>
		DateTime Updated { get; }

		/// <summary>
		/// Gets the published date.
		/// </summary>
		DateTime Published { get; }

		/// <summary>
		/// Gets the last published date.
		/// </summary>
		DateTime LastPublished { get; }
	}
}
