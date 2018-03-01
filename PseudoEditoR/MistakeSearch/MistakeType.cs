// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The type of a mistake.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR.MistakeSearch
{
    /// <summary>
    /// The type of a mistake.
    /// </summary>
    public enum MistakeType
    {
        /// <summary>
        /// A syntax error.
        /// </summary>
        Error,

        /// <summary>
        /// A warning mostly concerning semantic.
        /// </summary>
        Warning,

        /// <summary>
        /// Information for the user.
        /// </summary>
        Info
    }
}
