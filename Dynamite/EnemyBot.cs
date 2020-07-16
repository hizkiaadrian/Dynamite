using System;
using System.Collections.Generic;
using System.Linq;
using BotInterface.Bot;
using BotInterface.Game;

namespace Dynamite
{
    public class EnemyBot : IBot
    {
        private List<Move> moves = new List<Move> {Move.P, Move.R, Move.S, Move.W, Move.D};
        Random random = new Random();
        int _dynamiteSticksRemaining = 100;

        public Move MakeMove(Gamestate gamestate)
        {
            var move = moves[random.Next(moves.Count)];
            if (--_dynamiteSticksRemaining >= 0)
            {
                return move;
            }

            while (move == Move.D)
            {
                move = moves[random.Next(moves.Count)];
            }

            return move;
        }
    }
}