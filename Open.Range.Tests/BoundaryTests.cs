using FluentAssertions;
using System;
using Xunit;

namespace Open.RangeTests;

public class BoundaryTests
{
	[Fact]
	public void Construction()
	{
		Assert.Throws<ArgumentNullException>(() => Boundary.Create((string)null, true));
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

	[Fact]
	public void ArgumentAssertion()
	{
		var i = Boundary.Create(1, true);
		var nullB = (IBoundary<int>)null;
		Assert.Throws<ArgumentNullException>(() => i.CompareLowTo((object)null));
		Assert.Throws<ArgumentNullException>(() => i.CompareLowTo(nullB));
		Assert.Throws<ArgumentException>(() => i.CompareLowTo("hello"));
		Assert.Throws<ArgumentNullException>(() => i.CompareHighTo((object)null));
		Assert.Throws<ArgumentNullException>(() => i.CompareHighTo(nullB));
		Assert.Throws<ArgumentException>(() => i.CompareHighTo("hello"));

		var s = Boundary.Create("hi", true);
		Assert.Throws<ArgumentException>(() => i.CompareLowTo(s));
		Assert.Throws<ArgumentException>(() => i.CompareHighTo(s));
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

		var ib = new B(value, inclusive);
		a.CompareLowTo(ib).Should().Be(0);
		a.CompareHighTo(ib).Should().Be(0);

		a.Equals((object)b).Should().BeTrue();
		a.Equals("nope").Should().BeFalse();
		a.CanRangeWith("nope").Should().BeFalse();
		(a == b).Should().BeTrue();
		(a >= b).Should().BeTrue();
		(a <= b).Should().BeTrue();
		a.CompareLowTo(a).Should().Be(0);
		a.CompareHighTo(a).Should().Be(0);
		a.GetHashCode().Should().Be(b.GetHashCode());

		ib = new B(value + 1, inclusive);
		a.CompareLowTo(ib).Should().Be(-1);
		a.CompareHighTo(ib).Should().Be(-1);
		ib = new B(value - 1, inclusive);
		a.CompareLowTo(ib).Should().Be(+1);
		a.CompareHighTo(ib).Should().Be(+1);

		ib = new B(value + 1, !inclusive);
		a.CompareLowTo(ib).Should().Be(-1);
		a.CompareHighTo(ib).Should().Be(-1);
		ib = new B(value - 1, !inclusive);
		a.CompareLowTo(ib).Should().Be(+1);
		a.CompareHighTo(ib).Should().Be(+1);

		var c = Boundary.Create(value + 1, inclusive);
		var d = Boundary.Create(value + 1, !inclusive);
		a.Equals((object)c).Should().BeFalse();
		(a != c).Should().BeTrue();
		(a < c).Should().BeTrue();
		(a <= c).Should().BeTrue();
		a.CompareLowTo(c).Should().Be(-1);
		a.CompareHighTo(c).Should().Be(-1);
		c.CompareLowTo(a).Should().Be(+1);
		c.CompareHighTo(a).Should().Be(+1);
		a.CompareLowTo(d).Should().Be(-1);
		a.CompareHighTo(d).Should().Be(-1);
		d.CompareLowTo(a).Should().Be(+1);
		d.CompareHighTo(a).Should().Be(+1);
		a.GetHashCode().Should().NotBe(c.GetHashCode());

		c = Boundary.Create(value - 1, inclusive);
		(a == c).Should().BeFalse();
		(a > c).Should().BeTrue();
		(a >= c).Should().BeTrue();
		a.GetHashCode().Should().NotBe(c.GetHashCode());


		d = Boundary.Create(value, !inclusive);
		d.Equals(c).Should().BeFalse();
		d.CompareTo(c).Should().BePositive();

		var i = inclusive ? +1 : -1;
		ib = new B(value, !inclusive);
		a.CompareLowTo(ib).Should().Be(-i);
		a.CompareHighTo(ib).Should().Be(+i);

		a.CompareLowTo(d).Should().Be(-i);
		a.CompareHighTo(d).Should().Be(+i);
		d.CompareLowTo(a).Should().Be(+i);
		d.CompareHighTo(a).Should().Be(-i);
		d.GetHashCode().Should().NotBe(c.GetHashCode());
		d.GetHashCode().Should().NotBe(a.GetHashCode());
		a.Equals(d).Should().BeFalse();
		Assert.Throws<ArgumentException>(
			() => a.CompareTo(d));


	}

	record B(object Value, bool Inclusive) : IBoundary
	{
		public int CompareHighTo(object other)
			=> throw new NotImplementedException();
		public int CompareLowTo(object other)
			=> throw new NotImplementedException();
	}
}
