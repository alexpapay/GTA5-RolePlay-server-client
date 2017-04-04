using GTANetworkServer;
using GTANetworkShared;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using TheGodfatherGM.Data;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Vehicles;
using TheGodfatherGM.Server.User;

namespace TheGodfatherGM.Server
{
    class Main : Script
    {     
        public Main()
        {
            API.onResourceStart += OnResourceStart;
            API.onResourceStop += OnResourceStop;
            API.onUpdate += OnUpdateHandler;
            OnStartInit();
        }

        public DateTime MinuteAnnounce;
        public DateTime TenMinuteAnnounce;
        public DateTime HourAnnounce;
        public int Tiks = 0;

        // Добавление минуты к пребыванию в игре после авторизации пользователя
        public void OnUpdateHandler()
        {
            // Ежеминутный тик
            if (DateTime.Now.Subtract(MinuteAnnounce).TotalMinutes >= 1)
            {
                // Уровень пользователя
                try
                {
                    var characters = ContextFactory.Instance.Character.Where(x => x.Online == true);
                    
                    foreach (var character in characters)
                    {
                        character.PlayMinutes++;                        
                        if (character.PlayMinutes % 1920 == 0) character.Level = character.PlayMinutes / 1920;
                    }
                    ContextFactory.Instance.SaveChanges();
                }
                catch (Exception e){}
                // Прокат транспорта (каждую минуту вычитается 1 ед. Fuel):
                try
                {
                    VehicleController.RentVehicle();
                }
                catch (Exception e){}

                MinuteAnnounce = DateTime.Now;
                //API.consoleOutput("Foo! see you in 1 minutes!");
            }

            // Ежечасовой тик
            if (DateTime.Now.Subtract(HourAnnounce).TotalMinutes >= 60)
            {
                // Начисление зарплаты сюда
                HourAnnounce = DateTime.Now;
            }

            // Десятиминутный тик (для проката)
            if (DateTime.Now.Subtract(TenMinuteAnnounce).TotalMinutes >= 10)
            {                
                TenMinuteAnnounce = DateTime.Now;
            }
        }

        private void OnResourceStart()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-GB");
            Console.BackgroundColor = ConsoleColor.Blue;
            API.consoleOutput(Global.GlobalVars.ServerName + " was started at " + DateTime.Now);
            Console.ResetColor();
        }

        private void OnResourceStop()
        {
            API.consoleOutput("Resetting active sessions...");
            Task DBTerminate = Task.Run(() =>
            {
                DatabaseManager.ResetSessions();
            });
            DBTerminate.Wait();
            API.consoleOutput(Global.GlobalVars.ServerName + " was stopped at " + DateTime.Now);        
        }

        // Инициализация объектов без записи в БД
        private void OnStartInit()
        {
            API.createVehicle((VehicleHash)(410882957), new Vector3(-811.4462, 156.760788, 71.10809), new Vector3(-3.75210786, -1.92715907, 79.57234),0 ,0);
            API.createVehicle((VehicleHash)(2071877360), new Vector3(-853.3489, 145.799789, 62.40442), new Vector3(-14.759819, 2.69780254, 174.772949),0 ,0 );
            API.createVehicle((VehicleHash)(-1237253773), new Vector3(-828.9175, 177.392563, 70.89192), new Vector3(7.28627443, 1.417656, -28.5729618),0 ,0 );
            API.createVehicle((VehicleHash)(1488164764), new Vector3(-825.8212, 182.185547, 71.29874), new Vector3(5.665733, 1.7860254, -39.7945328),0 ,0 );
            API.createVehicle((VehicleHash)(1488164764), new Vector3(-807.6245, 162.41214, 71.17705), new Vector3(0.352598161, -0.5480969, 109.292976),0 ,0 );
            API.createVehicle((VehicleHash)(-1883869285), new Vector3(-838.866, 163.701859, 67.45779), new Vector3(13.1287975, 4.510339, -53.2682037), 0,0 );
            API.createVehicle((VehicleHash)(-394074634), new Vector3(-821.36084, 159.40419, 69.8871), new Vector3(9.785537, 0.89815104, -84.11095),0 ,0 );
            API.createVehicle((VehicleHash)(-394074634), new Vector3(-827.0035, 160.031509, 68.9108), new Vector3(8.406221, 4.45850134, -107.544876),0 ,0 );
            API.createVehicle((VehicleHash)(-789894171), new Vector3(-835.375549, 167.264526, 68.77638), new Vector3(12.5381193, 5.627327, -38.6188431),0 , 0);
            API.createVehicle((VehicleHash)(-1255452397), new Vector3(-831.915161, 171.998322, 69.82217), new Vector3(9.321765, 1.11654949, -29.14971),0 ,0 );
            API.createVehicle((VehicleHash)(-394074634), new Vector3(-871.543457, 131.464737, 58.6371078), new Vector3(-10.4597931, -1.41173232, -177.254532),0 ,0 );
            API.createVehicle((VehicleHash)(-394074634), new Vector3(-871.0496, 198.6547, 72.8442), new Vector3(-4.68141747, 4.06180143, -154.1463),0 ,0 );
            API.createVehicle((VehicleHash)(-394074634), new Vector3(-869.1414, 180.522827, 69.3245), new Vector3(-13.5749063, -0.9683589, 171.600876),0 , 0);
            API.createVehicle((VehicleHash)(-394074634), new Vector3(-871.111938, 146.9499, 61.9895554), new Vector3(-13.5842285, 0.3455443, 177.248779), 0,0 );
            API.createVehicle((VehicleHash)(-394074634), new Vector3(-852.392151, 125.467781, 57.30052), new Vector3(-0.2624593, 12.0376472, -88.044),0 , 0);
            API.createVehicle((VehicleHash)(-339587598), new Vector3(-806.6621, 175.39238, 85.8902359), new Vector3(0.350224763, 1.17678738, 105.538948),0 ,0 );
            API.createVehicle((VehicleHash)(-89291282), new Vector3(-867.796, 117.007721, 55.5313148), new Vector3(-13.42341, 6.39437962, -161.772949),0 , 0);
            API.createObject(-976225932, new Vector3(-854.500061, 132, -74.62742), new Vector3(-98.9718246, -43.3020668, -116.50267));
            API.createObject(-976225932, new Vector3(-857.1199, 140.183975, 61.5194626), new Vector3(-2.87501287, 11.4063816, -90.57063));
            API.createObject(-976225932, new Vector3(-857.0071, 144.647064, 62.4497337), new Vector3(1.20538306, 13.7505159, -90.28779));
            API.createObject(-976225932, new Vector3(-857.0071, 144.647064, 62.4497337), new Vector3(1.2409364, 13.7474136, -89.99284));
            API.createObject(-976225932, new Vector3(-856.9, 149.2, 63.58), new Vector3(-0.68727237, 13.6960573, -94.37713));
            API.createObject(-976225932, new Vector3(-856.9, 149.2, 63.58), new Vector3(-0.707384944, 13.6950541, -94.54461));
            API.createObject(-976225932, new Vector3(-854.2636, 151.255692, 64.29015), new Vector3(10.6477728, -5.28182459, -2.9654243));
            API.createObject(-450918183, new Vector3(-805.5, 174.81, 82.03), new Vector3(0, 0, -69.9996262));
            API.createObject(-679720048, new Vector3(-774.5981, 182.1754, 71.88822), new Vector3(-0, -0, -80.9999847));
            API.createObject(47332588, new Vector3(-773.4533, 182.243988, 71.86214), new Vector3(-0, -0, -79.9349747));
            API.createObject(47332588, new Vector3(-775.6964, 181.957214, 71.86117), new Vector3(0, -0, 98.17068));
            API.createObject(47332588, new Vector3(-774.9898, 184.033951, 71.85956), new Vector3(-0, -0, -53.9925041));
            API.createObject(-1063472968, new Vector3(-870.8388, 139.714172, 59.68305), new Vector3(0, -0, 95.9837341));
            API.createObject(-741944541, new Vector3(-781.582336, 160.510437, 66.46611), new Vector3(0, 0, 56.5416336));
            API.createObject(-1874162628, new Vector3(-84.0838242, -815.2666, 324.963623), new Vector3(180, -0, -111.456459));
            API.createPed((PedHash)(788443093), new Vector3(-816.6947, 175.910126, 83.3129349), 115.319557f);
            API.createPed((PedHash)(788443093), new Vector3(-812.292664, 167.1625, 81.0439148), 116.680992f);
            API.createPed((PedHash)(788443093), new Vector3(-799.997742, 168.0872, 82.33741), -112.420135f);
            API.createPed((PedHash)(788443093), new Vector3(-794.0702, 184.697266, 81.1983948), -46.67415f);
            API.createPed((PedHash)(788443093), new Vector3(-810.0276, 184.763763, 83.43241), 36.1677322f);
            API.createPed((PedHash)(788443093), new Vector3(-810.9114, 160.468582, 76.42172), 123.330711f);
            API.createPed((PedHash)(788443093), new Vector3(-800.7403, 162.931717, 76.42163), -60.3600731f);
            API.createPed((PedHash)(-1395868234), new Vector3(-848.337, 161.566254, 66.32044), 142.9611f);
            API.createPed((PedHash)(-1395868234), new Vector3(-848.0049, 156.359818, 66.00127), 44.0955467f);
            API.createPed((PedHash)(-1395868234), new Vector3(-819.6326, 179.139648, 72.094574), 127.8305f);
            API.createPed((PedHash)(-1395868234), new Vector3(-818.3742, 175.684555, 72.06653), 99.1623459f);
            API.createPed((PedHash)(-1613485779), new Vector3(-773.2832, 184.622787, 72.83531), 136.446457f);
            API.createPed((PedHash)(-1613485779), new Vector3(-772.607361, 183.436844, 72.83531), 128.54248f);
            API.createPed((PedHash)(-1613485779), new Vector3(-772.127258, 181.976486, 72.83531), 113.805443f);
            API.createPed((PedHash)(-1613485779), new Vector3(-772.0766, 180.4838, 72.83531), 106.007393f);
            API.createPed((PedHash)(351016938), new Vector3(-773.920166, 183.76239, 72.26175), 125.751236f);
            API.createPed((PedHash)(351016938), new Vector3(-773.16626, 182.570313, 72.2616959), 125.220322f);
            API.createPed((PedHash)(351016938), new Vector3(-772.688, 181.351425, 72.261734), 103.233162f);
            API.createPed((PedHash)(-1395868234), new Vector3(-824.922668, 174.815353, 70.83641), 79.6374741f);
            API.createPed((PedHash)(-1395868234), new Vector3(-825.2836, 173.757828, 70.68775), 80.5772f);
            API.createPed((PedHash)(-1395868234), new Vector3(-825.538757, 172.4996, 70.55012), 80.10505f);
            API.createPed((PedHash)(-1395868234), new Vector3(-826.206238, 171.345612, 70.41182), 77.39538f);
            API.createPed((PedHash)(-1395868234), new Vector3(-826.38324, 170.74559, 70.3461), 79.22092f);
            API.createPed((PedHash)(-1395868234), new Vector3(-826.68396, 170.138733, 70.265), 80.94886f);
            API.createPed((PedHash)(-1395868234), new Vector3(-827.001953, 169.341812, 70.1537857), 84.74752f);
            API.createPed((PedHash)(-905948951), new Vector3(-870.5061, 144.362061, 61.8497772), -121.105446f);
            API.createPed((PedHash)(-905948951), new Vector3(-870.422852, 149.876678, 63.1138), -121.443787f);
            API.createPed((PedHash)(-905948951), new Vector3(-868.9279, 204.992218, 73.71008), -126.586624f);
            API.createPed((PedHash)(-905948951), new Vector3(-869.1299, 198.484421, 73.3024), -125.5434f);
            API.createPed((PedHash)(-905948951), new Vector3(-869.1545, 201.289474, 73.5977859), -125.419617f);
            API.createPed((PedHash)(-905948951), new Vector3(-867.637939, 179.115753, 69.45193), -123.723122f);
            API.createPed((PedHash)(-905948951), new Vector3(-867.3633, 182.028641, 70.1333847), -124.53054f);
            API.createPed((PedHash)(-905948951), new Vector3(-870.0928, 130.925812, 58.8851662), -119.884949f);
            API.createPed((PedHash)(-905948951), new Vector3(-870.0467, 132.422577, 59.17178), -120.726921f);
            API.createPed((PedHash)(-1395868234), new Vector3(-813.1815, 167.01004, 72.2248154), 119.218704f);
            API.createPed((PedHash)(-1613485779), new Vector3(-786.8998, 169.022888, 71.36562), 121.957581f);
            API.createPed((PedHash)(-1613485779), new Vector3(-788.737, 163.036926, 71.29151), 75.82806f);
            API.createPed((PedHash)(-1613485779), new Vector3(-790.287231, 155.692566, 70.67525), 103.304214f);
            API.createPed((PedHash)(-1613485779), new Vector3(-794.8934, 145.166092, 69.67442), 168.1584f);
            API.createPed((PedHash)(-1613485779), new Vector3(-789.0035, 151.914017, 68.6749649), -89.92562f);
            API.createPed((PedHash)(-1613485779), new Vector3(-781.432861, 153.950256, 67.474556), -82.51383f);
            API.createPed((PedHash)(-1613485779), new Vector3(-781.7221, 158.532455, 67.474556), -76.0102539f);
            API.createPed((PedHash)(-1613485779), new Vector3(-796.4172, 175.639938, 72.83531), -148.454468f);
            API.createPed((PedHash)(-1613485779), new Vector3(-792.7339, 177.356583, 72.83531), -139.563232f);
            API.createPed((PedHash)(-905948951), new Vector3(-852.648743, 126.835381, 58.0034981), 8.896789f);
            API.createPed((PedHash)(-905948951), new Vector3(-855.959045, 125.935989, 57.8669624), -0.107610919f);
        }
    }
}