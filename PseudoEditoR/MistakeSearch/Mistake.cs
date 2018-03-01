// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Represents a mistake made by the user.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR.MistakeSearch
{
    using System;
    using System.Windows.Documents;
    using PseudoEditoR.WordRecognition;

    /// <summary>
    /// Represents a mistake in the code.
    /// </summary>
    /// <remarks>
    /// A mistake defined by its unique members.
    /// </remarks>
    public class Mistake
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mistake"/> class.
        /// </summary>
        /// <param name="standardSequence">
        /// The place of the <see cref="Mistake"/> in <see cref="MistakeEngine.Mistakes"/>.
        /// </param>
        /// <param name="category">
        /// The <see cref="MistakeType"/> representing the type of the <see cref="Mistake"/>.
        /// </param>
        /// <param name="word">
        /// The <see cref="Word"/> associated with the <see cref="Mistake"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The parameter was passed as null.
        /// </exception>
        public Mistake(int standardSequence, MistakeType category, Word word)
        {
            if (word == null)
            {
                throw new ArgumentNullException("word");
            }

            this.StandardSequence = standardSequence;
            var lineAndColumn = GetLineAndColumn(word);
            this.Line = lineAndColumn[0];
            this.Column = lineAndColumn[1];
            this.Type = category;
            this.Description = GetDescription(word);
            this.Word = word;
        }

        /// <summary>
        /// Gets a number, representing the place of the <see cref="Mistake"/> in <see cref="MistakeEngine.Mistakes"/>.
        /// </summary>
        public int StandardSequence { get; private set; }

        /// <summary>
        /// Gets a number representing the line in the code, where the <see cref="Mistake.Word"/> of the <see cref="Mistake"/> is located.
        /// </summary>
        public int Line { get; private set; }

        /// <summary>
        /// Gets a number representing the column in the code, where the <see cref="Mistake.Word"/> of the <see cref="Mistake"/> is located.
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the type of the <see cref="Mistake"/> as <see cref="MistakeType"/>.
        /// </summary>
        /// <seealso cref="MistakeType"/>
        public MistakeType Type { get; private set; }

        /// <summary>
        /// Gets the localized description of the <see cref="Mistake"/>.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the <see cref="Word"/> representing the <see cref="Mistake"/> in the code.
        /// </summary>
        /// <seealso cref="Word"/>
        public Word Word { get; private set; }

        /// <summary>
        /// Computes the line and column a <see cref="Word"/> is located in the code.
        /// </summary>
        /// <param name="word">A <see cref="Word"/> which exists in the code.</param>
        /// <returns>An array, which contains the line at the index 0 and the column at the index 1.</returns>
        private static int[] GetLineAndColumn(Word word)
        {
            int[] lineAndColumn = { 1, 1 };
            int lineNumber;
            word.StartPosition.GetLineStartPosition(-int.MaxValue, out lineNumber);
            TextPointer lineStartPosition = word.StartPosition.GetLineStartPosition(0);
            if (lineStartPosition != null)
            {
                int columnNumber = lineStartPosition.GetOffsetToPosition(word.StartPosition);
                if (lineNumber == 0)
                {
                    columnNumber--;
                }

                lineAndColumn[0] = -lineNumber + 1;
                lineAndColumn[1] = columnNumber + 1;
            }

            return lineAndColumn;
        }

        /// <summary>
        /// Retrieves, builds and localizes a description for the <see cref="Mistake"/>.
        /// </summary>
        /// <param name="word">A <see cref="Word"/> which exists in the code.</param>
        /// <returns>The localized description as <see cref="string"/>.</returns>
        private static string GetDescription(Word word)
        {
            return string.Format(MainClass.CurrentProgrammingLanguage.MistakeDescriptions["noDefinitionDesc"], MainClass.CurrentProgrammingLanguage.Name, word.Content);
        }
    }
}
