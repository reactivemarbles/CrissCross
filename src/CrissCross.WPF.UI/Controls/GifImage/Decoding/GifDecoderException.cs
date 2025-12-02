// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>
/// GifDecoderException.
/// </summary>
/// <seealso cref="System.Exception" />
[Serializable]
public abstract class GifDecoderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GifDecoderException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected GifDecoderException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GifDecoderException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner.</param>
    protected GifDecoderException(string message, Exception inner)
        : base(message, inner)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GifDecoderException"/> class.
    /// </summary>
    private GifDecoderException()
    {
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    ///   <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.
    /// </returns>
    public override bool Equals(object? obj) => obj is GifDecoderException exception && Message == exception.Message && EqualityComparer<IDictionary>.Default.Equals(Data, exception.Data) && EqualityComparer<Exception>.Default.Equals(InnerException, exception.InnerException) && EqualityComparer<MethodBase>.Default.Equals(TargetSite, exception.TargetSite) && StackTrace == exception.StackTrace && HelpLink == exception.HelpLink && Source == exception.Source && HResult == exception.HResult;

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
        var hashCode = 1144832672;
        hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Message);
        hashCode = (hashCode * -1521134295) + EqualityComparer<IDictionary>.Default.GetHashCode(Data);
        hashCode = (hashCode * -1521134295) + HResult.GetHashCode();
        return hashCode;
    }
}
