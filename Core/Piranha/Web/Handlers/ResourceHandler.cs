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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Piranha.Web.Handlers
{
	/// <summary>
	/// The resource handler delivers embedded resources for the manager
	/// with client caching enabled for better performance.
	/// </summary>
	internal class ResourceHandler : IRequestHandler
	{
		#region Inner classes
		/// <summary>
		/// Inner class representing an embedded resource.
		/// </summary>
		class Resource
		{
			/// <summary>
			/// Gets/sets the resource name.
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// Gets/sets the content type.
			/// </summary>
			public string ContentType { get; set; }
		}
		#endregion

		#region Members
		/// <summary>
		/// Mutexes
		/// </summary>
		private object LastModMutex = new object();
		private object ResourceMutex = new object();

		/// <summary>
		/// The last modification date of the assembly.
		/// </summary>
		private DateTime? LastMod = null;

		/// <summary>
		/// The available resources.
		/// </summary>
		private Dictionary<string, Resource> ResourceNames = null;
		#endregion

		/// <summary>
		/// Handles the current request.
		/// </summary>
		/// <param name="context">The current http context</param>
		/// <param name="args">the url data arguments</param>
		public void HandleRequest(HttpContext context, params string[] args) {
			var resource = args.Implode(".").ToLower();
			var assembly = Assembly.GetExecutingAssembly();

			//if (!LastMod.HasValue)
			EnsureLastMod(assembly);

			if (!Web.ClientCache.HandleClientCache(context, resource, LastMod.Value, false, 60)) {
				//if (ResourceNames == null)
				EnsureResourceNames(assembly);
				if (ResourceNames.ContainsKey(resource)) {
					var res = ResourceNames[resource];

					using (var stream = assembly.GetManifestResourceStream(res.Name)) {
						var bytes = new byte[stream.Length];
						stream.Read(bytes, 0, Convert.ToInt32(stream.Length));

						context.Response.ContentType = res.ContentType;
						context.Response.BinaryWrite(bytes);
						context.Response.StatusCode = 200;
						context.Response.EndClean();
					}
				} else {
					context.Response.StatusCode = 404;
				}
			}
		}

		#region Private methods
		/// <summary>
		/// Gets the last modification date from the assembly.
		/// </summary>
		/// <param name="assembly">The assembly</param>
		private void EnsureLastMod(Assembly assembly) {
			lock (LastModMutex) {
				if (!LastMod.HasValue)
					LastMod = new FileInfo(assembly.Location).LastWriteTime;
			}
		}

		/// <summary>
		/// Gets the currently available resources from the assembly.
		/// </summary>
		/// <param name="assembly">The assembly</param>
		private void EnsureResourceNames(Assembly assembly) {
			lock (ResourceMutex) {
				if (ResourceNames == null) {
					ResourceNames = new Dictionary<string, Resource>();

					foreach (var name in assembly.GetManifestResourceNames()) {
						ResourceNames.Add(name.Replace("Piranha.", "").ToLower(), new Resource() {
							Name = name, ContentType = GetContentType(name)
						});
					}
				}
			}
		}

		/// <summary>
		/// Gets the content type from the resource name.
		/// </summary>
		/// <param name="name">The resource name</param>
		/// <returns>The content type</returns>
		private string GetContentType(string name) {
			if (name.EndsWith(".js")) {
				return "text/javascript";
			} else if (name.EndsWith(".css")) {
				return "text/css";
			} else if (name.EndsWith(".png")) {
				return "image/png";
			} else if (name.EndsWith(".jpg")) {
				return "image/jpg";
			} else if (name.EndsWith(".gif")) {
				return "image/gif";
			} else if (name.EndsWith(".ico")) {
				return "image/ico";
			} else if (name.EndsWith(".eot")) {
				return "application/vnd.ms-fontobject";
			} else if (name.EndsWith(".ttf")) {
				return "application/octet-stream";
			} else if (name.EndsWith(".svg")) {
				return "image/svg+xml";
			} else if (name.EndsWith(".woff")) {
				return "application/x-woff";
			}
			return "application/unknown";
		}
		#endregion
	}
}