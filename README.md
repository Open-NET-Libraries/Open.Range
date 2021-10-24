# Open.Range

`Range<T>` (`.Low` and `.High`) implementation with useful extensions.

[![NuGet](https://img.shields.io/nuget/v/Open.Range.svg)](https://www.nuget.org/packages/Open.Range/)

## `readonly struct Range<T>`

Where `T` implements `IComparable<T>` and the `.Low` must be less than or equal to the `.High` value.  

## `readonly struct Boundary<T>`

Implements `IComparable<T>` and implicitly converts to `T` and declares a boolean `.Inclusive`. When using `Range<Boundary<T>>`, this allows for differentiating if a value can be equal to the boundary value or only up to it.