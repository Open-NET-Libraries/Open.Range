using FluentAssertions;
using System;
using Xunit;

namespace Open.RangeTests;

public static class RangeEquality
{
	[Theory]
	[InlineData(1d, 5d)]
	[InlineData(1d, 1d)]
	[InlineData(-19d, 7d)]
	public static void IsEqual(double low, double high)
	{
		{
			var a = Range.From(low).To(high);
			var b = Range.Include(low, high);
			(a == b).Should().BeTrue();
			(a != b).Should().BeFalse();
			a.Equals((object)b).Should().BeTrue();
			a.Equals(null).Should().BeFalse();
			a.GetHashCode().Equals(b.GetHashCode())
				.Should().BeTrue();

			if (low < high)
			{
				a = Range.Above(low).Below(high);
				b = Range.Above(low).Below(high);
				a.Equals(b).Should().BeTrue();

				a = Range.From(low).Below(high);
				b = Range.From(low).Below(high);
				a.Equals(b).Should().BeTrue();

				a = Range.Above(low).To(high);
				b = Range.Above(low).To(high);
				a.Equals(b).Should().BeTrue();
			}
			else
			{
				Assert.Throws<ArgumentException>(
					() => Range.Above(low).Below(high));
				Assert.Throws<ArgumentException>(
					() => Range.From(low).Below(high));
				Assert.Throws<ArgumentException>(
					() => Range.Above(low).To(high));
			}

		}

		{
			var a = Range.WithValue(low, high, 1);
			var b = Range.WithValue(low, high, 1);
			a.Equals(b).Should().BeTrue();
		}
	}
}
