# GenericServices.StatusGeneric

This [NuGet library](https://www.nuget.org/packages/GenericServices.StatusGeneric/) provides a way to return the status of a method/class that you run. It contains two main things

1. A IReadOnlyList of `Errors`, which may be empty. If the list is empty, then the `IsValid` property of the status will be true.
2. A `Message` which can be set by you (default value is "Success"), but if it has any errors the `Message` returns "Failed with nn errors".
3. The `IStatusGeneric<T>` version will return a `Result` which you can set, but returns `default(T)` if there are errors.

There are various methods to add errors to the `Errors` list, and a way to combine statuses which I show next.

*NOTE: There is a companion library called [EfCore.GenericServices.AspNetCore](https://github.com/JonPSmith/EfCore.GenericServices.AspNetCore) which can convert a GenericService into Model errors for ASP.NET Core MVC controllers or Razor pages, AND ASP.NET Core Web API - well worth looking at.*


## Simple usage examples

1. Returns just a status telling you if the method was successful. You can also set the `Message` in the status - default is "Success".

```c#
public IStatusGeneric NonStatusGenericString(string s)
{
    var status = new StatusGenericHandler();

    //add error and return immediately
    if (s == null)
        //You can return just an error message, but adding the property name
        //will improve the error feedback in ASP.NET Core etc.
        return status.AddError("input must not be null", nameof(s));

    status.Message = "All went well";

    //If no errors were added then its returns a IsValid status with the message
    //If there are errors then the Message says something like "Failed with 1 error"
    //and the HasErrors will be true, IsValid will be false
    return status;
}

```

2. Returns a status with a value

```c#
public IStatusGeneric<string> StatusGenericNumReturnString(int i)
{
    var status = new StatusGenericHandler<string>();

    //add error and return immediately
    if (i <= 0)
        return status.AddError("input must be positive", nameof(i));

    //This sets the Result property in the generic status
    status.SetResult(i.ToString());

    //If there are errors then the Result is set to the default value for generic type
    return status;
}
```

## Combining multiple checks

This approach makes sure all the errors are returned, which is best for the user

```c#
public IStatusGeneric CheckPassword(string password)
{
    var status = new StatusGenericHandler();

    //series of tests and then return all the errors together
    //Good because the user gets all the errors at once
    if (password.Length < 10)
        status.AddError("A password must be 10 or more in length",
            nameof(password));
    if (!password.Any(char.IsUpper))
        status.AddError("A password must contain an upper case character",
            nameof(password));
    if (!password.Any(char.IsLower))
        status.AddError("A password must contain a lower case character",
            nameof(password));
    if (!password.Any(char.IsDigit))
        status.AddError("A password must contain an number",
            nameof(password));
    
    return status;
}
```

## Combining errors from multiple statuses

Sometimes the testing for errors is best coded by called to other methods that return the IStatusGeneric interface. The code below shows a fictitious example of logging in with tests on the email, password and the actual login part.

```c#
public IStatusGeneric<string> Login
    (string email, string password)
{
    var status = new StatusGenericHandler<string>();

    status.CombineStatuses(
        CheckValidEmail(email));

    if (status.HasErrors)
        return status;

    if (status.CombineStatuses(
            CheckPassword(password)).HasErrors)
        return status;

    var loginStatus = LoginUser(email, password);
    status.CombineStatuses(loginStatus);

    status.SetResult(loginStatus.Result);

    return status;
}
```
  

## When unit testing...

If you are testing a service its always good to check that the service returns the status you expect. If you expect it to pass then you can use this following xUnit fluent test which will
1. Test that it was a successful result
2. Show you the errors in the unit test window

```c#
status.IsValid.ShouldBeTrue(status.GetAllErrors());
```