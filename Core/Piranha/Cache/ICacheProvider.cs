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

namespace Piranha.Cache
{
	/// <summary>
	/// Interface defining a cache provider
	/// </summary>
	public interface ICacheProvider
	{
		/// <summary>
		/// Gets/sets the object with the given id.
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>The cached object</returns>
		object this[string key] { get; set; }

		/// <summary>
		/// Removes the object with the given key from the cache.
		/// </summary>
		/// <param name="key">The key</param>
		void Remove(string key);

		/// <summary>
		/// Checks if an object exists for the given key.
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>Whether an object with the given key exists.</returns>
		bool Contains(string key);
	}
}
