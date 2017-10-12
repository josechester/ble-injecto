using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Injectoclean.Tools.Data
{
    class Db_connection
    {
        //private SQLiteConnection connection;
        private sqlite3 sqlite;


        private Db_connection() { }

        public Db_connection(String database)
        {
            String file = Directory.GetCurrentDirectory() + "\\Assets\\Data\\" + database;
            SQLitePCL.raw.SetProvider(new SQLite3Provider_sqlcipher());
            SQLitePCL.Batteries_V2.Init();
            SQLitePCL.raw.FreezeProvider();
            if (System.IO.File.Exists(file))
            {
                //connection = new SQLiteConnection(file, flag);
                Debug.Assert(SQLitePCL.raw.sqlite3_open(file, out sqlite) == SQLitePCL.raw.SQLITE_OK);
                Debug.Assert(SQLitePCL.raw.sqlite3_exec(sqlite, "PRAGMA key ='lalosebas'") == SQLitePCL.raw.SQLITE_OK);

            }

        }
        public String[][] GetConsultAsArray(String consult, int rows)
        {
            List<String[]> list = new List<String[]>();

            sqlite3_stmt statement = null;
            Debug.Assert(SQLitePCL.raw.sqlite3_prepare_v2(sqlite, consult, out statement) == SQLitePCL.raw.SQLITE_OK);
            while (SQLitePCL.raw.sqlite3_step(statement) == raw.SQLITE_ROW)
            {
                String[] element = new String[rows];
                for (int i = 0; i < rows; i++)
                    element[i] = raw.sqlite3_column_text(statement, i);
                list.Add(element);
            }
            return list.ToArray();
        }
        public List<String[]> GetConsultAsList(String consult, int rows)
        {
            List<String[]> list = new List<String[]>();

            sqlite3_stmt statement = null;
            Debug.Assert(SQLitePCL.raw.sqlite3_prepare_v2(sqlite, consult, out statement) == SQLitePCL.raw.SQLITE_OK);
            while (SQLitePCL.raw.sqlite3_step(statement) == raw.SQLITE_ROW)
            {
                String[] element = new String[rows];
                for (int i = 0; i < rows; i++)
                    element[i] = raw.sqlite3_column_text(statement, i);
                list.Add(element);
            }
            return list;
        }
        ~Db_connection()  // destructor
        {
            raw.sqlite3_close(this.sqlite);
        }
    }
}
