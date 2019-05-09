# das-providerapprenticeshipservice

### Setup

Insert the following record into the ProviderAgreementStatus database in order to simulate the given Provider (in this case, _10005077_) having signed their agreement with the SFA. This unlocks all of the approval functionality.

```SQL
insert into ContractFeedEvent (Id, ProviderId, HierarchyType, FundingTypeCode, [Status], ParentStatus, UpdatedInFeed, CreatedDate)
values (NEWID(), '10005077', 'CONTRACT', 'LEVY', 'APPROVED', 'APPROVED', GETDATE(), GETDATE())
```
