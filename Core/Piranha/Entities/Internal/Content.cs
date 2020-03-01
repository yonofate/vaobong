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
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

using Piranha.Data;
using Piranha.Web;

namespace Piranha.Models
{
	/// <summary>
	/// A content object is an image, document or other file that's been uploaded from
	/// the manager area. Content uploaded from the application is usually handled using
	/// the Upload record.
	/// 
	/// Changes made to records of this type are logged.
	/// </summary>
	[PrimaryKey(Column = "content_id,content_draft")]
	[Serializable]
	public class Content : MediaFile<Content>, ICacheRecord<Content>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name = "content_id")]
		public override Guid Id { get; set; }

		/// <summary>
		/// Gets/sets whether this is a draft.
		/// </summary>
		[Column(Name = "content_draft")]
		public bool IsDraft { get; set; }

		/// <summary>
		/// Gets/sets the optional parent id.
		/// </summary>
		[Column(Name = "content_parent_id")]
		[Display(ResourceType = typeof(Piranha.Resources.Content), Name = "ParentId")]
		public Guid ParentId { get; set; }

		/// <summary>
		/// Gets/sets the permalink id.
		/// </summary>
		[Column(Name = "content_permalink_id")]
		public Guid PermalinkId { get; set; }

		/// <summary>
		/// Gets/sets the filename.
		/// </summary>
		[Column(Name = "content_filename")]
		[Display(ResourceType = typeof(Piranha.Resources.Content), Name = "Filename")]
		public override string Filename { get; set; }

		/// <summary>
		/// Gets/sets the original url the media object was 
		/// fetched from.
		/// </summary>
		[Column(Name = "content_url")]
		[Display(ResourceType = typeof(Piranha.Resources.Content), Name = "OriginalUrl")]
		public string OriginalUrl { get; set; }

		/// <summary>
		/// Gets/sets the last time the media object was synced 
		/// from its external url.
		/// </summary>
		[Column(Name = "content_synced")]
		[Display(ResourceType = typeof(Piranha.Resources.Content), Name = "LastSynced")]
		public DateTime LastSynced { get; set; }

		/// <summary>
		/// Gets/sets the type
		/// </summary>
		[Column(Name = "content_type")]
		[Display(ResourceType = typeof(Piranha.Resources.Content), Name = "ContentType")]
		public override string Type { get; set; }

		/// <summary>
		/// Gets/sets the content size.
		/// </summary>
		[Column(Name = "content_size")]
		[Display(ResourceType = typeof(Piranha.Resources.Content), Name = "ContentSize")]
		public int Size { get; set; }

		/// <summary>
		/// Get/sets whether the content is an image or not.
		/// </summary>
		[Column(Name = "content_image")]
		public bool IsImage { get; set; }

		/// <summary>
		/// Gets/sets whether this is a content folder or not.
		/// </summary>
		[Column(Name = "content_folder")]
		public bool IsFolder { get; set; }

		/// <summary>
		/// Gets/sets the possible width of the content.
		/// </summary>
		[Column(Name = "content_width")]
		public int Width { get; set; }

		/// <summary>
		/// Gets/sets the possible height of the content.
		/// </summary>
		[Column(Name = "content_height")]
		public int Height { get; set; }

		/// <summary>
		/// Gets/sets the name of the content.
		/// </summary>
		[Column(Name = "content_name")]
		[Display(ResourceType = typeof(Piranha.Resources.Content), Name = "Name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the alternate text.
		/// </summary>
		[Column(Name = "content_alt")]
		[Display(ResourceType = typeof(Piranha.Resources.Content), Name = "AlternateText")]
		public string AlternateText { get; set; }

		/// <summary>
		/// Gets/sets the decription.
		/// </summary>
		[Column(Name = "content_description")]
		[Display(ResourceType = typeof(Piranha.Resources.Content), Name = "Description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name = "content_created")]
		public override DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name = "content_updated")]
		public override DateTime Updated { get; set; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name = "content_created_by")]
		public override Guid CreatedBy { get; set; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name = "content_updated_by")]
		public override Guid UpdatedBy { get; set; }

		/// <summary>
		/// Gets/sets the initial publish date.
		/// </summary>
		[Column(Name = "content_published")]
		public DateTime Published { get; set; }

		/// <summary>
		/// Gets/sets the last published date.
		/// </summary>
		[Column(Name = "content_last_published")]
		public DateTime LastPublished { get; set; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the current display name for the content object.
		/// </summary>
		public string DisplayName {
			get {
				return !String.IsNullOrEmpty(Name) ? Name : Filename;
			}
		}

		/// <summary>
		/// Gets/sets the possible child content if this is a folder.
		/// </summary>
		public List<Content> ChildContent { get; set; }
		#endregion

		#region Cache
		/// <summary>
		/// Maps permalink id to page id.
		/// </summary>
		private static Dictionary<Guid, Guid> PermalinkIdCache = new Dictionary<Guid, Guid>();
		#endregion

		/// <summary>
		/// Default constructor. Creates a new content object.
		/// </summary>
		public Content() {
			ExtensionType = Extend.ExtensionType.Media;
			ChildContent = new List<Content>();
			LogChanges = true;
			IsDraft = true;
		}

		#region Static accessors
		/// <summary>
		/// Gets a single record.
		/// </summary>
		/// <param name="id">The record id</param>
		/// <param name="draft">Whether to get the draft or not</param>
		/// <param name="tx">Optional transaction</param>
		/// <returns>The record</returns>
		public static Content GetSingle(Guid id, bool draft = false, IDbTransaction tx = null) {
			if (id != Guid.Empty) {
				if (!draft) {
					if (!Application.Current.CacheProvider.Contains(id.ToString()))
						Application.Current.CacheProvider[id.ToString()] = Content.GetSingle("content_id=@0 AND content_draft=@1", id, draft, tx);
					return (Content)Application.Current.CacheProvider[id.ToString()];
				}
				return Content.GetSingle("content_id=@0 AND content_draft=@1", id, draft, tx);
			}
			return null;
		}

		/// <summary>
		/// Gets the content specified by the given permalink.
		/// </summary>
		/// <param name="permalinkid">The permalink id</param>
		/// <param name="draft">Whether to get the current draft or not</param>
		/// <returns>The content</returns>
		public static Content GetByPermalinkId(Guid permalinkid, bool draft = false) {
			if (!PermalinkIdCache.ContainsKey(permalinkid)) {
				var c = Content.GetSingle("content_permalink_id = @0 AND content_draft = 1", permalinkid);

				if (c != null) {
					PermalinkIdCache.Add(c.PermalinkId, c.Id);
				} else return null;
			}
			return GetSingle(PermalinkIdCache[permalinkid], draft);
		}

		/// <summary>
		/// Gets the content for the matching id
		/// </summary>
		/// <param name="id">An array of id keys</param>
		/// <returns>A list of content records</returns>
		public static List<Content> GetIn(Guid[] id) {
			if (id.Length > 0) {
				string fmtWhere = PrimaryKeys[0] + " IN ({0})";
				string sqlIn = "";

				// Build where clause
				for (int n = 0; n < id.Length; n++)
					sqlIn += (sqlIn != "" ? "," : "") + "@" + n;

				// Format arguments
				object[] args = new object[id.Length];
				id.Each((i, e) => args[i] = e);

				// Get the records
				return Get(String.Format(fmtWhere, sqlIn), args);
			}
			return new List<Content>();
		}

		/// <summary>
		/// Gets the folder structure for the first level.
		/// </summary>
		/// <param name="published">Whether to get the published structure or not</param>
		/// <param name="images">Whether or not to only get images</param>
		/// <returns>The content</returns>
		public static List<Content> GetStructure(bool published = true, bool images = false) {
			return GetStructure(Guid.Empty, false, published);
		}

		/// <summary>
		/// Gets the folder structure for the given folder id.
		/// </summary>
		/// <param name="folderid">The folder id</param>
		/// <param name="published">Whether to get the published structure or not</param>
		/// <param name="images">Whether or not to only get images</param>
		/// <returns>The content</returns>
		public static List<Content> GetStructure(Guid folderid, bool includeparent = false, bool published = true, bool images = false) {
			List<Content> ret = new List<Content>();

			// Add parent
			if (folderid != Guid.Empty && includeparent) {
				var self = Content.GetSingle(folderid, !published);
				ret.Add(new Content() {
					Id = self.ParentId,
					ParentId = folderid,
					IsFolder = true,
					Name = ".."
				});
			}

			// Get the folders
			if (folderid == Guid.Empty)
				ret.AddRange(Content.Get("content_folder = 1 AND content_parent_id IS NULL AND content_draft = @0", !published,
					new Params() { OrderBy = "content_name" }));
			else ret.AddRange(Content.Get("content_folder = 1 AND content_parent_id = @0 AND content_draft = @1", folderid, !published,
					new Params() { OrderBy = "content_name" }));

			// Get the content
			if (folderid == Guid.Empty)
				ret.AddRange(Content.Get("content_folder = 0 AND content_parent_id IS NULL AND content_draft = @0", !published,
					new Params() { OrderBy = "COALESCE(content_name, content_filename)" }));
			else ret.AddRange(Content.Get("content_folder = 0 AND content_parent_id = @0 AND content_draft = @1", folderid, !published,
				new Params() { OrderBy = "COALESCE(content_name, content_filename)" }));

			return ret;
		}

		/// <summary>
		/// Gets the full folder structure for the media content.
		/// </summary>
		/// <param name="published">Whether to get the published structure or not</param>
		/// <returns>The folder structure.</returns>
		public static List<Content> GetFolderStructure(bool published = true) {
			return SortStructure(Content.Get("content_folder = 1 AND content_draft = @0", !published, new Params() { OrderBy = "content_parent_id, content_name" }), Guid.Empty);
		}

		/// <summary>
		/// Gets the content for the given category id.
		/// </summary>
		/// <param name="id">The category id</param>
		/// <param name="published">Whether to get published content or not</param>
		/// <returns>A list of content</returns>
		public static List<Content> GetByCategoryId(Guid id, bool published = true) {
			return Content.Get("content_id IN (" +
				"SELECT relation_data_id FROM relation WHERE relation_type = @0 AND relation_related_id = @1) AND content_draft = @2",
				Relation.RelationType.CONTENTCATEGORY, id, !published);
		}

		/// <summary>
		/// Gets the thumbnail for the embedded resource with the given id. The thumbnail
		/// resources are specified in Piranha.Drawing.Thumbnails.
		/// </summary>
		/// <param name="context">The current http context</param>
		/// <param name="id">The resource id</param>
		/// <param name="size">The desired size</param>
		public static bool GetResourceThumbnail(HttpContext context, Guid id, int size = 60) {
			var content = new Content() {
				Id = id
			};

			if (Drawing.Thumbnails.ContainsKey(id)) {
				if (!ClientCache.HandleClientCache(context, content.Id.ToString(), new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime)) {
					var data = Application.Current.MediaCacheProvider.Get(id, size, size);

					if (data == null) {
						var resource = Drawing.Thumbnails.GetById(id);
						if (size <= 32 && resource.Contains("ico-folder"))
							resource = Drawing.Thumbnails.GetByType("folder-small");

						Stream strm = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
						var img = Image.FromStream(strm);
						strm.Close();

						// Generate thumbnail from image
						using (Bitmap bmp = new Bitmap(size, size)) {
							Graphics grp = Graphics.FromImage(bmp);

							grp.SmoothingMode = SmoothingMode.HighQuality;
							grp.CompositingQuality = CompositingQuality.HighQuality;
							grp.InterpolationMode = InterpolationMode.High;

							// Resize and crop image
							Rectangle dst = new Rectangle(0, 0, bmp.Width, bmp.Height);
							grp.DrawImage(img, dst, img.Width > img.Height ? (img.Width - img.Height) / 2 : 0,
								img.Height > img.Width ? (img.Height - img.Width) / 2 : 0, Math.Min(img.Width, img.Height),
								Math.Min(img.Height, img.Width), GraphicsUnit.Pixel);

							using (var mem = new MemoryStream()) {
								bmp.Save(mem, img.RawFormat);
								data = mem.ToArray();
							}
							bmp.Dispose();
							grp.Dispose();
						}
						Application.Current.MediaCacheProvider.Put(id, data, size, size);
						content.WriteFile(context, data);

						img.Dispose();
					}
					if (data != null) {
						context.Response.StatusCode = 200;
						context.Response.ContentType = "image/png";
						context.Response.BinaryWrite(data);
						context.Response.EndClean();
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Reverts the content with the given id to it's last published state.
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="tx">Optional transaction</param>
		public static void Revert(Guid id, System.Data.IDbTransaction tx = null) {
			// Get the published version of
			var content = Content.GetSingle(id, tx);

			if (content != null) {
				// Turn it into a draft and save it.
				content.IsDraft = true;
				if (content.Save(tx)) {
					// Delete any possible draft version of the physical file.
					Application.Current.MediaProvider.DeleteDraft(id);

					// Now turn back the dates for the draft version
					Content.Execute("UPDATE content SET content_updated = content_last_published WHERE content_id = @0 AND content_draft = 1", null, id);
				}
			}
		}

		/// <summary>
		/// Unpublishes the content with the given id.
		/// </summary>
		/// <param name="id">The content id</param>
		/// <param name="tx">Optional transaction</param>
		public static void Unpublish(Guid id, System.Data.IDbTransaction tx = null) {
			using (IDbTransaction dbTx = (tx == null ? Database.OpenTransaction() : null)) {
				// Delete the published version
				Content.Execute("DELETE FROM content WHERE content_draft = 0 AND content_id = @0",
					tx != null ? tx : dbTx, id);
				// Remove published dates
				Content.Execute("UPDATE content SET content_published = NULL, content_last_published = NULL WHERE content_id = @0",
					tx != null ? tx : dbTx, id);

				// Now update all pages & posts which have a reference
				// to this media object.
				Page.Execute("UPDATE page SET page_last_modified = @0 WHERE page_attachments LIKE @1",
					tx != null ? tx : dbTx, DateTime.Now, "%" + id.ToString() + "%");
				Post.Execute("UPDATE post SET post_last_modified = @0 WHERE post_attachments LIKE @1",
					tx != null ? tx : dbTx, DateTime.Now, "%" + id.ToString() + "%");

				// Commit the transaction if it's local
				if (dbTx != null)
					dbTx.Commit();

				// Take the published physical file and move it to draft mode.
				var content = Content.GetSingle(id, true, tx);
				if (content != null) {
					Application.Current.MediaProvider.Unpublish(id);
					content.DeleteCache();

					// Invalidate record
					content.InvalidateRecord(content);
				}

				// Invalidate all pages & posts which have a reference
				// to this media object.
				Page.Get("page_attachments LIKE @1", tx, "%" + id.ToString() + "%").ForEach(p => p.InvalidateRecord(p));
				Post.Get("post_attachments LIKE @1", tx, "%" + id.ToString() + "%").ForEach(p => p.InvalidateRecord(p));
			}
		}
		#endregion

		/// <summary>
		/// Saves and publishes the current record.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Whether the operation succeeded or not</returns>
		public virtual bool SaveAndPublish(System.Data.IDbTransaction tx = null) {
			return SaveAndPublish(null, tx);
		}

		/// <summary>
		/// Saves and publishes the current record and physical file.
		/// </summary>
		/// <param name="content">The physical file</param>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Whether the operation succeeded or not</returns>
		public virtual bool SaveAndPublish(MediaFileContent content, System.Data.IDbTransaction tx = null) {
			//var user = HttpContext.Current != null ? HttpContext.Current.User : null ;

			if (Database.Identity != Guid.Empty || Application.Current.UserProvider.IsAuthenticated) {
				// Set file meta information
				SetFileMeta(content);

				// First get previously published record
				var self = Content.GetSingle(Id, false, tx);

				// Set up the dates.
				LastPublished = Updated = DateTime.Now;
				if (IsNew)
					Created = Updated;
				if (self == null)
					Published = Updated;

				// First save an up-to-date draft
				IsDraft = true;
				base.Save(content, tx, false, false);

				//var draftpath = PhysicalPath;

				// Now save a published version
				IsDraft = false;
				if (self == null)
					IsNew = true;
				base.Save(content, tx, false);

				// Check if we have have a drafted physical file
				Application.Current.MediaProvider.Publish(Id);
				DeleteCache();

				// Now update all pages & posts which have a reference
				// to this media object.
				Page.Execute("UPDATE page SET page_last_modified = @0 WHERE page_attachments LIKE @1", tx,
					DateTime.Now, "%" + Id.ToString() + "%");
				Post.Execute("UPDATE post SET post_last_modified = @0 WHERE post_attachments LIKE @1", tx,
					DateTime.Now, "%" + Id.ToString() + "%");

				// Invalidate all pages & posts which have a reference
				// to this media object.
				Page.Get("page_attachments LIKE @1", tx, "%" + Id.ToString() + "%").ForEach(p => p.InvalidateRecord(p));
				Post.Get("post_attachments LIKE @1", tx, "%" + Id.ToString() + "%").ForEach(p => p.InvalidateRecord(p));

				return true;
			}
			throw new AccessViolationException("User must be logged in to save data.");
		}

		/// <summary>
		/// Saves the current record together with the given physical file.
		/// </summary>
		/// <param name="content">The physical content</param>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Whether the operation succeeded or not</returns>
		public bool Save(MediaFileContent content, System.Data.IDbTransaction tx = null) {
			SetFileMeta(content);

			return base.Save(content, tx);
		}

		/// <summary>
		/// Gets a thumbnail representing the current content file.
		/// </summary>
		/// <param name="response">The http response</param>
		/// <param name="size">The desired size</param>
		public void GetThumbnail(HttpContext context, int size = 60, bool nocache = false) {
			if ((nocache && ClientCache.HandleNoCache(context)) || !ClientCache.HandleClientCache(context, Id.ToString() + size.ToString(), Updated)) {
				GetMedia(context, size, size);
			}
		}

		/// <summary>
		/// Gets the total size of the content on disk including all cached thumbnails.
		/// </summary>
		/// <returns>The total size in bytes</returns>
		public long GetTotalSize() {
			return Size + Application.Current.MediaCacheProvider.GetTotalSize(Id);
		}

		/// <summary>
		/// Invalidates the cache for the given record.
		/// </summary>
		/// <param name="record">The record</param>
		public void InvalidateRecord(Content record) {
			Application.Current.CacheProvider.Remove(record.Id.ToString());
		}

		/// <summary>
		/// Checks if the current entity has a child with the given id.
		/// </summary>
		/// <param name="id">The content id</param>
		/// <returns>Whether the child was found</returns>
		public bool HasChild(Guid id) {
			if (ChildContent != null) {
				foreach (var c in ChildContent)
					if (HasChild(c, id))
						return true;
			}
			return false;
		}

		#region Private methods
		/// <summary>
		/// Updates the current records meta data from the given physical file.
		/// </summary>
		/// <param name="content">The physical content</param>
		private void SetFileMeta(MediaFileContent content) {
			if (content != null) {
				try {
					using (var mem = new MemoryStream(content.Body)) {
						using (var img = Image.FromStream(mem)) {
							Image resized = null;

							// Check if we need to resize the image
							try {
								int max = Convert.ToInt32(SysParam.GetByName("IMAGE_MAX_WIDTH").Value);
								if (max > 0) {
									resized = Drawing.ImageUtils.Resize(img, max);
									using (var writer = new MemoryStream()) {
										resized.Save(writer, img.RawFormat);
										content.Body = writer.ToArray();
									}
								}
							} catch { }

							IsImage = true;
							Width = resized != null ? resized.Width : img.Width;
							Height = resized != null ? resized.Height : img.Height;

							if (resized != null)
								resized.Dispose();
						}
					}
				} catch {
					IsImage = false;
				}
				Size = content.Body.Length;
			}
		}

		/// <summary>
		/// Checks if the current entity has a child with the given id.
		/// </summary>
		/// <param name="content">The content</param>
		/// <param name="id">The content id</param>
		/// <returns>Whether the child was found</returns>
		private bool HasChild(Content content, Guid id) {
			if (content.Id == id)
				return true;
			if (content.ChildContent != null) {
				foreach (var c in content.ChildContent)
					if (HasChild(c, id))
						return true;
			}
			return false;
		}

		private static List<Content> SortStructure(List<Content> content, Guid parentid) {
			var ret = content.Where(c => c.ParentId == parentid).OrderBy(c => c.Name).ToList();

			ret.ForEach(r => r.ChildContent = SortStructure(content, r.Id));

			return ret;
		}
		#endregion
	}
}
