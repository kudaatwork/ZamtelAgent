using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace YomoneyApp.Storage
{
    public class DbConnection
    {
        public interface Isqlite
        {
            SQLiteConnection GetConnection();
        }
    }
}
