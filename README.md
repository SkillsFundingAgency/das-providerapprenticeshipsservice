## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

## Provider Apprenticeship Service
<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status%2FAdd%20and%20Pay%2Fdas-providerapprenticeshipsservice?repoName=SkillsFundingAgency%2Fdas-providerapprenticeshipsservice&branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2183&repoName=SkillsFundingAgency%2Fdas-providerapprenticeshipsservice&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-providerapprenticeshipsservice&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-providerapprenticeshipsservice)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

## Developer Setup

### Pre-Requisites

You will need following on your local:
* A clone of this repository
* Visual studio or similar IDE 
* .Net 10.0 SDK
* Azurite or similar local storage emulator
* SQL Database
* Ensure you have the latest .pfx certificates for employer and provider in both **Local Machine** and **Current User** certificate stores (DevOps can assist)
* Administrator Access
* Open Visual Studio as an administrator

#### Publish Database

- Build the solution SFA.DAS.ProviderApprenticeshipService.sln
- While running Visual studio in Administrator Mode: Either use Visual Studio's `Publish Database` tool to publish the database project SFA.DAS.ProviderAgreementStatus.Database with name **SFA.DAS.ProviderAgreementStatus.Database** on **ProjectsV13** (or whatever local instance you are using)

	or

- Create a database manually named {{database name}} on {{local instance name}} and run each of the `.sql` scripts in the SFA.DAS.ProviderApprenticeshipService.Database project.

#### PAS Seed Data

Insert the following record into the ProviderAgreementStatus database in order to simulate the given Provider (in this case, _10005077_) having signed their agreement with the SFA. This unlocks all of the approval functionality.

```SQL
insert into ContractFeedEvent (Id, ProviderId, HierarchyType, FundingTypeCode, [Status], ParentStatus, UpdatedInFeed, CreatedDate)
values (NEWID(), '10005077', 'CONTRACT', 'LEVY', 'APPROVED', 'APPROVED', GETDATE(), GETDATE())
```

## Configuration

#### Automatically obtain config (Recommended)

- Use the [das-employer-config-updater](https://github.com/SkillsFundingAgency/das-employer-config-updater) to obtain the latest config

## Running the solution
- Ensure you have the SFA.DAS.Commitments project set up (follow its readme file)
- Open the solution SFA.DAS.ProviderApprenticeshipService.sln as **administrator**
- Open the solution SFA.DAS.Commitments.sln as **administrator**
- Close all instances of "Microsoft Azure storage emulator" within the system tray

- Set SFA.DAS.ProviderApprenticeshipService startup projects to 'Multiple startup projects'. The **only** projects set to start should be: 
  - SFA.DAS.PAS.Account.Api (PAS Account API)
  - SFA.DAS.ProviderApprenticeshipsService.Web (PAS Web UI)
  
- Clean the SFA.DAS.ProviderApprenticeshipService solution
- Rebuild the SFA.DAS.ProviderApprenticeshipService solution

- Run the SFA.DAS.Commitments.sln (this should automatically start the Microsoft Azure storage emulator), it normally starts with an error page. You can check if it's running by calling the "/api/healthcheck" endpoint which should return status code 200.

- Run the SFA.DAS.ProviderApprenticeshipService.sln, it may several minutes to start the first time.

- Once both solutions are fully running, open an incognito tab in any browser and navigate to the "misc helpers" website (Get link from Approvals / Continous Improvement team)
- Under the "Providers" tab, click the "Test-U-Good Provider Home" link. Select Pirean PreProd and you will be directed to a log in page.
- Enter the log in credentials (can be found on the "misc helpers" website) and you should be taken to the "Apprentices" section landing page.

**Note**: The above startup projects are Azure Cloud Service Definitions and the Service Configurations (*.cscfg) contain the per environment configuration settings; e.g. the 'EnvironmentName' which is used to locate the Azure Storage Configuration Settings.


## Configuration (Manual method)

- Get the following configuration json files (which are in non-public repositories):
  - [SFA.DAS.ProviderApprenticeshipService](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.ProviderApprenticeshipsService.json)
  - [SFA.DAS.ProviderUrlHelper](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.ProviderUrlHelper.json)
  - [SFA.DAS.ContractAgreements](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.ContractAgreements.json)
  - [SFA.DAS.PasAccountApi](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.PASAccountApi.json)

- Create a Configuration table in your (Development) local Azure Storage account.
- Add a row to the Configuration table for each configuraiton json file with fields:
  - PartitionKey: LOCAL, RowKey: SFA.DAS.ProviderApprenticeshipService_1.0, Data: {{[The contents of the local config json file](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.ProviderApprenticeshipsService.json)}}.
  - PartitionKey: LOCAL, RowKey: SFA.DAS.ProviderUrlHelper_1.0, Data: {{[The contents of the local config json file](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.ProviderUrlHelper.json)}}.
  - PartitionKey: LOCAL, RowKey: SFA.DAS.ContractAgreements_1.0, Data: {{[The contents of the local config json file](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.ContractAgreements.json)}}.
  - PartitionKey: LOCAL, RowKey: SFA.DAS.PasAccountApi_1.0, Data: {{[The contents of the local config json file](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.PASAccountApi.json)}}.

- Update Configuration to amend database connection strings
  - SFA.DAS.ProviderApprenticeshipService_1.0, Data : "Data Source={{local instance name}};Initial Catalog={{database name}};Integrated Security=True;Pooling=False;Connect Timeout=30" }
  - SFA.DAS.ContractAgreements_1.0, Data : "Data Source={{local instance name}};Initial Catalog={{database name}};Integrated Security=True;Pooling=False;Connect Timeout=30" }

**Note:** The employer config updater will automatically update database connection strings; however it should be used with caution as it will overwrite **any** manual changes.

#### To run a local copy you may also require 
To run a fully operational local service you will also require the following fully operational local services or mocks:

- [Employer Account API](https://github.com/SkillsFundingAgency/das-employerapprenticeshipsservice)
- [Commitments API](https://github.com/SkillsFundingAgency/das-commitments)
- [Reservations API](https://github.com/SkillsFundingAgency/das-reservations-api)
- [Provider Relationships API](https://github.com/SkillsFundingAgency/das-provider-relationships)

**Note** It is possible to configure the use of test environments however this comes with the usual issues of not having full control of the version under test and the data in a remote service.

It will be sufficient to configure test versions or mocks of the following services:

- [Apprenticeship Info Service API](https://github.com/SkillsFundingAgency/das-apprenticeship-programs-api)
- [Notifications API](https://github.com/SkillsFundingAgency/das-notifications)

#### And you may also require 
The following services can be activated (browsed too) from this service; if this is required then having fully operational
local services is recommended or having them configured to be the same test services as the required API's to avoid
confusion of different data sources.

- [Provider Commitments](https://github.com/SkillsFundingAgency/das-providercommitments)     
- [Reservations](https://github.com/SkillsFundingAgency/das-reservations)
- [Recruit](https://github.com/SkillsFundingAgency/das-recruit)
- [Provider Registrations](https://github.com/SkillsFundingAgency/das-provider-registrations)

### Jobs
The `SFA.DAS.PAS.Jobs` project contains functions to import providers from Roatp and to update users from DfE Sign In. 

#### Configuration
- Obtain the [SFA.DAS.PAS.Jobs.json](https://github.com/SkillsFundingAgency/das-employer-config/tree/master/das-providerapprenticeshipservice/SFA.DAS.PAS.Jobs.json) from the `das-employer-config` 
- Add a row to the Configuration table with fields: 
  - PartitionKey: LOCAL
  - RowKey: SFA.DAS.PAS.Jobs_1.0
  - Data: {The contents of the `SFA.DAS.PAS.Jobs.json` file}

#### Functions summary
| Function Name | Trigger | Description |
|---------------|---------|-------------|
| SynchroniseProvidersFunction | Timer | Imports provider from Roatp V1 API. |
| SynchroniseUsersFunction | Timer | Updates users from DfE Sign In. |

- In the `SFA.DAS.PAS.Jobs` project, add `local.settings.json` file with following content:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true;",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
    "ConfigNames": "SFA.DAS.PAS.Jobs,SFA.DAS.Provider.DfeSignIn",
    "Version": "1.0",
    "EnvironmentName": "LOCAL",
    "SynchroniseProvidersFunctionSchedule": "0 0 0 * * *",
    "SynchroniseUsersFunctionSchedule": "* */15 * * * *"
  }
}
```








