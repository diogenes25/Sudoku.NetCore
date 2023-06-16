//-----------------------------------------------------------------------
// <copyright file="IHasCandidates.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku
{
    /// <summary>
    /// Contains candidates like Cell, Box, Row or Col. <see cref="EHouseType"/>
    /// </summary>
    /// <remarks>
    /// A candidate is a possible Digit.
    /// </remarks>
    public interface IHasCandidates
    {
        /// <summary>
        /// Gets a list of every candidate.
        /// </summary>
        /// <remarks>
        ///
        /// At the beginning a cell has 9 candidates (normal Sudoku)
        /// @see BaseValue
        /// </remarks>
        System.Collections.ObjectModel.ReadOnlyCollection<int> Candidates { get; }

        /// <summary>
        /// Gets a bitmask of every candidate
        /// </summary>
        /// <remarks>
        /// A possible solution for an unsolved cell.
        /// Each candidate represents a digit.
        /// Solving a sudoku puzzle is mainly done by elimination of candidates.
        /// When a cell contains a digit, the remaining values are no longer considered candidates for that cell.
        /// In addition, all peers of that cell lose their candidates for that digit, because each house can only contain one instance of each digit.
        /// @see RemoveCandidate(candidate, child)
        /// </remarks>
        int CandidateValue { get; }

        /// <summary>
        /// Gets the type of house (or cell).
        /// </summary>
        EHouseType HType { get; }

        /// <summary>
        /// Gets ID of the Cell/House.
        /// </summary>
        /// <remarks>
        /// There are 81 Cells and 9 horizontal rows, nine vertical columns, and nine 3 x 3 blocks (also called boxes).<br />
        /// The rows are numbered 0 to 8, with the top row being 0, and the bottom row being 8. <br />
        /// The columns are similarly numbered, with the leftmost column being 0, and the rightmost being column 9. <br />
        /// The boxes start counting with 0 on the leftmost corner on top.
        /// </remarks>
        int ID { get; }

        /// <summary>
        /// Reset all candidate to start value.
        /// </summary>
        void Clear();

        /// <summary>
        /// Removing a candidate from the grid, by means of logical deduction.
        /// </summary>
        /// <remarks>
        /// Most advanced solving techniques result in one or more eliminations.
        /// </remarks>
        /// <param name="candidateToRemove">Candidate to be removed</param>
        /// <param name="sudokuLog">Log information that stores all the steps that were necessary to perform the last solving run.</param>
        /// <returns>true = The candidate was successful removed. false = candidate was no in the cell.</returns>
        bool RemoveCandidate(int candidateToRemove, SudokuLog sudokuLog);
    }
}
