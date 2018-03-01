// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Recognizes and holds all Words given via a TextPointer of a FlowDocument.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR.WordRecognition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Documents;

    /// <summary>
    /// Recognizes and holds all <see cref="Word"/>s given via a <see cref="TextPointer"/> of a <see cref="FlowDocument"/>.
    /// </summary>
    public static class RecognitionEngine
    {
        /// <summary>
        /// The <see cref="ProgrammingLanguage"/> used to recognize the <see cref="Word"/>s properly.
        /// </summary>
        private static ProgrammingLanguage programmingLanguage;

        /// <summary>
        /// Initializes static members of the <see cref="RecognitionEngine"/> class.
        /// </summary>
        static RecognitionEngine()
        {
            RecognitionEngine.AllWordsInCode = new List<Word>();
        }

        /// <summary>
        /// Gets <see cref="List{Word}"/> containing all <see cref="Word"/>s calculated in <see cref="MainClass.CodeAlteredAsync"/> (sorted in their position).
        /// </summary>
        public static List<Word> AllWordsInCode { get; private set; }

        /// <summary>
        /// Gets the <see cref="Word"/> generated in the last execution of <see cref="MainClass.CodeAlteredAsync"/>. Changes if <see cref="MainClass.CodeAlteredAsync"/> executed successful.
        /// </summary>
        public static Word PreviousWord { get; private set; }

        /// <summary>
        /// Starts the operations to recognize the <see cref="Word"/>s in a <see cref="FlowDocument"/>.
        /// </summary>
        /// <param name="wordPosition">
        /// The <see cref="TextPointer"/> pointing at the <see cref="Word"/> to be recognized.
        /// </param>
        /// <param name="programmingLanguage">
        /// A <see cref="ProgrammingLanguage"/> used to recognize the <see cref="Word"/>s properly.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{Word}"/> containing the <see cref="Word"/>s which were added or updated.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// One or all parameters was/ were passed as null.
        /// </exception>
        public static async Task<IEnumerable<Word>> RecognizeWordsInCode(TextPointer wordPosition, ProgrammingLanguage programmingLanguage)
        {
            if (wordPosition == null)
            {
                throw new ArgumentNullException("wordPosition");
            }

            if (programmingLanguage == null)
            {
                throw new ArgumentNullException("programmingLanguage");
            }

            if (!RecognitionEngine.programmingLanguage.Equals(programmingLanguage))
            {
                RecognitionEngine.programmingLanguage = programmingLanguage;
            }

            var word = new Word(wordPosition, MainClass.CurrentProgrammingLanguage, AllWordsInCode);
            if (PreviousWord != null && (word.StartPosition.GetOffsetToPosition(PreviousWord.StartPosition) == 1 || word.EndPosition.GetOffsetToPosition(PreviousWord.EndPosition) == 1))
            {
                RemoveNonExistingWord(PreviousWord);
            }

            if (word.Content == string.Empty)
            {
                return new List<Word> { word };
            }

            PreviousWord = word;

            return UpdateList(word);
        }

        /// <summary>
        /// Updates the <see cref="Word"/>s in <see cref="AllWordsInCode"/> to the new <see cref="ProgrammingLanguage"/>.
        /// </summary>
        /// <param name="newProgrammingLanguage">
        /// The <see cref="ProgrammingLanguage"/> the <see cref="Word"/>s should be updated to.
        /// </param>
        public static async void UpdateCodeAsync(ProgrammingLanguage newProgrammingLanguage)
        {
            for (int i = 0; i < AllWordsInCode.Count; i++)
            {
                var oldWord = AllWordsInCode[i];
                var newWord = new Word(
                    oldWord.StartPosition,
                    oldWord.EndPosition,
                    newProgrammingLanguage,
                    AllWordsInCode);
                AllWordsInCode[i] = newWord;

                RemoveNonExistingWord(newWord);

                InsertWordAsync(newWord, AllWordsInCode);
            }
        }

        /// <summary>
        /// Updates <see cref="AllWordsInCode"/> to represent the code properly.
        /// </summary>
        /// <param name="thisWord">
        /// The <see cref="Word"/> to be added to <see cref="AllWordsInCode"/>.
        /// </param>
        /// <returns>
        /// A <see cref="List"/> containing the changed <see cref="Word"/>s.
        /// </returns>
        private static List<Word> UpdateList(Word thisWord)
        {
            RemoveNonExistingWord(thisWord);

            InsertWordAsync(thisWord, AllWordsInCode);

            var changedWords = UpdateWordTypes(thisWord, programmingLanguage);

            changedWords.Insert(0, thisWord);

            // For debug purposes.
            Console.WriteLine();
            Task.Factory.StartNew(() =>
            {
                foreach (var word in AllWordsInCode)
                {
                    Console.Write(word.Content + @" ");
                }
            });

            return changedWords;
        }

        /// <summary>
        /// Removes <see cref="Word"/>s which were overwritten or deleted.
        /// </summary>
        /// <param name="thisWord">The <see cref="Word"/> which has overwritten a <see cref="Word"/> in <see cref="MainClass.AllWordsInCode"/>.</param>
        private static void RemoveNonExistingWord(Word thisWord)
        {
            for (int i = 1; i < AllWordsInCode.Count + 1; i++)
            {
                if (thisWord.StartPosition.GetOffsetToPosition(AllWordsInCode[i - 1].StartPosition) == 0
                            || thisWord.EndPosition.GetOffsetToPosition(AllWordsInCode[i - 1].EndPosition) == 0)
                {
                    AllWordsInCode.Remove(AllWordsInCode[i - 1]);
                }
            }
        }

        /// <summary>
        /// Updates <see cref="WordType"/>s of the <see cref="Word"/>s in <see cref="AllWordsInCode"/> according to the given <see cref="Word"/>.
        /// The asynchronous version of <see cref="MainClass.UpdateWordTypes"/>, which is currently not used as it triggers <see cref="InvalidOperationException"/>s and fatal errors.
        /// </summary>
        /// <param name="thisWord">
        /// The <see cref="Word"/> which possibly changes its successors.
        /// </param>
        /// <param name="programmingLanguage">
        /// A <see cref="ProgrammingLanguage"/> which holds important information for the <see cref="WordType"/>s.
        /// </param>
        /// <returns>
        /// A <see cref="List{Word}"/> containing all <see cref="Word"/>s which <see cref="WordType"/>s were changed.
        /// </returns>
        private static List<Word> UpdateWordTypesAsync(Word thisWord, ProgrammingLanguage programmingLanguage)
        {
            var changedWords = new List<Word>();
            switch (thisWord.Type)
            {
                case WordType.LineCommentary:
                    var tempLineEnd = thisWord.StartPosition.GetLineStartPosition(1);
                    var lineEnd = (tempLineEnd ?? thisWord.StartPosition.DocumentEnd).GetInsertionPosition(LogicalDirection.Backward);
                    var lineCommentaryRange = new TextRange(thisWord.EndPosition, lineEnd);
                    var lineCommentaryWords = AllWordsInCode.Where(word => lineCommentaryRange.Contains(word.StartPosition)).ToList();
                    Parallel.ForEach(
                        lineCommentaryWords,
                        word =>
                        {
                            Word newWord;
                            ChangeWordType(word, out newWord);
                            changedWords.Add(newWord);
                        });

                    break;
                case WordType.BlockCommentary:
                    var endPointer = thisWord.StartPosition.DocumentEnd;
                    try
                    {
                        var endWord = AllWordsInCode.First(
                            word =>
                            thisWord.StartPosition.CompareTo(word.StartPosition) > 0
                            && word.Content.EndsWith(programmingLanguage.EndTokens["BlockCommentary"]));
                        endPointer = endWord.EndPosition;
                    }
                    catch (InvalidOperationException) { }
                    var blockCommentaryRange = new TextRange(thisWord.EndPosition, endPointer);
                    var blockCommentaryWords = AllWordsInCode.Where(word => blockCommentaryRange.Contains(word.StartPosition)).ToList();

                    Parallel.ForEach(
                        blockCommentaryWords,
                        word =>
                        {
                            Word newWord;
                            ChangeWordType(word, out newWord);
                            changedWords.Add(newWord);
                        });

                    break;
                case WordType.Value:
                    var endPosition = thisWord.StartPosition.DocumentEnd;
                    try
                    {
                        var lastWord = AllWordsInCode.First(
                            word =>
                            thisWord.StartPosition.CompareTo(word.StartPosition) > 0
                            && word.Content.EndsWith(programmingLanguage.EndTokens["String"]));
                        endPosition = lastWord.EndPosition;
                    }
                    catch (InvalidOperationException) { }
                    var stringRange = new TextRange(thisWord.EndPosition, endPosition);
                    var stringWords = AllWordsInCode.Where(word => stringRange.Contains(word.StartPosition)).ToList();

                    Parallel.ForEach(
                        stringWords,
                        word =>
                        {
                            Word newWord;
                            ChangeWordType(word, out newWord);
                            changedWords.Add(newWord);
                        });

                    break;
            }

            return changedWords;
        }

        /// <summary>
        /// Updates <see cref="WordType"/>s of the <see cref="Word"/>s in <see cref="MainClass.AllWordsInCode"/> according to the given <see cref="Word"/>.
        /// </summary>
        /// <param name="thisWord">
        /// The <see cref="Word"/> which possibly changes its successors.
        /// </param>
        /// <param name="programmingLanguage">
        /// A <see cref="ProgrammingLanguage"/> which holds important information for the <see cref="WordType"/>s.
        /// </param>
        /// <returns>
        /// A <see cref="List{Word}"/> containing all <see cref="Word"/>s which <see cref="WordType"/>s were changed.
        /// </returns>
        private static List<Word> UpdateWordTypes(Word thisWord, ProgrammingLanguage programmingLanguage)
        {
            var changedWords = new List<Word>();
            switch (thisWord.Type)
            {
                case WordType.LineCommentary:
                    var tempLineEnd = thisWord.StartPosition.GetLineStartPosition(1);
                    var lineEnd = (tempLineEnd ?? thisWord.StartPosition.DocumentEnd).GetInsertionPosition(LogicalDirection.Backward);
                    var lineCommentaryRange = new TextRange(thisWord.EndPosition, lineEnd);
                    var lineCommentaryWords = AllWordsInCode.Where(word => lineCommentaryRange.Contains(word.StartPosition)).ToList();
                    foreach (var word in lineCommentaryWords)
                    {
                        Word newWord;
                        ChangeWordType(word, out newWord);
                        changedWords.Add(newWord);
                    }

                    break;
                case WordType.BlockCommentary:
                    var endPointer = thisWord.StartPosition.DocumentEnd;
                    try
                    {
                        var endWord = AllWordsInCode.First(
                            word =>
                            thisWord.StartPosition.CompareTo(word.StartPosition) > 0
                            && word.Content.EndsWith(programmingLanguage.EndTokens["BlockCommentary"]));
                        endPointer = endWord.EndPosition;
                    }
                    catch (InvalidOperationException) { }
                    var blockCommentaryRange = new TextRange(thisWord.EndPosition, endPointer);
                    var blockCommentaryWords = AllWordsInCode.Where(word => blockCommentaryRange.Contains(word.StartPosition)).ToList();
                    foreach (var word in blockCommentaryWords)
                    {
                        Word newWord;
                        ChangeWordType(word, out newWord);
                        changedWords.Add(newWord);
                    }

                    break;
                case WordType.Value:
                    var endPosition = thisWord.StartPosition.DocumentEnd;
                    try
                    {
                        var lastWord = AllWordsInCode.First(
                            word =>
                            thisWord.StartPosition.CompareTo(word.StartPosition) > 0
                            && word.Content.EndsWith(programmingLanguage.EndTokens["String"]));
                        endPosition = lastWord.EndPosition;
                    }
                    catch (InvalidOperationException) { }
                    var stringRange = new TextRange(thisWord.EndPosition, endPosition);
                    var stringWords = AllWordsInCode.Where(word => stringRange.Contains(word.StartPosition)).ToList();
                    foreach (var word in stringWords)
                    {
                        Word newWord;
                        ChangeWordType(word, out newWord);
                        changedWords.Add(newWord);
                    }

                    break;
            }

            return changedWords;
        }

        /// <summary>
        /// Changes the <see cref="WordType"/> of an existing <see cref="Word"/> and updates <see cref="MainClass.AllWordsInCode"/> accordingly.
        /// </summary>
        /// <param name="oldWord">
        /// The <see cref="Word"/> with the old <see cref="WordType"/>.
        /// </param>
        /// <param name="newWord">
        /// The <see cref="Word"/> with the new <see cref="WordType"/>.
        /// </param>
        private static void ChangeWordType(Word oldWord, out Word newWord)
        {
            var newWordToInsert = new Word(oldWord.StartPosition, oldWord.EndPosition, programmingLanguage, AllWordsInCode);
            InsertWordAsync(newWordToInsert, AllWordsInCode);
            RemoveNonExistingWord(oldWord);
            newWord = newWordToInsert;
        }

        /// <summary>
        /// Inserts a <see cref="Word"/> in the right place in an <see cref="IList{Word}"/>.
        /// </summary>
        /// <param name="thisWord">The <see cref="Word"/> to be inserted.</param>
        /// <param name="listToInsertWord">The <see cref="IList{Word}"/> to be filled.</param>
        private static void InsertWordAsync(Word thisWord, IList<Word> listToInsertWord)
        {
            bool added = false;
            if (listToInsertWord.Count > 0)
            {
                Parallel.For(
                    0,
                    listToInsertWord.Count,
                    (i, loopState) =>
                    {
                        if (listToInsertWord[i].StartPosition.GetOffsetToPosition(thisWord.StartPosition) < 0)
                        {
                            listToInsertWord.Insert(i, thisWord);
                            added = true;
                            loopState.Stop();
                        }
                    });

                if (!added)
                {
                    listToInsertWord.Add(thisWord);
                }
            }
            else
            {
                listToInsertWord.Add(thisWord);
            }
        }
    }
}
