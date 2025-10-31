using Client.Models;
using Client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Service
{
    public class RankService
    {
        private readonly ConnectToServer _client;
        private List<Rank>? _ranks;
        public RankService(ConnectToServer client)
        {
            _client = client;
            _client.OnRankReceived += rank =>
            {
                //Console.WriteLine(rank);
                _ranks = rank;
            };
        }

        public List<Rank>? GetRanks()
        {
            return _ranks;
        }

    }
}
