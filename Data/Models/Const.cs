using GTANetworkShared;
using System.Collections.Generic;

namespace TheGodfatherGM.Data.Models
{    
    class BusOne
    {
        public static readonly Vector3 Marker1 = new Vector3   (-1039.623, -2714.09,   12.66907);  // Airport: 1
        public static readonly Vector3 Marker2 = new Vector3   (304.7786,  -1381.645,  30.47842);  // Medical: 2
        public static readonly Vector3 Marker3 = new Vector3   (136.69,    -946.8344,  28.32286);  // Meria: 3 
        public static readonly Vector3 Marker4 = new Vector3   (-146.7599, -919.225,   27.87529);  // Work loader: 4         
        public static readonly Vector3 MarkerFin = new Vector3 (-798.1025, -2368.539,  13.57907);  // BusStation: 5
    }
    class GasStation_Id3
    {
        public static readonly Vector3 Marker1 = new Vector3   (-58.83617, -1761.643,  28.0);
        public static readonly Vector3 Marker2 = new Vector3   (-61.65413, -1768.636,  28.0);
        public static readonly Vector3 Marker3 = new Vector3   (-69.89056, -1765.871,  28.2);
        public static readonly Vector3 Marker4 = new Vector3   (-67.24121, -1759.187,  28.2);         
        public static readonly Vector3 Marker5 = new Vector3   (-77.71686, -1763.026,  28.4);
        public static readonly Vector3 Marker6 = new Vector3   (-75.38932, -1756.052,  28.4);
    }
    class GasStation_Id4
    {
        public static readonly Vector3 Marker1 = new Vector3(167.8455, -1561.029, 28.3);
        public static readonly Vector3 Marker2 = new Vector3(174.147,  -1554.992, 28.3);
        public static readonly Vector3 Marker3 = new Vector3(180.5104, -1560.039, 28.3);
        public static readonly Vector3 Marker4 = new Vector3(173.2242, -1566.945, 28.3);
    }
    class WorkPay
    {
        public const int AllGusStationOwner = 10000;
        public const int BusDriver1Pay = 1000;
        public const int TaxiDriver = 600;
        public const int Unemployer = 300;

        public const int Loader1Pay = 10;       // Стройка
        public const int Loader2Pay = 7;        // Порт
    }
    class RentModels
    {
        public const int ScooterModel = -1842748181;
        public const int TaxiModel = -956048545;
    }
    // TODO: linking with DB
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
        public const int Homeless = 0;
        public const int Loader1 = 1;
        public const int Loader2 = 2;
        public const int TaxiDriver = 777;

        // BusDriver logic also in: VehicleController.cs | MenuManager.cs | JobController.cs | KeyManager.cs
        public const int BusDriver = 888;
        public const int BusDriver1 = 881;
        public const int BusDriver2 = 882;
        public const int BusDriver3 = 883;
        public const int BusDriver4 = 884;
        // 

        public const int Unemployer = 999;
    }
    // FUEL:
    class FuelByType
    {
        public static double GetFuel(int model)
        {
            switch (model)
            {
                case 1394036463: return 350;
                case -1600252419: return 250;
                case 321739290: return 80;
                case 1074326203: return 120;
                case -810318068: return 70;
                case 1507916787: return 50;
                case -825837129: return 50;
                case 2006667053: return 50;
                case 1923400478: return 50;
                case -713569950: return 80;
                case -956048545: return 40;
                case 782665360:  return 200;
                case 630371791:  return 120;
                default: return 0;
            }
        }
        public static double GetConsumption(int model)
        {
            switch (model)
            {
                case 1394036463: return 0.035;
                case -1600252419:return 0.020;                
                case 1074326203: return 0.015;
                case 321739290:  return 0.007;
                case -713569950: return 0.007;
                case -810318068: return 0.005;
                case 1507916787: return 0.003;
                case -825837129: return 0.003;
                case 2006667053: return 0.003;
                case 1923400478: return 0.003;                
                case -956048545: return 0.002;
                case 782665360:  return 0.005;
                case 630371791:  return 0.015;
                default: return 0.001;
            }
        }
    }
    class Stocks
    {
        public static int GetStockCapacity(string stockName)
        {
            switch (stockName)
            {
                case "Army1_stock": return 200000;
                case "Army2_stock": return 500000;
                case "Police_stock": return 200000;
                case "FBI_stock": return 200000;
                default: return 200000;
            }
        }
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
    class GroupsConst
    {
        public static readonly List<int> GroupsIndexes = new List<int> { 100, 200, 300, 1100, 1300, 1400, 1500, 1600, 1700, 2000, 2100, 3000, 3100 };
    }
}
