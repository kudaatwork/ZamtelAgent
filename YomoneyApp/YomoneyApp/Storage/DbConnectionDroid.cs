using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;
using YomoneyApp.Storage;
using static YomoneyApp.Storage.DbConnection;

[assembly: Dependency(typeof(DbConnectionDroid))]
namespace YomoneyApp.Storage
{
    public class DbConnectionDroid : Isqlite
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
