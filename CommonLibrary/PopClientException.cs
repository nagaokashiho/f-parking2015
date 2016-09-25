using System;

namespace CommonLibrary
{
    /// <summary>
    /// PopClient の例外クラスです。
    /// </summary>
    public class PopClientException : Exception
    {
        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <remarks></remarks>
        public PopClientException()
        {
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="message"></param>
        /// <remarks></remarks>
        public PopClientException(string message) : base(message)
        {
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <remarks></remarks>
        public PopClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
