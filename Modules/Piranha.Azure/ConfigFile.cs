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
using System.Configuration;

namespace Piranha.Azure
{
	/// <summary>
	/// The main configuration section for Piranha CMS Azure
	/// </summary>
	internal class ConfigFile : ConfigurationSection
	{
		#region Members
		private const string SETTINGS = "settings";
		private const string PROVIDERS = "providers";
		#endregion

		/// <summary>
		/// Gets/sets the settings.
		/// </summary>
		[ConfigurationProperty(SETTINGS, IsRequired = false)]
		public SettingsElement Settings {
			get { return (SettingsElement)this[SETTINGS]; }
			set { this[SETTINGS] = value; }
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ConfigFile() {
			Settings = new SettingsElement();
		}
	}

	/// <summary>
	/// The settings element of the configuration section.
	/// </summary>
	internal class SettingsElement : ConfigurationElement
	{
		#region Members
		private const string STORAGE_CONNECTION_STRING = "storageConnectionString";
		#endregion

		/// <summary>
		/// Gets/sets the connection string for the azure blob storage.
		/// </summary>
		[ConfigurationProperty(STORAGE_CONNECTION_STRING, IsRequired = false)]
		public StringElement StorageConnectionString {
			get { return (StringElement)this[STORAGE_CONNECTION_STRING]; }
			set { this[STORAGE_CONNECTION_STRING] = value; }
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public SettingsElement() {
			StorageConnectionString = new StringElement();
		}
	}

	/// <summary>
	/// A configuration element with a string value.
	/// </summary>
	internal class StringElement : ConfigurationElement
	{
		/// <summary>
		/// Gets/sets the element value.
		/// </summary>
		[ConfigurationProperty("value", IsRequired = true)]
		public string Value {
			get { return (string)this["value"]; }
			set { this["value"] = value; }
		}
	}
}