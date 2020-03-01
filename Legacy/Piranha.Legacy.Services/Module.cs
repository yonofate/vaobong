/*
 * Copyright (c) 2015 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace Piranha.Legacy.Services
{
	public class Module
	{
		/// <summary>
		/// Initializes the service module.
		/// </summary>
		public static void Init(ServiceType services = ServiceType.All) {
			if (services.HasFlag(ServiceType.All) || services.HasFlag(ServiceType.Category))
	            RouteTable.Routes.Add("REST_CATEGORY", new ServiceRoute("rest/category", new WebServiceHostFactory(), typeof(CategoryService)));
			if (services.HasFlag(ServiceType.All) || services.HasFlag(ServiceType.Changes))
	            RouteTable.Routes.Add("REST_CHANGES", new ServiceRoute("rest/changes", new WebServiceHostFactory(), typeof(ChangeService)));
			if (services.HasFlag(ServiceType.All) || services.HasFlag(ServiceType.Media))
	            RouteTable.Routes.Add("REST_CONTENT", new ServiceRoute("rest/content", new WebServiceHostFactory(), typeof(ContentService)));
			if (services.HasFlag(ServiceType.All) || services.HasFlag(ServiceType.Page))
	            RouteTable.Routes.Add("REST_PAGE", new ServiceRoute("rest/page", new WebServiceHostFactory(), typeof(PageService)));
			if (services.HasFlag(ServiceType.All) || services.HasFlag(ServiceType.PageType))
	            RouteTable.Routes.Add("REST_PAGETEMPLATE", new ServiceRoute("rest/pagetemplate", new WebServiceHostFactory(), typeof(PageTemplateService)));
			if (services.HasFlag(ServiceType.All) || services.HasFlag(ServiceType.Post))
	            RouteTable.Routes.Add("REST_POST", new ServiceRoute("rest/post", new WebServiceHostFactory(), typeof(PostService)));
			if (services.HasFlag(ServiceType.All) || services.HasFlag(ServiceType.PostType))
	            RouteTable.Routes.Add("REST_POSTTEMPLATE", new ServiceRoute("rest/posttemplate", new WebServiceHostFactory(), typeof(PostTemplateService)));
			if (services.HasFlag(ServiceType.All) || services.HasFlag(ServiceType.SiteMap))
				RouteTable.Routes.Add("REST_SITEMAP", new ServiceRoute("rest/sitemap", new WebServiceHostFactory(), typeof(SitemapServices)));
		}
	}
}
