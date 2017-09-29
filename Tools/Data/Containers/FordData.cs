using System;
using System.Collections.Generic;

namespace Injectoclean.Tools.Data
{
    static class FordData
    {
        public static List<String[]> GetIds()
        {
            
            String query = "SELECT name FROM sqlite_master where type='table'";
            return new Db_connection("dbFordVinGeneric.db").consult(query, 1);
        }
    }
}
