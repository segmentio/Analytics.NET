using Segment;
using Segment.Test;

namespace UnitTest.Net35
{
	class Program
	{
		static void Main(string[] args)
		{
			// Just init SDK with a mock Write Key:
			Analytics.Initialize(Constants.MOCK_WRITE_KEY);
			Analytics.Client.Flush();
		}
	}
}
