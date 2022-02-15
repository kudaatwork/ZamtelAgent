using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using YomoneyApp.Droid.DbStorage;
using static YomoneyApp.Storage.DbConnection;

[assembly: Dependency(typeof(StorageConnectionDroid))]
namespace YomoneyApp.Droid.DbStorage
{
    public class StorageConnectionDroid : Isqlite
    {
        public SQLiteConnection GetConnection()
    {
        var dbase = "app_db";
        var dbpath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
        var path = Path.Combine(dbpath, dbase);
        var connection = new SQLiteConnection(path);
        return connection;
    }
}
}