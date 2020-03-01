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
using System.ComponentModel;

namespace Piranha.Extend
{
	/// <summary>
	/// Interface defining the meta data availble to MEF for extensions.
	/// </summary>
	public interface IExtensionMeta
	{
		/// <summary>
		/// Gets the display name of the extension used in the manager.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the internal id of the extensions used to access it from
		/// the application. This data is not neccessary for Regions as
		/// they are assigned an internal id in the template.
		/// </summary>
		[DefaultValue("")]
		string InternalId { get; }

		/// <summary>
		/// Gets the optional virtual path to the icon that will be used in
		/// the manager when presenting the extension.
		/// </summary>
		[DefaultValue("~/res.ashx/areas/manager/content/img/ico-missing-ico.png")]
		string IconPath { get; }

		/// <summary>
		/// Gets the optional resource type that should be used to resolve the resource.
		/// </summary>
		[DefaultValue(null)]
		Type ResourceType { get; }

		/// <summary>
		/// Gets the extension type.
		/// </summary>
		ExtensionType Type { get; }
	}
}