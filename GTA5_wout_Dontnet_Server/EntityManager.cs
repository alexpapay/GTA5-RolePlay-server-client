using System;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using TheGodfatherGM.Data;
using System.Collections.Generic;
using TheGodfatherGM.Server.Jobs;
using TheGodfatherGM.Server.Groups;
using TheGodfatherGM.Server.Property;
using TheGodfatherGM.Server.Vehicles;
using TheGodfatherGM.Server.Characters;
using System.ComponentModel.DataAnnotations;

namespace TheGodfatherGM.Server
{
    public class EntityManager
    {
        private static List<VehicleController> _VehicleControllers = new List<VehicleController>();
        private static List<GroupController> Groups = new List<GroupController>();
        private static List<PropertyController> Properties = new List<PropertyController>();
        private static List<JobController> Jobs = new List<JobController>();

        public static void Init()
        {
            GroupController.LoadGroups();
            VehicleController.LoadVehicles();
            PropertyController.LoadProperties();
            JobController.LoadJobs();            
        }

        private static Dictionary<int, CharacterController> characterDictionary = new Dictionary<int, CharacterController>();

        public static CharacterController GetUserAccount(int id)
        {
            if(id > -1) return characterDictionary.Get(id);
            return null;
        }

        public static CharacterController GetUserAccount(Client player, string IDOrName)
        {
            int id;
            int count = 0;
            
            if (int.TryParse(IDOrName, out id))
            {
                return GetUserAccount(id);
            }

            CharacterController rAccount = null;

            foreach (KeyValuePair<int, CharacterController> account in characterDictionary)
            {
                if (account.Value.Character.Name.ToLower().StartsWith(IDOrName.ToLower()))
                {
                    if ((account.Value.Character.Name.Equals(IDOrName, StringComparison.OrdinalIgnoreCase)))
                    {
                        return account.Value;
                    }
                    rAccount = account.Value;
                    count++;
                }
            }
            if (count == 1) return rAccount;
            else if(count > 1)
            {
                API.shared.sendChatMessageToPlayer(player, "~r~ERROR: ~w~Multiple players found.");
            }
            return null;
        }

        public static void ListUserAccounts(Client player, string IDOrName)
        {
            int count = 0;
            foreach (KeyValuePair<int, CharacterController> userAccount in characterDictionary)
            {
                if (userAccount.Value.Character.Name.ToLower().Contains(IDOrName.ToLower()))
                {
                    API.shared.sendChatMessageToPlayer(player, "" + userAccount.Value.FormatName + " (ID: " + userAccount.Value.Character.Id + ") - (Level: " + userAccount.Value.Character.Level + ") - (Ping: " + API.shared.getPlayerPing(player /*FIX!!!*/) + ")");
                    count++;
                }
            }
            if(count == 0) API.shared.sendChatMessageToPlayer(player, "~r~[ERROR]: ~w~You specified an invalid player ID.");
        }        

        public static ICollection<Data.Vehicle> GetVehicles(Character character)
        {
            return character.Vehicle;
        }

        public static List<VehicleController> GetVehicleControllers(Character character)
        {
            return _VehicleControllers.Where(x => x.VehicleData.Character == character).ToList();
        }

        public static List<VehicleController> GetVehicleControllers()
        {
            return _VehicleControllers;
        }

        public static VehicleController GetVehicle(Data.Vehicle VehicleData)
        {
            return _VehicleControllers.Find(x => x.VehicleData == VehicleData); ;
        }

        public static VehicleController GetVehicle(GTANetworkServer.Vehicle vehicle)
        {
            return _VehicleControllers.Find(x => x.Vehicle == vehicle); ;
        }

        public static VehicleController GetVehicle(NetHandle vehicle)
        {
            return _VehicleControllers.Find(x => x.Vehicle.handle == vehicle); ;
        }

        public static VehicleController GetVehicle(int id)
        {
            return _VehicleControllers.Find(x => x.VehicleData.Id == id); ;
        }

        public static void Add(VehicleController vehicle)
        {
            _VehicleControllers.Add(vehicle);
        }

        public static void Remove(VehicleController vehicle)
        {
            _VehicleControllers.Remove(vehicle);
        }

        public static List<GroupController> GetGroups()
        {
            return Groups;
        }

        public static GroupController GetGroup(int id)
        {
            if (id > -1) return Groups.Find(x => x.Group.Id == id); ;
            return null;
        }

        public static GroupController GetGroup(Client player, string IDOrName)
        {
            int id;
            int count = 0;
            if (int.TryParse(IDOrName, out id))
            {
                return GetGroup(id);
            }

            GroupController rGroup = null;
            foreach (var group in Groups)
            {
                if (group.Group.Name.ToLower().StartsWith(IDOrName.ToLower()))
                {
                    if ((group.Group.Name.Equals(IDOrName, StringComparison.OrdinalIgnoreCase)))
                    {
                        return group;
                    }
                    rGroup = group;
                    count++;
                }
            }
            if (count == 1) return rGroup;
            else if (count > 1)
            {
                API.shared.sendChatMessageToPlayer(player, "~r~ERROR: ~w~Multiple groups found.");
            }
            return null;
        }

        public static void Add(GroupController group)
        {
            Groups.Add(group);
        }

        public static void Remove(GroupController group)
        {
            Groups.Remove(group);
        }

        public static List<PropertyController> GetProperties()
        {
            return Properties;
        }

        public static PropertyController GetProperty(int id)
        {
            return Properties.Find(x => x.PropertyData.PropertyID == id);
        }

        public static PropertyController GetProperty(ColShape shape)
        {
            PropertyController rProperty = Properties.Find(x => x.ExteriorColShape == shape);
            if(rProperty == null) rProperty = Properties.Find(x => x.InteteriorColShape == shape);
            return rProperty;
        }

        public static void Add(PropertyController property)
        {
            Properties.Add(property);
        }

        public static void Remove(PropertyController property)
        {
            Properties.Remove(property);
        }

        public static JobController GetJob(int id)
        {
            return Jobs.Find(x => x.JobData.Id == id);
        }

        public static void Add(JobController job)
        {
            Jobs.Add(job);
        }

        public static void Remove(JobController job)
        {
            Jobs.Remove(job);
        }

        public static int GetNextID()
        {
            // int maxplayers = API.shared.getSetting<int>("maxplayers");
            for (int i = 0; i < 1000; i++) // maybe do 1 cuz duplicate 0 okgui
            {
                if (!characterDictionary.ContainsKey(i)) return i;
            }
            return -1;
        }

        public static string GetDisplayName(Enum value)
        {
            var type = value.GetType();
            if (!type.IsEnum) throw new ArgumentException(String.Format("Type '{0}' is not Enum", type));

            var members = type.GetMember(value.ToString());
            if (members.Length == 0) throw new ArgumentException(String.Format("Member '{0}' not found in type '{1}'", value, type.Name));

            var member = members[0];
            var attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes.Length == 0) throw new ArgumentException(String.Format("'{0}.{1}' doesn't have DisplayAttribute", type.Name, value));

            var attribute = (DisplayAttribute)attributes[0];
            return attribute.GetName();
        }
    }
}
