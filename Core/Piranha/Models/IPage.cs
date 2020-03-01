﻿/*
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

namespace Piranha.Models
{
	/// <summary>
	/// Client API for the page
	/// </summary>
	public interface IPage
	{
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		Guid Id { get; }

		/// <summary>
		/// Gets whether this is a draft.
		/// </summary>
		bool IsDraft { get; }

		/// <summary>
		/// Gets/sets the group needed to view the page.
		/// </summary>
		Guid GroupId { get; }

		/// <summary>
		/// Gets/sets the parent id.
		/// </summary>
		Guid ParentId { get; }

		/// <summary>
		/// Gets/sets the title.
		/// </summary>
		string Title { get; }

		/// <summary>
		/// Gets/sets the optional navigation title.
		/// </summary>
		string NavigationTitle { get; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		string Permalink { get; }

		/// <summary>
		/// Gets/sets the meta keywords.
		/// </summary>
		string Keywords { get; }

		/// <summary>
		/// Gets/sets the meta description.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets/sets the template name.
		/// </summary>
		string TemplateName { get; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		DateTime Created { get; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		DateTime Updated { get; }

		/// <summary>
		/// Gets/sets the published date.
		/// </summary>
		DateTime Published { get; }

		/// <summary>
		/// Gets/sets the published date.
		/// </summary>
		DateTime LastPublished { get; }
	}
}
