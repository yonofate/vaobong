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

using Piranha.Entities;

public static class DataExtensions
{
	/// <summary>
	/// Gets the property with the given name from the property list.
	/// </summary>
	/// <param name="properties">The property list</param>
	/// <param name="name">The name</param>
	/// <returns>The property</returns>
	public static Property ByName(this IList<Property> properties, string name) {
		return properties.Where(p => p.Name == name).Single();
	}

	/// <summary>
	/// Gets the region by it's internal id. Note that this method is dependent on
	/// that the RegionTemplate entity is included with the region. 
	/// </summary>
	/// <param name="regions">The region list</param>
	/// <param name="internalId">The internal id</param>
	/// <returns>The region</returns>
	public static Region ByInternalId(this IList<Region> regions, string internalId) {
		using (var db = new Piranha.DataContext()) {
			foreach (var reg in regions) {
				if (reg.RegionTemplate == null)
					reg.RegionTemplate = db.RegionTemplates.Where(t => t.Id == reg.RegionTemplateId).Single();
			}
		}
		return regions.Where(r => r.RegionTemplate.InternalId == internalId).SingleOrDefault();
	}

	/// <summary>
	/// Gets the images available in the media list.
	/// </summary>
	/// <param name="media">The media list</param>
	/// <returns>The images</returns>
	public static IList<Media> Images(this IList<Media> media) {
		return media.Where(m => m.IsImage).ToList();
	}

	/// <summary>
	/// Gets the documents available in the document list.
	/// </summary>
	/// <param name="media">The media list</param>
	/// <returns>The documents</returns>
	public static IList<Media> Documents(this IList<Media> media) {
		return media.Where(m => !m.IsImage).ToList();
	}

	/// <summary>
	/// Gets the extension by it's internal id.
	/// </summary>
	/// <param name="extensions">The extension list</param>
	/// <param name="internalId">The internal id</param>
	/// <returns>The extension</returns>
	public static Extension ByInternalId(this IList<Extension> extensions, string internalId) {
		return extensions.Where(e => Piranha.Extend.ExtensionManager.Current.GetInternalIdByType(e.Type) == internalId).SingleOrDefault();
	}

	/// <summary>
	/// Gets the current string as an html string.
	/// </summary>
	/// <param name="str">The string</param>
	/// <returns>The string as html</returns>
	public static HtmlString Html(this string str) {
		return new HtmlString(str);
	}
}
