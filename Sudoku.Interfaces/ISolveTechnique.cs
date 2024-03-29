﻿//-----------------------------------------------------------------------
// <copyright file="ISolveTechnique.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace DE.Onnen.Sudoku.SolveTechniques
{
    /// <summary>
    /// Defines whether this solution technique is performed only once per solution-run (ECellView.GlobalView),
    /// or for each house (ECellView.OnlyHouse).
    /// </summary>
    public enum ECellView
    {
        /// <summary>
        /// The solution technique just need the cells of a House.
        /// </summary>
        OnlyHouse,

        /// <summary>
        /// The solution technique need every Cell in the board.
        /// </summary>
        GlobalView,
    }

    /// <summary>
    /// Solution techniques must implement this interface.
    /// </summary>
    /// <typeparam name="T">Type of Cell</typeparam>
    public interface ISolveTechnique<T> : IEquatable<ISolveTechnique<T>> where T : ICell
    {
        /// <summary>
        /// Gets solveTechniqueInfo that describes the technique.
        /// </summary>
        SolveTechniqueInfo Info { get; }

        /// <summary>
        /// Gets a value indicating whether the Technique is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Activate this solve-technique.
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivate this solve-technique.
        /// </summary>
        void Deactivate();

        /// <summary>
        /// This method is called for every house. Here, the activities are carried out according to the solution technique.
        /// </summary>
        /// <param name="board">The whole board</param>
        /// <param name="house">a specific house (box, row or column)</param>
        /// <param name="sudokuResult">Log-info to return the action that where made during the solve process.</param>
        void SolveHouse(IBoard<T> board, IHouse<T> house, SudokuLog sudokuResult);

        /// <summary>
        /// This method is called until no more changes where made. Here, the activities are carried out according to the solution technique.
        /// </summary>
        /// <param name="board">The whole board</param>
        /// <param name="sudokuResult">Log-info to return the action that where made during the solve process.</param>
        void SolveBoard(IBoard<T> board, SudokuLog sudokuResult);
    }
}
