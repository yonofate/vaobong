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
using System.ComponentModel.Composition;
using System.Web;

namespace Piranha.Extend.Regions
{
	/// <summary>
	/// Simple content region for markdown.
	/// </summary>
	[Export(typeof(IExtension))]
	[ExportMetadata("InternalId", "MarkdownRegion")]
	[ExportMetadata("Name", "Markdown")]
	[ExportMetadata("Type", ExtensionType.Region)]
	[Serializable]
	public class MarkdownRegion : HtmlString, IExtension
	{
		/// <summary>
		/// Creates an empty markdown region.
		/// </summary>
		public MarkdownRegion() : base("") { }

		/// <summary>
		/// Creates an markdown region from the given string.
		/// </summary>
		/// <param name="str">The string</param>
		public MarkdownRegion(string str) : base(str) { }

		/// <summary>
		/// Ensures the region
		/// </summary>
		/// <param name="db">The current context</param>
		public virtual void Ensure(DataContext db) { }

		/// <summary>
		/// Initializes the region,
		/// </summary>
		/// <param name="model">The page model</param>
		public virtual void Init(object model) { }

		/// <summary>
		/// Initializes the region for the manager interface.
		/// </summary>
		/// <param name="model">The page model</param>
		public virtual void InitManager(object model) { }

		/// <summary>
		/// Triggered when the region is saved from the manager interface.
		/// </summary>
		/// <param name="model">The page model</param>
		public virtual void OnManagerSave(object model) { }

		/// <summary>
		/// Triggered when the region is deleted from the manager interface.
		/// </summary>
		/// <param name="model">The page model</param>
		public virtual void OnManagerDelete(object model) { }

		/// <summary>
		/// Formats the content for client applications.
		/// </summary>
		/// <param name="model">The page model</param>
		/// <returns>The formatted data as an HtmlString</returns>
		public virtual object GetContent(object model) {
			var md = new MarkdownSharp.Markdown();

			return new HtmlString(md.Transform(this.ToString()));
		}
	}
}