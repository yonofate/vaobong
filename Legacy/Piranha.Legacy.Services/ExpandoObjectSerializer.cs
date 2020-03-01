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
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Piranha.Legacy.Services
{
	/// <summary>
	/// Serializer that serializes an expando object as a json object.
	/// </summary>
	public class ExpandoObjectSerializer : JavaScriptConverter
	{
		/// <summary>
		/// Deserializes an collection to an expando object.
		/// </summary>
		/// <param name="dictionary">The dicitionary</param>
		/// <param name="type">The type</param>
		/// <param name="serializer">The serializer</param>
		/// <returns>The deserialized expando object</returns>
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer) {
			var ret = new ExpandoObject();

			foreach (var key in dictionary.Keys) {
				((IDictionary<string, object>)ret).Add(key, dictionary[key]);
			}
			return ret;
		}

		/// <summary>
		/// Serializes an expando object as a json object.
		/// </summary>
		/// <param name="obj">The object to serialize</param>
		/// <param name="serializer">The serializer</param>
		/// <returns>The dictionary</returns>
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer) {
			var ret = new Dictionary<string, object>();

			if (obj != null)
				foreach (var key in ((IDictionary<string, object>)obj).Keys) {
					ret.Add(key, ((IDictionary<string, object>)obj)[key]);
				}
			return ret;
		}

		/// <summary>
		/// Gets the types supported for this serializer.
		/// </summary>
		public override IEnumerable<Type> SupportedTypes {
			get { return new List<Type>() { typeof(ExpandoObject) }; }
		}
	}
}