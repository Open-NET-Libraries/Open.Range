using System;
using System.Collections.Generic;

namespace Open.Range;

/// <summary>
/// Defines a value that can be used as a boundary.<br/>
/// By specifying as inclusive, the value can be included or excluded from a set or range.
/// </summary>
/// <remarks>Implicity converts to <typeparamref name="T"/>.</remarks>
public readonly struct Boundary<T> : IEquatable<Boundary<T>>
{
	/// <summary>
	/// Constructs a boundary value.
	/// </summary>
	/// <param name="value">The value of the boundary.</param>
	/// <param name="inclusive"></param>
	public Boundary(T value, bool inclusive)
	{
		Value = value;
		Inclusive = inclusive;
	}
	
	/// <summary>
	/// The value of the boundary.
	/// </summary>
	public T Value { get; }

	/// <summary>
	/// True if the boundary is inclusive; otherwise false.
	/// </summary>
	public bool Inclusive { get; }

	/// <inheritdoc />
	public bool Equals(Boundary<T> other)
		=> Inclusive == other.Inclusive
		&& EqualityComparer<T>.Default.Equals(Value, other.Value);

	/// <inheritdoc />
	public override bool Equals(object? obj)
		=> obj is Boundary<T> boundary && Equals(boundary);

	/// <inheritdoc />
#if NETSTANDARD2_1_OR_GREATER
	public override int GetHashCode()
		=> HashCode.Combine(Value, Inclusive);
#else
	public override int GetHashCode()
	{
		int hashCode = 593764356;
		hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Value);
		hashCode = hashCode * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(Inclusive);
		return hashCode;
	}
#endif
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Provided by 'Value' property.")]
	public static implicit operator T(Boundary<T> boundary) => boundary.Value;

	/// <inheritdoc />
	public static bool operator ==(Boundary<T> left, Boundary<T> right) => left.Equals(right);

	/// <inheritdoc />
	public static bool operator !=(Boundary<T> left, Boundary<T> right) => !left.Equals(right);
}
