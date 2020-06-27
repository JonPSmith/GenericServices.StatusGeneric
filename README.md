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


## Typical patterns

You can use StatusGeneric in a few ways, but here is an example of the patterns I often find myself using.

```c#
public IStatusGeneric<int> StatusGenericNum(int i)
{```
    var status = new StatusGenericHandler<int>();

    //series of tests and then return all the errors together
    //Good because the user gets all the errors at once
    if (i == 20)
        status.AddError("input must not be 20", nameof(i));
    if (i == 30)
        status.AddError("input must not be 30", nameof(i));
    if (i == 40)
        status.AddError("input must not be 40", nameof(i));
    if (i == 50)
        status.AddError("input must not be 50", nameof(i));
    if (!status.IsValid)
        return status;

    //add error and return immediately
    if (i <= 0)
        return status.AddError("input must be positive", nameof(i));

    //combine error from another non-StatusGeneric method and return if has errors
    status.CombineStatuses(NonStatusGenericString(i == 10 ? null : "good"));
    if (status.HasErrors)
        return status;

    //combine errors from another generic status, keeping the status to extract later
    var stringStatus = StatusGenericNumReturnString(i);
    if (status.CombineStatuses(stringStatus).HasErrors)
        return status;

    //Do something with the Result from the StatusGenericNumReturnString method
    var combinedNums = int.Parse(stringStatus.Result) + i;
    //Set the result to be returned from this method if there are no errors
    status.SetResult(combinedNums);

    //You can put tests just before a result would be returned  - any error will set the result to default value
    if (combinedNums <= 0)
        status.AddError("input must be positive", nameof(i));

    //If you return a IStatusGeneric<T> and there are errors then
    //1. the result will be set to default value for that type
    //2  The Message will be set to "Failed with xx errors"
    return status;
}