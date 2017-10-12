using System;
using System.Collections.Generic;

namespace Injectoclean.Tools.Ford.Data
{
    public class FordCarInfo
    {
        String group2, group3, type, subtype, model, name;
        int year;
        List<CarID> carsid;
        public FordCarInfo(string model, string name, int year, string group2, string group3, string type, string subtype, List<CarID> carsid)
        {
            this.group2 = group2;
            this.group3 = group3;
            this.type = type;
            this.subtype = subtype;
            this.model = model;
            this.name = name;
            this.year = year;
            this.carsid = carsid;
        }

        public string Group2 { get => group2; set => group2 = value; }
        public string Group3 { get => group3; set => group3 = value; }
        public string Type { get => type; set => type = value; }
        public string Subtype { get => subtype; set => subtype = value; }
        public string Model { get => model; set => model = value; }
        public string Name { get => name; set => name = value; }
        public int Year { get => year; set => year = value; }
        public List<CarID> Carsid { get => carsid; set => carsid = value; }
       
    }
}
