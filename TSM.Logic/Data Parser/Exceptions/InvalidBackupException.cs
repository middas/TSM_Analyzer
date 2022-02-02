using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TSM.Logic.Data_Parser.Exceptions
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
