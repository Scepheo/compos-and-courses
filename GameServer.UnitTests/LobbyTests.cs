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
                LobbyPollInterval = TimeSpan.FromMilliseconds(10),
                PlayerCount = 2,
                ServerPort = port
            });

            var createdGame = (Game)null;
            lobby.OnGameCreated += (sender, game) => createdGame = game;
            var source = new CancellationTokenSource();

            // Act
            lobby.Start(source.Token);
            var alice = Task.Run(
                () =>
                {
                    var client = new TestClient(port);
                    _ = client.Receive(1);
                    client.Send("Alice");
                }, source.Token);
            var bob = Task.Run(
                () =>
                {
                    var client = new TestClient(port);
                    _ = client.Receive(1);
                    client.Send("Bob");
                }, source.Token);
            await Task.WhenAll(alice, bob);
            await Task.Delay(100, source.Token);

            // Assert
            Assert.NotNull(createdGame);
            _ = createdGame.Start(source.Token);
            await Task.Delay(100, source.Token);
            Assert.Contains("Alice", logic.Players);
            Assert.Contains("Bob", logic.Players);

            source.Cancel();
        }
    }
}
