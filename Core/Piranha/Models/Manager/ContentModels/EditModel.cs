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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Data;
using Piranha.Extend;

namespace Piranha.Models.Manager.ContentModels
{
	/// <summary>
	/// Edit model for the content record.
	/// </summary>
	public class EditModel
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
				EditModel model = (EditModel)base.BindModel(controllerContext, bindingContext);

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

		#region Mimetypes
		/// <summary>
		/// Gets the mime type for the given filename
		/// </summary>
		public static class MimeType
		{
			#region Mime types
			private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string> {
				{"ai", "application/postscript"},
				{"aif", "audio/x-aiff"},
				{"aifc", "audio/x-aiff"},
				{"aiff", "audio/x-aiff"},
				{"asc", "text/plain"},
				{"atom", "application/atom+xml"},
				{"au", "audio/basic"},
				{"avi", "video/x-msvideo"},
				{"bcpio", "application/x-bcpio"},
				{"bin", "application/octet-stream"},
				{"bmp", "image/bmp"},
				{"cdf", "application/x-netcdf"},
				{"cgm", "image/cgm"},
				{"class", "application/octet-stream"},
				{"cpio", "application/x-cpio"},
				{"cpt", "application/mac-compactpro"},
				{"csh", "application/x-csh"},
				{"css", "text/css"},
				{"dcr", "application/x-director"},
				{"dif", "video/x-dv"},
				{"dir", "application/x-director"},
				{"djv", "image/vnd.djvu"},
				{"djvu", "image/vnd.djvu"},
				{"dll", "application/octet-stream"},
				{"dmg", "application/octet-stream"},
				{"dms", "application/octet-stream"},
				{"doc", "application/msword"},
				{"docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
				{"dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
				{"docm","application/vnd.ms-word.document.macroEnabled.12"},
				{"dotm","application/vnd.ms-word.template.macroEnabled.12"},
				{"dtd", "application/xml-dtd"},
				{"dv", "video/x-dv"},
				{"dvi", "application/x-dvi"},
				{"dxr", "application/x-director"},
				{"eps", "application/postscript"},
				{"etx", "text/x-setext"},
				{"exe", "application/octet-stream"},
				{"ez", "application/andrew-inset"},
				{"gif", "image/gif"},
				{"gram", "application/srgs"},
				{"grxml", "application/srgs+xml"},
				{"gtar", "application/x-gtar"},
				{"hdf", "application/x-hdf"},
				{"hqx", "application/mac-binhex40"},
				{"htm", "text/html"},
				{"html", "text/html"},
				{"ice", "x-conference/x-cooltalk"},
				{"ico", "image/x-icon"},
				{"ics", "text/calendar"},
				{"ief", "image/ief"},
				{"ifb", "text/calendar"},
				{"iges", "model/iges"},
				{"igs", "model/iges"},
				{"jnlp", "application/x-java-jnlp-file"},
				{"jp2", "image/jp2"},
				{"jpe", "image/jpeg"},
				{"jpeg", "image/jpeg"},
				{"jpg", "image/jpeg"},
				{"js", "application/x-javascript"},
				{"kar", "audio/midi"},
				{"latex", "application/x-latex"},
				{"lha", "application/octet-stream"},
				{"lzh", "application/octet-stream"},
				{"m3u", "audio/x-mpegurl"},
				{"m4a", "audio/mp4a-latm"},
				{"m4b", "audio/mp4a-latm"},
				{"m4p", "audio/mp4a-latm"},
				{"m4u", "video/vnd.mpegurl"},
				{"m4v", "video/x-m4v"},
				{"mac", "image/x-macpaint"},
				{"man", "application/x-troff-man"},
				{"mathml", "application/mathml+xml"},
				{"me", "application/x-troff-me"},
				{"mesh", "model/mesh"},
				{"mid", "audio/midi"},
				{"midi", "audio/midi"},
				{"mif", "application/vnd.mif"},
				{"mov", "video/quicktime"},
				{"movie", "video/x-sgi-movie"},
				{"mp2", "audio/mpeg"},
				{"mp3", "audio/mpeg"},
				{"mp4", "video/mp4"},
				{"mpe", "video/mpeg"},
				{"mpeg", "video/mpeg"},
				{"mpg", "video/mpeg"},
				{"mpga", "audio/mpeg"},
				{"ms", "application/x-troff-ms"},
				{"msh", "model/mesh"},
				{"mxu", "video/vnd.mpegurl"},
				{"nc", "application/x-netcdf"},
				{"oda", "application/oda"},
				{"ogg", "application/ogg"},
				{"pbm", "image/x-portable-bitmap"},
				{"pct", "image/pict"},
				{"pdb", "chemical/x-pdb"},
				{"pdf", "application/pdf"},
				{"pgm", "image/x-portable-graymap"},
				{"pgn", "application/x-chess-pgn"},
				{"pic", "image/pict"},
				{"pict", "image/pict"},
				{"png", "image/png"}, 
				{"pnm", "image/x-portable-anymap"},
				{"pnt", "image/x-macpaint"},
				{"pntg", "image/x-macpaint"},
				{"ppm", "image/x-portable-pixmap"},
				{"ppt", "application/vnd.ms-powerpoint"},
				{"pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation"},
				{"potx","application/vnd.openxmlformats-officedocument.presentationml.template"},
				{"ppsx","application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
				{"ppam","application/vnd.ms-powerpoint.addin.macroEnabled.12"},
				{"pptm","application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
				{"potm","application/vnd.ms-powerpoint.template.macroEnabled.12"},
				{"ppsm","application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
				{"ps", "application/postscript"},
				{"qt", "video/quicktime"},
				{"qti", "image/x-quicktime"},
				{"qtif", "image/x-quicktime"},
				{"ra", "audio/x-pn-realaudio"},
				{"ram", "audio/x-pn-realaudio"},
				{"ras", "image/x-cmu-raster"},
				{"rdf", "application/rdf+xml"},
				{"rgb", "image/x-rgb"},
				{"rm", "application/vnd.rn-realmedia"},
				{"roff", "application/x-troff"},
				{"rtf", "text/rtf"},
				{"rtx", "text/richtext"},
				{"sgm", "text/sgml"},
				{"sgml", "text/sgml"},
				{"sh", "application/x-sh"},
				{"shar", "application/x-shar"},
				{"silo", "model/mesh"},
				{"sit", "application/x-stuffit"},
				{"skd", "application/x-koan"},
				{"skm", "application/x-koan"},
				{"skp", "application/x-koan"},
				{"skt", "application/x-koan"},
				{"smi", "application/smil"},
				{"smil", "application/smil"},
				{"snd", "audio/basic"},
				{"so", "application/octet-stream"},
				{"spl", "application/x-futuresplash"},
				{"src", "application/x-wais-source"},
				{"sv4cpio", "application/x-sv4cpio"},
				{"sv4crc", "application/x-sv4crc"},
				{"svg", "image/svg+xml"},
				{"swf", "application/x-shockwave-flash"},
				{"t", "application/x-troff"},
				{"tar", "application/x-tar"},
				{"tcl", "application/x-tcl"},
				{"tex", "application/x-tex"},
				{"texi", "application/x-texinfo"},
				{"texinfo", "application/x-texinfo"},
				{"tif", "image/tiff"},
				{"tiff", "image/tiff"},
				{"tr", "application/x-troff"},
				{"tsv", "text/tab-separated-values"},
				{"txt", "text/plain"},
				{"ustar", "application/x-ustar"},
				{"vcd", "application/x-cdlink"},
				{"vrml", "model/vrml"},
				{"vxml", "application/voicexml+xml"},
				{"wav", "audio/x-wav"},
				{"wbmp", "image/vnd.wap.wbmp"},
				{"wbmxl", "application/vnd.wap.wbxml"},
				{"wml", "text/vnd.wap.wml"},
				{"wmlc", "application/vnd.wap.wmlc"},
				{"wmls", "text/vnd.wap.wmlscript"},
				{"wmlsc", "application/vnd.wap.wmlscriptc"},
				{"wrl", "model/vrml"},
				{"xbm", "image/x-xbitmap"},
				{"xht", "application/xhtml+xml"},
				{"xhtml", "application/xhtml+xml"},
				{"xls", "application/vnd.ms-excel"},                        
				{"xml", "application/xml"},
				{"xpm", "image/x-xpixmap"},
				{"xsl", "application/xml"},
				{"xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
				{"xltx","application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
				{"xlsm","application/vnd.ms-excel.sheet.macroEnabled.12"},
				{"xltm","application/vnd.ms-excel.template.macroEnabled.12"},
				{"xlam","application/vnd.ms-excel.addin.macroEnabled.12"},
				{"xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
				{"xslt", "application/xslt+xml"},
				{"xul", "application/vnd.mozilla.xul+xml"},
				{"xwd", "image/x-xwindowdump"},
				{"xyz", "chemical/x-xyz"},
				{"zip", "application/zip"} };
			#endregion

			/// <summary>
			/// Gets the mime type from the current file name.
			/// </summary>
			/// <param name="filename">The file name.</param>
			/// <returns>The mime type</returns>
			public static string Get(string filename) {
				if (MIMETypesDictionary.ContainsKey(Path.GetExtension(filename).Remove(0, 1)))
					return MIMETypesDictionary[Path.GetExtension(filename).Remove(0, 1)];
				return "unknown/unknown";
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the content record.
		/// </summary>
		public Models.Content Content { get; set; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		public Models.Permalink Permalink { get; set; }

		/// <summary>
		/// Gets/sets the categories associated with the post.
		/// </summary>
		public List<Guid> ContentCategories { get; set; }

		/// <summary>
		/// Gets/sets the available categories.
		/// </summary>
		public MultiSelectList Categories { get; set; }

		/// <summary>
		/// Gets/sets the available folders.
		/// </summary>
		public IList<Placement> Folders { get; set; }

		/// <summary>
		/// Gets/sets the available extensions.
		/// </summary>
		public List<Extension> Extensions { get; set; }

		/// <summary>
		/// Gets/sets the optional file.
		/// </summary>
		public HttpPostedFileBase UploadedFile { get; set; }

		/// <summary>
		/// Gets/sets the url to get the file from.
		/// </summary>
		[Display(Name = "FromUrl", ResourceType = typeof(Piranha.Resources.Content))]
		public string FileUrl { get; set; }

		/// <summary>
		/// Gets/sets the file object if this media is updated server side.
		/// </summary>
		public FileInfo ServerFile { get; set; }

		/// <summary>
		/// Gets the current handler prefix.
		/// </summary>
		public string HandlerPrefix { get; private set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new model.
		/// </summary>
		public EditModel() : this(false, Guid.Empty) { }

		/// <summary>
		/// Default constructor. Creates a new model.
		/// </summary>
		/// <param name="isfolder">Whether this is a folder or not.</param>
		public EditModel(bool isfolder, Guid parentid) {
			Permalink = new Models.Permalink() { Id = Guid.NewGuid(), NamespaceId = Config.MediaNamespaceId, Type = Models.Permalink.PermalinkType.MEDIA };
			Content = new Piranha.Models.Content() { Id = Guid.NewGuid(), IsFolder = isfolder, ParentId = parentid, PermalinkId = Permalink.Id };
			Extensions = Content.GetExtensions(true);

			GetMetaData();
		}

		/// <summary>
		/// Gets the edit model for the content with the given id.
		/// </summary>
		/// <param name="id">The content id</param>
		/// <returns>The model</returns>
		public static EditModel GetById(Guid id) {
			EditModel em = new EditModel();
			em.Content = Piranha.Models.Content.GetSingle(id, true);
			if (!em.Content.IsFolder) {
				// Don't get permalinks for folders
				if (em.Content.PermalinkId != Guid.Empty)
					em.Permalink = Models.Permalink.GetSingle(em.Content.PermalinkId);
				else em.Content.PermalinkId = em.Permalink.Id;
			}
			Relation.GetFieldsByDataId("relation_related_id", id, false).ForEach(r => em.ContentCategories.Add(r.RelatedId));
			em.Categories = new MultiSelectList(Category.GetFields("category_id, category_name",
				new Params() { OrderBy = "category_name" }), "Id", "Name", em.ContentCategories);
			em.Extensions = em.Content.GetExtensions(true);

			return em;
		}

		/// <summary>
		/// Saves the edit model.
		/// </summary>
		public bool SaveAll(bool draft) {
			var context = HttpContext.Current;
			var hasfile = UploadedFile != null || ServerFile != null;
			byte[] data = null;
			WebClient web = new WebClient();

			// Check if the original URL has been updated, and if so 
			if (!Content.IsNew && !String.IsNullOrEmpty(Content.OriginalUrl)) {
				var old = Content.GetSingle(Content.Id);
				if (old != null) {
					if (Content.OriginalUrl != old.OriginalUrl) {
						FileUrl = Content.OriginalUrl;
					}
				}
			}

			// Download file from web
			if (!hasfile && !String.IsNullOrEmpty(FileUrl)) {
				data = web.DownloadData(FileUrl);
				Content.OriginalUrl = FileUrl;
				Content.LastSynced = Convert.ToDateTime(web.ResponseHeaders[HttpResponseHeader.LastModified]);
			}

			var media = new MediaFileContent();
			if (hasfile) {
				if (UploadedFile != null) {
					media.Filename = UploadedFile.FileName;
					media.ContentType = UploadedFile.ContentType;
					using (var reader = new BinaryReader(UploadedFile.InputStream)) {
						media.Body = reader.ReadBytes(Convert.ToInt32(UploadedFile.InputStream.Length));
					}
				} else {
					media.Filename = ServerFile.Name;
					media.ContentType = MimeType.Get(ServerFile.Name);
					using (var stream = ServerFile.OpenRead()) {
						media.Body = new byte[ServerFile.Length];
						stream.Read(media.Body, 0, media.Body.Length);
					}
				}
			} else if (data != null) {
				media.Filename = FileUrl.Substring(FileUrl.LastIndexOf('/') + 1);
				media.ContentType = web.ResponseHeaders["Content-Type"];
				media.Body = data;
			} else {
				media = null;
			}

			var saved = false;

			if (!Content.IsFolder) {
				// Only save permalinks for non-folders
				var filename = !String.IsNullOrEmpty(Content.Filename) ? Content.Filename : (!String.IsNullOrEmpty(media.Filename) ? media.Filename : "");
				if (Permalink.IsNew && String.IsNullOrEmpty(Permalink.Name))
					Permalink.Name = Permalink.Generate(!Content.IsFolder ? filename : Content.Name, Models.Permalink.PermalinkType.MEDIA);
				try {
					Permalink.Save();
				} catch (DuplicatePermalinkException) {
					if (Permalink.IsNew) {
						Permalink.Name = Content.Id + Permalink.Name.Substring(Permalink.Name.LastIndexOf('.'));
						Permalink.Save();
					} else throw;
				}
			} else {
				Content.PermalinkId = Guid.Empty;
			}

			if (draft)
				saved = Content.Save(media);
			else saved = Content.SaveAndPublish(media);

			if (saved) {
				// Save related information
				Relation.DeleteByDataId(Content.Id);
				List<Relation> relations = new List<Relation>();
				ContentCategories.ForEach(c => relations.Add(new Relation() {
					DataId = Content.Id, RelatedId = c, IsDraft = false, Type = Relation.RelationType.CONTENTCATEGORY
				})
					);
				relations.ForEach(r => r.Save());

				// Save extensions
				foreach (var ext in Extensions) {
					// Call OnSave
					ext.Body.OnManagerSave(Content);

					ext.ParentId = Content.Id;
					ext.Save();
					if (!draft) {
						if (Extension.GetScalar("SELECT COUNT(extension_id) FROM extension WHERE extension_id=@0 AND extension_draft=0", ext.Id) == 0)
							ext.IsNew = true;
						ext.IsDraft = false;
						ext.Save();
					}
				}
				// Reset file url
				FileUrl = "";

				return true;
			}
			return false;
		}

		/// <summary>
		/// Syncronizes the media object from the original url.
		/// </summary>
		/// <returns></returns>
		public bool Sync(string url = "") {
			if (url == "")
				url = Content.OriginalUrl;

			if (!String.IsNullOrEmpty(url)) {
				var req = (HttpWebRequest)WebRequest.Create(url);
				var res = req.GetResponse();
				res.Close(); // Let's not read the response from this object

				if (((HttpWebResponse)res).StatusCode == HttpStatusCode.OK) {
					if (!String.IsNullOrEmpty(res.Headers[HttpResponseHeader.LastModified])) {
						var lastMod = Convert.ToDateTime(res.Headers[HttpResponseHeader.LastModified]);
						if (lastMod != Content.LastSynced) {
							// Update FileUrl and save the model
							FileUrl = url;
							return SaveAll(true);
						}
					}
				} else if (((HttpWebResponse)res).StatusCode == HttpStatusCode.MovedPermanently) {
					Content.OriginalUrl = res.Headers[HttpResponseHeader.Location]; // This will cause the Original URL to be updated.
					return Sync(Content.OriginalUrl);
				} else if (((HttpWebResponse)res).StatusCode == HttpStatusCode.Moved) {
					return Sync(Content.OriginalUrl);
				} else if (((HttpWebResponse)res).StatusCode == HttpStatusCode.NotFound) {
					throw new HttpException(404, "Not found");
				} else {
					throw new HttpException("Sync error");
				}
			}
			return false;
		}

		/// <summary>
		/// Deletes the specified content and its related file.
		/// </summary>
		public bool DeleteAll() {
			var content = Content.Get("content_id = @0", Content.Id);
			var permalinks = Permalink.Get("permalink_id = @0", Content.PermalinkId);

			using (IDbTransaction tx = Database.OpenTransaction()) {
				try {
					// Call OnDelete for all extensions
					foreach (var ext in Extensions)
						ext.Body.OnManagerDelete(Content);

					foreach (var c in content)
						c.Delete(tx);
					foreach (var p in permalinks)
						p.Delete(tx);
					tx.Commit();
					return true;
				} catch { tx.Rollback(); }
			}
			return false;
		}

		/// <summary>
		/// Refreshes the current object.
		/// </summary>
		public void Refresh() {
			GetMetaData();

			if (!Content.IsNew) {
				Relation.GetFieldsByDataId("relation_related_id", Content.Id).ForEach(r => ContentCategories.Add(r.RelatedId));
				Categories = new MultiSelectList(Category.GetFields("category_id, category_name",
					new Params() { OrderBy = "category_name" }), "Id", "Name", ContentCategories);
			}
		}

		#region Private methods
		private void GetMetaData() {
			ContentCategories = new List<Guid>();
			Categories = new MultiSelectList(Category.GetFields("category_id, category_name",
				new Params() { OrderBy = "category_name" }), "Id", "Name");

			var folders = Content.GetFields("content_id, content_name", "content_folder=1 AND content_draft=1", new Params() { OrderBy = "content_name" });
			folders.Insert(0, new Content());

			Folders = SortFolders(Content.GetFolderStructure(false));
			Folders.Insert(0, new Placement() { Text = "", Value = Guid.Empty });

			HandlerPrefix = Application.Current.Handlers.Where(h => h.Id == "CONTENTHANDLER").SingleOrDefault().UrlPrefix;
		}

		/// <summary>
		/// Flattens the folder structure.
		/// </summary>
		/// <param name="media"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		private List<Placement> SortFolders(List<Content> media, int level = 1) {
			var ret = new List<Placement>();

			foreach (var m in media) {
				var prefix = "";
				for (int n = 1; n < level; n++)
					prefix += "&nbsp;&nbsp;&nbsp;";
				ret.Add(new Placement() {
					Text = prefix + m.Name,
					Value = m.Id,
					Level = level
				});
				ret.AddRange(SortFolders(m.ChildContent, level + 1));
			}
			return ret;
		}
		#endregion
	}
}
