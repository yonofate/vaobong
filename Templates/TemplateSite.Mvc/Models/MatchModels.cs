using System.Collections.Generic;

namespace TemplateSite.Mvc.Models
{
    public class GameMatch
    {
        public string Id { get; set; }
        public bool IsPlaying {
            get { return string.IsNullOrEmpty(ElapsedTime) == false; }
        }
        public string ElapsedTime { get; set; }
        public string Date { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public string Score { get; set; }
        public string Url { get; set; }
    }

    public class GameChampion
    {
        public int Id { get; set; }
        public string FlagClass { get; set; }
        public string Champion { get; set; }

        public List<GameMatch> Matches { get; set; }

        public GameChampion()
        {
            Matches = new List<GameMatch>();
        }
    }

    public class SopcastLink
    {
        public string Url { get; set; }
        public string Lang { get; set; }
        public string Bitrate { get; set; }
        public string Channel { get; set; }
        public string LastUpdate { get; set; }
    }

    public class LiveMatch
    {
        public string Id { get; set; }       
        public string DateTime { get; set; }
        public List<LiveServer> Servers { get; set; }
        public string ElapsedTime { get; set; }
        public Team TeamA { get; set; } // home
        public Team TeamB { get; set; } // away
        public string Champion { get; set; }
        public string OriginServer { get; set; }
        public string Title
        {
            get
            {
                return string.Format("{0} vs {1}", TeamA.TeamName, TeamB.TeamName);
            }
        }

        public LiveMatch()
        {
            Servers = new List<LiveServer>();
            TeamA = new Team();
            TeamB = new Team();
        }
    }

    public class LiveServer
    {
        public bool IsVip { get; set; }
        public string Flags { get; set; }
        public string Text { get; set; }
        public string Id { get; set; }
    }

    public class Team
    {
        public string TeamName { get; set; }
        public string TeamFlag { get; set; }
        public string Score { get; set; }
    }
}