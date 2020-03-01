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
using System.Text;

namespace Piranha.Models.Manager.PostModels
{
	/// <summary>
	/// Post insert model for the manager area.
	/// </summary>
	public class InsertModel
	{
		#region Properties
		/// <summary>
		/// Post template id.
		/// </summary>
		public Guid TemplateId { get; set; }
		#endregion

		/// <summary>
		/// Default constructor, creates a new model.
		/// </summary>
		public InsertModel() {
			TemplateId = Guid.Empty;
		}
	}
}
