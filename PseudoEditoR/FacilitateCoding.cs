// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   A class which facilitates coding by highlighting syntax or creating auto-stub.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using PseudoEditoR.UserInteraction;
    using PseudoEditoR.WordRecognition;

    /// <summary>
    /// Facilitates, accelerates and simplifies coding for the user.
    /// </summary>
    /// <remarks>
    /// Facilitates coding by highlighting syntax, PseudoSense or auto generating algorithms.
    /// </remarks>
    public static class FacilitateCoding
    {
        /// <summary>
        /// Highlights the <see cref="TextRange"/> of every <see cref="Word"/> in an <see cref="IList{Word}"/> according to the <see cref="WordType"/> and the <see cref="ProgrammingLanguage"/>.
        /// </summary>
        /// <param name="words">
        /// A <see cref="List"/> of <see cref="Word"/>s which hold <see cref="TextRange"/>s to be highlighted.
        /// </param>
        /// <param name="programmingLanguage">
        /// The <see cref="ProgrammingLanguage"/> which holds the unique <see cref="Colors"/> for the <see cref="WordType"/>s.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static async Task HighlightSyntaxAsync(IList<Word> words, ProgrammingLanguage programmingLanguage)
        {
            if (words == null)
            {
                throw new ArgumentNullException("words");
            }

            if (programmingLanguage == null)
            {
                throw new ArgumentNullException("programmingLanguage");
            }

            MainClass.NoTextChangedPlox = true;

            // Parallel version. Faster, but generating hardly controllable InvalidOperationExceptions and FatalExecutionEngineErrors.
            ////Parallel.ForEach(
            ////    words,
            ////    word =>
            ////        {
            ////            TextRange range = new TextRange(word.StartPosition, word.EndPosition);

            ////            range.ApplyPropertyValue(
            ////                TextElement.ForegroundProperty,
            ////                word.Type == WordType.Value
            ////                    ? new SolidColorBrush(programmingLanguage.Colors[word.VariableValueType.ToString()])
            ////                    : new SolidColorBrush(programmingLanguage.Colors[word.Type.ToString()]));
            ////        });

            foreach (var word in words)
            {
                var range = new TextRange(word.StartPosition, word.EndPosition);

                range.ApplyPropertyValue(
                    TextElement.ForegroundProperty,
                    word.Type == WordType.Value
                        ? new SolidColorBrush(programmingLanguage.Colors[word.ValueType.ToString()])
                        : new SolidColorBrush(programmingLanguage.Colors[word.Type.ToString()]));
            }

            MainClass.NoTextChangedPlox = false;
        }

        /// <summary>
        /// Computes a <see cref="List{String}"/> connected with the given <see cref="Word.Content"/> and shows it in the given <see cref="ListBox"/>.
        /// </summary>
        /// <param name="word">
        /// The <see cref="Word"/> on which <see cref="Word.Content"/> a <see cref="List{String}"/> will be generated.
        /// </param>
        /// <param name="allWords">
        /// An <see cref="IEnumerable{Word}"/> containing all <see cref="Word"/>s in the code.
        /// </param>
        /// <param name="programmingLanguage">
        /// A <see cref="ProgrammingLanguage"/> containing the <see cref="string"/>s to be displayed.
        /// </param>
        /// <param name="listBox">
        /// The <see cref="ListBox"/> used to display the computed <see cref="List{String}"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static async Task PseudoSenseAsync(Word word, IEnumerable<Word> allWords, ProgrammingLanguage programmingLanguage, ListBox listBox)
        {
            if (word == null || listBox == null)
            {
                throw new ArgumentNullException();
            }

            if (word.Content == string.Empty || word.Type != WordType.NotDefined)
            {
                listBox.Visibility = Visibility.Hidden;
            }
            else
            {
                var pseudoSenseItems = PseudoSenseProvider(word.Content, allWords, programmingLanguage).ToList();
                if (pseudoSenseItems.Count == 0)
                {
                    listBox.Visibility = Visibility.Hidden;
                }
                else
                {
                    listBox.Items.Clear();
                    Parallel.ForEach(pseudoSenseItems, item => listBox.Items.Add(item));
                    if (listBox.Items.Count > 0)
                    {
                        listBox.SelectedIndex = 0;
                        listBox.Visibility = Visibility.Visible;
                        await Gui.PositionListBoxAsync(MainClass.MainWindow.CodeTextBox, listBox);
                    }
                }
            }
        }

        /// <summary>
        /// Provides an <see cref="IEnumerable{String}"/> for the PseudoSense, which contains <see cref="string"/>s which the user could intent to write.
        /// </summary>
        /// <param name="word">
        /// A <see cref="string"/> which is used to compute the <see cref="IEnumerable{String}"/>.
        /// </param>
        /// <param name="allWords">
        /// An <see cref="IEnumerable{Word}"/> containing all <see cref="Word"/>s in the code.
        /// </param>
        /// <param name="programmingLanguage">
        /// A <see cref="ProgrammingLanguage"/> containing the <see cref="string"/>s to be displayed.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{String}"/> containing <see cref="string"/>s which the user could intent to write.
        /// </returns>
        private static IEnumerable<string> PseudoSenseProvider(string word, IEnumerable<Word> allWords, ProgrammingLanguage programmingLanguage)
        {
            var tempList = new List<string>();
            foreach (var item in allWords.Where(item => item.Type == WordType.Variable && item.Content.StartsWith(word.ToLower()) && !tempList.Contains(item.Content)))
            {
                tempList.Add(item.Content);
            }

            tempList.AddRange(from item in programmingLanguage.Keywords where item.Value.ToLower().StartsWith(word.ToLower()) select item.Value);
            tempList.AddRange(from item in programmingLanguage.ValueTypes where item.Value.ToLower().StartsWith(word.ToLower()) select item.Value);
            tempList.AddRange(from item in programmingLanguage.Commands where item.Value.ToLower().StartsWith(word.ToLower()) select item.Value);
            return tempList;
        }
    }
}
