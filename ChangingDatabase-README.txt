Places to change in Ad Hoc Reporting in order to point at a new DB:
- 'dbo.User' view in ManagementInformation database
- 'GetReferenceData1' stored proc in ManagementInformation database
- 'Adep<App>Entities' e.g. 'AdepUcbEntities' ManagementInformation web config connection string
- '<APP>DATA' e.g.'UcbDATA'SSRS datasource in the root of the relevant report server