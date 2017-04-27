﻿using GTANetworkServer;
using TheGodfatherGM.Data.Enums;
using TheGodfatherGM.Server.Admin;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.DBManager;

namespace TheGodfatherGM.Server.Jobs
{
    public class Commands : Script
    {
        [Command("createjob", "~y~Usage: ~w~/createjob [type] [Level] [group (optional)]")]
        public void createjob(Client player, JobType type, int Level, string IDOrName = null)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;
            if (!AdminController.AdminRankCheck("createproperty", characterController.Character, player)) return;

            Groups.GroupController groupController = null;
            if (IDOrName != null)
            {
                groupController = EntityManager.GetGroup(player, IDOrName);
                if (groupController == null)
                {
                    player.sendChatMessage("~r~ERROR: ~w~You specified an invalid group.");
                    return;
                }
            }
            Data.Job jobData = new Data.Job();
            JobController jobController = new JobController(jobData);

            jobController.Group = groupController;
            // _JobData.GroupID = (_GroupController == null ? null : _GroupController._GroupData.GroupID);
            if (groupController == null) jobData.GroupId = null;
            else jobData.GroupId = groupController.Group.Id;
            jobData.Type = type;

            jobData.PosX = player.position.X;
            jobData.PosY = player.position.Y;
            jobData.PosZ = player.position.Z;

            jobData.Level = Level;
            ContextFactory.Instance.Job.Add(jobData);
            ContextFactory.Instance.SaveChanges();

            jobController.CreateWorldEntity();
            player.sendChatMessage("~g~Server: ~w~ Added job: " + jobController.Type());

        }

        [Command]
        public void editjob(Client player, int id, JobType type, int Level, string IDOrName = null)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;
            //if (!AdminController.AdminRankCheck("editjob", account)) return;

            JobController job = EntityManager.GetJob(id);
            if (job == null)
            {
                player.sendChatMessage("~r~ERROR: ~w~You specified an invalid job.");
            }
            else
            {
                Groups.GroupController _GroupController = EntityManager.GetGroup(player, IDOrName);
                if (_GroupController == null)
                {
                    player.sendChatMessage("~r~ERROR: ~w~You specified an invalid group.");
                    return;
                }

                job.Group = _GroupController;
                job.JobData.GroupId = _GroupController.Group.Id;
                job.JobData.Type = type;
                job.JobData.Level = Level;
                player.sendChatMessage("You successfully edited job ID " + id);
                ContextFactory.Instance.SaveChanges();
            }
        }

        [Command]
        public void switchjob(Client player, int id)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;
            //if (!AdminController.AdminRankCheck("switchgroup", account)) return;

            JobController job = EntityManager.GetJob(id);
            if (job == null)
            {
                player.sendChatMessage("~r~ERROR: ~w~You specified an invalid job.");
            }
            else
            {
                characterController.job = job;
                API.shared.sendChatMessageToPlayer(player, "You switched your job to: " + job.GetType());
            }
        }

        [Command]
        public void join(Client player)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;
            if (player.isInVehicle) return;

            JobController job;
            if ((job = player.getData("AT_JOB")) != null)
            {
                if (characterController.job == null)
                {
                    characterController.job = job;
                    player.sendChatMessage("~g~Congratulations! ~w~You just became a ~y~" + job.Type() + (job.Group != null ? "~w~ at " + job.Group.Group.Name : "") + ".");
                    //account.CharacterController.Save();
                }
                else if (characterController.job.JobData.Id == job.JobData.Id) player.sendChatMessage("~r~ERROR: ~w~You already have this job.");
                else player.sendChatMessage("~r~ERROR: ~w~You already have a job.");
            }
        }

        [Command]
        public void quitjob(Client player)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;

            if (characterController.job != null)
            {
                player.sendChatMessage("You ~r~quit ~w~being a ~y~" + characterController.job.Type() + "~w~ at " + characterController.job.Group.Group.Name + ".");
                characterController.job = null;
                //account.CharacterController.Save();
            }
            else player.sendChatMessage("~r~ERROR: ~w~You don't have a job.");
        }
    }
}
