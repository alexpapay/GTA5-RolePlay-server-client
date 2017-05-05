using TheGodfatherGM.Data.Extensions;
using GTANetworkServer;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Data;

namespace TheGodfatherGM.Server.Groups
{
    public class GroupController
    {

        /*public string GetRankName(CharacterController Character)
        {
            return GroupData.Members.FirstOrDefault(x => x.CharacterID == Character.CharacterData.CharacterID).DivisionID.ToString();
        }*/

        public Group Group;

        public GroupController() { }

        public GroupController(Group GroupData)
        {
            Group = GroupData;
            EntityManager.Add(this);
        }

        public static void LoadGroups()
        {
            foreach (var group in ContextFactory.Instance.Group)
            {
                new GroupController(group);
            }
            API.shared.consoleOutput("[GM] Загружено групп: " + ContextFactory.Instance.Group.Count() + " шт.");
        }

        public static string GetName(int groupId)
        {
            if (groupId > -1)
            {
                return EntityManager.GetGroups()[groupId].Group.Name;
            }
            return "Неверный ID группы!";
        }

        public string Type()
        {
            return Group.Type.GetDisplayName();
        }

        public string ExtraType()
        {
            return Group.ExtraType.GetDisplayName();
        }

        public static string GetGroupStockName (Character character)
        {
            string propertyName = null;
            if (character.ActiveGroupID > 1300 && character.ActiveGroupID <= 1310) propertyName = "Ballas_stock"; 
            if (character.ActiveGroupID > 1400 && character.ActiveGroupID <= 1410) propertyName = "Vagos_stock";
            if (character.ActiveGroupID > 1500 && character.ActiveGroupID <= 1510) propertyName = "LaCostaNotsa_stock";
            if (character.ActiveGroupID > 1600 && character.ActiveGroupID <= 1610) propertyName = "GroveStreet_stock";
            if (character.ActiveGroupID > 1700 && character.ActiveGroupID <= 1710) propertyName = "TheRifa_stock";
            if (character.ActiveGroupID > 2000 && character.ActiveGroupID <= 2015) propertyName = "Army1_stock";
            if (character.ActiveGroupID > 2100 && character.ActiveGroupID <= 2115) propertyName = "Army2_stock";
            if (character.ActiveGroupID > 100 && character.ActiveGroupID <= 114)   propertyName = "Police_stock";
            if (character.ActiveGroupID > 300 && character.ActiveGroupID <= 310)   propertyName = "FBI_stock";

            return propertyName;
        }

    }
}
