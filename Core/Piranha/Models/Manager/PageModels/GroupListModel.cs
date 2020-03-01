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
using System.Web;
using System.Web.Mvc;

using Piranha.Models;

namespace Piranha.Models.Manager.PageModels
{
	public class GroupListModel
	{
		public List<SysGroup> Groups { get; set; }
		public Page Page { get; set; }
	}
}