# GenericServices.StatusGeneric

This provides a way to return the status of a method/class that you run. It contains two main things

1. A IReadOnlyList of `Errors`, which may be empty. If the list is empty, then the `IsValid` property of the status will be true.
2. A `Message` which can be set by you (default value is "Success"), but if it has any errors the `Message` returns "Failed with nn errors".
3. The `IStatusGeneric<T>` version will return a `Result` which you can set, but returns `default(T)` if there are errors.

There are various methods to add errors to the `Errors` list, and a way to combine statuses.

