using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using DE.Onnen.Sudoku.SolveTechniques;

namespace DE.Onnen.Sudoku
{
    /// <summary>
    /// A static class for loading and retrieving solve techniques for Sudoku puzzles.
    /// </summary>
    public static class SudokuSolveTechniqueLoader
    {
        /// <summary>
        /// Gets the solve technique information for the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file containing the solve technique.</param>
        /// <returns>The solve technique information.</returns>
        /// <typeparam name="T">The type of the Sudoku puzzle cell.</typeparam>
        public static SolveTechniqueInfo GetSolveTechnicInfo<T>(string fileName) where T : ICell => LoadSolveTechnic<T>(fileName).Info;

        /// <summary>
        /// Loads the solve technique from the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file containing the solve technique.</param>
        /// <returns>The loaded solve technique.</returns>
        /// <typeparam name="T">The type of the Sudoku puzzle cell.</typeparam>
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        public static ISolveTechnique<T> LoadSolveTechnic<T>(string fileName) where T : ICell
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName), "the parameter filename cannot be null");
            }

            var solvetechnic = Assembly.LoadFrom(fileName) ?? throw new NotImplementedException($"No assembly found in {fileName}");
            var typeName = typeof(ISolveTechnique<T>).ToString();
            var types = solvetechnic.GetTypes();
            var mytype = new List<Type>();
            var typeFound = false;
            foreach (var t in types)
            {
                if (t.BaseType != null && t.BaseType.FullName == typeName)
                {
                    mytype.Add(t);
                    typeFound = true;
                }
            }

            if (!typeFound)
            {
                throw new NotImplementedException($"The type {typeName} is not implemented in file{fileName}");
            }

            var result = new List<ISolveTechnique<T>>();
            foreach (var type in mytype)
            {
                try
                {
                    var obj = Activator.CreateInstance(type);
                    result.Add(item: (ISolveTechnique<T>)obj);
                }
                catch (Exception ex)
                {
                    throw new NotImplementedException($"Could not Create or Add SolveTechnique {type.Name}: ", ex);
                }
            }

            if (result.Count > 0)
            {
                return result[0];
            }
            throw new NotImplementedException($"Could not Create or Add SolveTechnique {fileName}");
        }
    }
}
