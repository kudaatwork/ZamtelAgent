// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace YomoneyApp
{
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters. 
	/// </summary>
	public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string NeedSyncFeedbackKey = "need_sync_feedback";
        private static readonly bool NeedSyncFeedbackDefault = false;

        private const string LastSyncKey = "last_sync";
        private static readonly DateTime LastSyncDefault = DateTime.Now.AddDays(-30);



        #endregion

#if DEBUG
        //always refresh in debug
        public static bool NeedsSync
        {
            get { return true; }
        }
#else
		//public static bool NeedsSync
		//{
		//	get { return LastSync < DateTime.Now.AddDays (-3); }
		//}
#endif

        //public static DateTime LastSync
        //{
        //    get
        //    {
        //        return AppSettings.GetValueOrDefault<DateTime>(LastSyncKey, LastSyncDefault);
        //    }
        //    set
        //    {
        //        AppSettings.AddOrUpdateValue<DateTime>(LastSyncKey, value);
        //    }
        //}

        //public static string GeneralSettings
        //{
        //    get
        //    {
        //        return AppSettings.GetValueOrDefault(SettingsKey,true,SettingsDefault);
        //    }
        //    set
        //    {
        //        AppSettings.AddOrUpdateValue(SettingsKey, value);
        //    }
        //}

    }
}