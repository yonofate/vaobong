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

namespace Piranha.Cache
{
	/// <summary>
	/// Cache provider for keeping objects in memory.
	/// </summary>
	public class MemCacheProvider : ICacheProvider
	{
		#region Members
		private Dictionary<string, object> Cache = new Dictionary<string, object>();
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the cache object with the given key.
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>The cached value</returns>
		public object this[string key] {
			get { return Cache.ContainsKey(key) ? Cache[key] : null; }
			set { Cache[key] = value; }
		}
		#endregion

		/// <summary>
		/// Removes the object with the given key from the cache.
		/// </summary>
		/// <param name="key">The key</param>
		public void Remove(string key) {
			if (Cache.ContainsKey(key))
				Cache.Remove(key);
		}

		/// <summary>
		/// Checks if an object exists for the given key.
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>Whether an object with the given key exists.</returns>
		public bool Contains(string key) {
			return Cache.ContainsKey(key);
		}
	}
}