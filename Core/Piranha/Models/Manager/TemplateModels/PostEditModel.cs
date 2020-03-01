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
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Data;

namespace Piranha.Models.Manager.TemplateModels
{
	/// <summary>
	/// Post template edit model for the manager area.
	/// </summary>
	public class PostEditModel
	{
		#region Binder
		public class Binder : DefaultModelBinder
		{
			/// <summary>
			/// Extend the default binder so that html strings can be fetched from the post.
			/// </summary>
			/// <param name="controllerContext">Controller context</param>
			/// <param name="bindingContext">Binding context</param>
			/// <returns>The post edit model</returns>
			public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
				PostEditModel model = (PostEditModel)base.BindModel(controllerContext, bindingContext);

				bindingContext.ModelState.Remove("Template.Preview");
				model.Template.Preview =
					new HtmlString(bindingContext.ValueProvider.GetUnvalidatedValue("Template.Preview").AttemptedValue);
				return model;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the post template.
		/// </summary>
		public PostTemplate Template { get; set; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		public Permalink Permalink { get; set; }

		/// <summary>
		/// Gets the archive handler prefix.
		/// </summary>
		public string HandlerPrefix { get; private set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new model.
		/// </summary>
		public PostEditModel() {
			Permalink = new Permalink() {
				Id = Guid.NewGuid(),
				Type = Permalink.PermalinkType.ARCHIVE,
				NamespaceId = Config.ArchiveNamespaceId
			};
			Template = new PostTemplate() {
				PermalinkId = Permalink.Id,
				Preview = new HtmlString(
					"<table class=\"template\">\n" +
					"  <tr>\n" +
					"    <td></td>\n" +
					"  </tr>\n" +
					"</table>"
					)
			};
			HandlerPrefix = Application.Current.Handlers.Where(h => h.Id == "ARCHIVE").SingleOrDefault().UrlPrefix;
		}

		/// <summary>
		/// Gets the model for the template specified by the given id.
		/// </summary>
		/// <param name="id">The template id</param>
		/// <returns>The model</returns>
		public static PostEditModel GetById(Guid id) {
			PostEditModel m = new PostEditModel();
			m.Template = PostTemplate.GetSingle(id);
			if (m.Template.Properties == null)
				m.Template.Properties = new List<string>();
			if (m.Template.PermalinkId != Guid.Empty)
				m.Permalink = Permalink.GetSingle(m.Template.PermalinkId);
			else m.Template.PermalinkId = m.Permalink.Id;

			return m;
		}

		/// <summary>
		/// Saves the model.
		/// </summary>
		/// <returns>Whether the operation succeeded or not</returns>
		public bool SaveAll() {
			using (var tx = Database.OpenTransaction()) {
				// Permalink
				if (Permalink.IsNew && String.IsNullOrEmpty(Permalink.Name))
					Permalink.Name = Permalink.Generate(Template.Name);
				Permalink.Save(tx);
				Template.Save(tx);

				// Clear all implementing posts from the cache
				var posts = Post.Get("post_template_id = @0", tx, Template.Id);
				foreach (var post in posts)
					post.InvalidateRecord(post);

				tx.Commit();

				return true;
			}
		}

		/// <summary>
		/// Deletes the post template and all posts associated with it.
		/// </summary>
		/// <returns>Whether the operation succeeded or not</returns>
		public bool DeleteAll() {
			List<Post> posts = Post.Get("post_template_id = @0", Template.Id);

			using (IDbTransaction tx = Database.OpenTransaction()) {
				try {
					foreach (Post post in posts) {
						post.Delete(tx);
					}
					Template.Delete(tx);
					tx.Commit();
				} catch { tx.Rollback(); return false; }
			}
			return true;
		}
	}
}
