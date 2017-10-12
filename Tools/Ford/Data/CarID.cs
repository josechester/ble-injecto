using System;
namespace Injectoclean.Tools.Ford.Data
{
    public class CarID
    {
        long id;
        String type, subtype;

        private CarID()
        {
        }

        public CarID(long id, string type, string subtype)
        {
            this.id = id;
            this.type = type;
            this.subtype = subtype;
        }
        public long Id { get => id; set => id = value; }
        public string Type { get => type; set => type = value; }
        public string Subtype { get => subtype; set => subtype = value; }
    }
}
