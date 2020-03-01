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

namespace Piranha
{
	/// <summary>
	/// The main configuration section for Piranha CMS
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
		/// Gets/sets the providers.
		/// </summary>
		[ConfigurationProperty(PROVIDERS, IsRequired = false)]
		public ProviderElement Providers {
			get { return (ProviderElement)this[PROVIDERS]; }
			set { this[PROVIDERS] = value; }
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ConfigFile() {
			Settings = new SettingsElement();
			Providers = new ProviderElement();
		}
	}

	/// <summary>
	/// The settings element of the configuration section.
	/// </summary>
	internal class SettingsElement : ConfigurationElement
	{
		#region Members
		private const string DISABLE_METHOD_BINDING = "disableMethodBinding";
		private const string DISABLE_MODELSTATE_BINDING = "disableModelStateBinding";
		private const string DISABLE_MANAGER = "disableManager";
		private const string DISABLE_TYPE_BUILDER = "disableTypeBuilder";
		private const string DISABLE_CATALOG_SEARCH = "disableCatalogSearch";
		private const string DISABLE_COMPOSITION = "disableComposition";
		private const string MANAGER_NAMESPACES = "managerNamespaces";
		private const string PASSIVE_MODE = "passiveMode";
		private const string PREFIXLESS_PERMALINKS = "prefixlessPermalinks";
		private const string RENDERX_UA_COMPATIBLEFORIE = "renderX-UA-CompatibleForIE";
		private const string SHOWDBERRORS = "showDBErrors";
		private const string ENTITY_SCHEMA = "entitySchema";
	    private const string EXACT_PERMALINK_MATCHING = "exactPermalinkMatching";
		#endregion

		/// <summary>
		/// Gets/sets if method binding for Web Pages should be disabled or not.
		/// </summary>
		[ConfigurationProperty(DISABLE_METHOD_BINDING, IsRequired = false)]
		public BooleanElement DisableMethodBinding {
			get { return (BooleanElement)this[DISABLE_METHOD_BINDING]; }
			set { this[DISABLE_METHOD_BINDING] = value; }
		}

		/// <summary>
		/// Gets/sets if modelstate binding for Web Pages should be disabled or not.
		/// </summary>
		[ConfigurationProperty(DISABLE_MODELSTATE_BINDING, IsRequired = false)]
		public BooleanElement DisableModelStateBinding {
			get { return (BooleanElement)this[DISABLE_MODELSTATE_BINDING]; }
			set { this[DISABLE_MODELSTATE_BINDING] = value; }
		}

		/// <summary>
		/// Gets/sets if the manager interface should be disabled or not.
		/// </summary>
		[ConfigurationProperty(DISABLE_MANAGER, IsRequired = false)]
		public BooleanElement DisableManager {
			get { return (BooleanElement)this[DISABLE_MANAGER]; }
			set { this[DISABLE_MANAGER] = value; }
		}

		/// <summary>
		/// Gets/sets an optional default schema for Entity Framework.
		/// </summary>
		[ConfigurationProperty(ENTITY_SCHEMA, IsRequired = false)]
		public StringElement EntitySchema {
			get { return (StringElement)this[ENTITY_SCHEMA]; }
			set { this[ENTITY_SCHEMA] = value; }
		}

		/// <summary>
		/// Gets/sets if the page & post type builder of the Extension Manager
		/// should be disabled.
		/// </summary>
		[ConfigurationProperty(DISABLE_TYPE_BUILDER, IsRequired = false)]
		public BooleanElement DisableTypeBuilder {
			get { return (BooleanElement)this[DISABLE_TYPE_BUILDER]; }
			set { this[DISABLE_TYPE_BUILDER] = value; }
		}

		/// <summary>
		/// Gets if the providers should be searched outside of Piranha*.ddls
		/// </summary>
		[ConfigurationProperty(DISABLE_CATALOG_SEARCH, IsRequired = false)]
		public BooleanElement DisableCatalogSearch {
			get { return (BooleanElement)this[DISABLE_CATALOG_SEARCH]; }
			set { this[DISABLE_CATALOG_SEARCH] = value; }
		}

		/// <summary>
		/// Gets if the MEF composition should be enabled.
		/// </summary>
		[ConfigurationProperty(DISABLE_COMPOSITION, IsRequired = false)]
		public BooleanElement DisableComposition {
			get { return (BooleanElement)this[DISABLE_COMPOSITION]; }
			set { this[DISABLE_COMPOSITION] = value; }
		}

		/// <summary>
		/// Gets if "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge,chrome=1\"> should be rendered for IE
		/// </summary>
		[ConfigurationProperty(RENDERX_UA_COMPATIBLEFORIE, IsRequired = false)]
		public BooleanElement RenderX_UA_CompatibleForIE {
			get { return (BooleanElement)this[RENDERX_UA_COMPATIBLEFORIE]; }
			set { this[RENDERX_UA_COMPATIBLEFORIE] = value; }
		}

		/// <summary>
		/// Gets if database access errors should be shown instead of redirecting to /manager/install
		/// </summary>
		[ConfigurationProperty(SHOWDBERRORS, IsRequired = false)]
		public BooleanElement ShowDBErrors {
			get { return (BooleanElement)this[SHOWDBERRORS]; }
			set { this[SHOWDBERRORS] = value; }
		}

		/// <summary>
		/// Gets/sets the additional manager namespaces.
		/// </summary>
		[ConfigurationProperty(MANAGER_NAMESPACES, IsRequired = false)]
		public StringElement ManagerNamespaces {
			get { return (StringElement)this[MANAGER_NAMESPACES]; }
			set { this[MANAGER_NAMESPACES] = value; }
		}

		/// <summary>
		/// Gets/sets if the application is running in passive mode.
		/// </summary>
		[ConfigurationProperty(PASSIVE_MODE, IsRequired = false)]
		public BooleanElement PassiveMode {
			get { return (BooleanElement)this[PASSIVE_MODE]; }
			set { this[PASSIVE_MODE] = value; }
		}

        [ConfigurationProperty(EXACT_PERMALINK_MATCHING, IsRequired = false)]
	    public BooleanElement ExactPermalinkMatching
	    {
            get { return (BooleanElement) this[EXACT_PERMALINK_MATCHING]; }
            set { this[EXACT_PERMALINK_MATCHING] = value; }
	    }

		/// <summary>
		/// Gets/sets if the generated permalinks should be prefixless.
		/// </summary>
		[ConfigurationProperty(PREFIXLESS_PERMALINKS, IsRequired = false)]
		public BooleanElement PrefixlessPermalinks {
			get { return (BooleanElement)this[PREFIXLESS_PERMALINKS]; }
			set { this[PREFIXLESS_PERMALINKS] = value; }
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public SettingsElement() {
			DisableMethodBinding = new BooleanElement();
			DisableModelStateBinding = new BooleanElement();
			DisableManager = new BooleanElement();
			DisableCatalogSearch = new BooleanElement();
			DisableComposition = new BooleanElement();
			ManagerNamespaces = new StringElement();
			RenderX_UA_CompatibleForIE = new BooleanElement();
			ShowDBErrors = new BooleanElement();
		}
	}

	/// <summary>
	/// The provider element of the configuration section.
	/// </summary>
	internal class ProviderElement : ConfigurationElement
	{
		#region Members
		private const string MEDIA_PROVIDER = "mediaProvider";
		private const string MEDIA_CACHE_PROVIDER = "mediaCacheProvider";
		private const string CACHE_PROVIDER = "cacheProvider";
		private const string LOG_PROVIDER = "logProvider";
		#endregion

		/// <summary>
		/// Gets/sets the current configured media provider.
		/// </summary>
		[ConfigurationProperty(MEDIA_PROVIDER, IsRequired = false)]
		public StringElement MediaProvider {
			get { return (StringElement)this[MEDIA_PROVIDER]; }
			set { this[MEDIA_PROVIDER] = value; }
		}

		/// <summary>
		/// Gets/sets the current configured media cache provider.
		/// </summary>
		[ConfigurationProperty(MEDIA_CACHE_PROVIDER, IsRequired = false)]
		public StringElement MediaCacheProvider {
			get { return (StringElement)this[MEDIA_CACHE_PROVIDER]; }
			set { this[MEDIA_CACHE_PROVIDER] = value; }
		}

		/// <summary>
		/// Gets/sets the current configured cache provider.
		/// </summary>
		[ConfigurationProperty(CACHE_PROVIDER, IsRequired = false)]
		public StringElement CacheProvider {
			get { return (StringElement)this[CACHE_PROVIDER]; }
			set { this[CACHE_PROVIDER] = value; }
		}

		/// <summary>
		/// Gets/sets the current configured log provider.
		/// </summary>
		[ConfigurationProperty(LOG_PROVIDER, IsRequired = false)]
		public StringElement LogProvider {
			get { return (StringElement)this[LOG_PROVIDER]; }
			set { this[LOG_PROVIDER] = value; }
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ProviderElement() {
			MediaProvider = new StringElement();
			CacheProvider = new StringElement();
			LogProvider = new StringElement();
		}
	}

	/// <summary>
	/// A configuration element with a boolean value.
	/// </summary>
	internal class BooleanElement : ConfigurationElement
	{
		/// <summary>
		/// Gets/sets the element value.
		/// </summary>
		[ConfigurationProperty("value", IsRequired = true)]
		public bool Value {
			get { return (bool)this["value"]; }
			set { this["value"] = value; }
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