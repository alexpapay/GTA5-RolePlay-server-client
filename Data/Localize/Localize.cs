namespace TheGodfatherGM.Data.Localize
{
    class Localize
    {
        public static string Lang (int language, string word)
        {
            if (word == "you_sold_weapon")
                switch (language)
                {
                    case 1: return "~w~You sold weapon: ~g~";
                    case 2: return "~r~Вы продали оружие: ~g~";                    
                }
            if (word == "you_bought_weapon")
                switch (language)
                {
                    case 1: return "~w~You bought weapon: ~g~";
                    case 2: return "~w~Вы купили оружие: ~g~";                    
                }
            if (word == "death")
                switch (language)
                {
                    case 1: return " ~r~has died";
                    case 2: return " ~r~умер...";                    
                }
            if (word == "killed")
                switch (language)
                {
                    case 1: return " ~r~kill~w~ ";
                    case 2: return " ~r~убил~w~ ";                    
                }
            if (word == "kill_cloth_soldier")
                switch (language)
                {
                    case 1: return "~w~You are get a ~g~Soldier clothes~w~.";
                    case 2: return "~w~Вы забрали форму ~g~солдата~w~.";                    
                }
            if (word == "kill_cloth_officer")
                switch (language)
                {
                    case 1: return "~w~You are get a ~g~Officer clothes~w~.";
                    case 2: return "~w~Вы забрали форму ~g~офицера~w~.";                    
                }
            if (word == "kill_cloth_general")
                switch (language)
                {
                    case 1: return "~w~You are get a ~g~General clothes~w~.";
                    case 2: return "~w~Вы забрали форму ~g~генерала~w~.";                    
                }
            if (word == "kick")
                switch (language)
                {
                    case 1: return "~r~You are banned on this server!";
                    case 2: return "~r~Вы забанены на данном сервере!";                 
                }

            if (word == "buy_weapon")
                switch (language)
                {
                case 1: return ""; 
                case 2: return "";
                }

            return "";
        }
    }
}
