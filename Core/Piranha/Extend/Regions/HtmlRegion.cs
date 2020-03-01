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
	/// Standard html region.
	/// </summary>
	[Export(typeof(IExtension))]
	[ExportMetadata("InternalId", "HtmlRegion")]
	[ExportMetadata("Name", "HtmlRegionName")]
	[ExportMetadata("ResourceType", typeof(Resources.Extensions))]
	[ExportMetadata("Type", ExtensionType.Region)]
	[Serializable]
	public class HtmlRegion : HtmlString, IExtension
	{
		/// <summary>
		/// Creates an empty html region.
		/// </summary>
		public HtmlRegion() : base("") { }

		/// <summary>
		/// Creates an html region from the given string.
		/// </summary>
		/// <param name="str">The string</param>
		public HtmlRegion(string str) : base(str) { }

		public virtual void Ensure(DataContext db) { }
		public virtual void Init(object model) { }
		public virtual void InitManager(object model) { }
		public virtual void OnManagerSave(object model) { }
		public virtual void OnManagerDelete(object model) { }
		public virtual object GetContent(object model) {
			return this;
		}
	}
}
