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
using System.Web.Mvc;

using Piranha.Models;

namespace Piranha.Areas.Manager.Controllers
{
	/// <summary>
	/// Controller for handling uploaded application content in the manager area.
	/// </summary>
	public class UploadController : ManagerController
	{
		/// <summary>
		/// Gets the file content for the upload.
		/// </summary>
		/// <param name="id">The upload id</param>
		/// <returns>The file content</returns>
		public FileResult Get(string id) {
			var ul = Upload.GetSingle(new Guid(id));

			if (ul != null) {
				var data = Application.Current.MediaProvider.Get(ul.Id, IO.MediaType.Upload);
				if (data != null)
					return new FileStreamResult(new MemoryStream(data), ul.Type);
			}
			throw new FileNotFoundException("Could not find the upload with the given id");
		}
	}
}
