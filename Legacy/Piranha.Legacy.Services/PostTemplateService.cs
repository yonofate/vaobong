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
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;

namespace Piranha.Legacy.Services
{
	/// <summary>
	/// ReST API for post tenplates.
	/// </summary>
	[ServiceContract()]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class PostTemplateService
	{
		/// <summary>
		/// Gets all of the available post templates.
		/// </summary>
		/// <returns>The available post templates.</returns>
		[OperationContract()]
		[WebGet(UriTemplate = "getall", ResponseFormat = WebMessageFormat.Json)]
		public IList<DataContracts.PostTemplate> GetAll() {
			using (var db = new DataContext()) {
				return db.PostTemplates.OrderBy(pt => pt.Name).Select(pt => new DataContracts.PostTemplate() {
					Id = pt.Id,
					Name = pt.Name,
					Description = pt.Description,
					View = pt.ViewTemplate
				}).ToList();
			}
		}

		/// <summary>
		/// Gets all of the available post templates as xml.
		/// </summary>
		/// <returns>The available post templates.</returns>
		[OperationContract()]
		[WebGet(UriTemplate = "getall/xml", ResponseFormat = WebMessageFormat.Xml)]
		public IList<DataContracts.PostTemplate> GetAllXml() {
			return GetAll();
		}

		/// <summary>
		/// Gets the post template vith the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns>The post template</returns>
		[OperationContract()]
		[WebGet(UriTemplate = "get/{id}", ResponseFormat = WebMessageFormat.Json)]
		public DataContracts.PostTemplate Get(string id) {
			using (var db = new DataContext()) {
				return db.PostTemplates.Where(pt => pt.Id == new Guid(id)).Select(pt => new DataContracts.PostTemplate() {
					Id = pt.Id,
					Name = pt.Name,
					Description = pt.Description,
					View = pt.ViewTemplate
				}).Single();
			}
		}

		/// <summary>
		/// Gets the post template vith the given id as xml.
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns>The post template</returns>
		[OperationContract()]
		[WebGet(UriTemplate = "get/xml/{id}", ResponseFormat = WebMessageFormat.Xml)]
		public DataContracts.PostTemplate GetXml(string id) {
			return Get(id);
		}
	}
}