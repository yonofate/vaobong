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
	/// Class for manually composing the extension manager.
	/// </summary>
	internal class ExtensionMeta : IExtensionMeta
	{
		/// <summary>
		/// Gets the display name of the extension used in the manager.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets the internal id of the extensions used to access it from
		/// the application. This data is not neccessary for Regions as
		/// they are assigned an internal id in the template.
		/// </summary>
		public string InternalId { get; set; }

		/// <summary>
		/// Gets the optional virtual path to the icon that will be used in
		/// the manager when presenting the extension.
		/// </summary>
		public string IconPath { get; set; }

		/// <summary>
		/// Gets the optional resource type that should be used to resolve the resource.
		/// </summary>
		public Type ResourceType { get; set; }

		/// <summary>
		/// Gets the extension type.
		/// </summary>
		public ExtensionType Type { get; set; }

		public ExtensionMeta() {
			IconPath = "~/res.ashx/areas/manager/content/img/ico-missing-ico.png";
			InternalId = "";
		}
	}
}