using System;
using MTCG.Database;

namespace MTCG.Entities
{
    public class StatsEntity
    {
        [Column(Name="rank")]
        public long Rank { get; set; }
        
        [Column(Name="username")]
        public string Username { get; set; }
        
        [Column(Name="elo")]
        public double Elo { get; set; }
        
        [Column(Name="total")]
        public int Total { get; set; }
        
        [Column(Name="winRate")]
        public double WinRate { get; set; }
        
        [Column(Name="wins")]
        public int Wins { get; set; }
        
        [Column(Name="loses")]
        public int Loses { get; set; }
        
        [Column(Name="draws")]
        public int Draws { get; set; }
    }
}