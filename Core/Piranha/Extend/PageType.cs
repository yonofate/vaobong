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
	/// Base class for easily defining a page type. 
	/// </summary>
	public abstract class PageType : IPageType
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
		/// Gets/sets the optional permalink of a page this sould redirect to.
		/// </summary>
		public virtual string Redirect { get; protected set; }

		/// <summary>
		/// Gets/sets if the redirect can be overriden by the implementing page.
		/// </summary>
		public virtual bool ShowRedirect { get; protected set; }

		/// <summary>
		/// Gets/sets if this page type is a block.
		/// </summary>
		public virtual bool IsBlock { get; protected set; }

		/// <summary>
		/// Gets the page types a block can be created for.
		/// </summary>
		public virtual IList<Type> ParentTypes { get; protected set; }

		/// <summary>
		/// Gets if a page type can contain subpages or not.
		/// </summary>
		public virtual bool Subpages { get; protected set; }

		/// <summary>
		/// Gets the defíned properties.
		/// </summary>
		public virtual IList<string> Properties { get; protected set; }

		/// <summary>
		/// Gets the defined regions.
		/// </summary>
		public virtual IList<RegionType> Regions { get; protected set; }

		public PageType() {
			Properties = new List<string>();
			Regions = new List<RegionType>();
			ParentTypes = new List<Type>();
			Subpages = true;

			Preview = "<table class=\"template\"><tr><td></td></tr></table>";
		}
	}
}