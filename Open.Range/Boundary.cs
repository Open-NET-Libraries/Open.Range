using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using static Open.Utility;

namespace Open;

/// <summary>
/// Exposes "Inclusive" property.
/// </summary>
public interface IBoundary
{
	/// <summary>
	/// The value of the boundary.
	/// </summary>
	object Value { get; }

	/// <summary>
	/// True if the boundary is inclusive; otherwise false.
	/// </summary>
	bool Inclusive { get; }

	/// <summary>
	/// Compares another boundary as if each is a lower boundary.
	/// </summary>
	/// <remarks>
	/// <em>Boundaries are only guaranteed to be equal when inclusive.<br/>
	/// If their values are equal but one of them is not inclusive, then they cannot be compared.
	/// But if compared under the context of lower or upper then a comparison can be resolved.
	/// </em></remarks>
	/// <param name="other">The other boundary to compare to.</param>
	/// <returns>0 if equal; +1 if this is greater than the other; -1 if less than the other.</returns>
	int CompareLowTo(object other);

	/// <summary>
	/// Compares another boundary as if each is an upper boundary.
	/// </summary>
	/// <inheritdoc cref="CompareLowTo(IBoundary)"/>
	int CompareHighTo(object other);
}

/// <inheritdoc />
public interface IBoundary<T> : IBoundary
	where T : IComparable<T>
{
	/// <inheritdoc cref="IBoundary.Value" />
	new T Value { get; }

	/// <inheritdoc cref="IBoundary.CompareLowTo(IBoundary)"/>
	int CompareLowTo(IBoundary<T> other);

	/// <inheritdoc cref="IBoundary.CompareHighTo(IBoundary)"/>
	int CompareHighTo(IBoundary<T> other);
}

/// <summary>
/// Defines a value that can be used as a boundary.<br/>
/// By specifying as "Inclusive", the value can be included or excluded from a set or range.
/// </summary>
/// <remarks>
/// Implicity converts to <typeparamref name="T"/>.
/// When using Range&lt;Boundary&lt;T&gt;&gt;, this allows for differentiating if a value can be equal to the boundary value or only up to it.
/// </remarks>
public readonly record struct Boundary<T>
	: IBoundary<T>, IComparable<Boundary<T>>, ICanRange<Boundary<T>>
	where T : IComparable<T>
{
	/// <summary>
	/// Constructs a boundary value.
	/// </summary>
	/// <param name="value">The value of the boundary.</param>
	/// <param name="inclusive"></param>
	public Boundary(T value, bool inclusive)
	{
		if (value is null) throw new ArgumentNullException(nameof(value));
		if (IsNaN(value)) throw new ArgumentException("Boundaries cannot be NaN.");
		Value = value;
		Inclusive = inclusive;
	}

	/// <inheritdoc />
	public T Value { get; }

	/// <summary>
	/// True if the boundary is inclusive; otherwise false.
	/// </summary>
	public bool Inclusive { get; }

	[ExcludeFromCodeCoverage]
	object IBoundary.Value => Value;

	/// <inheritdoc />
	public void Deconstruct(out T value, out bool inclusive)
	{
		value = Value;
		inclusive = Inclusive;
	}

	public int CompareTo(Boundary<T> other)
	{
		var c = Value.CompareTo(other.Value);
		if (Inclusive != other.Inclusive && c == 0)
			throw new ArgumentException("Cannot compare an inclusive against a non-inclusive of equal value.");
		return c;
	}

	/// <summary>
	/// Returns true if the boundary values are not equal or both are inclusive.
	/// </summary>
	public bool CanRangeWith(Boundary<T> other)
		=> Inclusive && other.Inclusive
		|| Value.CompareTo(other.Value) != 0;

	/// <inheritdoc />
	public bool CanRangeWith(object other)
		=> other is Boundary<T> o
		&& CanRangeWith(o);

	public override string ToString()
		=> $"Boundary<{typeof(T)}>({Value}, {Inclusive})";

	/// <inheritdoc />
	public int CompareLowTo(IBoundary<T> other)
	{
		if (other is null) throw new ArgumentNullException(nameof(other));
		var c = Value.CompareTo(other.Value);
		if (Inclusive == other.Inclusive || c != 0) return c;
		return Inclusive && !other.Inclusive ? -1 : +1;
	}

	/// <inheritdoc />
	public int CompareHighTo(IBoundary<T> other)
	{
		if (other is null) throw new ArgumentNullException(nameof(other));
		Contract.EndContractBlock();

		var c = Value.CompareTo(other.Value);
		if (Inclusive == other.Inclusive || c != 0) return c;
		return Inclusive && !other.Inclusive ? +1 : -1;
	}

	/// <inheritdoc />
	public int CompareLowTo(object other)
	{
		if (other is IBoundary<T> bT) return CompareLowTo(bT);
		if (other is null) throw new ArgumentNullException(nameof(other));
		if (other is not IBoundary b) throw new ArgumentException($"Expected IBoundary. Actual: {other.GetType()}", nameof(other));
		if (b.Value is not T t) throw new ArgumentException($"Expected other.Value of type {typeof(T)}. Actual: {b.Value.GetType()}", nameof(other));
		Contract.EndContractBlock();

		var c = Value.CompareTo(t);
		if (Inclusive == b.Inclusive || c != 0) return c;
		return Inclusive && !b.Inclusive ? -1 : +1;
	}

	/// <inheritdoc />
	public int CompareHighTo(object other)
	{
		if (other is IBoundary<T> bT) return CompareHighTo(bT);
		if (other is null) throw new ArgumentNullException(nameof(other));
		if (other is not IBoundary b) throw new ArgumentException($"Expected IBoundary. Actual: {other.GetType()}", nameof(other));
		if (b.Value is not T t) throw new ArgumentException($"Expected other.Value of type {typeof(T)}. Actual: {b.Value.GetType()}", nameof(other));
		Contract.EndContractBlock();

		var c = Value.CompareTo(t);
		if (Inclusive == b.Inclusive || c != 0) return c;
		return Inclusive && !b.Inclusive ? +1 : -1;
	}

	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Provided by 'Value' property.")]
	public static implicit operator T(Boundary<T> boundary) => boundary.Value;

	/// <inheritdoc />
	public static bool operator <(Boundary<T> left, Boundary<T> right) => left.CompareTo(right) < 0;

	/// <inheritdoc />
	public static bool operator <=(Boundary<T> left, Boundary<T> right) => left.CompareTo(right) <= 0;

	/// <inheritdoc />
	public static bool operator >(Boundary<T> left, Boundary<T> right) => left.CompareTo(right) > 0;

	/// <inheritdoc />
	public static bool operator >=(Boundary<T> left, Boundary<T> right) => left.CompareTo(right) >= 0;
}

public static class Boundary
{
	public static Boundary<T> Create<T>(T value, bool inclusive)
		where T : IComparable<T>
		=> new(value, inclusive);
}