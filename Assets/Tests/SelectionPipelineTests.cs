using NUnit.Framework;
using System.Collections.Generic;

public class SelectionPipelineTests
{
    [Test]
    [TestCase()]
    public void 사용_가능한_칸인지_검증(int x, int y)
    {
        var allowedSquares = new List<ChessSquare>() { DataFactory.CreateSquare(1, 1) };

        var result = PuzzleDomain.ValidateSquare(DataFactory.CreateSquare(x, y), allowedSquares);

        // Assert.AreEqual(result, );
    }
}