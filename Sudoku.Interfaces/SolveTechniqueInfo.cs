//-----------------------------------------------------------------------
// <copyright file="SolveTechniqueInfo.cs" company="Onnen.de">
//    Onnen.de
// </copyright>
//-----------------------------------------------------------------------
namespace DE.Onnen.Sudoku
{
    /// <summary>
    /// Information about the implemented solution technology
    /// </summary>
    public class SolveTechniqueInfo
    {
        private SolveTechniqueInfo()
        {
        }

        /// <summary>
        /// Short title like a headline.
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Gets a description of the solving technique
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Creates an information instance
        /// </summary>
        /// <param name="caption">Short title like a headline</param>
        /// <param name="descr">description of the solving technique</param>
        /// <returns>Reference of SolveTechniqueInfo</returns>
        public static SolveTechniqueInfo GetTechniqueInfo(string caption, string descr) => new()
        {
            Caption = caption,
            Description = descr
        };
    }
}
