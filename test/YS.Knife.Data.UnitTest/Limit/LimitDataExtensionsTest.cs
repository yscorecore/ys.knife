using System.Linq;
using FluentAssertions;
using Xunit;
namespace YS.Knife.Data.UnitTest
{
    
    public class LimitDataExtensionsTest
    {
        [Fact]
        public void ShouldGetListSourceWhenAsListSource()
        {
            var listSource = Enumerable.Range(1, 100).AsQueryable()
                .ToPagedList(50, 15).ToListSource();
            listSource.ContainsListCollection.Should().Be(true);
            listSource.GetList().Count.Should().Be(15);
        }

        [Fact]
        public void ShouldGetExpectedLimitData()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToPagedList(50, 15);
            data.HasNext.Should().Be(true);
            data.TotalCount.Should().Be(100);
            data.Limit.Should().Be(15);
            data.Offset.Should().Be(50);
            data.Items.Count.Should().Be(15);
            data.Items.First().Should().Be(51);
            data.Items.Last().Should().Be(65);

        }

        [Fact]
        public void ShouldGetExpectedLimitDataWhenAtStart()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToPagedList(0, 10);
            data.HasNext.Should().Be(true);
            data.TotalCount.Should().Be(100);
            data.Limit.Should().Be(10);
            data.Offset.Should().Be(0);
            data.Items.Count.Should().Be(10);
            data.Items.First().Should().Be(1);
            data.Items.Last().Should().Be(10);

        }
        [Fact]
        public void ShouldGetExpectedLimitDataWhenAtEnd()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToPagedList(90, 10);
            data.HasNext.Should().Be(false);
            data.TotalCount.Should().Be(100);
            data.Limit.Should().Be(10);
            data.Offset.Should().Be(90);
            data.Items.Count.Should().Be(10);
            data.Items.First().Should().Be(91);
            data.Items.Last().Should().Be(100);

        }

        [Fact]
        public void ShouldGetExpectedLimitDataWhenOverlopRange()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToPagedList(95, 15);
            data.HasNext.Should().Be(false);
            data.TotalCount.Should().Be(100);
            data.Limit.Should().Be(15);
            data.Offset.Should().Be(95);
            data.Items.Count.Should().Be(5);
            data.Items.First().Should().Be(96);
            data.Items.Last().Should().Be(100);
        }

        [Fact]
        public void ShouldGetExpectedLimitDataWhenOutRange()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToPagedList(120, 15);
            data.HasNext.Should().Be(false);
            data.TotalCount.Should().Be(100);
            data.Limit.Should().Be(15);
            data.Offset.Should().Be(120);
            data.Items.Count.Should().Be(0);
        }

    }
}
