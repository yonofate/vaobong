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
	/// Interface for defining a page type programmatically.
	/// </summary>
	public interface IPageType : IType
	{
		/// <summary>
		/// Gets the optional permalink of a page this sould redirect to.
		/// </summary>
		string Redirect { get; }

		/// <summary>
		/// Gets if the redirect can be overriden by the implementing page.
		/// </summary>
		bool ShowRedirect { get; }

		/// <summary>
		/// Gets if this page type is a block.
		/// </summary>
		bool IsBlock { get; }

		/// <summary>
		/// Gets the page types a block can be created for.
		/// </summary>
		IList<Type> ParentTypes { get; }

		/// <summary>
		/// Gets if a page type can contain subpages or not.
		/// </summary>
		bool Subpages { get; }

		/// <summary>
		/// Gets the defined regions.
		/// </summary>
		IList<RegionType> Regions { get; }
	}
}
