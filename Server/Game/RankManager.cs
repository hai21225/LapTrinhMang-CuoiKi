using Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class RankManager
    {
        private readonly PlayerManager _playerManager;
        private List<Rank> _top3= new();

        public RankManager (PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void UpdateRank()
        {
            var players = _playerManager.GetAllPlayer();
            _top3 = players.
                OrderByDescending(p => p.Score).
                Take(3).
                Select(p => new Rank
                {
                    Name = p.Name,
                    Score = p.Score,
                }).
                ToList();

        }
        public List<Rank> GetTop3()
        {
            return _top3;
        }
    }
}
