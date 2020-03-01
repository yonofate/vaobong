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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

using Piranha.Web;

namespace Piranha.Models
{
	/// <summary>
	/// Uploadable media file content.
	/// </summary>
	[Serializable]
	public class MediaFileContent
	{
		#region Properties
		public string Filename { get; set; }
		public string ContentType { get; set; }
		public byte[] Body { get; set; }
		#endregion
	}

	/// <summary>
	/// Abstract base class for all media files.
	/// </summary>
	/// <typeparam name="T">The entity type</typeparam>
	[Serializable]
	public abstract class MediaFile<T> : PiranhaRecord<T>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the filename.
		/// </summary>
		public abstract string Filename { get; set; }

		/// <summary>
		/// Gets/sets the type of the media file.
		/// </summary>
		public abstract string Type { get; set; }
		#endregion

		/// <summary>
		/// Gets the physical media related to the content record and writes it to
		/// the given http response.
		/// </summary>
		/// <param name="response">The http response</param>
		public void GetMedia(HttpContext context, int? width = null, int? height = null) {
			if (!ClientCache.HandleClientCache(context, Id.ToString(), Updated)) {
				byte[] data = null;
				var compress = false;
				bool draft = (this is Content ? ((Content)(object)this).IsDraft : false);

				if (width.HasValue) {
					// Try to get cached media from the provider
					if (draft) {
						data = Application.Current.MediaCacheProvider.GetDraft(Id, width.Value, height, Piranha.IO.MediaType.Media);
					} else {
						data = Application.Current.MediaCacheProvider.Get(Id, width.Value, height,
							(this is Upload ? Piranha.IO.MediaType.Upload : Piranha.IO.MediaType.Media));
					}

					if (data == null) {
						// No cached media exists. Let's get it
						if (draft) {
							data = Application.Current.MediaProvider.GetDraft(Id, Piranha.IO.MediaType.Media);
							if (data == null)
								draft = false;
						}
						if (!draft) {
							data = Application.Current.MediaProvider.Get(Id,
								(this is Upload ? Piranha.IO.MediaType.Upload : Piranha.IO.MediaType.Media));
						}

						if (data != null) {
							Image img = null;
							try {
								// We're requesting different dimensions, try to get the image
								using (var mem = new MemoryStream(data)) {
									img = Image.FromStream(mem);
								}
								if (img != null) {
									var newWidth = width.HasValue && width.Value < img.Width ? width : img.Width;
									var newHeight = height;
									if (!newHeight.HasValue)
										newHeight = Convert.ToInt32(((double)width / img.Width) * img.Height);

									int orgWidth = img.Width, orgHeight = img.Height;

									using (var resized = Drawing.ImageUtils.Resize(img, newWidth.Value, newHeight.Value)) {
										if (resized.Width != orgWidth || resized.Height != orgHeight) {
											// Check for optional compression
											var param = SysParam.GetByName("COMPRESS_IMAGES");
											if (param != null && param.Value == "1")
												compress = true;

											using (var mem = new MemoryStream()) {
												resized.Save(mem, compress ? ImageFormat.Jpeg : img.RawFormat);
												data = mem.ToArray();
											}
											if (draft) {
												Application.Current.MediaCacheProvider.PutDraft(Id, data, width.Value, height,
													Piranha.IO.MediaType.Media);
											} else {
												Application.Current.MediaCacheProvider.Put(Id, data, width.Value, height,
													(this is Upload ? Piranha.IO.MediaType.Upload : Piranha.IO.MediaType.Media));
											}
										}
									}
									img.Dispose();
								}
							} catch { }
						}
					}
				} else {
					// Get the original media from the current provider
					if (draft) {
						data = Application.Current.MediaProvider.GetDraft(Id, Piranha.IO.MediaType.Media);
						if (data == null)
							draft = false;
					}
					if (!draft) {
						data = Application.Current.MediaProvider.Get(Id,
							(this is Upload ? Piranha.IO.MediaType.Upload : Piranha.IO.MediaType.Media));
					}
				}
				if (data != null)
					WriteFile(context, data, compress);
			}
		}

		/// <summary>
		/// Saves the given media content.
		/// </summary>
		/// <param name="content">The content</param>
		/// <param name="tx">The optional database transaction</param>
		/// <param name="setdates">If dates should be set automatically</param>
		/// <param name="writefile">If the file should be written or not</param>
		/// <returns></returns>
		public virtual bool Save(MediaFileContent content, System.Data.IDbTransaction tx = null, bool setdates = true, bool writefile = true) {
			if (content != null) {
				// Set filename
				if (!String.IsNullOrEmpty(content.Filename))
					Filename = content.Filename;
				// Set content type
				if (!String.IsNullOrEmpty(content.ContentType))
					Type = content.ContentType;
			}
			if (base.Save(tx) && content != null) {
				// Delete the old data
				if (!(this is Content) || !((Content)(object)this).IsDraft)
					DeleteFile();
				DeleteCache();

				// Save the new
				if (writefile) {
					// TODO: Dirty double cast, redesign!
					if (this is Content && ((Content)(object)this).IsDraft)
						Application.Current.MediaProvider.PutDraft(Id, content.Body);
					else Application.Current.MediaProvider.Put(Id, content.Body);
				}
			}
			return base.Save(tx, setdates);
		}

		/// <summary>
		/// Deletes the media file record and all related files.
		/// </summary>
		/// <param name="tx">Optional database transaction</param>
		/// <returns>Whether the action was a success</returns>
		public override bool Delete(System.Data.IDbTransaction tx = null) {
			if (base.Delete(tx)) {
				// Delete original files
				DeleteFile();

				// Delete Cache
				DeleteCache();

				return true;
			}
			return false;
		}

		/// <summary>
		/// Deletes the published and working copy of the media file.
		/// </summary>
		public void DeleteFile() {
			Application.Current.MediaProvider.DeleteDraft(Id);
			Application.Current.MediaProvider.Delete(Id);
		}

		/// <summary>
		/// Deletes all cached versions of the media file.
		/// </summary>
		public void DeleteCache() {
			Application.Current.MediaCacheProvider.Delete(Id,
				(this is Upload ? Piranha.IO.MediaType.Upload : Piranha.IO.MediaType.Media));
		}

		#region Protected methods
		/// <summary>
		/// Writes the given file to the http response
		/// </summary>
		/// <param name="context">The http context to write the file to</param>
		/// <param name="path">The path to the physical file</param>
		/// <param name="compressed">Whether or not the file is a compressed image</param>
		protected void WriteFile(HttpContext context, string path, bool compressed = false) {
			if (File.Exists(path)) {
				context.Response.StatusCode = 200;
				context.Response.ContentType = compressed ? "image/jpg" : Type;
				context.Response.WriteFile(path);
				context.Response.EndClean();
			} else {
				context.Response.StatusCode = 404;
			}
		}

		/// <summary>
		/// Writes the given file to the http response
		/// </summary>
		/// <param name="context">The http context to write the file to</param>
		/// <param name="data">The data of the physical file</param>
		/// <param name="compressed">Whether or not the file is a compressed image</param>
		protected void WriteFile(HttpContext context, byte[] data, bool compressed = false) {
			if (data != null) {
				context.Response.StatusCode = 200;
				context.Response.ContentType = compressed ? "image/jpg" : Type;
				context.Response.BinaryWrite(data);
				context.Response.EndClean();
			} else {
				context.Response.StatusCode = 404;
			}
		}
		#endregion
	}
}