using FluentAssertions;
using System;
using Xunit;

namespace Open.RangeTests;

public class RangeBasic
{
	[Theory]
	[InlineData(5d, -1d)]
	[InlineData(15d, 2d)]
	[InlineData(double.NaN, 2d)]
	[InlineData(15d, double.NaN)]
	public void ConstructionInvalid(double low, double high)
	{
		Assert.Throws<ArgumentException>(
			() => Range.Create(low, high));
		if (double.IsNaN(low) || double.IsNaN(high)) return;
		Assert.Throws<ArgumentException>(
			() => Range.Create(Boundary.Create(low, true), Boundary.Create(high, true)));
	}

	[Fact]
	public void Deconstruction()
	{
		var (low, high) = Range.Create(10d, 20d);
		low.Should().Be(10d);
		high.Should().Be(20d);
	}

	[Fact]
	public void ToStringConsistency()
	{
		var r = Range.From(10d).To(20d);
		var (low, high) = r;
		var lowString = ValidateBoundaryToString(low);
		var highString = ValidateBoundaryToString(high);
		r.ToString().Should()
			.Be($"Range<Open.Boundary`1[System.Double]>[{lowString} - {highString}]");

		static string ValidateBoundaryToString<T>(Boundary<T> b)
			where T : IComparable<T>
		{
			var s = b.ToString();
			s.Should().Be($"Boundary<{typeof(T)}>({b.Value}, {b.Inclusive})");
			return s;
		}
	}
}
