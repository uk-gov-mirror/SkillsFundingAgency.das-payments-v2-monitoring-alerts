using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Models;

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

        private static readonly DateTime Timestamp = new(2003, 11, 11, 10, 10, 10);

        private object _result;

        [SetUp]
        public void SetUp()
        {
            _result = new TeamsAlertHelper().BuildAlertPayload(new AlertParameters(){
                AccountedForPayments = "accountedForPayments",
                AlertEmoji = AlertEmoji,
                Timestamp = Timestamp,
                JobId = JobId,
                AcademicYear = AcademicYear,
                CollectionPeriod = CollectionPeriod,
                AlertTitle = AlertTitle,
                AppInsightsSearchResultsUiLink = AppInsightsSearchResultsUiLink
            });
        }

        [Test]
        public void BuildTeamsPayload_ConstructsExpectedFactValuesForTimestampJobAcademicYearAndCollectionPeriod()
        {
            //Act
            var items = (List<object>)_result.GetType().GetProperty("items")?.GetValue(_result, null);
            items.Should().NotBeNull();

            var factsContainer = items.Single(x => (string)x.GetType().GetProperty("type")?.GetValue(x, null) == "FactSet");
            factsContainer.Should().NotBeNull();

            var facts = (List<object>)factsContainer.GetType().GetProperty("facts")?.GetValue(factsContainer, null);
            facts.Should().NotBeNull();

            var factValuesByTitle = facts.ToDictionary(
                x => (string)x.GetType().GetProperty("title")?.GetValue(x, null),
                x => (string)x.GetType().GetProperty("value")?.GetValue(x, null));

            //Assert
            factValuesByTitle.Should().ContainKey("Timestamp").WhoseValue.Should().Be(Timestamp.ToString("f"));
            factValuesByTitle.Should().ContainKey("Job").WhoseValue.Should().Be(JobId);
            factValuesByTitle.Should().ContainKey("Academic Year").WhoseValue.Should().Be(AcademicYear);
            factValuesByTitle.Should().ContainKey("Collection Period").WhoseValue.Should().Be(CollectionPeriod);
        }
    }
}
