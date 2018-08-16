using System;
using System.Threading;
using System.Threading.Tasks;
using GameServer.UnitTests.Util;
using Xunit;

namespace GameServer.UnitTests
{
    public class RockPaperScissorsTests : IDisposable
    {
        private const string Alice = "Alice";
        private const string Bob = "Bob";
        private const string Charlie = "Charlie";

        private const string Rock = "ROCK";
        private const string Paper = "PAPER";
        private const string Scissors = "SCISSORS";

        private readonly Lobby _lobby;
        private Game _game;

        private readonly CancellationTokenSource _tokenSource =
            new CancellationTokenSource();

        public RockPaperScissorsTests()
        {
            _lobby = StartLobby();
        }

        private static Config GetConfig() => new Config
        {
            GameLogicFactory = () => new RockPaperScissors(),
            LobbyPollInterval = TimeSpan.FromMilliseconds(10),
            PlayerCount = 2,
            ServerPort = 0
        };

        private Lobby StartLobby()
        {
            var lobby = new Lobby(GetConfig());
            lobby.OnGameCreated +=
                (sender, game) =>
                {
                    _game = game;
                    _game.Start();
                };
            lobby.Start();
            return lobby;
        }

        [Fact]
        public async Task RockBeatsScissors()
        {
            // Arrange
            var lobby = StartLobby();
            var port = await lobby.Port;
            const string expectedResult = "WINNER: " + Alice;

            // Act
            string aliceResult = null, bobResult = null;
            var alice = Task.Run(
                () => aliceResult = Play(Alice, port, Rock, Scissors),
                _tokenSource.Token);
            var bob = Task.Run(
                () => bobResult = Play(Bob, port, Rock, Paper),
                _tokenSource.Token);

            await Task.WhenAll(alice, bob);

            // Assert
            Assert.Equal(expectedResult, aliceResult);
            Assert.Equal(expectedResult, bobResult);
        }

        [Fact]
        public async Task Tie_PlayAgain()
        {
            // Arrange
            var lobby = StartLobby();
            var port = await lobby.Port;
            const string expectedResult = "WINNER: " + Alice;

            // Act
            string aliceResult = null, bobResult = null;
            var alice = Task.Run(
                () => aliceResult = Play(Alice, port, Rock, Scissors),
                _tokenSource.Token);
            var bob = Task.Run(
                () => bobResult = Play(Bob, port, Rock, Paper),
                _tokenSource.Token);
            await Task.WhenAll(alice, bob);

            // Assert
            Assert.Equal(expectedResult, aliceResult);
            Assert.Equal(expectedResult, bobResult);
        }

        [Fact]
        public async Task Disconnect_WaitForNewPlayer()
        {
            // Arrange
            var lobby = StartLobby();
            var port = await lobby.Port;
            const string expectedResult = "WINNER: " + RockPaperScissorsTests.Bob;
            var gate = new TaskCompletionSource<bool>();

            // Act
            string bobResult = null, charlieResult = null;

            var alice = Task.Run(
                () =>
                {
                    using (var client = new TestClient(port))
                    {
                        _ = client.Receive();
                        client.Send(Alice);
                    }

                    gate.SetResult(true);
                });

            var bob = Task.Run(
                async () =>
                {
                    await gate.Task;
                    bobResult = Play(Bob, port, Paper);
                },
                _tokenSource.Token);

            var charlie = Task.Run(
                async () =>
                {
                    await gate.Task;
                    charlieResult = Play(Charlie, port, Rock);
                },
                _tokenSource.Token);

            await Task.WhenAll(alice, bob, charlie);

            // Assert
            Assert.Equal(expectedResult, bobResult);
            Assert.Equal(expectedResult, charlieResult);
        }

        private static string Play(string name, int port, params string[] plays)
        {
            using (var client = new TestClient(port))
            {
                var lobby = client.Receive();
                Assert.Equal("LOBBY", lobby);

                client.Send(name);

                var start = client.Receive();
                Assert.Equal("START", start);

                for (var i = 0; i < plays.Length; i++)
                {
                    client.Send(plays[i]);
                    var response = client.Receive();
                    var expected = i < plays.Length - 1 ? "AGAIN" : "END";
                    Assert.Equal(expected, response);
                }

                var result = client.Receive();
                return result;
            }
        }

        public void Dispose()
        {
            _lobby.Stop();
            _game?.Stop();
            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }
    }
}
