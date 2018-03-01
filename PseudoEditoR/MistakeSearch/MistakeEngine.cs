// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Handles everything with <see cref="Mistake"/>s in the code.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR.MistakeSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using PseudoEditoR.WordRecognition;

    /// <summary>
    /// Handles everything with <see cref="Mistake"/>s in the code.
    /// </summary>
    ///  <remarks>
    /// Responsible for finding, holding, sorting and displaying <see cref="Mistakes"/> in the code.
    /// </remarks>
    public static class MistakeEngine
    {
        /// <summary>
        /// Gets or sets a <see cref="List{TextRange}"/> containing <see cref="Mistake.Word"/>s which <see cref="Inline.TextDecorationsProperty"/> should be cleared (set to null).
        /// </summary>
        private static List<TextRange> toClear;

        /// <summary>
        /// Gets or sets a <see cref="List{TextRange}"/> of <see cref="Mistake.Word"/>s which <see cref="Inline.TextDecorationsProperty"/> should be filled with a <see cref="Mistake"/>-decoration.
        /// </summary>
        private static List<TextRange> toUnderline;

        /// <summary>
        /// Initializes static members of the <see cref="MistakeEngine"/> class.
        /// </summary>
        static MistakeEngine()
        {
            MistakeEngine.Mistakes = new List<Mistake>();
            MistakeEngine.toClear = new List<TextRange>();
            MistakeEngine.toUnderline = new List<TextRange>();
        }

        /// <summary>
        /// Gets or sets a <see cref="List{Mistake}"/> containing all <see cref="Mistakes"/> located in the last execution of <see cref="MistakeEngine.MistakeSearchAsync"/>. 
        /// At the release of C# 6.0, the set accessor should be made private and the property should be auto initialized.
        /// </summary>
        public static List<Mistake> Mistakes { get; set; }

        /// <summary>
        /// Starts and awaits all operations with <see cref="Mistakes"/>.
        /// </summary>
        /// <param name="allWords">
        /// A <see cref="List{Word}"/> containing the <see cref="Word"/>s to be checked.
        /// </param>
        /// <param name="mistakeList">
        /// A <see cref="Selector"/> which should be filled with the found <see cref="Mistake"/>s or null, if filling is not necessary.
        /// </param>
        /// <param name="scheduler">
        /// The <see cref="TaskScheduler"/> with GUI-context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static async Task MistakeSearchAsync(ICollection<Word> allWords, Selector mistakeList, TaskScheduler scheduler)
        {
            if (allWords == null)
            {
                throw new ArgumentNullException("allWords");
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }

            try
            {
                Mistakes = new List<Mistake>();
                toClear = new List<TextRange>();
                toUnderline = new List<TextRange>();

                CheckWordsForError(allWords);

                await Task.Factory.StartNew(() => UnderlineErrors(), CancellationToken.None, TaskCreationOptions.LongRunning, scheduler);

                if (mistakeList != null)
                {
                    mistakeList.Items.Clear();
                    foreach (var error in Mistakes)
                    {
                        mistakeList.Items.Add(error);
                    }
                }
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine(
                    @"InvalidOperationException thrown in MistakeSearchAsync(IEnumerable<Word>, TaskScheduler): "
                    + exception.Message + @"; " + exception.InnerException.Message);
            }
        }

        /// <summary>
        /// Checks a <see cref="List{Word}"/> for <see cref="Word"/>s committing a <see cref="Mistake"/>.
        /// </summary>
        /// <param name="allWords">A <see cref="List{Word}"/> containing the <see cref="Word"/>s to be checked.</param>
        private static void CheckWordsForError(IEnumerable<Word> allWords)
        {
            var standardSequence = 1;
            var lastEndPos = MainClass.MainWindow.CodeTextBox.Document.ContentStart;
            foreach (var word in allWords)
            {
                var wordRange = new TextRange(word.StartPosition, word.EndPosition);

                if (word.Type == WordType.NotDefined)
                {
                    var error = new Mistake(standardSequence, MistakeType.Error, word);

                    toUnderline.Add(wordRange);
                    if (!Mistakes.Any(thisError => thisError.Column == error.Column && thisError.Line == error.Line))
                    {
                        Mistakes.Add(error);
                    }

                    standardSequence++;
                }
                else 
                {
                    var whiteSpaceRange = new TextRange(lastEndPos, word.StartPosition);
                    toClear.Add(wordRange);
                    toClear.Add(whiteSpaceRange);
                }

                lastEndPos = word.EndPosition;
            }
        }

        /// <summary>
        /// Changes the <see cref="Inline.TextDecorationsProperty"/> of the <see cref="Word"/>s committing a <see cref="Mistake"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task UnderlineErrors()
        {
            MainClass.NoTextChangedPlox = true;
            foreach (var item in toClear.Where(item => item.GetPropertyValue(Inline.TextDecorationsProperty) != null))
            {
                item.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            }

            foreach (var item in toUnderline)
            {
                var wavyUnderline = new TextDecoration
                                        {
                                            Pen = (Pen)MainClass.MainWindow.CodeTextBox.FindResource("wavyPen")
                                        };
                var tdc = new TextDecorationCollection { wavyUnderline };
                item.ApplyPropertyValue(Inline.TextDecorationsProperty, tdc);
            }

            MainClass.NoTextChangedPlox = false;
        }
    }
}
