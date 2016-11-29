using System;
using System.Runtime.Serialization;

namespace Stubbornium
{
    public static class Assertions
    {
        public static void AreEqual(bool expected, bool actual, string message = null)
        {
            if (expected != actual)
                throw AssertionException.EqualException(expected.ToString(), actual.ToString(), message ?? string.Empty);
        }

        public static void AreNotEqual(object expected, object actual, string message = null)
        {            
            if ((expected == null && actual == null) || (expected != null && expected.Equals(actual)))
                throw AssertionException.NotEqualException(expected.ToString(), actual.ToString(), message ?? string.Empty);
        }

        [Serializable]
        public class AssertionException : Exception
        {
            public static AssertionException EqualException(string expected, string actual, string userMessage)
            {
                return new AssertionException($"Assertion failed. {userMessage}{Environment.NewLine}Expected: {expected}{Environment.NewLine}But was: {actual}");
            }

            public AssertionException()
            {
            }

            public AssertionException(string message) : base(message)
            {
            }

            public AssertionException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected AssertionException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public static AssertionException NotEqualException(string notExpected, string actual, string message)
            {
                return new AssertionException($"Assertion failed. {message}{Environment.NewLine}Not expected: {notExpected}{Environment.NewLine}    But was: {actual}");
            }
        }
    }
}