using System.Collections.ObjectModel;

namespace DE.Onnen.Sudoku.SolveTechniques.KillerSudoku
{
    public enum EDirection
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    public class KillerHouse(int sumValue, int x, int y, int id) : IHasCandidates
    {
        private readonly List<EDirection> _directionPath = [];
        private readonly List<ICell> _cells = [];
        private KillerCell _startKillerCell;
        private IBoard<ICell> _board;
        private readonly int _sumValue = sumValue;
        private readonly int _x = x;
        private readonly int _y = y;

        public ReadOnlyCollection<int> Candidates => throw new NotImplementedException();

        public int CandidateValue => throw new NotImplementedException();

        public EHouseType HType => EHouseType.Box;

        public int ID { get; private set; } = id;

        public void Set(IBoard<ICell> board) => _board = board;

        public KillerCell Start()
        {
            _startKillerCell ??= new KillerCell(this);
            return _startKillerCell;
        }

        public void Clear() => throw new NotImplementedException();

        public bool RemoveCandidate(int candidateToRemove, SudokuLog sudokuLog) => throw new NotImplementedException();
    }

    public class KillerCell
    {
        private readonly Dictionary<EDirection, KillerCell?> _nextCell = new() {
            { EDirection.UP, null} ,
            { EDirection.DOWN, null} ,
            { EDirection.LEFT, null} ,
            { EDirection.RIGHT, null} ,
        };

        private readonly KillerCell _parentCell;
        private ICell _cell;
        private readonly KillerHouse _kbox;

        public KillerCell ParentCell => _parentCell;

        public KillerCell(KillerHouse kbox) => _kbox = kbox;

        public KillerCell(KillerCell parentCell)
        {
            _parentCell = parentCell;
        }

        public void SetCell(ICell cell) => _cell = cell;

        public KillerCell AddDirection(EDirection direction)
        {
            var nextCell = new KillerCell(this);
            _nextCell[direction] = nextCell;
            return nextCell;
        }

        public int Leafs()
        {
            return _nextCell.Values.Where(c => c is not null).Sum(s => s.Leafs()) + _nextCell.Values.Where(c => c is not null).Count();
        }
    }

    public static class KillerCellExtensions
    {
        public static KillerCell Left(this KillerCell kbox) => kbox.AddDirection(EDirection.LEFT);

        public static KillerCell Right(this KillerCell kbox) => kbox.AddDirection(EDirection.RIGHT);

        public static KillerCell Down(this KillerCell kbox) => kbox.AddDirection(EDirection.DOWN);

        public static KillerCell UP(this KillerCell kbox) => kbox.AddDirection(EDirection.UP);
    }
}
