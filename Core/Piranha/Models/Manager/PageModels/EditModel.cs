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
using Piranha.Extend;

namespace Piranha.Models.Manager.PageModels
{
	/// <summary>
	/// Page edit model for the manager area.
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

				model.Regions.Each<Region>((i, m) => {
					if (m.Body is HtmlString) {
						bindingContext.ModelState.Remove("Regions[" + i + "].Body");
						m.Body = ExtensionManager.Current.CreateInstance(m.Type,
							bindingContext.ValueProvider.GetUnvalidatedValue("Regions[" + i + "].Body").AttemptedValue);
					}
				});
				return model;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the page.
		/// </summary>
		public virtual Page Page { get; set; }

		/// <summary>
		/// Related page title used as reference when describing the page placement.
		/// </summary>
		public string PlaceRef { get; set; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		public Permalink Permalink { get; set; }

		/// <summary>
		/// Gets/sets the regions.
		/// </summary>
		public virtual List<Region> Regions { get; set; }

		/// <summary>
		/// Gets/sets the Properties.
		/// </summary>
		public virtual List<Property> Properties { get; set; }

		/// <summary>
		/// Gets/sets the attached content.
		/// </summary>
		public virtual List<Content> AttachedContent { get; set; }

		/// <summary>
		/// Gets/sets the current template.
		/// </summary>
		public virtual PageTemplate Template { get; private set; }

		/// <summary>
		/// Gets/sets the groups.
		/// </summary>
		public virtual SelectList Groups { get; set; }

		/// <summary>
		/// Gets/sets the groups that can be disabled.
		/// </summary>
		public List<SysGroup> DisableGroups { get; set; }

		/// <summary>
		/// Gets/sets the available parent pages.
		/// </summary>
		public List<PagePlacement> Parents { get; set; }

		/// <summary>
		/// Gets/sets the available siblings.
		/// </summary>
		public List<PagePlacement> Siblings { get; set; }

		/// <summary>
		/// Gets/sets the extensions.
		/// </summary>
		public List<Extension> Extensions { get; set; }

		/// <summary>
		/// Gets/sets whether or not the page can be removed.
		/// </summary>
		public bool CanDelete { get; set; }

		/// <summary>
		/// Gets/sets whether or not the page can be published. Pages that are copies
		/// can't be published if the original is unpublished.
		/// </summary>
		public bool CanPublish { get; set; }

		/// <summary>
		/// Gets whether this is a site page or not.
		/// </summary>
		public bool IsSite { get { return Permalink.Type == Models.Permalink.PermalinkType.SITE; } }

		/// <summary>
		/// Gets/sets whether comments should be enabled or not.
		/// </summary>
		public bool EnableComments { get; set; }

		/// <summary>
		/// Gets/sets the currently available comments for the current post.
		/// </summary>
		public List<Entities.Comment> Comments { get; set; }

		/// <summary>
		/// Gets/sets the site tree for the current page.
		/// </summary>
		public Entities.SiteTree SiteTree { get; set; }
		#endregion

		#region Inner classes
		public class PagePlacement
		{
			public Guid Id { get; set; }
			public string Title { get; set; }
			public int Level { get; set; }
			public bool IsSelected { get; set; }
			public int Seqno { get; set; }
		}
		#endregion

		/// <summary>
		/// Default constructor, creates a new model.
		/// </summary>
		public EditModel() {
			Regions = new List<Region>();
			Properties = new List<Property>();
			Extensions = new List<Extension>();
			AttachedContent = new List<Piranha.Models.Content>();
			DisableGroups = SysGroup.GetParents(Guid.Empty);
			DisableGroups.Reverse();
			CanDelete = true;
			CanPublish = true;
			Comments = new List<Entities.Comment>();

			List<SysGroup> groups = SysGroup.GetStructure().Flatten();
			groups.Reverse();
			groups.Insert(0, new SysGroup() { Name = Piranha.Resources.Global.Everyone });
			Groups = new SelectList(groups, "Id", "Name");
		}

		/// <summary>
		/// Gets the model for the page specified by the given id.
		/// </summary>
		/// <param name="id">The page id</param>
		/// <param name="draft">Whether to get the draft or not.</param>
		/// <returns>The model</returns>
		public static EditModel GetById(Guid id, bool draft = true) {
			EditModel m = new EditModel();

			m.Page = Piranha.Models.Page.GetSingle(id, draft);
			if (m.Page == null)
				m.Page = Piranha.Models.Page.GetSingle(id);

			if (m.Page != null) {
				m.GetRelated();
				m.CanDelete = Page.GetScalar("SELECT count(*) FROM page WHERE page_parent_id=@0", id) == 0;
			} else throw new ArgumentException("Could not find page with id {" + id.ToString() + "}");

			return m;
		}

		/// <summary>
		/// Creates a new page from the given template and position and returns it
		/// as an edit model.
		/// </summary>
		/// <param name="templateId">The template id</param>
		/// <param name="parentId">The parent id</param>
		/// <param name="seqno">The position sequence number</param>
		/// <param name="siteTreeId">The id of the site tree</param>
		/// <param name="siteTree">The internal id of the site tree</param>
		/// <returns>The edit model</returns>
		public static EditModel CreateByTemplateAndPosition(Guid templateId, Guid parentId, int seqno, Guid siteTreeId, string siteTree) {
			EditModel m = new EditModel();

			m.Page = new Piranha.Models.Page() {
				Id = Guid.NewGuid(),
				TemplateId = templateId,
				SiteTreeId = siteTreeId,
				SiteTreeInternalId = siteTree,
				ParentId = parentId,
				Seqno = seqno
			};
			m.GetRelated();

			return m;
		}

		public static EditModel CreateByOriginalAndPosition(Guid originalId, Guid parentId, int seqno, Guid siteTreeId, string siteTree) {
			var m = new EditModel();
			var p = Page.GetSingle(originalId, true);

			m.Page = new Piranha.Models.Page() {
				Id = Guid.NewGuid(),
				Title = p.Title,
				NavigationTitle = p.NavigationTitle,
				TemplateId = p.TemplateId,
				OriginalId = originalId,
				SiteTreeId = siteTreeId,
				SiteTreeInternalId = siteTree,
				ParentId = parentId,
				Seqno = seqno,
				PageController = p.PageController,
				PageRedirect = p.PageRedirect
			};
			m.GetRelated();

			return m;
		}

		/// <summary>
		/// Reverts to the latest published version.
		/// </summary>
		/// <param name="id">The page id</param>
		public static void Revert(Guid id) {
			EditModel m = EditModel.GetById(id, false);
			m.Page.IsDraft = true;

			// Saving this baby will overwrite the current draft
			m.SaveAll();

			// Now we just have to "turn back time"
			Page.Execute("UPDATE page SET page_updated = page_last_published WHERE page_id = @0 AND page_draft = 1", null, id);
		}

		/// <summary>
		/// Unpublishes the page with the given id.
		/// </summary>
		/// <param name="id">The page id.</param>
		public static void Unpublish(Guid id) {
			using (IDbTransaction tx = Database.OpenTransaction()) {
				// Delete all published content
				Property.Execute("DELETE FROM property WHERE property_draft = 0 AND property_parent_id = @0", tx, id);
				Region.Execute("DELETE FROM region WHERE region_page_draft = 0 AND region_page_id = @0", tx, id);
				Page.Execute("DELETE FROM page WHERE page_draft = 0 AND page_id = @0", tx, id);

				// Remove published dates
				Page.Execute("UPDATE page SET page_published = NULL, page_last_published = NULL WHERE page_id = @0", tx, id);

				// Change global last modified
				Web.ClientCache.SetSiteLastModified(tx);

				// Commit transaction
				tx.Commit();
			}
			var page = Page.GetSingle(id, true);
			page.InvalidateRecord(page);
		}

		public static bool Detach(Guid id) {
			using (var tx = Database.OpenTransaction()) {
				// Get the copied page
				var copy = Page.GetSingle(id, true);

				if (copy.OriginalId != Guid.Empty) {
					// Get the original page
					var m = GetById(copy.OriginalId, true);

					// Copy the readonly data from the original
					copy.OriginalId = Guid.Empty;
					copy.Attachments = m.Page.Attachments;
					copy.GroupId = m.Page.GroupId;
					copy.DisabledGroups = m.Page.DisabledGroups;
					copy.TemplateId = m.Page.TemplateId;
					copy.Keywords = m.Page.Keywords;
					copy.Description = m.Page.Description;
					if (!m.Template.ShowController)
						copy.PageController = null;
					if (!m.Template.ShowRedirect)
						copy.PageRedirect = null;
					copy.Save(tx);

					// Update the regions
					foreach (var reg in m.Regions) {
						reg.Id = Guid.NewGuid();
						reg.PageId = copy.Id;
						reg.IsNew = true;
						reg.Save(tx);
					}

					// Clone the properties
					foreach (var prop in m.Properties) {
						prop.Id = Guid.NewGuid();
						prop.ParentId = copy.Id;
						prop.IsNew = true;
						prop.Save(tx);
					}

					// Clone the extensions
					foreach (var ext in m.Extensions) {
						ext.Id = Guid.NewGuid();
						ext.ParentId = copy.Id;
						ext.IsNew = true;
						ext.Save(tx);
					}
					tx.Commit();
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Saves the page and all of it's related regions.
		/// </summary>
		/// <param name="publish">If the page should be published</param>
		/// <returns>If the operation succeeded</returns>
		public virtual bool SaveAll(bool draft = true) {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					bool permalinkfirst = Page.IsNew;

					// Save permalink first if the page is new
					if (permalinkfirst) {
						if (Permalink.IsNew && String.IsNullOrEmpty(Permalink.Name)) {
							Permalink.Name = Permalink.Generate(!String.IsNullOrEmpty(Page.NavigationTitle) ?
								Page.NavigationTitle : Page.Title);
							var param = SysParam.GetByName("HIERARCHICAL_PERMALINKS");
							if (param != null && param.Value == "1" && Page.ParentId != Guid.Empty) {
								var parent = Page.GetSingle(Page.ParentId, true);
								Permalink.Name = parent.Permalink + "/" + Permalink.Name;
							}
						}
						Permalink.Save(tx);
					}

					// Save page
					if (draft)
						Page.Save(tx);
					else Page.SaveAndPublish(tx);

					// Save regions & properties
					Regions.ForEach(r => {
						r.IsDraft = r.IsPageDraft = true;

						// Call OnSave
						r.Body.OnManagerSave(Page);

						r.Save(tx);
						if (!draft) {
							if (Region.GetScalar("SELECT COUNT(region_id) FROM region WHERE region_id=@0 AND region_draft=0", r.Id) == 0)
								r.IsNew = true;
							r.IsDraft = r.IsPageDraft = false;
							r.Save(tx);
						}
					});
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
						ext.Body.OnManagerSave(Page);

						ext.ParentId = Page.Id;
						ext.Save(tx);
						if (!draft) {
							if (Extension.GetScalar("SELECT COUNT(extension_id) FROM extension WHERE extension_id=@0 AND extension_draft=0", ext.Id) == 0)
								ext.IsNew = true;
							ext.IsDraft = false;
							ext.Save(tx);
						}
					}

					// Save permalink last if the page isn't new
					if (!permalinkfirst)
						Permalink.Save(tx);

					// Change global last modified
					if (!draft)
						Web.ClientCache.SetSiteLastModified(tx);

					// Clear cache for all post regions if we're publishing
					if (!String.IsNullOrEmpty(Page.Permalink) && !draft) {
						foreach (var reg in Regions)
							if (reg.Body is Extend.Regions.PostRegion)
								((Extend.Regions.PostRegion)reg.Body).ClearCache(Page, reg);
					}

					tx.Commit();

					if (IsSite) {
						using (var db = new DataContext()) {
							var site = db.SiteTrees.Where(s => s.Id == Page.SiteTreeId).Single();
							site.MetaTitle = SiteTree.MetaTitle;
							site.MetaDescription = SiteTree.MetaDescription;
							db.SaveChanges();
						}
						if (!draft) {
							PageModel.RemoveSitePageFromCache(Page.SiteTreeId);
							WebPages.WebPiranha.RegisterDefaultHostNames();
						}
					}
				} catch { tx.Rollback(); throw; }
			}
			return true;
		}

		/// <summary>
		/// Deletes the page and all of it's related regions.
		/// </summary>
		/// <returns></returns>
		public virtual bool DeleteAll() {
			// Since we can have multiple rows for all id's, get everything.
			List<Region> regions = Region.GetAllByPageId(Page.Id);
			List<Property> properties = Property.GetAllByParentId(Page.Id);
			List<Page> pages = Page.Get("page_id=@0", Page.Id);

			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				// Call OnDelete for all extensions
				Extensions.ForEach(e => e.Body.OnManagerDelete(Page));

				// Call OnDelete for all regions
				Regions.ForEach(r => r.Body.OnManagerDelete(Page));

				// Delete all entities
				regions.ForEach(r => r.Delete(tx));
				properties.ForEach(p => p.Delete(tx));
				pages.ForEach(p => p.Delete(tx));
				Permalink.Delete(tx);

				// Change global last modified
				Web.ClientCache.SetSiteLastModified(tx);

				tx.Commit();
			}
			return true;
		}

		/// <summary>
		/// Refreshes the model from the database.
		/// </summary>
		public virtual void Refresh() {
			if (Page != null) {
				if (!Page.IsNew) { // Page.Id != Guid.Empty) {
					Page = Page.GetSingle(Page.Id, true);
					CanDelete = Page.GetScalar("SELECT count(*) FROM page WHERE page_parent_id = @0", Page.Id) == 0;
					GetRelated();
				} else {
					Template = PageTemplate.GetSingle("pagetemplate_id = @0", Page.TemplateId);

					// Get placement ref title
					if (Page.ParentId != Guid.Empty || Page.Seqno > 1) {
						Page refpage = null;
						if (Page.Seqno > 1) {
							if (Page.ParentId != Guid.Empty)
								refpage = Page.GetSingle("page_parent_id = @0 AND page_seqno = @1", Page.ParentId, Page.Seqno - 1);
                            else refpage = Page.GetSingle("page_parent_id IS NULL AND page_seqno = @0 AND page_sitetree_id = @1", Page.Seqno - 1, Page.SiteTreeId); //ÖS 2015-03-18 added siteid to the query
						} else {
							refpage = Page.GetSingle(Page.ParentId, true);
						}
						PlaceRef = refpage.Title;
					}

					// Get page position
					Parents = BuildParentPages(Sitemap.GetStructure(Page.SiteTreeInternalId, false), Page);
					Parents.Insert(0, new PagePlacement() { Level = 1, IsSelected = Page.ParentId == Guid.Empty });
					Siblings = BuildSiblingPages(Page.Id, Page.ParentId, Page.Seqno, Page.ParentId, Page.SiteTreeInternalId);

					// Initialize regions
					foreach (var reg in Regions)
						reg.Body.Init(this);
				}
			}
		}

		#region Private methods
		private void GetRelated() {
			// Clear related
			Regions.Clear();
			Properties.Clear();
			AttachedContent.Clear();

			// Get group parents
			DisableGroups = SysGroup.GetParents(Page.GroupId);
			DisableGroups.Reverse();

			// Get template & permalink
			Template = PageTemplate.GetSingle("pagetemplate_id = @0", Page.TemplateId);
			Permalink = Permalink.GetSingle(Page.PermalinkId);
			if (Permalink == null) {
				// Get the site tree
				using (var db = new DataContext()) {
					var sitetree = db.SiteTrees.Where(s => s.Id == Page.SiteTreeId).Single();

					Permalink = new Permalink() { Id = Guid.NewGuid(), Type = Permalink.PermalinkType.PAGE, NamespaceId = sitetree.NamespaceId };
					Page.PermalinkId = Permalink.Id;
				}
			}
			Page.IsBlock = Template.IsBlock;

			// Get placement ref title
			if (!IsSite) {
				if (Page.ParentId != Guid.Empty || Page.Seqno > 1) {
					Page refpage = null;
					if (Page.Seqno > 1) {
						if (Page.ParentId != Guid.Empty)
							refpage = Page.GetSingle("page_parent_id = @0 AND page_seqno = @1", Page.ParentId, Page.Seqno - 1);
                        else refpage = Page.GetSingle("page_parent_id IS NULL AND page_seqno = @0 AND page_sitetree_id = @1", Page.Seqno - 1, Page.SiteTreeId); //ÖS 2015-03-18 added siteid to the query
					} else {
						refpage = Page.GetSingle(Page.ParentId, true);
					}
					PlaceRef = !String.IsNullOrWhiteSpace(refpage.NavigationTitle) ? refpage.NavigationTitle : refpage.Title;
				}
			}

			if (Template != null) {
				// Only load regions & properties if this is an original
				if (Page.OriginalId == Guid.Empty) {
					// Get regions
					var regions = RegionTemplate.Get("regiontemplate_template_id = @0", Template.Id, new Params() { OrderBy = "regiontemplate_seqno" });
					foreach (var rt in regions) {
						var reg = Region.GetSingle("region_regiontemplate_id = @0 AND region_page_id = @1 and region_draft = @2",
							rt.Id, Page.Id, Page.IsDraft);
						if (reg != null)
							Regions.Add(reg);
						else Regions.Add(new Region() {
							InternalId = rt.InternalId,
							Name = rt.Name,
							Type = rt.Type,
							PageId = Page.Id,
							RegiontemplateId = rt.Id,
							IsDraft = Page.IsDraft,
							IsPageDraft = Page.IsDraft
						});
					}

					// Get Properties
					foreach (string name in Template.Properties) {
						Property prp = Property.GetSingle("property_name = @0 AND property_parent_id = @1 AND property_draft = @2",
							name, Page.Id, Page.IsDraft);
						if (prp != null)
							Properties.Add(prp);
						else Properties.Add(new Property() { Name = name, ParentId = Page.Id, IsDraft = Page.IsDraft });
					}
				}
			} else throw new ArgumentException("Could not find page template for page {" + Page.Id.ToString() + "}");

			// Only load attachments if this is an original
			if (Page.OriginalId == Guid.Empty) {
				// Get attached content
				if (Page.Attachments.Count > 0) {
					// Content meta data is actually memcached, so this won't result in multiple queries
					Page.Attachments.ForEach(a => {
						Models.Content c = Models.Content.GetSingle(a, true);
						if (c != null)
							AttachedContent.Add(c);
					});
				}
			}

			// Get page position
			Parents = BuildParentPages(Sitemap.GetStructure(Page.SiteTreeInternalId, false), Page);
			Parents.Insert(0, new PagePlacement() { Level = 1, IsSelected = Page.ParentId == Guid.Empty });
			Siblings = BuildSiblingPages(Page.Id, Page.ParentId, Page.Seqno, Page.ParentId, Page.SiteTreeInternalId);

			// Only load extensions if this is an original
			if (Page.OriginalId == Guid.Empty) {
				// Get extensions
				Extensions = Page.GetExtensions(true);
			}

			// Initialize regions
			foreach (var reg in Regions)
				reg.Body.InitManager(this);

			// Get whether comments should be enabled
			EnableComments = Areas.Manager.Models.CommentSettingsModel.Get().EnablePages;
			if (!Page.IsNew && EnableComments) {
				using (var db = new DataContext()) {
					Comments = db.Comments.
						Include("CreatedBy").
						Where(c => c.ParentId == Page.Id && c.ParentIsDraft == false).
						OrderByDescending(c => c.Created).ToList();
				}
			}

			// Get the site if this is a site page
			if (Permalink.Type == Models.Permalink.PermalinkType.SITE) {
				using (var db = new DataContext()) {
					SiteTree = db.SiteTrees.Where(s => s.Id == Page.SiteTreeId).Single();
				}
			}

			// Check if the page can be published
			if (Page.OriginalId != Guid.Empty) {
				CanPublish = Page.GetScalar("SELECT count(*) FROM page WHERE page_id=@0 AND page_draft=0", Page.OriginalId) > 0;
			}
		}

		private static List<PagePlacement> BuildParentPages(List<Sitemap> sm, Page p = null) {
			var ret = new List<PagePlacement>();

			foreach (Sitemap s in sm) {
				if (p == null || s.Id != p.Id) {
					var prefix = "";
					for (int n = 1; n < s.Level; n++)
						prefix += "&nbsp;&nbsp;&nbsp;";
					ret.Add(new PagePlacement() {
						Id = s.Id,
						Level = s.Level,
						Title = prefix + (!String.IsNullOrWhiteSpace(s.NavigationTitle) ? s.NavigationTitle : s.Title),
						IsSelected = (p != null && s.Id == p.ParentId)
					});
					if (s.Pages.Count > 0)
						ret.AddRange(BuildParentPages(s.Pages, p));
				}
			}
			return ret;
		}

		internal static List<PagePlacement> BuildSiblingPages(Guid page_id, Guid page_parentid, int page_seqno, Guid parentid, string siteTree) {
			List<Page> sib = null;
			if (parentid != Guid.Empty)
				sib = Page.Get("page_parent_id = @0 AND page_id != @1 AND page_draft = 1 AND sitetree_internal_id = @2",
					parentid, page_id, siteTree, new Params() { OrderBy = "page_seqno" });
			else sib = Page.Get("page_parent_id IS NULL AND page_id != @0 AND page_draft = 1 AND sitetree_internal_id = @1",
				page_id, siteTree, new Params() { OrderBy = "page_seqno" });

			var ret = new List<PagePlacement>();
			ret.Add(new PagePlacement() { Seqno = 1, IsSelected = page_parentid == parentid && page_seqno == 1 });
			var selected = page_parentid == parentid && page_seqno == 1;

			foreach (var page in sib) {
				ret.Add(new PagePlacement() {
					Id = page.Id,
					Title = (!String.IsNullOrWhiteSpace(page.NavigationTitle) ? page.NavigationTitle : page.Title),
					Seqno = page_parentid == parentid && page.Seqno > page_seqno ? page.Seqno : page.Seqno + 1,
					IsSelected = page_parentid == parentid && (page.Seqno + 1) == page_seqno
				});
				selected = selected || ret[ret.Count - 1].IsSelected;
			}
			if (!selected)
				ret[ret.Count - 1].IsSelected = true;
			return ret;
		}

		private static string FormatTitle(Sitemap s) {
			string prefix = "";

			for (int n = 0; n < s.Level - 1; n++)
				prefix += "-";
			return prefix + s.Title;
		}
		#endregion
	}
}
