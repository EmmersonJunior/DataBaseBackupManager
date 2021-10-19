using System;
using System.Runtime.Serialization;

namespace DataBaseBackupManager.Exceptions
{
    /// <summary>
    /// The Column not found exception.
    /// </summary>
    [Serializable]
    public class ColumnNotFoundException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ColumnNotFoundException()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ColumnNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception who threw this exception.</param>
        public ColumnNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Source and destinations of a context.</param>
        protected ColumnNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}