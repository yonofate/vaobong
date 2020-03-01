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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

using Piranha.Data;
using Piranha.Globalization;

namespace Piranha.Models
{
	[PrimaryKey(Column = "permalink_id")]
	[Serializable]
	public class Permalink : PiranhaRecord<Permalink>, ICacheRecord<Permalink>
	{
		#region Inner classes
		/// <summary>
		/// Inner class for mapping permalink names in namespaces to permalink id.
		/// </summary>
		private class NamespaceDictionary
		{
			#region Members
			private Dictionary<Guid, Dictionary<string, Guid>> InnerCache = new Dictionary<Guid, Dictionary<string, Guid>>();
			#endregion

			public Dictionary<string, Guid> this[Guid namespaceId] {
				get {
					if (!InnerCache.ContainsKey(namespaceId))
						InnerCache.Add(namespaceId, new Dictionary<string, Guid>());
					return InnerCache[namespaceId];
				}
			}
		}
		#endregion

		#region Inner classes
		public enum PermalinkType
		{
			PAGE, POST, CATEGORY, SITE, ARCHIVE, MEDIA
		}
		#endregion

		#region Fields
		[Column(Name = "permalink_id")]
		[Required()]
		public override Guid Id { get; set; }

		[Column(Name = "permalink_namespace_id")]
		public Guid NamespaceId { get; set; }

		[Column(Name = "permalink_type")]
		public PermalinkType Type { get; set; }

		[Column(Name = "permalink_name", OnSave = "ValidatePermalink")]
		public string Name { get; set; }

		[Column(Name = "permalink_created")]
		public override DateTime Created { get; set; }

		[Column(Name = "permalink_updated")]
		public override DateTime Updated { get; set; }

		[Column(Name = "permalink_created_by")]
		public override Guid CreatedBy { get; set; }

		[Column(Name = "permalink_updated_by")]
		public override Guid UpdatedBy { get; set; }
		#endregion

		#region Cache
		private static NamespaceDictionary NamespaceCache = new NamespaceDictionary();
		#endregion

		#region Handlers
		/// <summary>
		/// Validates the current permalink.
		/// </summary>
		/// <param name="str">The permalink name</param>
		/// <returns>The validated name</returns>
		protected string ValidatePermalink(string str) {
			return Generate(str, Type);
		}
		#endregion

		#region Static accessors
		/// <summary>
		/// Gets the permalink with the given id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns>The permalink</returns>
		public static Permalink GetSingle(Guid id) {
			if (id != Guid.Empty) {
				if (!Application.Current.CacheProvider.Contains(id.ToString())) {
					var perm = GetSingle((object)id);
					if (perm != null)
						AddToCache(perm);
					return perm;
				}
				if (!Application.Current.CacheProvider.Contains(id.ToString()))
					Application.Current.CacheProvider[id.ToString()] = GetSingle((object)id);
				return (Permalink)Application.Current.CacheProvider[id.ToString()];
			}
			return null;
		}

		/// <summary>
		/// Gets the permalink with the given name from the default namespace.
		/// </summary>
		/// <param name="name">The permalink name</param>
		/// <returns>The permalink</returns>
		public static Permalink GetByName(string name) {
			return GetByName(Config.DefaultNamespaceId, name);
		}

		/// <summary>
		/// Gets the permalink with the given name.
		/// </summary>
		/// <param name="namespaceid">The namespace id</param>
		/// <param name="name">The permalink name</param>
		/// <returns>The permalink</returns>
		public static Permalink GetByName(Guid namespaceid, string name) {
			if (!NamespaceCache[namespaceid].ContainsKey(name)) {
				var perm = GetSingle("permalink_name = @0 AND permalink_namespace_id = @1", name, namespaceid);

				if (perm != null) {
					AddToCache(perm);
					return perm;
				}
				return null;
			}
			if (!Application.Current.CacheProvider.Contains(NamespaceCache[namespaceid][name].ToString()))
				Application.Current.CacheProvider[NamespaceCache[namespaceid][name].ToString()] = GetSingle("permalink_name = @0 AND permalink_namespace_id = @1", name, namespaceid);
			return (Permalink)Application.Current.CacheProvider[NamespaceCache[namespaceid][name].ToString()];
		}
		#endregion

		/// <summary>
		/// Saves the current entity.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Whether the operation succeeded or not</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			// Check for duplicates 
			if (Permalink.GetSingle("permalink_name = @0 AND permalink_namespace_id = @2" + (!IsNew ? " AND permalink_id != @1" : ""), Name, Id, NamespaceId) != null)
				throw new DuplicatePermalinkException();
			return base.Save(tx);
		}

		/// <summary>
		/// Converts the given string to a web safe permalink.
		/// </summary>
		/// <param name="str">The string</param>
		/// <param name="type">Optional permalink type</param>
		/// <returns>A permalink</returns>
        /// 
        public static string ConvertVN(string chucodau)
        {
            const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            int index = -1;
            char[] arrChar = FindText.ToCharArray();
            while ((index = chucodau.IndexOfAny(arrChar)) != -1)
            {
                int index2 = FindText.IndexOf(chucodau[index]);
                chucodau = chucodau.Replace(chucodau[index], ReplText[index2]);
            }
            return chucodau;
        } 
		public static string Generate(string str, PermalinkType type = PermalinkType.PAGE) {
			var suffix = "";

			if (type == PermalinkType.MEDIA) {
				var segments = str.Split(new char[] { '.' });
				if (segments.Length > 1) {
					suffix = segments[segments.Length - 1];

					str = segments.Subset(0, segments.Length - 1).Implode(".");
				}
			}

			str = str.ToLower().TransliterateRussianToLatin();
            str = ConvertVN(str);
			var perm = Regex.Replace(str
				.Replace(" ", "-")
				.Replace("å", "a")
				.Replace("ä", "a")
				.Replace("á", "a")
				.Replace("à", "a")
				.Replace("ö", "o")
				.Replace("ó", "o")
				.Replace("ò", "o")
				.Replace("é", "e")
				.Replace("è", "e")
				.Replace("í", "i")
				.Replace("ì", "i"), @"[^a-z0-9-/]", "").Replace("--", "-");

			if (perm.EndsWith("-"))
				perm = perm.Substring(0, perm.LastIndexOf("-"));
			if (perm.StartsWith("-"))
				perm = perm.Substring(Math.Min(perm.IndexOf("-") + 1, perm.Length));

			return perm + (!String.IsNullOrEmpty(suffix) ? "." + suffix : "");
		}

		/// <summary>
		/// Invalidates the current record from the cache.
		/// </summary>
		/// <param name="record">The record</param>
		public void InvalidateRecord(Permalink record) {
			Application.Current.CacheProvider.Remove(record.Id.ToString());
			if (NamespaceCache[record.NamespaceId].ContainsKey(record.Name))
				NamespaceCache[record.NamespaceId].Remove(record.Name);
		}

		/// <summary>
		/// Adds the given permalink to the cache.
		/// </summary>
		/// <param name="perm">The permalink</param>
		private static void AddToCache(Permalink perm) {
			Application.Current.CacheProvider[perm.Id.ToString()] = perm;
			NamespaceCache[perm.NamespaceId][perm.Name] = perm.Id;
		}
	}
}
