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
using System.Web.Script.Serialization;

using Piranha.Data;
using Piranha.Extend;

namespace Piranha.Models.Manager.PostModels
{
	/// <summary>
	/// Edit model for a post in the manager area.
	/// </summary>
	public class EditModel
	{
		#region Inner classes
		public enum ActionType { NORMAL, SEO, ATTACHMENTS }
		#endregion

		#region Members
		/// <summary>
		/// The current page action.
		/// </summary>
		public ActionType Action = ActionType.NORMAL;
		#endregion

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
				EditModel model = (EditModel)base.BindModel(controllerContext, bindingContext);

				bindingContext.ModelState.Remove("Post.Body");

				model.Post.Body =
					new HtmlString(bindingContext.ValueProvider.GetUnvalidatedValue("Post.Body").AttemptedValue);

				// Allow HtmlString extensions
				model.Extensions.Each((i, m) => {
					if (m.Body is HtmlString) {
						bindingContext.ModelState.Remove("Extensions[" + i + "].Body");
						m.Body = ExtensionManager.Current.CreateInstance(m.Type,
							bindingContext.ValueProvider.GetUnvalidatedValue("Extensions[" + i + "].Body").AttemptedValue);
					}
				});
				return model;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the post.
		/// </summary>
		public Post Post { get; set; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		public Permalink Permalink { get; set; }

		/// <summary>
		/// Gets/sets the Properties.
		/// </summary>
		public virtual List<Property> Properties { get; set; }

		/// <summary>
		/// Gets/sets the post template.
		/// </summary>
		public PostTemplate Template { get; set; }

		/// <summary>
		/// Gets/sets the categories associated with the post.
		/// </summary>
		public List<Guid> PostCategories { get; set; }

		/// <summary>
		/// Gets/sets the available categories.
		/// </summary>
		public MultiSelectList Categories { get; set; }

		/// <summary>
		/// Gets/sets the attached content.
		/// </summary>
		public virtual List<Content> AttachedContent { get; set; }

		/// <summary>
		/// Gets/sets the available content.
		/// </summary>
		public List<Content> Content { get; set; }

		/// <summary>
		/// Gets/sets the available extensions.
		/// </summary>
		public List<Extension> Extensions { get; set; }

		/// <summary>
		/// Gets/sets whether comments should be enabled or not.
		/// </summary>
		public bool EnableComments { get; set; }

		/// <summary>
		/// Gets/sets the currently available comments for the current post.
		/// </summary>
		public List<Entities.Comment> Comments { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public EditModel() {
			Post = new Post();
			PostCategories = new List<Guid>();
			Properties = new List<Property>();
			Extensions = new List<Extension>();
			AttachedContent = new List<Piranha.Models.Content>();
			Content = Piranha.Models.Content.Get();
			Comments = new List<Entities.Comment>();
			Categories = new MultiSelectList(Category.GetFields("category_id, category_name",
				new Params() { OrderBy = "category_name" }), "Id", "Name");
		}

		/// <summary>
		/// Creates a new post from the given template and return it 
		/// as a edit model.
		/// </summary>
		/// <param name="templateId">The template id</param>
		/// <returns>The edit model</returns>
		public static EditModel CreateByTemplate(Guid templateId) {
			EditModel m = new EditModel();

			m.Template = PostTemplate.GetSingle(templateId);
			m.Permalink = new Permalink() {
				Id = Guid.NewGuid(),
				Type = Permalink.PermalinkType.POST,
				NamespaceId = Config.DefaultNamespaceId
			};
			m.Post = new Piranha.Models.Post() {
				Id = Guid.NewGuid(),
				TemplateId = templateId,
				PermalinkId = m.Permalink.Id,
				AllowRss = m.Template.AllowRss
			};
			m.GetRelated();

			return m;
		}

		/// <summary>
		/// Gets the model for the post
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static EditModel GetById(Guid id, bool draft = true) {
			EditModel m = new EditModel();
			m.Post = Piranha.Models.Post.GetSingle(id, draft);
			m.Template = PostTemplate.GetSingle(m.Post.TemplateId);
			m.Permalink = Permalink.GetSingle(m.Post.PermalinkId);
			Category.GetByPostId(m.Post.Id, draft).ForEach(c => m.PostCategories.Add(c.Id));
			m.Categories = new MultiSelectList(Category.GetFields("category_id, category_name",
				new Params() { OrderBy = "category_name" }), "Id", "Name", m.PostCategories);
			m.GetRelated();

			return m;
		}

		/// <summary>
		/// Reverts to the latest published version.
		/// </summary>
		/// <param name="id">The post id</param>
		public static void Revert(Guid id) {
			EditModel m = EditModel.GetById(id, false);
			m.Post.IsDraft = true;

			// Saving this baby will overwrite the current draft
			m.SaveAll();

			// Now we just have to "turn back time"
			Post.Execute("UPDATE post SET post_updated = post_last_published WHERE post_id = @0 AND post_draft = 1", null, id);
		}

		/// <summary>
		/// Unpublishes the page with the given id.
		/// </summary>
		/// <param name="id">The page id.</param>
		public static void Unpublish(Guid id) {
			using (IDbTransaction tx = Database.OpenTransaction()) {
				// Delete all published content
				Relation.Execute("DELETE FROM relation WHERE relation_draft = 0 AND relation_data_id = @0", tx, id);
				Post.Execute("DELETE FROM post WHERE post_draft = 0 AND post_id = @0", tx, id);

				// Remove published dates
				Post.Execute("UPDATE post SET post_published = NULL, post_last_published = NULL WHERE post_id = @0", tx, id);

				// Commit transaction
				tx.Commit();
			}
		}

		/// <summary>
		/// Saves the model.
		/// </summary>
		/// <returns>Whether the action was successful or not</returns>
		public bool SaveAll(bool draft = true) {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					bool permalinkfirst = Post.IsNew;

					// Save permalink before the post if this is an insert
					if (permalinkfirst) {
						// Permalink
						if (Permalink.IsNew && String.IsNullOrEmpty(Permalink.Name))
							Permalink.Name = Permalink.Generate(Post.Title);
						Permalink.Save(tx);
					}

					// Post
					if (draft)
						Post.Save(tx);
					else Post.SaveAndPublish(tx);

					// Save permalink after the post if this is an update
					if (!permalinkfirst) {
						Permalink.Save(tx);
					}
					// Properties
					Properties.ForEach(p => {
						p.IsDraft = true;
						p.Save(tx);
						if (!draft) {
							if (Property.GetScalar("SELECT COUNT(property_id) FROM property WHERE property_id=@0 AND property_draft=0", p.Id) == 0)
								p.IsNew = true;
							p.IsDraft = false;
							p.Save(tx);
						}
					});

					// Save extensions
					foreach (var ext in Extensions) {
						// Call OnSave
						ext.Body.OnManagerSave(Post);

						ext.ParentId = Post.Id;
						ext.Save(tx);
						if (!draft) {
							if (Extension.GetScalar("SELECT COUNT(extension_id) FROM extension WHERE extension_id=@0 AND extension_draft=0", ext.Id) == 0)
								ext.IsNew = true;
							ext.IsDraft = false;
							ext.Save(tx);
						}
					}

					// Update categories
					Relation.DeleteByDataId(Post.Id, tx, true);
					List<Relation> relations = new List<Relation>();
					PostCategories.ForEach(pc => relations.Add(new Relation() {
						DataId = Post.Id, RelatedId = pc, Type = Relation.RelationType.POSTCATEGORY
					})
						);
					relations.ForEach(r => r.Save(tx));

					// Publish categories
					if (!draft) {
						Relation.DeleteByDataId(Post.Id, tx, false);
						relations.ForEach(r => {
							r.IsDraft = false;
							r.IsNew = true;
						});
						relations.ForEach(r => r.Save(tx));
					}
					tx.Commit();
				} catch { tx.Rollback(); throw; }
			}
			return true;
		}

		/// <summary>
		/// Deletes the post.
		/// </summary>
		/// <returns></returns>
		public virtual bool DeleteAll() {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				List<Relation> rel = Relation.Get("relation_data_id = @0", Post.Id);
				List<Post> posts = Post.Get("post_id = @0", Post.Id);

				// Call OnDelete for all extensions
				Extensions.ForEach(e => e.Body.OnManagerDelete(Post));

				// Delete all entities
				rel.ForEach(r => r.Delete(tx));
				posts.ForEach(p => p.Delete(tx));
				Permalink.Delete(tx);

				tx.Commit();
			}
			return true;
		}

		/// <summary>
		/// Refreshes the current model.
		/// </summary>
		public void Refresh() {
			if (Post != null) {
				if (!Post.IsNew) {
					Post = Piranha.Models.Post.GetSingle(Post.Id, true);
					Permalink = Permalink.GetSingle(Post.PermalinkId);
					Category.GetByPostId(Post.Id).ForEach(c => PostCategories.Add(c.Id));
					Categories = new MultiSelectList(Category.GetFields("category_id, category_name",
						new Params() { OrderBy = "category_name" }), "Id", "Name", PostCategories);
				}
				Template = PostTemplate.GetSingle(Post.TemplateId);
				GetRelated();
			}
		}

		#region Private methods
		private void GetRelated() {
			// Clear related
			Properties.Clear();

			if (Template != null) {
				// Get Properties
				foreach (string name in Template.Properties) {
					Property prp = Property.GetSingle("property_name = @0 AND property_parent_id = @1 AND property_draft = @2",
						name, Post.Id, Post.IsDraft);
					if (prp != null)
						Properties.Add(prp);
					else Properties.Add(new Property() { Name = name, ParentId = Post.Id, IsDraft = Post.IsDraft });
				}
			}

			// Get selected categories
			if (PostCategories.Count > 0)
				Categories = new MultiSelectList(Category.GetFields("category_id, category_name",
					new Params() { OrderBy = "category_name" }), "Id", "Name", PostCategories);

			// Get attached content
			if (Post.Attachments.Count > 0) {
				// Content meta data is actually memcached, so this won't result in multiple queries
				Post.Attachments.ForEach(a => {
					Models.Content c = Models.Content.GetSingle(a, true);
					if (c != null)
						AttachedContent.Add(c);
				});
			}

			// Get extensions
			Extensions = Post.GetExtensions(true);

			// Get whether comments should be enabled
			EnableComments = Areas.Manager.Models.CommentSettingsModel.Get().EnablePosts;
			if (!Post.IsNew && EnableComments) {
				using (var db = new DataContext()) {
					Comments = db.Comments.
						Include("CreatedBy").
						Where(c => c.ParentId == Post.Id && c.ParentIsDraft == false).
						OrderByDescending(c => c.Created).ToList();
				}
			}
		}
		#endregion
	}
}
