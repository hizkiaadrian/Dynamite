using System;
using BotInterface.Bot;
using BotInterface.Game;

namespace Dynamite
{
    public class EnemyBot : IBot
    {
        Array values = Enum.GetValues(typeof(Move));
        Random random = new Random();
        public Move MakeMove(Gamestate gamestate)
        {
            return (Move)values.GetValue(random.Next(values.Length));
        }
    }
}