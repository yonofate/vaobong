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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha.Models;

namespace Piranha.Models.Manager.UploadModels
{
	/// <summary>
	/// Edit model for the upload entity.
	/// </summary>
	public class EditModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the upload.
		/// </summary>
		public Upload Upload { get; set; }

		/// <summary>
		/// Gets/sets the upload file.
		/// </summary>
		public HttpPostedFileBase UploadedFile { get; set; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new model.
		/// </summary>
		public EditModel() {
			Upload = new Upload();
		}

		/// <summary>
		/// Gets the edit model for the given id.
		/// </summary>
		/// <param name="id">The upload id</param>
		/// <returns>The model</returns>
		public static EditModel GetById(Guid id) {
			var m = new EditModel() {
				Upload = Upload.GetSingle(id)
			};
			if (m.Upload == null)
				m.Upload = new Upload();
			return m;
		}

		/// <summary>
		/// Gets the upload model for the given parent id. Note that this method
		/// is only valid for parents with a single upload attached to them.
		/// </summary>
		/// <param name="id">The parent id</param>
		/// <returns>The model</returns>
		public static EditModel GetByParentId(Guid id) {
			var m = new EditModel() {
				Upload = Upload.GetByParentId(id).FirstOrDefault()
			};
			if (m.Upload == null)
				m.Upload = new Upload();
			return m;
		}

		/// <summary>
		/// Saves the model.
		/// </summary>
		public void SaveAll() {
			if (UploadedFile != null) {
				Upload.Type = UploadedFile.ContentType;
				Upload.Filename = UploadedFile.FileName;
			}
			Upload.Save();
			Upload.DeleteCache();

			var data = new byte[UploadedFile.ContentLength];
			UploadedFile.InputStream.Read(data, 0, UploadedFile.ContentLength);
			Application.Current.MediaProvider.Put(Upload.Id, data, IO.MediaType.Upload);
		}

		/// <summary>
		/// Deletes the model.
		/// </summary>
		public void DeleteAll() {
			Upload.Delete();
		}
	}
}