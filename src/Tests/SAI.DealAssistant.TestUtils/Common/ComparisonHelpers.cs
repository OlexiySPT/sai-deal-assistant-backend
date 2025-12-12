using Sai.DealAssistant.Domain.Entities.Samples;
using Xunit;

namespace SAI.DealAssistant.TestUtils.Common
{
	public static class ComparisonHelpers
	{
		public static void AssertSampleEmployeeListEqualToSource(
			IEnumerable<SampleEmployee> source, IEnumerable<SampleEmployee> result)
		{
			Assert.True(source.Count() > 0);
			Assert.Equal(source.Count(), result.Count());

			for (int i = 0; i < source.Count(); i++)
			{
				result.ToArray()[i].AssertEqualsTo(source.ToArray()[i]);
			}
		}

		public static void AssertEqualsTo(this SampleEmployee employee, SampleEmployee src)
		{
			Assert.Equal(employee?.FirstName, src?.FirstName);
			Assert.Equal(employee?.LastName, src?.LastName);
			Assert.Equal(employee?.Email, src?.Email);
		}
	}
}
