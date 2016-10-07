# plato
ASP.NET Core Development



Plato_Editions

- Id
- Name
- Discriminators
- IsActive
- IsDeleted
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId
- DeletedDate
- DeletedUserId

Plato_Tennets

- Id
- Name
- SafeUrlName
- Description
- ConnectionString
- IsActive
- IsDeleted
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId
- DeletedDate
- DeletedUserId

Plato_Sites

- Id
- TennetId
- EditionId
- Name
- Theme
- Description
- SafeUrlName
- IsActive
- IsDeleted
- ApiKey
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId
- DeletedDate
- DeletedUserId

Plato_SiteMigrations

- Id
- SiteId
- AppVersion
- SchemaUpdates
- CreatedDate
- CreatedUserId

Plato_LoginAttempts

- Id
- UserId
- UserNameOrEmailAddress
- ClientIpAddress
- ClientName
- BrowserInfo
- DateCreated

Plato_Spaces

- Id
- SiteId
- ParentId
- Name
- SafeUrlName
- ModuleId
- IsActive
- IsDeleted
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId
- DeletedDate
- DeletedUserId

Plato_SpaceRoles

- Id
- SiteId
- SpaceId
- UserId
- RoleId
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId

Plato_OrganizationUnits

- Id
- SiteId
- Name
- IsDeleted
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId
- DeletedDate
- DeletedUserId

Plato_Settings

- Id
- SiteId
- Key
- Value
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId

Plato_Users

- Id
- SiteId
- RoleId
- UserName
- DisplayName
- EmailAddress
- Password
- PasswordSalt
- SamAccountName
- Culture
- EmailConfirmationCode
- PasswordResetCode
- IsEmailConfirmed
- IsBanned
- IsSpam
- IsDeleted
- IsLocked
- ApiKey
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId
- DeletedDate
- DeletedUserId
- BannedDate
- BannedUserId
- LockedDate
- LockedUserId
- LastLoginDate

Plato_UserDetails

- Id
- SiteId
- UserId
- Visits
- PhotoId
- PhotoUrl
- BannerUrl
- Reputation
- 


Plato_UserFields

- Id
- SiteId
- UserId
- Key
- Values
- Value
- FieldTypeName
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId

Plato_UserSettings

- Id
- UserId
- Key
- Value
- CreatedDate
- ModifiedDate

Plato_Roles

- ID
- Name
- IsAdministrator
- IsEmployee
- IsDeleted
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId
- DeletedDate
- DeletedUserId

Plato_UserRoles

- Id
- UserId
- RoleId
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId

Plato_Entities

- Id
- SiteId
- ParentId
- UserId
- Title
- IsQueued
- IsDeleted
- IsSpam
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId
- DeletedDate
- DeletedUserId

Plato_AuditLog

- Id
- EntityId
- SiteId
- UserId
- TableName
- ColumnName
- Values

Plato_EntityDetails

- Id
- SiteId
- EntityId
- Views
- Replies
- UpVotes
- DownVotes
- Mentions
- Reactions

Plato_EntityMessage

- Id
- SiteId
- EntityId
- Message
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId
- DeletedDate
- DeletedUserId

Plato_EntityMentions

- Id
- EntityId
- UserId

Plato_Labels

- Id
- ParentId
- SiteId
- Name
- BackColor
- ForeColor

Plato_EntityLabels

- Id
- EntityId
- LabelId

Plato_EntityViews

- Id
- UserId
- EntityId
- ViewDate

Plato_Notifications

- Id
- Key
- UserId

Plato_Reactions

- Id
- SiteId
- Name
- Icon
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId
- IsVisible
- IsDeleted
- DeletedDate
- DeletedUserId

Plato_EntityReactions

- Id
- EntityId
- ReactionId
- UserId
- CreatedDate
- CreatedUserId
- ModifiedDate
- ModifiedUserId
- DeletedDate
- DeletedUserId

