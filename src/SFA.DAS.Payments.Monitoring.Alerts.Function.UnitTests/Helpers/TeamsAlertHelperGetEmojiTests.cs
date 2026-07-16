using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.UnitTests.Helpers
{
    public class TeamsAlertHelperGetEmojiTests
    {
        [TestCase("Sev0", "🚨")]
        [TestCase("Sev1", "🚨")]
        [TestCase("Sev2", "⚠️")]
        [TestCase("Sev3", "✅")]
        [TestCase("Sev28", "")]
        
        public void GetEmojiReturnsCorrectEmojiCodeBasedOnSeverity(string input, string expectedOutput)
        {
            //Arrange
            var helper = new TeamsAlertHelper();

            //Act
            var act = helper.GetEmoji(input);

            //Assert
            act.Should().Be(expectedOutput);
        }
    }
}