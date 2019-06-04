# das-providerapprenticeshipservice

## Getting Started ##

* Clone das-providerapprenticeshipservice repo
* Open das-providerapprenticeshipservice solution - set Startup projects as Multiple - SFA.DAS.API, SFA.DAS.CloudService (this is for PAS API and PAS UI respectively)
* Publish the database project to local db server (use default db name "SFA.DAS.ProviderAgreementStatus.Database")
* Execute sql below to seed data
* Obtain cloud config
* Workaround for "rosylyn csc exe error" - 'Update-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -r'
* Start

Insert the following record into the ProviderAgreementStatus database in order to simulate the given Provider (in this case, _10005077_) having signed their agreement with the SFA. This unlocks all of the approval functionality.

```SQL
insert into ContractFeedEvent (Id, ProviderId, HierarchyType, FundingTypeCode, [Status], ParentStatus, UpdatedInFeed, CreatedDate)
values (NEWID(), '10005077', 'CONTRACT', 'LEVY', 'APPROVED', 'APPROVED', GETDATE(), GETDATE())
```
