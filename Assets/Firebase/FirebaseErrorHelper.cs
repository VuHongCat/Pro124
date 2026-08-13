using Firebase;
using Firebase.Auth;
using System;

public static class FirebaseErrorHelper
{
    public static string GetErrorMessage(Exception exception)
    {
        if (exception == null) return "An unknown error occurred.";

        // Firebase returns errors as AggregateException, so we need to extract the underlying FirebaseException
        Exception innerException = exception;
        while (innerException is AggregateException aggEx && aggEx.InnerException != null)
        {
            innerException = aggEx.InnerException;
        }

        if (innerException is FirebaseException firebaseEx)
        {
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    return "Please enter an email address.";
                case AuthError.MissingPassword:
                    return "Please enter a password.";
                case AuthError.WeakPassword:
                    return "Password is too weak. It must be at least 6 characters long.";
                case AuthError.InvalidEmail:
                    return "Invalid email format.";
                case AuthError.EmailAlreadyInUse:
                    return "This email address is already registered.";
                case AuthError.WrongPassword:
                    return "Incorrect password.";
                case AuthError.UserNotFound:
                    return "Account does not exist.";
                case AuthError.UserDisabled:
                    return "This account has been disabled.";
                case AuthError.NetworkRequestFailed:
                    return "Network connection error. Please check your internet connection.";
                case AuthError.TooManyRequests:
                    return "Too many requests. Please try again later.";
                case AuthError.OperationNotAllowed:
                    return "Email/password sign-in is not enabled.";
                case AuthError.Failure:
                    return "Invalid email or password.";
                default:
                    return $"Error code: {errorCode}";
            }
        }

        return exception.Message;
    }
}