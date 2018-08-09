using System;
using System.Runtime.CompilerServices;
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
            const string alice = "Alice";
            const string bob = "Bob";
            const string initMessage = "LOBBY";
            const string startMessage = "START";

            var logic = new TestGame
            {
                InitialCommands = new[]
                {
                    new PlayerCommand(alice, startMessage),
                    new PlayerCommand(bob, startMessage),
                }
            };

            var lobby = new Lobby(
                new Config
                {
                    GameLogicFactory = () => logic,
                    LobbyPollInterval = TimeSpan.FromMilliseconds(10),
                    PlayerCount = 2,
                    ServerPort = 0
                });

            var createdGame = (Game)null;
            var source = new CancellationTokenSource();

            // Act
            lobby.OnGameCreated += (sender, game) =>
            {
                createdGame = game;
                _ = game.Start(source.Token);
            };

            lobby.Start(source.Token);

            var port = lobby.Port;

            string aliceInit = null,
                   bobInit = null,
                   aliceStart = null,
                   bobStart = null;

            var aliceTask = Task.Run(
                () =>
                {
                    var client = new TestClient(port);
                    aliceInit = client.Receive();
                    client.Send(alice);
                    aliceStart = client.Receive();
                },
                source.Token);

            var bobTask = Task.Run(
                () =>
                {
                    var client = new TestClient(port);
                    bobInit = client.Receive();
                    client.Send(bob);
                    bobStart = client.Receive();
                },
                source.Token);

            await Task.WhenAll(aliceTask, bobTask);

            // Assert
            Assert.Equal(initMessage, aliceInit);
            Assert.Equal(startMessage, aliceStart);

            Assert.Equal(initMessage, bobInit);
            Assert.Equal(startMessage, bobStart);

            Assert.NotNull(createdGame);
            Assert.Contains(alice, logic.Players);
            Assert.Contains(bob, logic.Players);

            source.Cancel();
        }
    }
}
