using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using DE.Onnen.Sudoku.SolveTechniques;

namespace DE.Onnen.Sudoku
{
    public static class SudokuSolveTechniqueLoader<C> where C : ICell
    {
        public static SolveTechniqueInfo GetSolveTechnicInfo(string fileName)
        {
            var solveTechnic = LoadSolveTechnic(fileName);
            SolveTechniqueInfo info;
            if (solveTechnic is ISolveTechnique<C>)
            {
                info = solveTechnic.Info;
            }
            else
            {
                throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture, "The type {0} is not implemented in file {1}", typeof(ASolveTechnique<C>), fileName));
            }

            return info;
        }

        public static ISolveTechnique<C> LoadSolveTechnic(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName), "the parameter filename cannot be null");
            }

            var solvetechnic = Assembly.LoadFrom(fileName);
            var typeName = typeof(ISolveTechnique<C>).ToString();
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
                throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture, "The type {0} is not implemented in file{1}", typeName, fileName));
            }

            var result = new List<ISolveTechnique<C>>();
            foreach (var type in mytype)
            {
                try
                {
                    var obj = Activator.CreateInstance(type);
                    result.Add((ISolveTechnique<C>)obj);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Could not Create or Add SolveTechnique {type.Name}: ", ex);
                }
            }

            if (result.Count > 0)
            {
                return result[0];
            }

            return null;
        }
    }
}
