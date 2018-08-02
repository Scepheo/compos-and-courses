using System;
using System.Threading;
using System.Threading.Tasks;
using GameServer.UnitTests.Util;
using Xunit;

namespace GameServer.UnitTests
{
    public class LobbyTests
    {
        [Fact]
        public async Task EnoughPlayers_GameStarted()
        {
            // Arrange
            const int port = 12345;

            var logic = new TestGame();

            var lobby = new Lobby(new Config
            {
                GameLogicFactory = () => logic,
                LobbyPollInterval = TimeSpan.FromMilliseconds(100),
                PlayerCount = 2,
                ServerPort = port
            });

            var startedGame = default(Game);
            lobby.OnGameCreated += (sender, game) => startedGame = game;

            // Act
            _ = lobby.Start(CancellationToken.None);
            var client1 = new TestClient(port);
            var client2 = new TestClient(port);
            await Task.Delay(500);

            // Assert
            Assert.NotNull(startedGame);
        }
    }
}
