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

namespace Piranha.Extend
{
	/// <summary>
	/// Enum defining the different types of extensions.
	/// </summary>
	[Flags]
	public enum ExtensionType
	{
		NotSet = 0,
		Region = 1,
		User = 2,
		Page = 4,
		Post = 8,
		Category = 16,
		Media = 32
	}
}