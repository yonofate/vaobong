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
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Data;
using Piranha.Extend;

namespace Piranha.Models.Manager.TemplateModels
{
	/// <summary>
	/// Page template edit model for the manager area.
	/// </summary>
	public class PageEditModel
	{
		#region Binder
		public class Binder : DefaultModelBinder
		{
			/// <summary>
			/// Extend the default binder so that html strings can be fetched from the post.
			/// </summary>
			/// <param name="controllerContext">Controller context</param>
			/// <param name="bindingContext">Binding context</param>
			/// <returns>The page edit model</returns>
			public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
				PageEditModel model = (PageEditModel)base.BindModel(controllerContext, bindingContext);

				bindingContext.ModelState.Remove("Template.Preview");
				model.Template.Preview =
					new HtmlString(bindingContext.ValueProvider.GetUnvalidatedValue("Template.Preview").AttemptedValue);
				return model;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the page template
		/// </summary>
		public virtual PageTemplate Template { get; set; }

		/// <summary>
		/// Gets/sets the available region templates.
		/// </summary>
		public List<RegionTemplate> Regions { get; set; }

		/// <summary>
		/// Gets/sets the available templates.
		/// </summary>
		public List<PageTemplate> Templates { get; set; }

		/// <summary>
		/// Gets/sets the available region types.
		/// </summary>
		public List<dynamic> RegionTypes { get; set; }
		#endregion

		/// <summary>
		/// Default constructor, creates a new model.
		/// </summary>
		public PageEditModel() : this(true) { }

		/// <summary>
		/// Creates a new model.
		/// </summary>
		/// <param name="loadRegionTypes">If the region types should be loaded</param>
		/// <param name="loadTemplates">If the templates should be loaded</param>
		internal PageEditModel(bool loadRegionTypes = true, bool loadTemplates = true) {
			Template = new PageTemplate() {
				Preview = new HtmlString(
					"<table class=\"template\">" +
					"<tr><td></td></tr>" +
					"</table>"
					)
			};
			Regions = new List<RegionTemplate>();
			RegionTypes = new List<dynamic>();

			if (loadRegionTypes) {
				ExtensionManager.Current.GetByExtensionType(ExtensionType.Region).OrderBy(e => e.Metadata.Name).Each((i, r) =>
					RegionTypes.Add(new { Name = ExtensionManager.Current.GetNameByType(r.Value.GetType().FullName), Type = r.Value.GetType().FullName }));
				RegionTypes.Insert(0, new { Name = "", Type = "" });
			}

			if (loadTemplates) {
				Templates = PageTemplate.GetFields("pagetemplate_id, pagetemplate_name, pagetemplate_type", "pagetemplate_site_template = 0", new Data.Params() { OrderBy = "pagetemplate_name" });
			}
		}

		/// <summary>
		/// Gets the model for the template specified by the given id.
		/// </summary>
		/// <param name="id">The template id</param>
		/// <param name="loadRegionTypes">If the region types should be loaded</param>
		/// <param name="loadTemplates">If the templates should be loaded</param>
		/// <returns>The model</returns>
		public static PageEditModel GetById(Guid id, bool loadRegionTypes = true, bool loadTemplates = true) {
			PageEditModel m = new PageEditModel(loadRegionTypes, loadTemplates);
			m.Template = PageTemplate.GetSingle(id);
			m.Regions = RegionTemplate.Get("regiontemplate_template_id = @0", id, new Params() { OrderBy = "regiontemplate_seqno" });

			return m;
		}

		/// <summary>
		/// Saves the model.
		/// </summary>
		/// <returns>Whether the operation succeeded</returns>
		public bool SaveAll() {
			using (IDbTransaction tx = Database.OpenTransaction()) {
				List<object> args = new List<object>();
				string sql = "";
				var isNew = Template.IsNew;

				// Delete all unattached properties
				args.Add(Template.Id);
				Template.Properties.Each((n, p) => {
					sql += (sql != "" ? "," : "") + "@" + (n + 1).ToString();
					args.Add(p);
				});
				Property.Execute("DELETE FROM property WHERE property_parent_id IN (" +
					"SELECT page_id FROM page WHERE page_template_id = @0) " +
					(sql != "" ? "AND property_name NOT IN (" + sql + ")" : ""), tx, args.ToArray());

				// Save the template
				Template.Save(tx);

				// Update all regiontemplates with the id if this is an insert
				if (isNew)
					Regions.ForEach(r => r.TemplateId = Template.Id);

				// Delete removed regions templates
				sql = "";
				args.Clear();
				args.Add(Template.Id);
				var pos = 1;
				foreach (var reg in Regions) {
					if (reg.Id != Guid.Empty) {
						sql += (sql != "" ? "," : "") + "@" + pos.ToString();
						args.Add(reg.Id);
						pos++;
					}
				}
				RegionTemplate.Execute("DELETE FROM regiontemplate WHERE regiontemplate_template_id = @0 " +
					(sql != "" ? "AND regiontemplate_id NOT IN (" + sql + ")" : ""), tx, args.ToArray());
				// Save the regions
				foreach (var reg in Regions)
					reg.Save(tx);

				// Clear all implementing pages from the cache
				var pages = Page.Get("page_template_id = @0", tx, Template.Id);
				foreach (var page in pages)
					page.InvalidateRecord(page);

				tx.Commit();
			}
			// Reload regions
			Regions = RegionTemplate.Get("regiontemplate_template_id = @0", Template.Id,
				new Params() { OrderBy = "regiontemplate_seqno" });

			return true;
		}

		/// <summary>
		/// Deletes the page template and all pages associated with it.
		/// </summary>
		/// <returns>Whether the operation succeeded</returns>
		public bool DeleteAll() {
			List<Piranha.Models.Page> pages = Piranha.Models.Page.Get("page_template_id = @0", Template.Id);

			using (IDbTransaction tx = Database.OpenTransaction()) {
				try {
					foreach (Piranha.Models.Page page in pages) {
						Region.Get("region_page_id = @0", page.Id).ForEach((r) => r.Delete(tx));
						page.Delete(tx);
					}
					Template.Delete(tx);
					tx.Commit();
				} catch { tx.Rollback(); return false; }
			}
			return true;
		}
	}
}
