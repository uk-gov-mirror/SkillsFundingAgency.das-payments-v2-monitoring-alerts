# Payments V2 Monitoring Alerts

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/DCT/_apis/build/status/GitHub/DAS-Functions/SkillsFundingAgency.das-payments-v2-monitoring-alerts?branchName=main)](https://dev.azure.com/sfa-gov-uk/DCT/_apis/build/status/GitHub/DAS-Functions/SkillsFundingAgency.das-payments-v2-monitoring-alerts?branchName=main)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/secure/RapidBoard.jspa?rapidView=782&projectKey=PV2)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3700621400/Provider+and+Employer+Payments+Payments+BAU)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)


The Payments V2 Monitoring Alerts function posts information on the operation of the Payments V2 system to the appropriate Slack channels for monitoring by the Payments team.

More information here: https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/2966454344/8.+Payments+v2+-+KPI+Alerts+DAS+Space

## How It Works

A set of alerts are configured in Application Insights by DevOps that will be triggered based on operational conditions, such as:

* Submission Job Failed
* Submission Metrics Generation Failed
* Estimated Provider Accuracy Outside Tolerance

When triggered, these alerts are set up to execute the Monitoring Alerts function using its HTTP endpoint, passing a JSON payload containing the Application Insights data related to the alert.

The function then calls the Slack API, posting the alert message, available metrics related to the alert, and a link to the Application Insights trace information.

## 🚀 Installation

### Pre-Requisites

Setup instructions can be found at the following link, which will help you set up your environment and access the correct repositories: https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/950927878/Development+Environment+-+Payments+V2+DAS+Space

### Config

For local running, create a file called `local.settings.json`

Populate as follows:

```
{
  "IsEncrypted": false,
  "Values": {
    "SlackBaseUrl": "https://hooks.slack.com",
    "AppInsightsAuthHeader": "x-api-key",
    "AppInsightsAuthValue": "api-key",
    "SlackChannelUri": "[CREATE A TEST SLACK CHANNEL INSTANCE AND CONFIGURE THE /services URL HERE]",
    "SlackChannelUri2": "[CREATE A TEST SLACK CHANNEL INSTANCE AND CONFIGURE THE /services URL HERE]"
  }
}
```

Slack JSON payloads can be tested using the Block Kit Builder application - https://app.slack.com/block-kit-builder



## 🔗 External Dependencies

N/A

## Technologies

* .NET6
* Azure Functions
* Application Insights

## 🐛 Known Issues

N/A
