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
using System.ComponentModel.DataAnnotations;

namespace Piranha.Extend.Regions
{
	/// <summary>
	/// Simple text region.
	/// </summary>
	[Export(typeof(IExtension))]
	[ExportMetadata("InternalId", "TextRegion")]
	[ExportMetadata("Name", "TextRegionName")]
	[ExportMetadata("ResourceType", typeof(Resources.Extensions))]
	[ExportMetadata("Type", ExtensionType.Region)]
	[Serializable]
	public class TextRegion : Extension
	{
		#region Properties
		/// <summary>
		/// Gets/sets the text title.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Extensions), Name = "TextRegionTitle")]
		public string Title { get; set; }

		/// <summary>
		/// Gets/sets the text body.
		/// </summary>
		[Display(ResourceType = typeof(Piranha.Resources.Extensions), Name = "TextRegionBody")]
		public string Body { get; set; }
		#endregion
	}
}