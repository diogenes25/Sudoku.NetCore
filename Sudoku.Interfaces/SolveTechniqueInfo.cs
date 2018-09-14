namespace DE.Onnen.Sudoku
{
    public class SolveTechniqueInfo
    {
        private SolveTechniqueInfo()
        {
        }

        public static SolveTechniqueInfo GetTechniqueInfo(string caption, string descr)
        {
            return new SolveTechniqueInfo
            {
                Caption = caption,
                Description = descr
            };
        }

        /// <summary>
        /// Short title like a headline
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Gets a description of the solving technique
        /// </summary>
        public string Description { get; private set; }
    }
}