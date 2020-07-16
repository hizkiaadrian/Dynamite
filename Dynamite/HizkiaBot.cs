using System;
using System.Collections.Generic;
using System.Linq;
using BotInterface.Bot;
using BotInterface.Game;

namespace Dynamite
{
    public class HizkiaBot : IBot
    {
        Dictionary<Move, int> _ordinals = (Enum.GetValues(typeof(Move)) as Move[])
            .Select((value, index) => new {value, index})
            .ToDictionary(pair => pair.value, pair => pair.index);

        private Dictionary<Move, List<Move>> _losesTo = new Dictionary<Move, List<Move>>()
        {
            {Move.D, new List<Move> {Move.W}},
            {Move.W, new List<Move> {Move.R, Move.S, Move.P}},
            {Move.P, new List<Move> {Move.S, Move.D}},
            {Move.S, new List<Move> {Move.D, Move.R}},
            {Move.R, new List<Move> {Move.P, Move.D}},
        };

        private int _dynamiteSticksRemaining = 100;

        private Random _random = new Random();

        int[,] markovChain = new int[5, 5];

        void UpdateMarkovChain(Move prev, Move next)
        {
            markovChain[_ordinals[prev], _ordinals[next]]++;
        }

        public Move MakeMove(Gamestate gamestate)
        {
            var history = gamestate.GetRounds().Where(x => x != null).SkipLast(1);

            if (!history.Any())
            {
                var moveNum = _random.Next(_ordinals.Count);
                return _ordinals.Single(x => x.Value == moveNum).Key;
            }

            var prev = history.Last().GetP2();
            
            if (history.Count() >= 2)
            {
                var beforePrev = history.ElementAt(history.Count() - 2).GetP2();
                UpdateMarkovChain(beforePrev,prev);
            }

            int nextIndex = 0;
            
            for (int i = 0; i < _ordinals.Count; i++)
            {
                int prevIndex = _ordinals[prev];

                if (markovChain[prevIndex,i] > markovChain[prevIndex,nextIndex])
                {
                    nextIndex = i;
                }
            }
            
            Move predictedNext = _ordinals.Single(x => x.Value == nextIndex).Key;

            List<Move> losesTo = _losesTo[predictedNext];

            var choice = losesTo[_random.Next(losesTo.Count)];

            if (choice == Move.D)
            {
                if (--_dynamiteSticksRemaining < 0)
                {
                    choice = losesTo.Single(x => x != Move.D);
                }
            }

            return choice;
        }
    }
}