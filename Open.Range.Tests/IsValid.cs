using FluentAssertions;
using System;
using Xunit;

namespace Open.RangeTests;

public static class IsValid
{
	[Theory]
	[InlineData(1d, 2d)]
	[InlineData(-100d, -6d)]
	[InlineData(-50d, 40d)]
	public static void CheckValidity(double low, double high)
	{
		Range.IsValid(low, high).Should().BeTrue();

		var a = Boundary.Create(low, true);
		Range.IsValid(a, a).Should().BeTrue();

		var b = Boundary.Create(high, true);
		Range.AssertIsValid(a, b).Should().BeTrue();

		var c = Boundary.Create(low, false);
		Range.IsValid(c, b).Should().BeTrue();
		Range.IsValid(c, a).Should().BeFalse();
		Range.IsValid(c, c).Should().BeFalse();
		Assert.Throws<ArgumentException>(() => Range.AssertIsValid(a, c));

		var d = Boundary.Create(high, false);
		Range.IsValid(b, d).Should().BeFalse();
	}

	[Theory]
	[InlineData(5d, 2d)]
	[InlineData(-100d, -600d)]
	[InlineData(50d, 40d)]
	[InlineData(double.NaN, 40d)]
	[InlineData(50d, double.NaN)]
	public static void CheckInvalid(double low, double high)
	{
		Range.IsValid(low, high).Should().BeFalse();
		if (double.IsNaN(low) || double.IsNaN(high)) return;
		var a = Boundary.Create(low, false);
		var b = Boundary.Create(high, false);
		Range.IsValid(a, b).Should().BeFalse();
		a = Boundary.Create(low, true);
		Range.IsValid(a, b).Should().BeFalse();
		b = Boundary.Create(high, true);
		Range.IsValid(a, b).Should().BeFalse();

		Range.IsValid(new TestComparable1(), new TestComparable1())
			.Should().BeFalse();
		Assert.Throws<InvalidOperationException>(
			()=>Range.IsValid(new TestComparable2(), new TestComparable2()));
	}

	class TestComparable1 : IComparable<TestComparable1>
	{
		public int CompareTo(TestComparable1 other)
			=> throw new ArgumentException("Expected");
	}

	class TestComparable2 : IComparable<TestComparable2>
	{
		public int CompareTo(TestComparable2 other)
			=> throw new InvalidOperationException();
	}
}
