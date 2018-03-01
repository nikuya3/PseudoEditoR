// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Indicates a type, a <see cref="Word"/> can represent.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR.WordRecognition
{
    /// <summary>
    /// Indicates a type, a <see cref="Word"/> can represent.
    /// </summary>
    public enum WordType
    {
        /// <summary>
        /// The <see cref="ProgrammingLanguage"/> contains no definition of this <see cref="Word"/>.
        /// </summary>
        NotDefined,

        /// <summary>
        /// Core syntax, often used to define loops or branches.
        /// </summary>
        Keyword,

        /// <summary>
        /// Often used to define functions.
        /// </summary>
        Command,

        /// <summary>
        /// Represents values like <see cref="bool"/> or <see cref="int"/>.
        /// </summary>
        ValueType,

        /// <summary>
        /// The user generated name of a <see cref="Value"/>.
        /// </summary>
        Variable,

        /// <summary>
        /// The <see cref="Value"/> of a <see cref="ValueType"/>.
        /// </summary>
        Value,

        /// <summary>
        /// A commentary which spans multiple lines.
        /// </summary>
        BlockCommentary,

        /// <summary>
        /// A commentary which ends with a word-wrap.
        /// </summary>
        LineCommentary,

        /// <summary>
        /// An operator like = or >.
        /// </summary>
        Operator,

        /// <summary>
        /// Special and unique syntax provided by the <see cref="ProgrammingLanguage"/> used.
        /// </summary>
        SpecialSyntax
    }
}
