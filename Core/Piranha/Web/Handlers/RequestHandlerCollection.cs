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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Piranha.Web.Handlers
{
	/// <summary>
	/// Represents a collection of request handlers.
	/// </summary>
	public sealed class RequestHandlerCollection : IEnumerable<RequestHandlerRegistration>
	{
		#region Members
		/// <summary>
		/// Private dictionary for holding the registered handlers.
		/// </summary>
		private readonly Dictionary<string, RequestHandlerRegistration> handlers = new Dictionary<string, RequestHandlerRegistration>();

		/// <summary>
		/// Private lock mutex for thread safety.
		/// </summary>
		private readonly object mutex = new object();
		#endregion

		/// <summary>
		/// Gets the request handler with the given id.
		/// </summary>
		/// <param name="id">The handler id</param>
		/// <returns>The handler, null if not found</returns>
		public IRequestHandler this[string id] {
			get {
				if (handlers.ContainsKey(id.ToUpper()))
					return handlers[id.ToUpper()].Handler;
				return null;
			}
		}

		/// <summary>
		/// Default internal constructor.
		/// </summary>
		internal RequestHandlerCollection() { }

		/// <summary>
		/// Clears the handler collection of all of the currently requested handlers.
		/// </summary>
		public void Clear() {
			lock (mutex) {
				handlers.Clear();
			}
		}

		/// <summary>
		/// Adds the given request handler to the collection.
		/// </summary>
		/// <param name="urlprefix">The url prefix to trigger on</param>
		/// <param name="id">The id of the handler</param>
		/// <param name="handler">The handler</param>
		public void Add(string urlprefix, string id, IRequestHandler handler) {
			lock (mutex) {
				handlers.Add(id.ToUpper(), new RequestHandlerRegistration() { UrlPrefix = urlprefix, Id = id, Handler = handler });
			}
		}

		/// <summary>
		/// Removes the request handler with the given id.
		/// </summary>
		/// <param name="id">The handler id</param>
		public void Remove(string id) {
			lock (mutex) {
				if (handlers.ContainsKey(id.ToUpper()))
					handlers.Remove(id.ToUpper());
			}
		}

		/// <summary>
		/// Gets the url prefix for the handler with the given id.
		/// </summary>
		/// <param name="id">The id of the handler</param>
		/// <returns>The url prefix</returns>
		public string GetUrlPrefix(string id) {
			if (handlers.ContainsKey(id.ToUpper()))
				return handlers[id.ToUpper()].UrlPrefix;
			throw new KeyNotFoundException("No handler registered with the id " + id);
		}

		/// <summary>
		/// Gets the enumerator for the currently registered handlers.
		/// </summary>
		/// <returns>The enumerator</returns>
		public IEnumerator<RequestHandlerRegistration> GetEnumerator() {
			return handlers.Values.GetEnumerator();
		}

		/// <summary>
		/// Gets the enumerator for the currently registered handlers.
		/// </summary>
		/// <returns>The enumerator</returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return handlers.Values.GetEnumerator();
		}
	}
}