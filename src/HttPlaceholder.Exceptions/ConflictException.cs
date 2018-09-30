using System;

namespace HttPlaceholder.Exceptions
{
   public class ConflictException : Exception
   {
      public ConflictException(string message) : base($"Conflict detected: {message}")
      {
      }
   }
}
