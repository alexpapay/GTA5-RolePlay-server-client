using GTANetworkServer;
using GTANetworkShared;
using System.Linq;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Data;
using TheGodfatherGM.Data.Extensions;
using TheGodfatherGM.Data.Attributes;
using TheGodfatherGM.Server.Characters;

namespace TheGodfatherGM.Server.Jobs
{
    class JobsPos
    {
        public static Vector3 GetJobPosition(int jobId)
        {
            var coord = new Vector3(-1020.5, -2722.14, 13.8);
            if (jobId == 1)
            {
                return coord;
            }
            return coord;
        }
    }
}
