// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Handles almost everything with the GUI.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR.UserInteraction
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using PseudoEditoR.MistakeSearch;
    using PseudoEditoR.WordRecognition;

    /// <summary>
    /// Handles almost everything with the GUI.
    /// </summary>
    /// <remarks>
    /// Used in classes which are not intended to do GUI-work. 
    /// </remarks>
    public static class Gui
    {
        /// <summary>
        /// Fills a <see cref="ComboBox"/> with the name of the localization languages given as a <see cref="Dictionary{String, String}"/>.
        /// </summary>
        /// <param name="localizationLanguages">
        /// An <see cref="ICollection{T}"/> holding <see cref="Dictionary{String, String}"/>s representing localization languages.
        /// </param>
        /// <param name="comboBox">
        /// The <see cref="ComboBox"/> to be filled.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static async Task FillLocalizationLanguageComboBoxAsync(ICollection<Dictionary<string, string>> localizationLanguages, ComboBox comboBox)
        {
            if (localizationLanguages == null)
            {
                throw new ArgumentNullException("localizationLanguages");
            }

            if (comboBox == null)
            {
                throw new ArgumentNullException("comboBox");
            }

            Parallel.ForEach(
                localizationLanguages,
                localizationLanguage =>
                    {
                        comboBox.Items.Add(localizationLanguage["language"]);
                        if (localizationLanguage["language"] == "English")
                        {
                            comboBox.SelectedItem = "English";
                        }
                    });
            comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Fills the given <see cref="ComboBox"/> with the names of the given <see cref="ProgrammingLanguage"/>s.
        /// </summary>
        /// <param name="programmingLanguages">
        /// An <see cref="ICollection{ProgrammingLanguage}"/> containing <see cref="ProgrammingLanguage"/>s which <see cref="ProgrammingLanguage.Name"/> will be used.
        /// </param>
        /// <param name="comboBox">
        /// The <see cref="ComboBox"/> to be filled.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static async Task FillProgrammingLanguageComboBoxAsync(ICollection<ProgrammingLanguage> programmingLanguages, ComboBox comboBox)
        {
            if (programmingLanguages == null)
            {
                throw new ArgumentNullException("programmingLanguages");
            }

            if (comboBox == null)
            {
                throw new ArgumentNullException("comboBox");
            }

            foreach (var programmingLanguage in programmingLanguages)
            {
                comboBox.Items.Add(programmingLanguage.Name);
            }

            comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Called if a <see cref="ListBoxItem"/> of the <see cref="Selector"/> used for the PseudoSense is clicked. Implements operations connected to the event.
        /// </summary>
        /// <param name="richTextBox">
        /// The <see cref="RichTextBox"/> associated with the code.
        /// </param>
        /// <param name="selector">
        /// The <see cref="Selector"/> which triggered the event.
        /// </param>
        /// <param name="word">
        /// The <see cref="Word"/> used for the PseudoSense.
        /// </param>
        /// <param name="e">
        /// The specific <see cref="KeyEventArgs"/> holding information about the event.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static async Task PseudoSenseUserInteraction(RichTextBox richTextBox, Selector selector, Word word, KeyEventArgs e)
        {
            Console.ReadLine();
            if (richTextBox == null)
            {
                throw new ArgumentNullException("richTextBox");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            if (word == null)
            {
                throw new ArgumentNullException("word");
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (selector.SelectedItem != null)
                {
                    InsertText(richTextBox, selector.SelectedItem.ToString());
                    selector.Visibility = Visibility.Hidden;
                    richTextBox.Focus();
                    e.Handled = true;
                }
            }
            else if (e.Key != Key.Up && e.Key != Key.Down)
            {
                richTextBox.Focus();
            }
        }

        /// <summary>
        /// Called if a <see cref="ListBoxItem"/> of the <see cref="Selector"/> used for the PseudoSense is clicked. Implements operations connected to the event.
        /// </summary>
        /// <param name="richTextBox">
        /// The <see cref="RichTextBox"/> associated with the code.
        /// </param>
        /// <param name="selector">
        /// The <see cref="Selector"/> which triggered the event.
        /// </param>
        /// <param name="word">
        /// The <see cref="Word"/> used for the PseudoSense.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static async Task PseudoSenseUserInteraction(RichTextBox richTextBox, Selector selector, Word word)
        {
            if (richTextBox == null)
            {
                throw new ArgumentNullException("richTextBox");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            if (word == null)
            {
                throw new ArgumentNullException("word");
            }

            InsertText(richTextBox, selector.SelectedItem.ToString());
            selector.Visibility = Visibility.Hidden;
            richTextBox.Focus();
        }

        /// <summary>
        /// Called if a key is pressed in a <see cref="RichTextBox"/>. Implements operations connected to the event.
        /// </summary>
        /// <param name="richTextBox">
        /// The <see cref="RichTextBox"/> which triggered the event.
        /// </param>
        /// <param name="selector">
        /// The <see cref="Selector"/> used for PseudoSense. May be focused.
        /// </param>
        /// <param name="e">
        /// The specific <see cref="KeyEventArgs"/> holding information about the event.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static void RichTextBoxUserInteraction(RichTextBox richTextBox, Selector selector, KeyEventArgs e)
        {
            if (richTextBox == null)
            {
                throw new ArgumentNullException("richTextBox");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if ((e.Key == Key.Down || e.Key == Key.Up) && selector.Visibility == Visibility.Visible)
            {
                selector.Focus();
            }

            if (e.Key == Key.Back && richTextBox.Selection.Text.Length > 0)
            {
                RemoveDeletedTextRange(new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End));
            }
        }

        /// <summary>
        /// Called if the <see cref="Selector"/> used to display <see cref="Mistake"/>s triggered an event indicating user interaction.
        /// </summary>
        /// <param name="richTextBox">
        /// The <see cref="RichTextBox"/> holding the <see cref="Mistake"/>s.
        /// </param>
        /// <param name="selector">
        /// The <see cref="Selector"/> used to display <see cref="Mistake"/>s.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static async Task MistakeSelectorUserInteraction(RichTextBox richTextBox, Selector selector)
        {
            if (richTextBox == null)
            {
                throw new ArgumentNullException("richTextBox");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            if (selector.SelectedIndex > -1)
            {
                richTextBox.Selection.Select(MistakeEngine.Mistakes[selector.SelectedIndex].Word.StartPosition, MistakeEngine.Mistakes[selector.SelectedIndex].Word.EndPosition);
                richTextBox.Focus();
            }
        }

        /// <summary>
        /// Positions the <see cref="Selector"/> used for the PseudoSense at the right position.
        /// </summary>
        /// <param name="richTextBox">
        /// The <see cref="RichTextBox"/> which holds the code containing the <see cref="Word.Content"/> used for the PseudoSense.
        /// </param>
        /// <param name="listBox">
        /// The <see cref="Selector"/> used for the PseudoSense.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static async Task PositionListBoxAsync(RichTextBox richTextBox, Selector listBox)
        {
            if (richTextBox == null)
            {
                throw new ArgumentNullException("richTextBox");
            }

            if (listBox == null)
            {
                throw new ArgumentNullException("listBox");
            }

            var rect = new Rect();
            if (richTextBox.CaretPosition != null)
            {
                rect = richTextBox.CaretPosition.GetCharacterRect(LogicalDirection.Forward);
            }

            var point = rect.BottomRight;
            listBox.Margin = new Thickness(point.X + 30, point.Y + 85, 0, 0);
        }

        /// <summary>
        /// Inserts the given <see cref="string"/> in the <see cref="RichTextBox"/> at the <see cref="RichTextBox.CaretPosition"/>.
        /// Also replaces the <see cref="Word.Content"/> at the <see cref="RichTextBox.CaretPosition"/>.
        /// </summary>
        /// <param name="richTextBox">
        /// The <see cref="RichTextBox"/> in which the <see cref="Selector.SelectedItem"/> should be inserted.
        /// </param>
        /// <param name="toInsert">
        /// The <see cref="string"/> to be inserted.
        /// </param>
        private static void InsertText(RichTextBox richTextBox, string toInsert)
        {
            ////string selectedWord = listBox.SelectedItem.ToString();
            ////string toInsert = string.Empty;
            ////for (int i = 0; i < selectedWord.Length; i++)
            ////{
            ////    if (selectedWord.Substring(0, i).Equals(word.Content))
            ////    {
            ////        toInsert = selectedWord.Substring(i);
            ////    }
            ////}
            ////word.EndPosition.InsertTextInRun(toInsert);
            
            var wordToRemove = new Word(richTextBox.CaretPosition, MainClass.CurrentProgrammingLanguage, RecognitionEngine.AllWordsInCode);
            wordToRemove.StartPosition.DeleteTextInRun(wordToRemove.Content.Length);
            RemoveDeletedTextRange(new TextRange(wordToRemove.StartPosition, wordToRemove.EndPosition));
            richTextBox.CaretPosition = wordToRemove.StartPosition;
            richTextBox.CaretPosition.InsertTextInRun(toInsert);
            ////var newWord = new Word(richTextBox, MainClass.CurrentProgrammingLanguage, MainClass.AllWordsInCode);
            ////richTextBox.CaretPosition = newWord.EndPosition;
        }

        /// <summary>
        /// Removes the <see cref="Word"/>s in a given <see cref="TextRange"/> from variables still holding them.
        /// </summary>
        /// <param name="deletedRange">
        /// A <see cref="TextRange"/> containing the text which was deleted.
        /// </param>
        private static void RemoveDeletedTextRange(TextRange deletedRange)
        {
            foreach (
                var deletedWord in
                    RecognitionEngine.AllWordsInCode.Where(
                        word => deletedRange.Contains(word.StartPosition) || deletedRange.Contains(word.EndPosition))
                        .ToList())
            {
                RecognitionEngine.AllWordsInCode.Remove(deletedWord);
            }
        }
    }
}
