using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using System;
using System.Linq;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Models;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Models.TeamsPayload;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.UnitTests.Helpers
{
    public class TeamsAlertHelperBuildPayloadTests
    {
        private const string AlertEmoji = "alert_emoji";
        private const string AlertTitle = "alertTitle";
        private const string AppInsightsSearchResultsUiLink = "linktoui";
        private const string JobId = "jobid";
        private const string AcademicYear = "academicYear";
        private const string CollectionPeriod = "collectionPeriod";
        private const string CollectionPeriodPayments = "collectionPeriodPayments";
        private const string YearToDatePayments = "yearToDatePayments";
        private const string NumberOfLearners = "numberOfLearners";
        private const string AccountedForPayments = "accountedForPayments";

        private static readonly DateTime Timestamp = new(2003, 11, 11, 10, 10, 10);

        private TeamsCardContainer _result;

        [SetUp]
        public void SetUp()
        {
            _result = new TeamsAlertHelper().BuildAlertPayload(new AlertParameters()
            {
                AccountedForPayments = AccountedForPayments,
                AlertEmoji = AlertEmoji,
                Timestamp = Timestamp,
                JobId = JobId,
                AcademicYear = AcademicYear,
                CollectionPeriod = CollectionPeriod,
                CollectionPeriodPayments = CollectionPeriodPayments,
                YearToDatePayments = YearToDatePayments,
                NumberOfLearners = NumberOfLearners,
                AlertTitle = AlertTitle,
                AppInsightsSearchResultsUiLink = AppInsightsSearchResultsUiLink
            });
        }

        [Test]
        public void BuildTeamsPayload_Contains_Expected_Fact_Values_For_Mandatory_And_Optional_Fact_Containers()
        {
            //Act
            var items = _result.Items;
            var factSets = items.Where(x => x.Type == "FactSet").ToList();

            var mandatoryFactsContainer = factSets[0];
            var optionalFactsContainer = factSets[1];

            var mandatoryFacts = mandatoryFactsContainer.Facts;
            var optionalFacts = optionalFactsContainer.Facts;

            var mandatoryFactValuesByTitle = mandatoryFacts.ToDictionary(
                x => x.Title,
                x => x.Value);

            var optionalFactValuesByTitle = optionalFacts.ToDictionary(
                x => x.Title,
                x => x.Value);

            //Assert
            items.Should().NotBeNull();
            factSets.Should().HaveCount(2);
            mandatoryFactsContainer.Should().NotBeNull();
            optionalFactsContainer.Should().NotBeNull();
            mandatoryFacts.Should().NotBeNull();
            optionalFacts.Should().NotBeNull();

            mandatoryFactValuesByTitle.Should().ContainKey("Timestamp").WhoseValue.Should().Be(Timestamp.ToString("f"));
            mandatoryFactValuesByTitle.Should().ContainKey("Job").WhoseValue.Should().Be(JobId);
            mandatoryFactValuesByTitle.Should().ContainKey("Academic Year").WhoseValue.Should().Be(AcademicYear);
            mandatoryFactValuesByTitle.Should().ContainKey("Collection Period").WhoseValue.Should().Be(CollectionPeriod);

            optionalFactValuesByTitle.Should().ContainKey("Previous Payments Year To Date").WhoseValue.Should().Be($"£{YearToDatePayments}");
            optionalFactValuesByTitle.Should().ContainKey("Collection Period Payments").WhoseValue.Should().Be($"£{CollectionPeriodPayments}");
            optionalFactValuesByTitle.Should().ContainKey("In Learning").WhoseValue.Should().Be(NumberOfLearners);
            optionalFactValuesByTitle.Should().ContainKey("Accounted For Payments").WhoseValue.Should().Be($"£{AccountedForPayments}");
        }
    }
}
