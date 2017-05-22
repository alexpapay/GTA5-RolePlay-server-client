using TheGodfatherGM.Data.Extensions;
using GTANetworkServer;
using System.Linq;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Data;
using System;

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
            API.shared.consoleOutput("[GM] Параметры капта обнулены.");
            SetDefaultCaption(1);
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
            if (character.ActiveGroupID > 1400 && character.ActiveGroupID <= 1410) propertyName = "Azcas_stock";
            if (character.ActiveGroupID > 1500 && character.ActiveGroupID <= 1510) propertyName = "Vagos_stock";
            if (character.ActiveGroupID > 1600 && character.ActiveGroupID <= 1610) propertyName = "Grove_stock";
            if (character.ActiveGroupID > 1700 && character.ActiveGroupID <= 1710) propertyName = "Rifa_stock";
            if (character.ActiveGroupID > 2000 && character.ActiveGroupID <= 2015) propertyName = "Army1_stock";
            if (character.ActiveGroupID > 2100 && character.ActiveGroupID <= 2115) propertyName = "Army2_stock";
            if (character.ActiveGroupID > 3000 && character.ActiveGroupID <= 3010) propertyName = "RussianMafia_stock";
            if (character.ActiveGroupID > 3100 && character.ActiveGroupID <= 3110) propertyName = "MafiaLKN_stock";
            if (character.ActiveGroupID > 3200 && character.ActiveGroupID <= 3210) propertyName = "MafiaArmeny_stock";
            if (character.ActiveGroupID > 100 && character.ActiveGroupID <= 114)   propertyName = "Police_stock";
            if (character.ActiveGroupID > 300 && character.ActiveGroupID <= 310)   propertyName = "FBI_stock";

            return propertyName;
        }

        public static void SetDefaultCaption (int id)
        {
            var caption = ContextFactory.Instance.Caption.FirstOrDefault(x => x.Id == id);
            caption.Time = DateTime.Now;
            caption.Sector = "0;0";
            caption.GangAttack = 0;
            caption.FragsAttack = 0;
            caption.GangDefend = 0;
            caption.FragsDefend = 0;
            ContextFactory.Instance.SaveChanges();
        }

        public static void SetGangSectorData(int row, int col, int data)
        {
            var selectedSector = ContextFactory.Instance.GangSectors.First(x => x.IdRow == row);
            switch (col)
            {
                case 1:  selectedSector.Col1 = data; break;
                case 2:  selectedSector.Col2 = data; break;
                case 3:  selectedSector.Col3 = data; break;
                case 4:  selectedSector.Col4 = data; break;
                case 5:  selectedSector.Col5 = data; break;
                case 6:  selectedSector.Col6 = data; break;
                case 7:  selectedSector.Col7 = data; break;
                case 8:  selectedSector.Col8 = data; break;
                case 9:  selectedSector.Col9 = data; break;
                case 10: selectedSector.Col10 = data; break;
                case 11: selectedSector.Col11 = data; break;
                case 12: selectedSector.Col12 = data; break;
                case 13: selectedSector.Col13 = data; break;
            }
            ContextFactory.Instance.SaveChanges();
        }
        public static int GetGangSectorData(int row, int col)
        {
            try
            {
                var selectedSector = ContextFactory.Instance.GangSectors.First(x => x.IdRow == row);
                switch (col)
                {
                    case 1: return selectedSector.Col1;
                    case 2: return selectedSector.Col2;
                    case 3: return selectedSector.Col3;
                    case 4: return selectedSector.Col4;
                    case 5: return selectedSector.Col5;
                    case 6: return selectedSector.Col6;
                    case 7: return selectedSector.Col7;
                    case 8: return selectedSector.Col8;
                    case 9: return selectedSector.Col9;
                    case 10: return selectedSector.Col10;
                    case 11: return selectedSector.Col11;
                    case 12: return selectedSector.Col12;
                    case 13: return selectedSector.Col13;
                }
            }
            catch (Exception e) { }
            return 0;
        }
        public static int [] GetCountOfGangsSectors()
        {
            var allSectors = GetGangsSectors();
            var countOfSectors = new int[5] { 0,0,0,0,0 };

            for (var i = 0; i < 169; i++)
            {
                switch (allSectors[i])
                {
                    case 13: countOfSectors[0] += 1; break;
                    case 14: countOfSectors[1] += 1; break;
                    case 15: countOfSectors[2] += 1; break;
                    case 16: countOfSectors[3] += 1; break;
                    case 17: countOfSectors[4] += 1; break;
                    case 130: countOfSectors[0] += 1; break;
                    case 140: countOfSectors[1] += 1; break;
                    case 150: countOfSectors[2] += 1; break;
                    case 160: countOfSectors[3] += 1; break;
                    case 170: countOfSectors[4] += 1; break;
                }
            }
            return countOfSectors;
        }
        public static int [] GetGangsSectors ()
        {
            int[] sectorsArray = new int [169];
            var inc = 0;

            for (var i = 0; i < 13; i++)
            {
                var currentRow = ContextFactory.Instance.GangSectors.First(x => x.IdRow == i+1);

                for (var j = 0; j <13; j++)
                {
                    switch (j)
                    {
                        // maybe foreach???
                        case 0: sectorsArray[j + inc]  = currentRow.Col1; break;
                        case 1: sectorsArray[j + inc]  = currentRow.Col2; break;
                        case 2: sectorsArray[j + inc]  = currentRow.Col3; break;
                        case 3: sectorsArray[j + inc]  = currentRow.Col4; break;
                        case 4: sectorsArray[j + inc]  = currentRow.Col5; break;
                        case 5: sectorsArray[j + inc]  = currentRow.Col6; break;
                        case 6: sectorsArray[j + inc]  = currentRow.Col7; break;
                        case 7: sectorsArray[j + inc]  = currentRow.Col8; break;
                        case 8: sectorsArray[j + inc]  = currentRow.Col9; break;
                        case 9: sectorsArray[j + inc]  = currentRow.Col10; break;
                        case 10: sectorsArray[j + inc] = currentRow.Col11; break;
                        case 11: sectorsArray[j + inc] = currentRow.Col12; break;
                        case 12: sectorsArray[j + inc] = currentRow.Col13; break;
                    }                    
                }
                inc += 13;
            }
            return sectorsArray;
        }
    }
}
