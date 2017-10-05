using Injectoclean.Tools.Data;
using System;
using System.Collections.Generic;

namespace Injectoclean.Tools.Ford.Data
{
    public static class FordData
    {
        public static List<String[]> GetIds()
        {
            
            String query = "SELECT name FROM sqlite_master where type='table'";
            return new Db_connection("dbFordVinGeneric.db").consult(query, 1);
        }
        public static List<String[]> GetCarIds()
        {

            String query = "";
            return new Db_connection("dbFordVinGeneric.db").consult(query, 1);
        }
    }
}
