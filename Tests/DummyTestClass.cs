using FluentAssertions;
using NUnit.Framework;
using static System.Console;

namespace Tests
{
	[TestFixture]
	public class DummyTestClass
	{
		[Test]
		public void TestMethod()
		{
			int i = 5;
			i.Should().Be(5);
			WriteLine("Yo nibbas");
		}
	}
}
