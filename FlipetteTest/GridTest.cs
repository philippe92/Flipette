namespace FlipetteTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestSmallGrid()
        {
            var data = "[1, \"* 4\", \"* 4\", 1, 1, \"* 2\", \"* 3\", \"M\", 7, \"* 2\", 1, \"* 4\", \"S\", 5, 5, 1, \"* 5\", \"/2\", \"F\", \"M\", \"* 5\", 7, 5, \"/2\", \"M\"]";
            var game = Program.PrepareGame(data, 5, 1);
            game.Run();
            Assert.That(game.BestScore, Is.EqualTo(5445221));
            Assert.That(game.BestPath.Count, Is.EqualTo(21));
        }

        [Test]
        public void TestBigGrid()
        {
            var data = "[\"M\", \"F\", 4, 4, \"* 2\", 4, \"M\", \"* 2\", 4, 4, 1, \"* 4\", \"* 4\", \"/2\", 4, 4, 1, 1, 4, \"* 2\", 4, \"* 5\", \"/2\", 4, \"* 2\", \"/2\", 4, 4, 4, \"* 3\", \"* 4\", \"M\", \"S\", 1, 4, \"* 2\", \"* 2\", 1, \"* 5\", 4, 4, \"* 2\", 1, \"/2\", 1, 4, 1, \"M\", 1]";
            var game = Program.PrepareGame(data, 7, 1);
            game.Run();
            Assert.That(game.BestScore, Is.EqualTo(898824011));
            Assert.That(game.BestPath.Count, Is.EqualTo(40));
        }
    }
}