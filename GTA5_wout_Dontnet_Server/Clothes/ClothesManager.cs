using GTANetworkServer;
using TheGodfatherGM.Server.DBManager;
using System.Linq;
using TheGodfatherGM.Data;

namespace TheGodfatherGM.Server
{
    class ClothesManager : Script
    {
        public static void SetPlayerSkinClothesToDb(Client player, int type, Character character, int check)
        {
            if (character == null) return;

            var playerClothes = ContextFactory.Instance.Clothes.FirstOrDefault(x => x.CharacterId == character.Id);
            var typeClothes = ContextFactory.Instance.ClothesTypes.FirstOrDefault(x => x.Type == type);

            playerClothes.MaskSlot = typeClothes.MaskSlot; playerClothes.MaskDraw = typeClothes.MaskDraw;
            playerClothes.TorsoSlot = typeClothes.TorsoSlot; playerClothes.TorsoDraw = typeClothes.TorsoDraw;
            playerClothes.LegsSlot = typeClothes.LegsSlot; playerClothes.LegsDraw = typeClothes.LegsDraw;
            playerClothes.BagsSlot = typeClothes.BagsSlot; playerClothes.BagsDraw = typeClothes.BagsDraw;
            playerClothes.FeetSlot = typeClothes.FeetSlot; playerClothes.FeetDraw = typeClothes.FeetDraw;
            playerClothes.AccessSlot = typeClothes.AccessSlot; playerClothes.AccessDraw = typeClothes.AccessDraw;
            playerClothes.UndershirtSlot = typeClothes.UndershirtSlot; playerClothes.UndershirtDraw = typeClothes.UndershirtDraw;
            playerClothes.ArmorSlot = typeClothes.ArmorSlot; playerClothes.ArmorDraw = typeClothes.ArmorDraw;
            playerClothes.TopsSlot = typeClothes.TopsSlot; playerClothes.TopsDraw = typeClothes.TopsDraw;
            playerClothes.HatsSlot = typeClothes.HatsSlot; playerClothes.HatsDraw = typeClothes.HatsDraw;
            playerClothes.GlassesSlot = typeClothes.GlassesSlot; playerClothes.GlassesDraw = typeClothes.GlassesDraw;

            ContextFactory.Instance.SaveChanges();
            SetPlayerSkinClothes(player, type, character, check);
        }
        public static void SetPlayerSkinClothes(Client player, int type, Character character, int check)
        {
            Clothes playerClothes = null;
            ClothesTypes playerTypeClothes = null;

            if (type == 0)
                playerClothes = ContextFactory.Instance.Clothes.FirstOrDefault(x => x.CharacterId == character.Id);
            else
                playerTypeClothes = ContextFactory.Instance.ClothesTypes.FirstOrDefault(x => x.Type == type);

            // First registration of character clothes
            if (check == 0)
            {
                playerClothes = new Clothes();
                var typeClothes = ContextFactory.Instance.ClothesTypes.FirstOrDefault(x => x.Type == 101);
                playerClothes.CharacterId = character.Id;
                playerClothes.MaskSlot = typeClothes.MaskSlot;
                playerClothes.MaskDraw = typeClothes.MaskDraw;
                playerClothes.TorsoSlot = typeClothes.TorsoSlot;
                playerClothes.TorsoDraw = typeClothes.TorsoDraw;
                playerClothes.LegsSlot = typeClothes.LegsSlot;
                playerClothes.LegsDraw = typeClothes.LegsDraw;
                playerClothes.BagsSlot = typeClothes.BagsSlot;
                playerClothes.BagsDraw = typeClothes.BagsDraw;
                playerClothes.FeetSlot = typeClothes.FeetSlot;
                playerClothes.FeetDraw = typeClothes.FeetDraw;
                playerClothes.AccessSlot = typeClothes.AccessSlot;
                playerClothes.AccessDraw = typeClothes.AccessDraw;
                playerClothes.UndershirtSlot = typeClothes.UndershirtSlot;
                playerClothes.UndershirtDraw = typeClothes.UndershirtDraw;
                playerClothes.ArmorSlot = typeClothes.ArmorSlot;
                playerClothes.ArmorDraw = typeClothes.ArmorDraw;
                playerClothes.TopsSlot = typeClothes.TopsSlot;
                playerClothes.TopsDraw = typeClothes.TopsDraw;
                playerClothes.HatsSlot = typeClothes.HatsSlot;
                playerClothes.HatsDraw = typeClothes.HatsDraw;
                playerClothes.GlassesSlot = typeClothes.GlassesSlot;
                playerClothes.GlassesDraw = typeClothes.GlassesDraw;

                ContextFactory.Instance.Clothes.Add(playerClothes);
                ContextFactory.Instance.SaveChanges();

                //API.shared.setPlayerSkin(player, (PedHash)character.Model); // TODO: delete after face custom
                API.shared.setPlayerClothes(player, 0, 0, 0);
                API.shared.setPlayerClothes(player, 1, playerClothes.MaskSlot, playerClothes.MaskDraw);
                API.shared.setPlayerClothes(player, 3, playerClothes.TorsoSlot, playerClothes.TorsoDraw);
                API.shared.setPlayerClothes(player, 4, playerClothes.LegsSlot, playerClothes.LegsDraw);
                API.shared.setPlayerClothes(player, 5, playerClothes.BagsSlot, playerClothes.BagsDraw);
                API.shared.setPlayerClothes(player, 6, playerClothes.FeetSlot, playerClothes.FeetDraw);
                API.shared.setPlayerClothes(player, 7, playerClothes.AccessSlot, playerClothes.AccessDraw);
                API.shared.setPlayerClothes(player, 8, playerClothes.UndershirtSlot, playerClothes.UndershirtDraw);
                API.shared.setPlayerClothes(player, 9, playerClothes.ArmorSlot, playerClothes.ArmorDraw);
                API.shared.setPlayerClothes(player, 11, playerClothes.TopsSlot, playerClothes.TopsDraw);
                API.shared.setPlayerAccessory(player, 0, playerClothes.HatsSlot, playerClothes.HatsDraw);
                API.shared.setPlayerAccessory(player, 1, playerClothes.GlassesSlot, playerClothes.GlassesDraw);
            }

            if (check == 1 && type == 0)
            {
                //API.shared.setPlayerSkin(player, (PedHash)character.Model); // TODO: delete after face custom
                API.shared.setPlayerClothes(player, 1, playerClothes.MaskSlot, playerClothes.MaskDraw);
                API.shared.setPlayerClothes(player, 3, playerClothes.TorsoSlot, playerClothes.TorsoDraw);
                API.shared.setPlayerClothes(player, 4, playerClothes.LegsSlot, playerClothes.LegsDraw);
                API.shared.setPlayerClothes(player, 5, playerClothes.BagsSlot, playerClothes.BagsDraw);
                API.shared.setPlayerClothes(player, 6, playerClothes.FeetSlot, playerClothes.FeetDraw);
                API.shared.setPlayerClothes(player, 7, playerClothes.AccessSlot, playerClothes.AccessDraw);
                API.shared.setPlayerClothes(player, 8, playerClothes.UndershirtSlot, playerClothes.UndershirtDraw);
                API.shared.setPlayerClothes(player, 9, playerClothes.ArmorSlot, playerClothes.ArmorDraw);
                API.shared.setPlayerClothes(player, 11, playerClothes.TopsSlot, playerClothes.TopsDraw);
                API.shared.setPlayerAccessory(player, 0, playerClothes.HatsSlot, playerClothes.HatsDraw);
                API.shared.setPlayerAccessory(player, 1, playerClothes.GlassesSlot, playerClothes.GlassesDraw);
            }
            if (check == 1 && type != 0)
            {
                //API.shared.setPlayerSkin(player, (PedHash)character.Model); // TODO: delete after face custom
                API.shared.setPlayerClothes(player, 1, playerTypeClothes.MaskSlot, playerTypeClothes.MaskDraw);
                API.shared.setPlayerClothes(player, 3, playerTypeClothes.TorsoSlot, playerTypeClothes.TorsoDraw);
                API.shared.setPlayerClothes(player, 4, playerTypeClothes.LegsSlot, playerTypeClothes.LegsDraw);
                API.shared.setPlayerClothes(player, 5, playerTypeClothes.BagsSlot, playerTypeClothes.BagsDraw);
                API.shared.setPlayerClothes(player, 6, playerTypeClothes.FeetSlot, playerTypeClothes.FeetDraw);
                API.shared.setPlayerClothes(player, 7, playerTypeClothes.AccessSlot, playerTypeClothes.AccessDraw);
                API.shared.setPlayerClothes(player, 8, playerTypeClothes.UndershirtSlot, playerTypeClothes.UndershirtDraw);
                API.shared.setPlayerClothes(player, 9, playerTypeClothes.ArmorSlot, playerTypeClothes.ArmorDraw);
                API.shared.setPlayerClothes(player, 11, playerTypeClothes.TopsSlot, playerTypeClothes.TopsDraw);
                API.shared.setPlayerAccessory(player, 0, playerTypeClothes.HatsSlot, playerTypeClothes.HatsDraw);
                API.shared.setPlayerAccessory(player, 1, playerTypeClothes.GlassesSlot, playerTypeClothes.GlassesDraw);
            }
        }
        // TODO: add another clothes for different gangs!
        public static int SetFractionClothes(Client player, int fractionId, Character character)
        {
            switch (fractionId)
            {
                case 2: SetPlayerSkinClothesToDb(player, 2, character, 1); return 2;
                case 1301: SetPlayerSkinClothesToDb(player, 131, character, 1); return 131;
                case 1401: SetPlayerSkinClothesToDb(player, 141, character, 1); return 141;
                case 1501: SetPlayerSkinClothesToDb(player, 151, character, 1); return 151;
                case 1601: SetPlayerSkinClothesToDb(player, 161, character, 1); return 161;
                case 1701: SetPlayerSkinClothesToDb(player, 171, character, 1); return 171;
            }
            return 101; // Default clothes
        }
    }
}
