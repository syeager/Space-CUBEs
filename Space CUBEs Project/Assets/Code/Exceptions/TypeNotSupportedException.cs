// Steve Yeager
// 5.20.2014

using System;


/// <summary>
/// Exception for when a certain function doesn't support the passed type.
/// </summary>
public class TypeNotSupportedException : NotSupportedException
{
    #region Readonly Fields

    public readonly object objectNotSupported;
    public readonly string message;

    #endregion

    #region Exception Overrides

    public override string Message
    {
        get { return message; }
    }

    #endregion

    #region Constructors

    public TypeNotSupportedException(object objectNotSupported, string message = "")
        : base(message)
    {
        this.objectNotSupported = objectNotSupported;
        this.message = string.Format("Type: \"{0}\" not supported. {1}", objectNotSupported.GetType().Name, message);
    }

    #endregion
}