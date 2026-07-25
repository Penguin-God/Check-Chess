using NUnit.Framework;
using System.Collections.Generic;

public class SelectionPipelineTests
{
    //private BoardState _mockBoard;
    //private List<ChessSquare> _allowedSquares;

    //[SetUp]
    //public void Setup()
    //{
    //    // Given: 테스트용 초기 보드와 허용 목록 세팅
    //    _mockBoard = new BoardState();
    //    _mockBoard.Grid[1, 1] = new PiecesType("Knight"); // (1,1)에 기물 배치
    //    _mockBoard.Grid[2, 2] = null;                     // (2,2)는 빈 칸

    //    _allowedSquares = new List<ChessSquare> { new ChessSquare(1, 1), new ChessSquare(2, 2) };
    //}

    //// ✅ 테스트 1: 모든 조건이 완벽할 때 (성공 트랙)
    //[Test]
    //public void Select_ValidSquareWithPiece_Returns_PuzzlePlayState()
    //{
    //    // Given
    //    var target = new ChessSquare(1, 1);

    //    // When: 파이프라인 실행 (Bind와 Map 활용)
    //    var result = MoveDomain.ValidateSquare(target, _allowedSquares)
    //                    .Bind(sq => MoveDomain.GetPiece(_mockBoard, sq))
    //                    .Map(piece => MoveDomain.CreateState(piece));

    //    // Then: 결과가 Some이어야 하고, 그 안의 상태가 예상과 같아야 함
    //    Assert.IsTrue(result.IsSome);
    //    Assert.AreEqual("Select", result.Value.Status);
    //    Assert.AreEqual("Knight", result.Value.Piece.Name);
    //}

    //// ❌ 테스트 2: 허용되지 않은 칸을 선택했을 때 (실패 트랙 1 - 조기 종료)
    //[Test]
    //public void Select_InvalidSquare_Returns_None()
    //{
    //    // Given: 허용 목록에 없는 칸 (3, 3)
    //    var target = new ChessSquare(3, 3);

    //    // When
    //    var result = MoveDomain.ValidateSquare(target, _allowedSquares)
    //                    .Bind(sq => MoveDomain.GetPiece(_mockBoard, sq))
    //                    .Map(piece => MoveDomain.CreateState(piece));

    //    // Then: 파이프라인이 즉시 실패 처리되어 None을 반환해야 함
    //    Assert.IsTrue(result.IsNone);
    //}

    //// ❌ 테스트 3: 허용된 칸이지만 기물이 없을 때 (실패 트랙 2 - 중간 종료)
    //[Test]
    //public void Select_ValidSquare_But_Empty_Returns_None()
    //{
    //    // Given: 허용은 되었지만 기물이 없는 칸 (2, 2)
    //    var target = new ChessSquare(2, 2);

    //    // When
    //    var result = MoveDomain.ValidateSquare(target, _allowedSquares)
    //                    .Bind(sq => MoveDomain.GetPiece(_mockBoard, sq))
    //                    .Map(piece => MoveDomain.CreateState(piece));

    //    // Then: GetPiece 단계에서 실패하여 결국 None을 반환해야 함
    //    Assert.IsTrue(result.IsNone);
    //}
}