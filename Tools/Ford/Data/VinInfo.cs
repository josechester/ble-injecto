using System;
namespace Injectoclean.Tools.Ford.Data
{
    public class VinInfo
    {

        public String getVinGroup2() => vinGroup2;

        public void setVinGroup2(String vinGroup2) => this.vinGroup2 = vinGroup2;

        public String getVinGroup3() => vinGroup3;

        public void setVinGroup3(String vinGroup3) => this.vinGroup3 = vinGroup3;

        public int getVinYear() => vinYear;

        public void setVinYear(int vinYear) => this.vinYear = vinYear;

        private String vinGroup2;
        private String vinGroup3;
        private int vinYear;

        public VinInfo(String g2, String g3, int year)
        {
            this.vinGroup2 = g2;
            this.vinGroup3 = g3;
            this.vinYear = year;
        }
        public String ToString
        {
            get
            {
                return "VinYear: "+vinYear+" VinGroup2: " + vinGroup2 + " VinGroup3: " + vinGroup3;
            }
        }
    }
}
