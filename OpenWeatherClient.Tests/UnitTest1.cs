using System;
using Xunit;
using FluentAssertions;

namespace OpenWeatherClient.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var actual = true;
            actual.Should().BeTrue();
        }
    }
}
