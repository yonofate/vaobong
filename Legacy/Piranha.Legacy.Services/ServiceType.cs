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

namespace Piranha.Legacy.Services
{
	/// <summary>
	/// The different services available.
	/// </summary>
	[Flags]
	public enum ServiceType
	{
		All = 1,
		Category = 2,
		Changes = 4,
		Media = 8,
		Page = 16,
		PageType = 32,
		Post = 64,
		PostType = 128,
		SiteMap = 256
	}
}
