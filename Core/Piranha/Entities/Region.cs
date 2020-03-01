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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

using Piranha.Extend;

namespace Piranha.Entities
{
	/// <summary>
	/// The region entity
	/// </summary>
	[Serializable]
	public class Region : StandardEntity<Region>
	{
		#region Members
		private IExtension body;
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets whether this is a draft or not.
		/// </summary>
		public bool IsDraft { get; set; }

		/// <summary>
		/// Gets/sets the template id.
		/// </summary>
		public Guid RegionTemplateId { get; set; }

		/// <summary>
		/// Gets/sets the page id.
		/// </summary>
		public Guid PageId { get; set; }

		/// <summary>
		/// Gets/sets whether the page is a draft or not.
		/// </summary>
		public bool IsPageDraft { get; set; }

		/// <summary>
		/// Gets/sets the region name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the body of the extension.
		/// </summary>
		public IExtension Body {
			get {
				if (body == null)
					body = GetBody();
				return body;
			}
			set {
				SetBody(value);
			}
		}

		/// <summary>
		/// Gets/sets the private Json serialized body.
		/// </summary>
		public string InternalBody { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the page this region is related to.
		/// </summary>
		public Page Page { get; set; }

		/// <summary>
		/// Gets/sets the region template.
		/// </summary>
		public RegionTemplate RegionTemplate { get; set; }
		#endregion

		/// <summary>
		/// Gets the typed body element.
		/// </summary>
		/// <typeparam name="T">The body type</typeparam>
		/// <returns>The body</returns>
		public T GetBody<T>() where T : IExtension {
			return (T)Body;
		}

		#region Private methods
		/// <summary>
		/// Gets the Json deserialized body for the region.
		/// </summary>
		/// <returns>The body</returns>
		private IExtension GetBody() {
			if (RegionTemplate == null) {
				using (var db = new DataContext()) {
					RegionTemplate = db.RegionTemplates.Where(t => t.Id == RegionTemplateId).Single();
				}
			}

			if (!String.IsNullOrEmpty(RegionTemplate.Type)) {
				var js = new JavaScriptSerializer();

				if (!String.IsNullOrEmpty(InternalBody)) {
					if (typeof(HtmlString).IsAssignableFrom(ExtensionManager.Current.GetType(RegionTemplate.Type)))
						return ExtensionManager.Current.CreateInstance(RegionTemplate.Type, InternalBody);
					return (IExtension)js.Deserialize(InternalBody, ExtensionManager.Current.GetType(RegionTemplate.Type));
				}
				return ExtensionManager.Current.CreateInstance(RegionTemplate.Type);
			}
			return null;
		}

		/// <summary>
		/// Updates the internal & external body data.
		/// </summary>
		private void SetBody(IExtension data) {
			if (RegionTemplate == null) {
				using (var db = new DataContext()) {
					RegionTemplate = db.RegionTemplates.Where(t => t.Id == RegionTemplateId).Single();
				}
			}

			if (!String.IsNullOrEmpty(RegionTemplate.Type)) {
				var js = new JavaScriptSerializer();
				body = data;

				if (typeof(HtmlString).IsAssignableFrom(ExtensionManager.Current.GetType(RegionTemplate.Type)))
					InternalBody = ((HtmlString)data).ToString();
				else InternalBody = js.Serialize(data);
			}
		}
		#endregion

		#region Events
		public override void OnSave(DataContext db, EntityState state) {
			if (RegionTemplate == null)
				RegionTemplate = db.RegionTemplates.Where(t => t.Id == RegionTemplateId).Single();

			if (!String.IsNullOrEmpty(RegionTemplate.Type)) {
				var js = new JavaScriptSerializer();

				if (typeof(HtmlString).IsAssignableFrom(ExtensionManager.Current.GetType(RegionTemplate.Type)))
					InternalBody = ((HtmlString)Body).ToString();
				else InternalBody = js.Serialize(Body);
			}
			base.OnSave(db, state);
		}
		#endregion
	}
}
