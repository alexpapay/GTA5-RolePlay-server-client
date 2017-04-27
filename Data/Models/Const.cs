namespace TheGodfatherGM.Data.Models
{
    class RentModels
    {
        public const int ScooterModel = -1842748181;
        public const int TaxiModel = -956048545;
    }
    class AutoModels
    {
        public const int GangVagon = -810318068;
        public const int GangCar = 1507916787;
    }

    class Materials
    {
        public const int GangVagonCapacity = 10000;
        public const int GangCarCapacity = 5000;
        public const int GangPersonCapacity = 500;
    }

    class Time
    {
        public const int ScooterRentTime = 30;
        public const int TaxiRentTime = 60;        
    }

    class Prices
    {
        public const int ScooterRentPrice  = 30; 
        public const int TaxiRentPrice = 100;
        public const int DriverLicensePrice = 50;
    }

    class JobsIdNonDataBase
    {
        public const int TaxiDriver = 777;
        public const int Unemployer = 888;
    }

    class PayDayMoney
    {
        public static int GetPayDaYMoney(int groupId)
        {
            switch(groupId)
            {
                case 1: return 0;       // Homeless
                case 2: return 100;     // Unemployer

                case 101: return 5000;
                case 102: return 5500;
                case 103: return 6000;
                case 104: return 6500;
                case 105: return 7000;
                case 106: return 7500;
                case 107: return 8000;
                case 108: return 8500;
                case 109: return 9000;
                case 110: return 9500;
                case 111: return 10000;
                case 112: return 10500;
                case 113: return 11000;
                case 114: return 12000;

                case 201: return 4900;
                case 202: return 5400;
                case 203: return 5900;
                case 204: return 6400;
                case 205: return 6900;
                case 206: return 7400;
                case 207: return 7900;
                case 208: return 8400;
                case 209: return 10400;
                case 210: return 12400;

                case 1101: return 6000;
                case 1102: return 6000;
                case 1103: return 8000;
                case 1104: return 10000;
                case 1105: return 16000;
                case 1106: return 20000;

                case 1201: return 4000;
                case 1202: return 10000;
                case 1203: return 10500;
                case 1204: return 11000;
                case 1205: return 11500;
                case 1206: return 12000;
                case 1207: return 13000;
                case 1208: return 14000;
                case 1209: return 16000;
                case 1210: return 19000;

                case 2001: return 3000;
                case 2002: return 3500;
                case 2003: return 4000;
                case 2004: return 4500;
                case 2005: return 5000;
                case 2006: return 5500;
                case 2007: return 6000;
                case 2008: return 6500;
                case 2009: return 7000;
                case 2010: return 7500;
                case 2011: return 8000;
                case 2012: return 8500;
                case 2013: return 9000;
                case 2014: return 9500;
                case 2015: return 10000;

                case 2101: return 3000;
                case 2102: return 3500;
                case 2103: return 4000;
                case 2104: return 4500;
                case 2105: return 5000;
                case 2106: return 5500;
                case 2107: return 6000;
                case 2108: return 6500;
                case 2109: return 7000;
                case 2110: return 7500;
                case 2111: return 8000;
                case 2112: return 8500;
                case 2113: return 9000;
                case 2114: return 9500;
                case 2115: return 10000;

                case 3001: return 3500;
                case 3002: return 4500;
                case 3003: return 5500;
                case 3004: return 6500;
                case 3005: return 7500;
                case 3006: return 8500;
                case 3007: return 9500;
                case 3008: return 10500;
                case 3009: return 11500;
                case 3010: return 12500;

                case 3101: return 3500;
                case 3102: return 4500;
                case 3103: return 5500;
                case 3104: return 6500;
                case 3105: return 7500;
                case 3106: return 8500;
                case 3107: return 9500;
                case 3108: return 10500;
                case 3109: return 11500;
                case 3110: return 12500;

                case 3201: return 3500;
                case 3202: return 4500;
                case 3203: return 5500;
                case 3204: return 6500;
                case 3205: return 7500;
                case 3206: return 8500;
                case 3207: return 9500;
                case 3208: return 10500;
                case 3209: return 11500;
                case 3210: return 12500;
            }
            return 0;
        }
    }
}
