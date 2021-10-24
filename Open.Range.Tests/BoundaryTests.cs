using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Open.RangeTests;

public class BoundaryTests
{
	[Fact]
	public void Construction()
	{
		Assert.Throws<ArgumentException>(() => Boundary.Create(double.NaN, true));
		Assert.Throws<ArgumentException>(() => Boundary.Create(double.NaN, false));
		Assert.Throws<ArgumentException>(() => Boundary.Create(float.NaN, true));
		Assert.Throws<ArgumentException>(() => Boundary.Create(float.NaN, false));
	}

	[Fact]
	public void Deconstruction()
	{
		var (value, inclusive) = Boundary.Create(10, true);
		value.Should().Be(10);
		inclusive.Should().Be(true);
	}

	[Theory]
	[InlineData(10d, true)]
	[InlineData(-20d, true)]
	[InlineData(-10d, false)]
	[InlineData(20d, false)]
	public static void Equality(double value, bool inclusive)
	{
		var a = Boundary.Create(value, inclusive);
		var b = Boundary.Create(value, inclusive);
		a.Equals(b).Should().BeTrue();
		a.CompareTo(b).Should().Be(0);
		a.GetHashCode().Should().Be(b.GetHashCode());
		var c = Boundary.Create(value + 1, inclusive);
		a.Equals(c).Should().BeFalse();
		a.CompareTo(c).Should().BeNegative();
		a.GetHashCode().Should().NotBe(c.GetHashCode());
		c = Boundary.Create(value - 1, inclusive);
		a.Equals(c).Should().BeFalse();
		a.CompareTo(c).Should().BePositive();
		a.GetHashCode().Should().NotBe(c.GetHashCode());
		var d = Boundary.Create(value, !inclusive);
		d.Equals(c).Should().BeFalse();
		d.CompareTo(c).Should().BePositive();
		d.GetHashCode().Should().NotBe(c.GetHashCode());
		d.GetHashCode().Should().NotBe(a.GetHashCode());
		a.Equals(d).Should().BeFalse();
		Assert.Throws<ArgumentException>(
			()=> a.CompareTo(d));
	}

}
