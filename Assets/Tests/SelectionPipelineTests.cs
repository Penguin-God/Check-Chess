using NUnit.Framework;
using System.Collections.Generic;
using static DataFactory;

public class SelectionPipelineTests
{
    [Test]
    [TestCase()]
    public void 사용_가능한_칸인지_검증(int x, int y)
    {
        var allowedSquares = new List<ChessSquare>() { CreateSquare(1, 1) };

        var result = PuzzleDomain.ValidateSquare(CreateSquare(x, y), allowedSquares);

        // Assert.AreEqual(result, );
    }
}