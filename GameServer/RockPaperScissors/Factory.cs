using GameServer;

namespace RockPaperScissors
{
    public class Factory : IGameFactory
    {
        public int PlayerCount => 2;

        public IGame CreateGame() => new Game();
    }
}
