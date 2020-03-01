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
	/// The extension entity.
	/// </summary>
	[Serializable]
	public class Extension : StandardEntity<Extension>
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
		/// Gets/sets the parent id.
		/// </summary>
		public Guid ParentId { get; set; }

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
		/// Gets/sets the type.
		/// </summary>
		public string Type { get; set; }
		#endregion

		#region Internal properties
		/// <summary>
		/// Gets/sets the private Json serialized body.
		/// </summary>
		internal string InternalBody { get; set; }
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
			if (!String.IsNullOrEmpty(Type)) {
				var js = new JavaScriptSerializer();

				if (!String.IsNullOrEmpty(InternalBody)) {
					if (typeof(HtmlString).IsAssignableFrom(ExtensionManager.Current.GetType(Type)))
						return ExtensionManager.Current.CreateInstance(Type, InternalBody);
					return (IExtension)js.Deserialize(InternalBody, ExtensionManager.Current.GetType(Type));
				}
				return ExtensionManager.Current.CreateInstance(Type);
			}
			return null;
		}

		/// <summary>
		/// Updates the internal & external body data.
		/// </summary>
		private void SetBody(IExtension data) {
			JavaScriptSerializer js = new JavaScriptSerializer();

			body = data;
			InternalBody = js.Serialize(data);
		}
		#endregion

		#region Events
		public override void OnSave(DataContext db, EntityState state) {
			JavaScriptSerializer js = new JavaScriptSerializer();
			InternalBody = js.Serialize(Body);

			base.OnSave(db, state);
		}
		#endregion
	}
}
