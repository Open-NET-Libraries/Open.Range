# Open.Range

`Range<T>` (`.Low` and `.High`) implementation with useful extensions.

[![NuGet](https://img.shields.io/nuget/v/Open.Range.svg)](https://www.nuget.org/packages/Open.Range/)

## Structs & Interfaces

The basic structs are there for the majority use but there are other implementations that may benefit from added properties.

It is of key importance that a range is valid upon creation so the subsequent validations are not necessary.

### `readonly struct Boundary<T>`

Implements `IComparable<T>` and implicitly converts to `T` and declares a boolean `.Inclusive`. When using `Range<Boundary<T>>`, this allows for differentiating if a value can be equal to the boundary value or only up to it.

### `readonly struct Range<T>`

Where `T` implements `IComparable<T>` and the `.Low` must be less than or equal to the `.High` value.  

#### `Range<int>` Example

```cs
var intRange = Range.Create(5, 20);
```

#### `Range<Boundary<int>>` Examples

```cs
// 0 < value < 100
var between    = Range.Between(0, 100);
var aboveBelow = Range.Above(0).Below(100);

// 0 <= value <= 100
var inclusive  = Range.Include(0, 100);
var fromTo     = Range.From(0).To(100);

// 0 <= value < 100
var fromBelow  = Range.From(0).Below(100);

// 0 < value <= 100
var aboveTo    = Range.Above(0).To(100);
```

## Extensions

### `IRange<Boundary<T>>.Contains<T>(T value)`
Does the value (`T`) exist within the range?

### `IComparable<T>.IsInRange<T>(T min, T max)`
Is the value of the comparable in the range?

#### Examples
```cs
4.IsInRange(2, 6) // true
4.IsInRange(4, 6) // true
100.IsInRange(-5, 50) // false
100.IsInRange(-5, 100) // true
```

### `IComparable<T>.IsInBounds<T>(T min, T max)`
Is the value of the comparable between the bounds?

#### Examples
```cs
4.IsInRange(2, 6) // true
4.IsInRange(4, 6) // false
100.IsInRange(-5, 50) // false
100.IsInRange(-5, 100) // false
```