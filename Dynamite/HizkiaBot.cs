using System;
using System.Collections.Generic;
using System.Linq;
using BotInterface.Bot;
using BotInterface.Game;

namespace Dynamite
{
    public class HizkiaBot : IBot
    {
        #region Fields

        private readonly Dictionary<Move, int> _ordinals = new Dictionary<Move, int>
        {
            {Move.D, 0},
            {Move.W, 1},
            {Move.P, 2},
            {Move.S, 3},
            {Move.R, 4}
        };
        readonly Dictionary<Move, List<Move>> _losesTo = new Dictionary<Move, List<Move>>()
        {
            {Move.D, new List<Move> {Move.W}},
            {Move.W, new List<Move> {Move.R, Move.S, Move.P}},
            {Move.P, new List<Move> {Move.S, Move.D}},
            {Move.S, new List<Move> {Move.D, Move.R}},
            {Move.R, new List<Move> {Move.P, Move.D}},
        };
        int _dynamiteSticksRemaining = 100;
        readonly Random _random = new Random();
        readonly int[,] _markovChain = new int[5, 5];

        #endregion

        #region Methods
        private Move MakeRandomChoice()
        {
            var moveNum = _random.Next(_ordinals.Count);
            var choice1 = _ordinals.Single(x => x.Value == moveNum).Key;
            if (choice1 == Move.D) _dynamiteSticksRemaining--;
            return choice1;
        }
        void UpdateMarkovChain(Move prev, Move next)
        {
            _markovChain[_ordinals[prev], _ordinals[next]]++;
        }
        
        private Move PredictNextMove(Move previousMove)
        {
            int nextIndex = 0;

            for (int i = 0; i < _ordinals.Count; i++)
            {
                int prevIndex = _ordinals[previousMove];

                if (_markovChain[prevIndex, i] > _markovChain[prevIndex, nextIndex])
                {
                    nextIndex = i;
                }
            }

            Move predictedNext = _ordinals.Single(x => x.Value == nextIndex).Key;
            return predictedNext;
        }
        
        private Move FindWinningMove(Move predictedNext)
        {
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
        
        #endregion

        public Move MakeMove(Gamestate gamestate)
        {
            var history = gamestate.GetRounds();

            if (!history.Any())
            {
                return MakeRandomChoice();
            }

            var previousMove = history.Last().GetP2();
            
            if (history.Count() >= 2)
            {
                var twoMovesAgo = history.ElementAt(history.Count() - 2).GetP2();
                UpdateMarkovChain(twoMovesAgo,previousMove);
            }

            var predictedNext = PredictNextMove(previousMove);

            return FindWinningMove(predictedNext);
        }
    }
}