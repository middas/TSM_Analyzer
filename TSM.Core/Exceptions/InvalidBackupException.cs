using System.Runtime.Serialization;

namespace TSM.Core.Exceptions
{
    [Serializable]
    public class InvalidBackupException : Exception
    {
        public InvalidBackupException()
        {
        }

        public InvalidBackupException(string? message) : base(message)
        {
        }

        public InvalidBackupException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidBackupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}