// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Represents a word in the code.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR.WordRecognition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Documents;

    /// <summary>
    /// Represents a <see cref="Word"/> in the code.
    /// </summary>
    /// <remarks>
    /// A <see cref="Word"/> specified by its unique members.
    /// </remarks>
    public class Word
    {
        /// <summary>
        /// The <see cref="programmingLanguage"/> the <see cref="Word"/> was originally written in. 
        /// Does not get updated if the <see cref="MainClass.CurrentProgrammingLanguage"/> changes as it is only needed at the initialization.
        /// </summary>
        private readonly ProgrammingLanguage programmingLanguage;

        /// <summary>
        /// An <see cref="IEnumerable{Word}"/> holding all <see cref="Word"/>s to be associated with this <see cref="Word"/>. 
        /// Does not get updated if the context changes, as it is only used for initialization.
        /// </summary>
        private readonly List<Word> allWords;

        /// <summary>
        /// Initializes a new instance of the <see cref="Word"/> class.
        /// </summary>
        public Word()
        {
            this.Content = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Word"/> class via the <see cref="RichTextBox"/> the <see cref="Word.Content"/> is written in.
        /// </summary>
        /// <param name="contentPointer">
        /// The <see cref="RichTextBox"/> the <see cref="Word.Content"/> is written in.
        /// </param>
        /// <param name="programmingLanguage">
        /// The <see cref="programmingLanguage"/> used to recognize the <see cref="Word"/>.
        /// </param>
        /// <param name="allWords">
        /// An <see cref="IEnumerable{Word}"/> containing all <see cref="Word"/>s in the code.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters were passed as null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The given <see cref="RichTextBox"/> contains no text.
        /// </exception>
        public Word(TextPointer contentPointer, ProgrammingLanguage programmingLanguage, List<Word> allWords)
        {
            if (contentPointer == null)
            {
                throw new ArgumentNullException("contentPointer");
            }

            if (programmingLanguage == null)
            {
                throw new ArgumentNullException("programmingLanguage");
            }

            if (allWords == null)
            {
                throw new ArgumentNullException("allWords");
            }

            if (new TextRange(contentPointer.DocumentStart, contentPointer.DocumentEnd).Text.Replace(" ", string.Empty).Replace("\r\n", string.Empty).Equals(string.Empty))
            {
                throw new ArgumentException(@"The FlowDocument must contain a word.", "contentPointer");
            }

            this.programmingLanguage = programmingLanguage;
            this.allWords = allWords;
            var newWord = GetWordAtPointer(contentPointer, programmingLanguage, allWords);
            this.StartPosition = newWord.StartPosition;
            this.EndPosition = newWord.EndPosition;
            this.Content = newWord.Content;
            this.Type = newWord.Type;
            this.ValueType = newWord.ValueType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Word"/> class, which computes the <see cref="Word.Content"/> itself.
        /// </summary>
        /// <param name="startPosition">
        /// The <see cref="TextPointer"/> pointing at the first letter of the <see cref="Word"/>.
        /// </param>
        /// <param name="endPosition">
        /// The <see cref="TextPointer"/> pointing at the last letter of the <see cref="Word"/>.
        /// </param>
        /// <param name="programmingLanguage">
        /// The <see cref="programmingLanguage"/> used to recognize the <see cref="Word"/>.
        /// </param>
        /// <param name="allWords">
        /// An <see cref="IEnumerable{Word}"/> containing all <see cref="Word"/>s in the code.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters were passed as null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <see cref="endPosition"/> points at a <see cref="char"/> behind  <see cref="startPosition"/>.
        /// </exception>
        public Word(TextPointer startPosition, TextPointer endPosition, ProgrammingLanguage programmingLanguage, List<Word> allWords)
        {
            if (startPosition == null)
            {
                throw new ArgumentNullException("startPosition");
            }

            if (endPosition == null)
            {
                throw new ArgumentNullException("endPosition");
            }

            if (programmingLanguage == null)
            {
                throw new ArgumentNullException("programmingLanguage");
            }

            if (allWords == null)
            {
                throw new ArgumentNullException("allWords");
            }

            ////if (startPosition.CompareTo(endPosition) == 1)
            ////{
            ////    Console.WriteLine(new TextRange(startPosition, endPosition).Text);
            ////    throw new ArgumentException(@"The TextPointer referring to the last letter of the Word has to be located after the TextPointer referring to the first letter of the Word", "endPosition");
            ////}
            
            this.programmingLanguage = programmingLanguage;
            this.allWords = allWords;
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
            this.Content = GetContentOfWord(this.StartPosition);
            this.Type = this.GetTypeOfWord(this, allWords);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Word"/> class.
        /// </summary>
        /// <param name="startPosition">
        /// The <see cref="TextPointer"/> pointing at the first letter of the <see cref="Word"/>.
        /// </param>
        /// <param name="endPosition">
        /// The <see cref="TextPointer"/> pointing at the last letter of the <see cref="Word"/>.
        /// </param>
        /// <param name="content">
        /// A <see cref="string"/> representing the <see cref="Word.Content"/>. Should be exactly in the range of the two <see cref="TextPointer"/>.
        /// </param>
        /// <param name="programmingLanguage">
        /// The <see cref="programmingLanguage"/> used to recognize the <see cref="Word"/>.
        /// </param>
        /// <param name="allWords">
        /// An <see cref="IEnumerable{Word}"/> containing all <see cref="Word"/>s in the code.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters were passed as null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <see cref="endPosition"/> points at a <see cref="char"/> behind  <see cref="startPosition"/>.
        /// </exception>
        private Word(TextPointer startPosition, TextPointer endPosition, string content, ProgrammingLanguage programmingLanguage, List<Word> allWords)
        {
            this.programmingLanguage = programmingLanguage;
            this.allWords = allWords;
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
            this.Content = content;
            this.Type = this.GetTypeOfWord(this, allWords);
        }

        /// <summary>
        /// Gets the <see cref="TextPointer"/> pointing at the first letter of the <see cref="Content"/>.
        /// </summary>
        public TextPointer StartPosition { get; private set; }

        /// <summary>
        /// Gets the <see cref="TextPointer"/> pointing at the last letter of the <see cref="Content"/>.
        /// </summary>
        public TextPointer EndPosition { get; private set; }

        /// <summary>
        /// Gets the text content of the <see cref="Word"/>.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Gets the <see cref="WordType"/> of the <see cref="Word"/>.
        /// </summary>
        public WordType Type { get; private set; }

        /// <summary>
        /// Gets the <see cref="ValueType"/> of the <see cref="Word"/>.
        /// </summary>
        public ValueType ValueType { get; private set; }

        /// <summary>
        /// Computes the <see cref="Content"/> of a <see cref="Word"/>.
        /// </summary>
        /// <param name="textPointer">
        /// A <see cref="TextPointer"/>, pointing at a <see cref="char"/>. Text will be computed from this point to white spaces in both <see cref="LogicalDirection"/>s.
        /// </param>
        /// <returns>The <see cref="Content"/> of a <see cref="Word"/> to which the <see cref="TextPointer"/> points.</returns>
        private static string GetContentOfWord(TextPointer textPointer)
        {
            string backwards = textPointer.GetTextInRun(LogicalDirection.Backward);
            var wordCharactersBeforePointer = new string(backwards.Reverse().TakeWhile(c => !char.IsWhiteSpace(c)).Reverse().ToArray());

            string fowards = textPointer.GetTextInRun(LogicalDirection.Forward);
            var wordCharactersAfterPointer = new string(fowards.TakeWhile(c => !char.IsWhiteSpace(c)).ToArray());

            return string.Join(string.Empty, wordCharactersBeforePointer, wordCharactersAfterPointer);
        }

        /// <summary>
        /// Computes a <see cref="Word"/> from a <see cref="TextPointer"/>, pointing at its <see cref="Content"/>.
        /// </summary>
        /// <param name="textPointer">
        /// The <see cref="TextPointer"/> pointing at any character in the <see cref="RichTextBox"/>.
        /// </param>
        /// <param name="programmingLanguage">
        /// The <see cref="programmingLanguage"/> used to recognize the <see cref="Word"/>.
        /// </param>
        /// <param name="allWords">
        /// An <see cref="IEnumerable{Word}"/> containing all <see cref="Word"/>s in the code.
        /// </param>
        /// <returns>
        /// A <see cref="Word"/>, to which the given <see cref="TextPointer"/> refers to.
        /// </returns>
        private static Word GetWordAtPointer(TextPointer textPointer, ProgrammingLanguage programmingLanguage, List<Word> allWords)
        {
            string backwards = textPointer.GetTextInRun(LogicalDirection.Backward);
            var wordCharactersBeforePointer = new string(backwards.Reverse().TakeWhile(c => !char.IsWhiteSpace(c)).Reverse().ToArray());
            string forwards = textPointer.GetTextInRun(LogicalDirection.Forward);
            var wordCharactersAfterPointer = new string(forwards.TakeWhile(c => !char.IsWhiteSpace(c)).ToArray());

            var startPos = textPointer.DocumentStart;
            var endPos = textPointer.DocumentEnd;
            string content = wordCharactersBeforePointer + wordCharactersAfterPointer;
            startPos = startPos.GetPositionAtOffset(startPos.GetOffsetToPosition(textPointer) - wordCharactersBeforePointer.Length, LogicalDirection.Forward);
            endPos = endPos.GetPositionAtOffset(endPos.GetOffsetToPosition(textPointer) + wordCharactersAfterPointer.Length, LogicalDirection.Backward);

            return new Word(startPos, endPos, content, programmingLanguage, allWords);
        }

        /// <summary>
        /// Tries to set the <see cref="WordType"/> to <see cref="WordType.Value"/> and thus setting the <see cref="ValueType"/>.
        /// </summary>
        /// <param name="thisWord">
        /// The <see cref="Word"/> from where the <see cref="WordType"/> is being computed.
        /// </param>
        /// <param name="previousWord">
        /// The previous located <see cref="Word"/> in the code.
        /// </param>
        /// <returns>
        /// <see cref="WordType.Value"/> if the operation succeeded or <see cref="WordType.NotDefined"/> if not.
        /// </returns>
        private static WordType TrySetWordTypeAsValue(Word thisWord, Word previousWord)
        {
            ValueType valueType;
            double tempDouble;
            int tempInt;
            bool tempBool;

            // ToDo: If C# 6.0 is released, the TryParse()-Methods no longer need explicit declared fields and thus, these fields can be deleted.
            if (thisWord.Content.StartsWith(thisWord.programmingLanguage.StartTokens["String"])
                || (previousWord.ValueType == ValueType.String
                    && !previousWord.Content.EndsWith(thisWord.programmingLanguage.EndTokens["String"])))
            {
                valueType = ValueType.String;
            }
            else if (thisWord.Content.StartsWith(thisWord.programmingLanguage.StartTokens["Character"]) && thisWord.Content.Length < 4)
            {
                valueType = ValueType.Character;
            }
            else if (double.TryParse(thisWord.Content, out tempDouble))
            {
                valueType = ValueType.Float;
            }
            else if (int.TryParse(thisWord.Content, out tempInt))
            {
                valueType = ValueType.Integer;
            }
            else if (bool.TryParse(thisWord.Content, out tempBool))
            {
                valueType = ValueType.Boolean;
            }
            else
            {
                thisWord.ValueType = ValueType.None;
                return WordType.NotDefined;
            }

            thisWord.ValueType = valueType;
            const WordType Type = WordType.Value;
            return Type;
        }

        /// <summary>
        /// Sets the <see cref="WordType"/> of a given <see cref="Word"/> to <see cref="WordType.Variable"/> and thus sets the <see cref="ValueType"/>.
        /// </summary>
        /// <param name="thisWord">
        /// The <see cref="Word"/> from where the <see cref="WordType"/> is being computed.
        /// </param>
        /// <param name="previousWord">
        /// The previous located <see cref="Word"/> in the code.
        /// </param>
        /// <param name="exists">
        /// A value indicating whether the word exists as a <see cref="WordType.Variable"/> in <see cref="MainClass.AllWordsInCode"/>.
        /// </param>
        /// <returns>
        /// The <see cref="WordType"/> set to <see cref="WordType.Variable"/>.
        /// </returns>
        private WordType SetWordTypeAsVariable(Word thisWord, Word previousWord, bool exists)
        {
            const WordType Type = WordType.Variable;
            if (exists)
            {
                foreach (var word in this.allWords.Where(word => word.Type == WordType.Variable && word.Content.Equals(thisWord.Content)))
                {
                    thisWord.ValueType = word.ValueType;
                }
            }
            else
            {
                thisWord.ValueType = previousWord.ValueType;
            }

            return Type;
        }

        /// <summary>
        /// Computes the <see cref="WordType"/> of a given <see cref="Word"/>.
        /// </summary>
        /// <param name="thisWord">
        /// The <see cref="Word"/> from where a <see cref="WordType"/> is computed.
        /// </param>
        /// <param name="allWords">
        /// An <see cref="IEnumerable{Word}"/> containing all <see cref="Word"/>s of the code.
        /// </param>
        /// <returns>
        /// The computed <see cref="WordType"/>.
        /// </returns>
        private WordType GetTypeOfWord(Word thisWord, IEnumerable<Word> allWords)
        {
            var type = WordType.NotDefined;
            if (thisWord.Content == string.Empty)
            {
                return type;
            }

            var previousWord = new Word();

            var words =
                allWords.Where(word => thisWord.StartPosition.GetOffsetToPosition(word.StartPosition) < 0)
                    .OrderBy(word => word.StartPosition.GetOffsetToPosition(thisWord.StartPosition)).ToList();
            if (words.Count > 0)
            {
                previousWord = words[0];
            }

            type = this.TrySetWordTypeAsCommentary(thisWord, previousWord, type);

            if (type == WordType.LineCommentary || type == WordType.BlockCommentary)
            {
                return type;
            }

            type = TrySetWordTypeAsValue(thisWord, previousWord);
            if (type != WordType.NotDefined)
            {
                return type;
            }

            if (this.programmingLanguage.Keywords.ContainsValue(thisWord.Content))
            {
                type = WordType.Keyword;
            }
            else if (this.programmingLanguage.ValueTypes.ContainsValue(thisWord.Content))
            {
                type = WordType.ValueType;
            }
            else if (this.programmingLanguage.Commands.ContainsValue(thisWord.Content))
            {
                type = WordType.Command;
            }
            else if (this.allWords.Exists(item => item.Type == WordType.Variable && item.Content.Equals(thisWord.Content))
                     || this.programmingLanguage.ValueTypes.ContainsValue(previousWord.Content))
            {
                type = this.SetWordTypeAsVariable(
                    thisWord,
                    previousWord,
                    this.allWords.Exists(item => item.Type == WordType.Variable && item.Content.Equals(thisWord.Content)));
            }
            else if (this.programmingLanguage.Operators.ContainsValue(thisWord.Content))
            {
                type = WordType.Operator;
            }
            else if (this.programmingLanguage.SpecialSyntax.ContainsValue(thisWord.Content))
            {
                type = WordType.SpecialSyntax;
                type = this.SpecifySpecialSyntax(thisWord, type);
            }

            return type;
        }

        /// <summary>
        /// Tries to specify the <see cref="WordType"/> if it is <see cref="WordType.SpecialSyntax"/>.
        /// </summary>
        /// <param name="thisWord">
        /// The <see cref="Word"/> from where the <see cref="WordType"/> is computed.
        /// </param>
        /// <param name="type">
        /// The <see cref="WordType"/> to be possibly specified.
        /// </param>
        /// <returns>
        /// The specified <see cref="WordType"/>.
        /// </returns>
        private WordType SpecifySpecialSyntax(Word thisWord, WordType type)
        {
            var specialSyntax =
                this.programmingLanguage.SpecialSyntax.ElementAt(
                    this.programmingLanguage.SpecialSyntax.Values.ToList().IndexOf(thisWord.Content));
            if (this.programmingLanguage.Keywords.ContainsKey(specialSyntax.Key))
            {
                type = WordType.Keyword;
            }
            else if (this.programmingLanguage.Commands.ContainsKey(specialSyntax.Key))
            {
                type = WordType.Command;
            }
            else if (this.programmingLanguage.Operators.ContainsKey(specialSyntax.Key))
            {
                type = WordType.Operator;
            }

            return type;
        }

        /// <summary>
        /// Tries to set the given <see cref="WordType"/> as <see cref="WordType.LineCommentary"/> or <see cref="WordType.BlockCommentary"/>.
        /// </summary>
        /// <param name="thisWord">
        /// The <see cref="Word"/> from where the <see cref="WordType"/> is computed.
        /// </param>
        /// <param name="previousWord">
        /// The previous located <see cref="Word"/> in the code.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="WordType"/>.
        /// </returns>
        private WordType TrySetWordTypeAsCommentary(Word thisWord, Word previousWord, WordType type)
        {
            if ((thisWord.Content.IndexOf(this.programmingLanguage.StartTokens["LineCommentary"], StringComparison.Ordinal) == 0
                 || previousWord.Type == WordType.LineCommentary) && !thisWord.StartPosition.IsAtLineStartPosition)
            {
                type = WordType.LineCommentary;
            }
            else if (previousWord.Type == WordType.BlockCommentary)
            {
                type = WordType.BlockCommentary;
            }
            else if (thisWord.Content.StartsWith(this.programmingLanguage.StartTokens["BlockCommentary"]))
            {
                type = WordType.BlockCommentary;
            }

            if (previousWord.Content.EndsWith(this.programmingLanguage.EndTokens["BlockCommentary"]))
            {
                type = WordType.NotDefined;
            }

            return type;
        }
    }
}
