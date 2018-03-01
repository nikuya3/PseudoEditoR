// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   A Programming language defined by its unique properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using Newtonsoft.Json;

    /// <summary>
    /// A Programming language defined by its unique properties.
    /// </summary>
    /// <remarks>
    /// The <see cref="ProgrammingLanguage"/> fills the dynamic written methods with its unique properties.
    /// </remarks>
    [Serializable]
    public class ProgrammingLanguage
    {
        /// <summary>
        /// Gets or sets the name of the <see cref="ProgrammingLanguage"/>.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// Gets the <see cref="WordType.Keyword"/>s of the <see cref="ProgrammingLanguage"/> as <see cref="Dictionary{String, String}"/>, 
        /// while the key represents the standard value ("if") and the value represents the unique value ("falls").
        /// These often define loops or branches.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, string> Keywords { get; private set; }

        /// <summary>
        /// Gets the <see cref="WordType.ValueType"/>s of the <see cref="ProgrammingLanguage"/> as <see cref="Dictionary{String, String}"/>, 
        /// while he key represents the standard value ("boolean") and the value represents the unique value ("bool"). 
        /// These often represent values like <see cref="int"/>s or <see cref="string"/>s.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, string> ValueTypes { get; private set; }

        /// <summary>
        /// Gets the <see cref="WordType.Command"/>s of the <see cref="ProgrammingLanguage"/> as <see cref="Dictionary{String, String}"/>, 
        /// while the key represents the standard value ("write") and the value represents the unique value ("println").
        /// These often define functions like reading user input or printing output.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, string> Commands { get; private set; }

        /// <summary>
        /// Gets the operators of the <see cref="ProgrammingLanguage"/> as <see cref="Dictionary{String, String}"/>,
        /// while the key represents the name of the operator ("equals") and the value represents the operator value ("=").
        /// These are often used for intuitive arithmetic algorithms as well as expressions and assignments.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, string> Operators { get; private set; }

        /// <summary>
        /// Gets the special syntax, which the <see cref="ProgrammingLanguage"/> may have, as <see cref="Dictionary{String, String}"/>,
        /// while the key represents the "standard" value (if possible, if not, one of the standard value of other properties, 
        /// which will result in the special syntax being handled as the properties <see cref="WordType"/>) and the value represents the unique value.
        /// These can be special syntax out of the range of <see cref="WordType"/> or a part of a <see cref="WordType"/>, but not considered by the word recognition 
        /// (this may be additional functions or higher types of keywords).
        /// </summary>
        [JsonProperty]
        public Dictionary<string, string> SpecialSyntax { get; private set; }

        /// <summary>
        /// Gets a <see cref="Dictionary{WordType, Color}"/> containing color codes for all <see cref="WordType"/>s. 
        /// <see cref="FacilitateCoding.HighlightSyntaxAsync"/> will recognize these and enable unique highlighting for every language.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, Color> Colors { get; private set; }

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{String, String}"/> holding <see cref="Mistake"/>s, 
        /// while the key holds the name of the <see cref="Mistake"/> and the value the specific description. The description will be localized by the localization language.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, string> MistakeDescriptions { get; set; } 

        /// <summary>
        /// Gets the tokens which indicate a <see cref="WordType"/> when located at a <see cref="Word.Content"/>s start. ('/*' for <see cref="WordType.BlockCommentary"/>).
        /// </summary>
        [JsonProperty]
        public Dictionary<string, string> StartTokens { get; private set; }

        /// <summary>
        /// Gets the tokens, which indicate that this <see cref="Word"/> stops the chain of specific <see cref="WordType"/>s if located at the end. ('*/' for <see cref="WordType.BlockCommentary"/>).
        /// </summary>
        [JsonProperty]
        public Dictionary<string, string> EndTokens { get; private set; }
    }
}
