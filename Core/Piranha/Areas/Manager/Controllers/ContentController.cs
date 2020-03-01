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
using System.IO;
using System.Web;
using System.Web.Mvc;

using Piranha.Models;
using Piranha.Models.Manager.ContentModels;

namespace Piranha.Areas.Manager.Controllers
{
	/// <summary>
	/// Manager controll for media files.
	/// </summary>
	public class ContentController : ManagerController
	{
		/// <summary>
		/// Gets the list view.
		/// </summary>
		[Access(Function = "ADMIN_CONTENT")]
		public ActionResult Index(string id = "") {
			var m = ListModel.Get();

			try {
				if (!String.IsNullOrEmpty(id))
					ViewBag.Expanded = new Guid(id);
				else ViewBag.Expanded = Guid.Empty;
			} catch {
				ViewBag.Expanded = Guid.Empty;
			}

			// Executes the media list loaded hook, if registered
			if (WebPages.Hooks.Manager.MediaListModelLoaded != null)
				WebPages.Hooks.Manager.MediaListModelLoaded(this, WebPages.Manager.GetActiveMenuItem(), m);

			return View("Index", ListModel.Get());
		}

		/// <summary>
		/// Gets the popup list.
		/// </summary>
		public ActionResult Popup(string id = "") {
			var published = false;
			if (!String.IsNullOrEmpty(Request["tinymce"]))
				published = Request["tinymce"] == "true";
			string filter = Request["filter"];

			// Disable caching for the popup
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			// Return view
			return View("Popup", PopupModel.Get(id, published, filter));
		}

		/// <summary>
		/// Gets the files list.
		/// </summary>
		/// <returns></returns>
		[Access(Function = "ADMIN_CONTENT")]
		public ActionResult Uploads() {
			return View("Uploads", UploadModel.Get());
		}

		/// <summary>
		/// Inserts a new media object.
		/// </summary>
		/// <param name="parentid">The optional parent folder id.</param>
		[Access(Function = "ADMIN_CONTENT")]
		public ActionResult Insert(string id) {
			return EditInternal(new EditModel(false, !String.IsNullOrEmpty(id) ? new Guid(id) : Guid.Empty), true);
		}

		/// <summary>
		/// Inserts a new media folder object.
		/// </summary>
		/// <param name="parentid">The optional parent folder id.</param>
		[Access(Function = "ADMIN_CONTENT")]
		public ActionResult InsertFolder(string id) {
			return EditInternal(new EditModel(true, !String.IsNullOrEmpty(id) ? new Guid(id) : Guid.Empty), true);
		}

		/// <summary>
		/// Edits or inserts a new content model.
		/// </summary>
		/// <param name="id">The id of the content</param>
		[Access(Function = "ADMIN_CONTENT")]
		public ActionResult Edit(string id) {
			return EditInternal(EditModel.GetById(new Guid(id)));
		}

		/// <summary>
		/// Insert or update of a media object.
		/// </summary>
		/// <param name="m">The new or existing model</param>
		/// <param name="insert">Whether to insert or update</param>
		/// <returns></returns>
		private ActionResult EditInternal(EditModel m, bool insert = false) {
			ViewBag.Folder = m.Content.IsFolder;

			if (insert)
				ViewBag.Title = Piranha.Resources.Content.EditTitleNew;
			else if (m.Content.IsImage)
				ViewBag.Title = Piranha.Resources.Content.EditTitleExistingImage;
			else if (m.Content.IsFolder)
				ViewBag.Title = Piranha.Resources.Content.EditTitleExistingFolder;
			else ViewBag.Title = Piranha.Resources.Content.EditTitleExistingDocument;

			// Executes the media edit loaded hook, if registered
			if (WebPages.Hooks.Manager.MediaEditModelLoaded != null)
				WebPages.Hooks.Manager.MediaEditModelLoaded(this, WebPages.Manager.GetActiveMenuItem(), m);

			return View("Edit", m);
		}

		/// <summary>
		/// Saves the current edit model.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost(), ValidateInput(false)]
		[Access(Function = "ADMIN_CONTENT")]
		public ActionResult Edit(bool draft, EditModel m) {
			try {
				// Executes the media edit before save hook, if registered
				if (WebPages.Hooks.Manager.MediaEditModelBeforeSave != null)
					WebPages.Hooks.Manager.MediaEditModelBeforeSave(this, WebPages.Manager.GetActiveMenuItem(), m, !draft);

				if (m.SaveAll(draft)) {
					// Executes the media edit after save hook, if registered
					if (WebPages.Hooks.Manager.MediaEditModelAfterSave != null)
						WebPages.Hooks.Manager.MediaEditModelAfterSave(this, WebPages.Manager.GetActiveMenuItem(), m, !draft);

					ViewBag.Folder = m.Content.IsFolder;
					if (m.Content.IsImage) {
						ViewBag.Title = Piranha.Resources.Content.EditTitleExistingImage;
						if (draft) {
							SuccessMessage(Piranha.Resources.Content.MessageImageSaved, true);
						} else {
							if (m.Content.Published == m.Content.LastPublished)
								SuccessMessage(Piranha.Resources.Content.MessageImagePublished, true);
							else SuccessMessage(Piranha.Resources.Content.MessageUpdated, true);
						}
					} else if (m.Content.IsFolder) {
						ViewBag.Title = Piranha.Resources.Content.EditTitleExistingFolder;
						if (draft) {
							SuccessMessage(Piranha.Resources.Content.MessageFolderSaved, true);
						} else {
							if (m.Content.Published == m.Content.LastPublished)
								SuccessMessage(Piranha.Resources.Content.MessageFolderPublished, true);
							else SuccessMessage(Piranha.Resources.Content.MessageUpdated, true);
						}
					} else {
						ViewBag.Title = Piranha.Resources.Content.EditTitleExistingDocument;
						if (draft) {
							SuccessMessage(Piranha.Resources.Content.MessageDocumentSaved, true);
						} else {
							if (m.Content.Published == m.Content.LastPublished)
								SuccessMessage(Piranha.Resources.Content.MessageDocumentPublished, true);
							else SuccessMessage(Piranha.Resources.Content.MessageUpdated, true);
						}
					}
					return RedirectToAction("edit", new { id = m.Content.Id, returl = ViewBag.ReturnUrl });
				} else {
					ViewBag.Title = Piranha.Resources.Content.EditTitleNew;
					ErrorMessage(Piranha.Resources.Content.MessageNotSaved);
					return View("Edit", m);
				}
			} catch (DuplicatePermalinkException) {
				// Manually set the duplicate error.
				ModelState.AddModelError("Permalink", @Piranha.Resources.Global.PermalinkDuplicate);
			} catch (Exception e) {
				ErrorMessage(e.ToString());
			}
			m.Refresh();
			ViewBag.Folder = m.Content.IsFolder;
			return View("Edit", m);
		}

		/// <summary>
		/// Syncs the media object with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Access(Function = "ADMIN_CONTENT")]
		public ActionResult Sync(string id) {
			var m = EditModel.GetById(new Guid(id));

			try {
				if (m.Sync()) {
					if (m.Content.IsImage)
						SuccessMessage(Piranha.Resources.Content.MessageImageSynced, true);
					else SuccessMessage(Piranha.Resources.Content.MessageDocumentSynced, true);
				} else {
					if (m.Content.IsImage)
						InformationMessage(Piranha.Resources.Content.MessageImageNotSynced, true);
					else InformationMessage(Piranha.Resources.Content.MessageDocumentNotSynced, true);
				}
			} catch (HttpException e) {
				if (e.GetHttpCode() == 404) {
					if (m.Content.IsImage)
						ErrorMessage(Piranha.Resources.Content.MessageImageNotFound, true);
					else ErrorMessage(Piranha.Resources.Content.MessageDocumentNotFound, true);
				} else {
					if (m.Content.IsImage)
						ErrorMessage(Piranha.Resources.Content.MessageImageSyncError, true);
					else ErrorMessage(Piranha.Resources.Content.MessageDocumentSyncError, true);
				}
			}
			return RedirectToAction("edit", new { id = id, returl = ViewBag.ReturnUrl });
		}

		/// <summary>
		/// Deletes the specified content record.
		/// </summary>
		/// <param name="id">The content id</param>
		[Access(Function = "ADMIN_CONTENT")]
		public ActionResult Delete(string id) {
			EditModel m = EditModel.GetById(new Guid(id));

			if (m.DeleteAll()) {
				if (m.Content.IsImage)
					SuccessMessage(Piranha.Resources.Content.MessageImageDeleted, true);
				else if (m.Content.IsFolder)
					SuccessMessage(Piranha.Resources.Content.MessageFolderDeleted, true);
				else SuccessMessage(Piranha.Resources.Content.MessageDocumentDeleted, true);
			} else {
				if (m.Content.IsImage)
					ErrorMessage(Piranha.Resources.Content.MessageImageNotDeleted, true);
				else if (m.Content.IsFolder)
					ErrorMessage(Piranha.Resources.Content.MessageFolderNotDeleted, true);
				else ErrorMessage(Piranha.Resources.Content.MessageDocumentNotDeleted, true);
			}
			if (!String.IsNullOrEmpty(ViewBag.ReturnUrl))
				return Redirect(ViewBag.ReturnUrl);
			if (m.Content.ParentId != Guid.Empty)
				return RedirectToAction("index", new { id = m.Content.ParentId });
			return RedirectToAction("index");
		}

		/// <summary>
		/// Reverts to latest published verison.
		/// </summary>
		/// <param name="id">The page id.</param>
		[Access(Function = "ADMIN_CONTENT")]
		public ActionResult Revert(string id) {
			Piranha.Models.Content.Revert(new Guid(id));

			var content = Piranha.Models.Content.GetSingle(new Guid(id));
			if (content != null) {
				SuccessMessage(Piranha.Resources.Content.MessageReverted, true);
			} else {
				ErrorMessage(Piranha.Resources.Content.MessageNotFound, true);
			}
			return RedirectToAction("edit", new { id = id, returl = ViewBag.ReturnUrl });
		}

		/// <summary>
		/// Unpublishes the specified page.
		/// </summary>
		/// <param name="id">The page id</param>
		[Access(Function = "ADMIN_PAGE")]
		public ActionResult Unpublish(string id) {
			Piranha.Models.Content.Unpublish(new Guid(id));

			var content = Piranha.Models.Content.GetSingle(new Guid(id), true);
			if (content != null) {
				if (content.IsFolder)
					SuccessMessage(Piranha.Resources.Content.MessageFolderUnpublished, true);
				else if (content.IsImage)
					SuccessMessage(Piranha.Resources.Content.MessageImageUnpublished, true);
				else SuccessMessage(Piranha.Resources.Content.MessageDocumentUnpublished, true);
			} else {
				ErrorMessage(Piranha.Resources.Content.MessageNotFound, true);
			}
			return RedirectToAction("edit", new { id = id, returl = ViewBag.ReturlUrl });
		}

		/// <summary>
		/// Gets the content object with the given id
		/// </summary>
		/// <param name="id">The content id</param>
		[Access(Function = "ADMIN_CONTENT")]
		public JsonResult Get(string id) {
			var draft = true;
			if (!String.IsNullOrEmpty(Request["tinymce"]))
				draft = Request["tinymce"] != "true";

			return Get(id, draft);
		}

		/// <summary>
		/// Gets the content object with the given id
		/// </summary>
		/// <param name="id">The content id</param>
		/// <param name="draft">Whether or not to get the draft</param>
		private JsonResult Get(string id, bool draft) {
			try {
				var c = Piranha.Models.Content.GetSingle(new Guid(id), draft);

				if (c != null) {
					var media = new {
						Id = c.Id,
						ParentId = c.ParentId,
						Filename = c.Filename,
						Name = c.Name,
						DisplayName = c.DisplayName,
						Description = c.Description,
						Type = c.Type,
						Size = c.Size,
						Width = c.Width > 0 ? (int?)c.Width : null,
						Height = c.Height > 0 ? (int?)c.Height : null,
						ThumbnailUrl = WebPages.WebPiranha.ApplicationPath +
							(!draft ? Application.Current.Handlers.GetUrlPrefix("THUMBNAIL") :
							Application.Current.Handlers.GetUrlPrefix("THUMBNAILDRAFT")) + "/" + c.Id,
						ContentUrl = WebPages.WebPiranha.ApplicationPath +
							(!draft ? Application.Current.Handlers.GetUrlPrefix("CONTENT") :
							Application.Current.Handlers.GetUrlPrefix("CONTENTDRAFT")) + "/" + c.Id,
						Created = c.Created.ToString(),
						Updated = c.Updated.ToString()
					};
					return Json(media, JsonRequestBehavior.AllowGet);
				}
			} catch { }
			return null;
		}

		/// <summary>
		/// Uploads a file using ajax.
		/// </summary>
		/// <returns>The status of the action</returns>
		[HttpPost()]
		[Access(Function = "ADMIN_CONTENT")]
		public JsonResult Upload() {
			// Get custom headers
			var filename = Request.Headers["X-File-Name"];
			var type = Request.Headers["X-File-Type"];
			var size = Convert.ToInt32(Request.Headers["X-File-Size"]);
			var parentId = !String.IsNullOrEmpty(Request.Headers["X-File-ParentId"]) ? new Guid(Request.Headers["X-File-ParentId"]) : Guid.Empty;
			var name = Request.Headers["X-File-DisplayName"];
			var alt = Request.Headers["X-File-Alt"];
			var desc = Request.Headers["X-File-Desc"];

			using (var mem = new MemoryStream()) {
				var stream = Request.InputStream;

				stream.Seek(0, SeekOrigin.Begin);
				stream.CopyTo(mem);
				mem.Position = 0;

				using (var binary = new BinaryReader(mem)) {
					var media = new Piranha.Models.MediaFileContent() {
						ContentType = type,
						Filename = filename,
						Body = binary.ReadBytes(size)
					};
					var content = new Piranha.Models.Content() {
						Id = Guid.NewGuid(),
						ParentId = parentId,
						IsFolder = false,
						Name = name,
						AlternateText = alt,
						Description = desc
					};
					if (content.SaveAndPublish(media))
						return Get(content.Id.ToString(), false);
				}
			}
			return Json(new {
				Success = false
			});
		}
	}
}
