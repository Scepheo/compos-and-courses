using GameServer;
using System.Text.RegularExpressions;
using Xunit;

namespace RockPaperScissors.UnitTests
{
    public class RockPaperScissorsTests
    {
        private static readonly IGameFactory _factory = new Factory();

        private readonly IGame _game = _factory.CreateGame();

        private const string Alice = "Alice";
        private const string Bob = "Bob";
        private static readonly string[] _names = { Alice, Bob };

        private const string Rock = "ROCK";
        private const string Paper = "PAPER";
        private const string Scissors = "SCISSORS";

        [Theory]
        [InlineData(Rock, Scissors)]
        [InlineData(Paper, Rock)]
        [InlineData(Scissors, Paper)]
        public void Wins(string winningChoice, string losingChoice)
        {
            // Arrange
            _game.Initialize(_names);

            // Act
            var aliceResponse = new PlayerResponse(Alice, winningChoice);
            var bobResponse = new PlayerResponse(Bob, losingChoice);
            var responses = new[] { aliceResponse, bobResponse };
            var updateCommands = _game.Update(responses);

            // Assert
            AssertIsEnd(updateCommands);
            var endCommands = _game.Complete();
            AssertIsWinner(endCommands, Alice);
        }

        [Theory]
        [InlineData(Rock)]
        [InlineData(Paper)]
        [InlineData(Scissors)]
        public void Ties(string choice)
        {
            // Arrange
            _game.Initialize(_names);

            // Act
            var aliceResponse = new PlayerResponse(Alice, choice);
            var bobResponse = new PlayerResponse(Bob, choice);
            var responses = new[] { aliceResponse, bobResponse };
            var updateCommands = _game.Update(responses);

            // Assert
            AssertIsAgain(updateCommands);
        }

        [Fact]
        public void InvalidChoiceLosesToValidChoice()
        {
            // Arrange
            _game.Initialize(_names);

            // Act
            var aliceResponse = new PlayerResponse(Alice, "nonsense");
            var bobResponse = new PlayerResponse(Bob, Rock);
            var responses = new[] { aliceResponse, bobResponse };
            var updateCommands = _game.Update(responses);

            // Assert
            AssertIsEnd(updateCommands);
            var endCommands = _game.Complete();
            AssertIsWinner(endCommands, Bob);
        }

        [Fact]
        public void InvalidChoicesTie()
        {
            // Arrange
            _game.Initialize(_names);

            // Act
            var aliceResponse = new PlayerResponse(Alice, "nonsense");
            var bobResponse = new PlayerResponse(Bob, "also nonsense");
            var responses = new[] { aliceResponse, bobResponse };
            var updateCommands = _game.Update(responses);

            // Assert
            AssertIsAgain(updateCommands);
        }

        [Fact]
        public void CanWinAfterTie()
        {
            // Arrange
            _game.Initialize(_names);

            // Act
            var aliceResponse1 = new PlayerResponse(Alice, Rock);
            var bobResponse1 = new PlayerResponse(Bob, Rock);
            var responses1 = new[] { aliceResponse1, bobResponse1 };
            var updateCommands1 = _game.Update(responses1);
            var aliceResponse2 = new PlayerResponse(Alice, Paper);
            var bobResponse2 = new PlayerResponse(Bob, Scissors);
            var responses2 = new[] { aliceResponse2, bobResponse2 };
            var updateCommands2 = _game.Update(responses2);

            // Assert
            AssertIsAgain(updateCommands1);
            AssertIsEnd(updateCommands2);
            var endCommands = _game.Complete();
            AssertIsWinner(endCommands, Bob);
        }

        [Fact]
        public void SinglePlayerDisconnectedLosesGame()
        {

            // Arrange
            _game.Initialize(_names);

            // Act
            _game.PlayerDisconnected(Alice);
            var bobResponse = new PlayerResponse(Bob, Rock);
            var responses = new[] { bobResponse };
            var updateCommands = _game.Update(responses);

            // Assert
            AssertIsEnd(updateCommands);
            var endCommands = _game.Complete();
            AssertIsWinner(endCommands, Bob);
        }

        [Fact]
        public void BothPlayersDisconnectedTiesGame()
        {

            // Arrange
            _game.Initialize(_names);

            // Act
            _game.PlayerDisconnected(Alice);
            _game.PlayerDisconnected(Bob);
            var responses = new PlayerResponse[0];
            var updateCommands = _game.Update(responses);

            // Assert
            AssertIsEnd(updateCommands);
            var endCommands = _game.Complete();
            AssertIsTie(endCommands);
        }

        private static GlobalCommand AssertIsGlobalCommand(ICommand[] commands)
        {
            var command = Assert.Single(commands);
            var globalCommand = Assert.IsType<GlobalCommand>(command);
            return globalCommand;
        }

        private static void AssertIsEnd(ICommand[] commands)
        {
            var globalCommand = AssertIsGlobalCommand(commands);
            Assert.Equal("END", globalCommand.Command);
        }

        private static void AssertIsAgain(ICommand[] commands)
        {
            var globalCommand = AssertIsGlobalCommand(commands);
            Assert.Equal("AGAIN", globalCommand.Command);
        }

        private static readonly Regex _winnerRegex
            = new Regex("^WINNER: (?<winner>.*)$");

        private static void AssertIsWinner(ICommand[] commands, string winner)
        {
            var globalCommand = AssertIsGlobalCommand(commands);
            var match = _winnerRegex.Match(globalCommand.Command);
            Assert.True(match.Success);
            Assert.Equal(match.Groups["winner"].Value, winner);
        }

        private static void AssertIsTie(ICommand[] commands)
        {
            var globalCommand = AssertIsGlobalCommand(commands);
            Assert.Equal("TIE", globalCommand.Command);
        }
    }
}
