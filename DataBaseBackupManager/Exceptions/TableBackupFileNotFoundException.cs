using System;
using System.Runtime.Serialization;

namespace DataBaseBackupManager.Exceptions
{
    /// <summary>
    /// The table backup file not found exception.
    /// </summary>
    [Serializable]
    public class TableBackupFileNotFoundException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TableBackupFileNotFoundException()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public TableBackupFileNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception who threw this exception.</param>
        public TableBackupFileNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Source and destinations of a context.</param>
        protected TableBackupFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}