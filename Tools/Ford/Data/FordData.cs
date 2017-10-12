using Injectoclean.Tools.Data;
using System;
using System.Collections.Generic;

namespace Injectoclean.Tools.Ford.Data
{
    public static class FordData
    {
        public static String[][] GetIds()
        {
            
            String query = "SELECT name FROM sqlite_master where type='table'";
            return new Db_connection("dbFordVinGeneric.db").GetConsultAsArray(query, 1);
        }

        public static FordCarInfo getFordCarInfo(VinInfo vinInfo)
        {
            String year, nameindb, subtype,type;
           
            Db_connection connection =new Db_connection("dbFordVinGeneric.db");
            String query = "SELECT years.year, years.nameInDB FROM years INNER JOIN vin_group_2 ON years.convID = vin_group_2.id " +
            "WHERE years.vinGroup = 2 AND vin_group_2.code = " + vinInfo.getVinGroup2() +" AND years.year = " + vinInfo.getVinYear();
            String[][] info1=connection.GetConsultAsArray(query,2);

            year = info1[0][0];
            nameindb = info1[0][1];

            query = " SELECT vin_group_3.subType, vin_group_3.type FROM vin_group_3 INNER JOIN " +
            "years ON years.convID = vin_group_3.id WHERE years.vinGroup = 3 AND years.nameInDB = '" + nameindb+"'" +
            " AND years.year = " + year + " AND vin_group_3.code = " + vinInfo.getVinGroup3();
            info1 = connection.GetConsultAsArray(query, 2);

            subtype= info1[0][0];
            type = info1[0][1];

            query = "SELECT vehicle_list.VehicleID, vehicle_list.Type, vehicle_list.SubType FROM vehicle_list WHERE Model = '" + 
                nameindb + "'  AND Year LIKE '" + year + "%' " +" AND (SubType = '" + subtype + "' OR SubType = 'ANY') " +
                " AND (Type = '" + type + "' OR Type = 'ANY' OR Type = 'Null' ) ";
            String[][] result= connection.GetConsultAsArray(query, 3);

            List<CarID> carsid = new List<CarID>();

            for (int i = 0; i < result.Length; i++)
                carsid.Add(new CarID(Convert.ToInt64(result[i][0]), result[i][1], result[i][2]));

            return new FordCarInfo(" ", nameindb, Convert.ToInt32(year), vinInfo.getVinGroup2() ,vinInfo.getVinGroup3(),type,subtype,carsid);
        }
    }
}
