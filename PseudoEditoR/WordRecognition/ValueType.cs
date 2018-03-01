// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Indicates a value type, variables and values can represent.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR.WordRecognition
{
    /// <summary>
    /// Indicates a value type, <see cref="WordType.Variable"/>s and <see cref="WordType.Value"/>s can represent.
    /// </summary>
    public enum ValueType
    {
        /// <summary>
        /// Represents no value type.
        /// </summary>
        None,

        /// <summary>
        /// Represents a <see cref="bool"/>.
        /// </summary>
        Boolean,

        /// <summary>
        /// Represents a <see cref="char"/>.
        /// </summary>
        Character,

        /// <summary>
        /// Represents a <see cref="float"/>.
        /// </summary>
        Float,

        /// <summary>
        /// Represents an <see cref="int"/>.
        /// </summary>
        Integer,

        /// <summary>
        /// Represents a <see cref="string"/>.
        /// </summary>
        String,

        /// <summary>
        /// A user defined value (structure).
        /// </summary>
        UserDefined
    }
}
