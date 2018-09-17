using DE.Onnen.Sudoku.SolveTechniques;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace DE.Onnen.Sudoku
{
    public static class SudokuSolveTechniqueLoader<C> where C : ICell
    {
        public static SolveTechniqueInfo GetSolveTechnicInfo(string fileName)
        {
            ISolveTechnique<C> solveTechnic = LoadSolveTechnic(fileName);
            SolveTechniqueInfo info;
            if (solveTechnic is ISolveTechnique<C>)
            {
                info = ((ISolveTechnique<C>)solveTechnic).Info;
            }
            else
            {
                throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture, "The type {0} is not implemented in file {1}", typeof(ASolveTechnique<C>), fileName));
            }

            return info;
        }

        public static ISolveTechnique<C> LoadSolveTechnic(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName), "the parameter filename cannot be null");
            }

            Assembly solvetechnic = Assembly.LoadFrom(fileName);
            string typeName = typeof(ISolveTechnique<C>).ToString();
            Type[] types = solvetechnic.GetTypes();
            List<Type> mytype = new List<Type>();
            bool typeFound = false;
            foreach (Type t in types)
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

            List<ISolveTechnique<C>> result = new List<ISolveTechnique<C>>();
            foreach (Type type in mytype)
            {
                try
                {
                    object obj = Activator.CreateInstance(type);
                    result.Add((ISolveTechnique<C>)obj);
                }
                catch
                {
                    continue;
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