# SFA.DAS.ProviderApprenticeshipsService

## Developer Setup

#### Requirements

- Ensure you have the latest .pfx certificates for employer and provider in both **Local Machine** and **Current User** certificate stores (DevOps can assist)
- Install [.NET Framework 4.6.2](https://dotnet.microsoft.com/download/dotnet-framework/net462)
- Install [Visual Studio 2019](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
    - Azure development
- Install [SQL Server 2017 Developer Edition](https://go.microsoft.com/fwlink/?linkid=853016)
- Install [SQL Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- Install [Azure Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409) (Make sure you are on atleast v5.3)
- Install [Azure Storage Explorer](http://storageexplorer.com/) 
- Administrator Access

#### Setup

- Clone this repository
- Open Visual Studio as an administrator

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
  - SFA.DAS.PAS.Account.Api
  - SFA.DAS.ProviderApprenticeshipsService.Web
  
- Clean the SFA.DAS.ProviderApprenticeshipService solution
- Rebuild the SFA.DAS.ProviderApprenticeshipService solution

- Run the SFA.DAS.Commitments.sln (this should automatically start the Microsoft Azure storage emulator), it normally starts with an error page. You can check if it's running by calling the "/api/healthcheck" endpoint which should return status code 200.

- Run the SFA.DAS.ProviderApprenticeshipService.sln, it may several minutes to start the first time.

- Once both solutions are fully running, open an incognito tab in any browser and navigate to the port it is running on [https://localhost:{port}/account].Account details can be found on the "Repositories and Environments" confluence page.
- Under the "Sign in with one of these accounts" option, Select Pirean PreProd and you will be directed to a log in page.
- Enter the log in credentials (can be found on the "Repositories and Environments" confluence page) and you should be taken to the "Apprentices" section landing page.

**Note**: The above startup projects are Azure Cloud Service Definitions and the Service Configurations (*.cscfg) contain the per environment configuration settings; e.g. the 'EnvironmentName' which is used to locate the Azure Storage Configuration Settings.


## Configuration (Manual method)

- Get the following configuration json files (which are in non-public repositories):
  - [SFA.DAS.ProviderApprenticeshipService](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.ProviderApprenticeshipsService.json)
  - [SFA.DAS.ProviderUrlHelper](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.ProviderUrlHelper.json)
  - [SFA.DAS.ContractAgreements](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.ContractAgreements.json)

- Create a Configuration table in your (Development) local Azure Storage account.
- Add a row to the Configuration table for each configuraiton json file with fields:
  - PartitionKey: LOCAL, RowKey: SFA.DAS.ProviderApprenticeshipService_1.0, Data: {{[The contents of the local config json file](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.ProviderApprenticeshipsService.json)}}.
  - PartitionKey: LOCAL, RowKey: SFA.DAS.ProviderUrlHelper_1.0, Data: {{[The contents of the local config json file](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.ProviderUrlHelper.json)}}.
  - PartitionKey: LOCAL, RowKey: SFA.DAS.ContractAgreements_1.0, Data: {{[The contents of the local config json file](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.ContractAgreements.json)}}.
  - PartitionKey: LOCAL, RowKey: SFA.DAS.PasAccountApiClient_1.0, Data: {{[The contents of the local config json file](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-providerapprenticeshipservice/SFA.DAS.PASAccountApiClient.json)}}.

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

## Provider Apprenticeship Service

Licensed under the [MIT license](https://github.com/SkillsFundingAgency/das-providerapprenticeshipsservice/blob/master/LICENSE)

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|Provider Apprenticeship Service (PAS) Web|
| Info | A service enabling the Apprenticeship Service to be managed from the Provider perspective. |
| Build | [![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Add%20and%20Pay/das-providerapprenticeshipsservice?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=150&branchName=master) |
| Web  | https://localhost:44347/ |

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|PAS Account Internal API|
| Info | An internal API giving access to user accounts and agreements for providers. |
| Build | [![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Add%20and%20Pay/das-providerapprenticeshipsservice?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=150&branchName=master) |
| Web  | https://localhost:44378/ |

|               | <div style="width:500px"></div>              |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)| PAS Account Api Client |
| Info  | .Net Framework client library for the internal Account API |
| Build  | [![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.PAS.Account.Api.Client)](https://www.nuget.org/packages/SFA.DAS.PAS.Account.Api.Client)  |

|               | <div style="width:500px"></div>              |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)| PAS Account Api Client V2 |
| Info  | .Net Standard client library for the internal Account API |
| Build  | [![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.PAS.Account.Api.ClientV2)](https://www.nuget.org/packages/SFA.DAS.PAS.Account.Api.ClientV2)  |

|               | <div style="width:500px"></div>              |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)| PAS Account Api Types |
| Info  | .Net Standard types library for client libraries |
| Build  | [![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.PAS.Account.Api.Types)](https://www.nuget.org/packages/SFA.DAS.PAS.Account.Api.Types)  |

See [Support Site]() for EFSA developer details.





