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
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

using Piranha.Extend;

namespace Piranha.Entities
{
	/// <summary>
	/// Entity page model.
	/// </summary>
	public class PageModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the page.
		/// </summary>
		public Page Page { get; set; }

		/// <summary>
		/// Gets/sets the attachments.
		/// </summary>
		public IList<Media> Attachments { get; set; }

		/// <summary>
		/// Gets/sets the regions.
		/// </summary>
		public dynamic Regions { get; set; }

		/// <summary>
		/// Gets/sets the properties.
		/// </summary>
		public dynamic Properties { get; set; }

		/// <summary>
		/// Gets/sets the extensions.
		/// </summary>
		public dynamic Extensions { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected PageModel() {
			Regions = new ExpandoObject();
			Properties = new ExpandoObject();
			Extensions = new ExpandoObject();
			Attachments = new List<Media>();
		}

		/// <summary>
		/// Gets the page models matching the given predicate.
		/// </summary>
		/// <param name="predicate">The predicate</param>
		/// <returns>A list of models</returns>
		public static IList<PageModel> Where(Expression<Func<Page, bool>> predicate) {
			var ret = new List<PageModel>();

			using (var db = new DataContext()) {
				var pages = db.Pages
					.Include(p => p.Template)
					.Include(p => p.Template.RegionTemplates)
					.Include(p => p.Regions)
					.Include(p => p.Properties)
					.Include(p => p.Extensions)
					.Where(predicate)
					.ToList();

				foreach (var page in pages)
					ret.Add(BuildModel(db, page));
			}
			return ret;
		}

		/// <summary>
		/// Builds a page model object from the given page.
		/// </summary>
		/// <param name="db">The data context</param>
		/// <param name="page">The page</param>
		/// <returns>The model</returns>
		private static PageModel BuildModel(DataContext db, Page page) {
			var m = new PageModel();

			m.Page = page;

			// Get Properties
			foreach (var pt in page.Template.Properties) {
				var val = page.Properties.Where(p => p.Name == pt).SingleOrDefault();

				if (val != null)
					((IDictionary<string, object>)m.Properties).Add(pt, val.Value);
				else ((IDictionary<string, object>)m.Properties).Add(pt, "");
			}
			// Get Media
			var media = db.Media.Where(med => page.Attachments.Contains(med.Id)).ToList();
			foreach (var attachment in page.Attachments) {
				var val = media.Where(med => med.Id == attachment).SingleOrDefault();
				if (val != null)
					m.Attachments.Add(val);
			}
			// Get Regions
			foreach (var rt in page.Template.RegionTemplates) {
				if (ExtensionManager.Current.HasType(rt.Type)) {
					object val = null;
					var reg = page.Regions.Where(r => r.RegionTemplateId == rt.Id).SingleOrDefault();

					if (reg != null) {
						if (reg.RegionTemplate == null)
							reg.RegionTemplate = rt;
						val = reg.Body;
					} else val = ExtensionManager.Current.CreateInstance(rt.Type);

					// Initialize region
					val = ((IExtension)val).GetContent(m);
					// Check for post region
					if (val is Extend.Regions.PostRegion)
						val = ((Extend.Regions.PostRegion)val).GetMatchingPosts();

					((IDictionary<string, object>)m.Regions).Add(rt.InternalId, val);
				} else ((IDictionary<string, object>)m.Regions).Add(rt.InternalId, null);
			}
			// Get Extensions
			foreach (var ext in page.Extensions) {
				object val = null;

				if (ExtensionManager.Current.HasType(ext.Type)) {
					val = ext.Body;
				}
			}
			return m;
		}
	}
}