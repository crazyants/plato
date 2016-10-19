

CREATE TABLE Plato_AuditLog
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT NOT NULL,
SiteId							INT NOT NULL,
TableName						NVARCHAR(75) DEFAULT('') NOT NULL,
ColumnName						NVARCHAR(75) DEFAULT('') NOT NULL,
[Values]						NVARCHAR(75) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_AuditLog_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_Editions
(
Id								INT IDENTITY(1,1) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Discriminators					NVARCHAR(255) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDisabled						BIT DEFAULT(0) NOT NULL,
DisabledDate					DATETIME2 NULL,
DisabledUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Editions_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_Sites
(
Id								INT IDENTITY(1,1) NOT NULL,
EditionId						INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(MAX) DEFAULT('') NOT NULL,
SafeUrlName						NVARCHAR(100) DEFAULT('') NOT NULL,
DefaultTheme					NVARCHAR(50) DEFAULT('') NOT NULL,
ApiKey							NVARCHAR(255) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
IsDisabled						BIT DEFAULT(0) NOT NULL,
DisabledDate					DATETIME2 NULL,
DisabledUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Sites_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_SiteMigrations
(
Id								INT IDENTITY(1,1) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
AppVersion						NVARCHAR(10) DEFAULT ('') NOT NULL,
SchemaUpdates					NVARCHAR(MAX) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_SiteMigrations_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_LoginAttempts
(
Id								INT IDENTITY(1,1) NOT NULL,
UserNameOrEmail					NVARCHAR(255) DEFAULT('') NOT NULL,
ClientIpAddress					NVARCHAR(255) DEFAULT('') NOT NULL,
ClientName						NVARCHAR(255) DEFAULT('') NOT NULL,
BrowserInfo						NVARCHAR(255) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CONSTRAINT PK_Plato_LoginAttempts_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_Spaces
(
Id								INT IDENTITY(1,1) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
ParentId						INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
NameLocalized					NVARCHAR(100) DEFAULT('') NOT NULL,
Description						NVARCHAR(MAX) DEFAULT('') NOT NULL,
DescriptionLocalized			NVARCHAR(100) DEFAULT('') NOT NULL,
SafeUrlName						NVARCHAR(MAX) DEFAULT('') NOT NULL,
ModuleId						NVARCHAR(100) DEFAULT('') NOT NULL,
IconClassName					NVARCHAR(50) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
IsDisabled						BIT DEFAULT(0) NOT NULL,
DisabledDate					DATETIME2 NULL,
DisabledUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Spaces_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_SpaceRoles
(
Id								INT IDENTITY(1,1) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
RoleId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_SpaceRoles_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_SpaceFollows
(
Id								INT IDENTITY(1,1) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
OptOutCode						NVARCHAR(255) DEFAULT('') NOT NULL,
CONSTRAINT PK_Plato_SpaceFollows_Id PRIMARY KEY CLUSTERED ( Id )
)


CREATE TABLE Plato_Teams
(
Id								INT IDENTITY(1,1) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
IsDisabled						BIT DEFAULT(0) NOT NULL,
DisabledDate					DATETIME2 NULL,
DisabledUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Teams_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_Products
(
Id								INT IDENTITY(1,1) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
Image							IMAGE NOT NULL,
WebSiteUrl						NVARCHAR(255) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
IsDisabled						BIT DEFAULT(0) NOT NULL,
DisabledDate					DATETIME2 NULL,
DisabledUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Products_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_TeamProducts
(
Id								INT IDENTITY(1,1) NOT NULL,
TeamId							INT DEFAULT (0) NOT NULL,
ProductId						INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_TeamProducts_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_UserTeams
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
TeamId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_UserTeams_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_Settings
(
Id								INT IDENTITY(1,1) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
[Key]							NVARCHAR(255) DEFAULT('') NOT NULL,
Value							NVARCHAR(MAX) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Settings_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_Users
(
Id								INT IDENTITY(1,1) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
UserName						NVARCHAR(255) DEFAULT('') NOT NULL,
Email							NVARCHAR(255) DEFAULT('') NOT NULL,
DisplayName						NVARCHAR(255) DEFAULT('') NOT NULL,
SamAccountName					NVARCHAR(255) DEFAULT('') NOT NULL,
CONSTRAINT PK_Plato_Users_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_UserPhoto
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
BackColor						NVARCHAR(20) DEFAULT('') NOT NULL,
ForeColor						NVARCHAR(20) DEFAULT('') NOT NULL,
ContentBlob						IMAGE NOT NULL,
ContentType						NVARCHAR(75) DEFAULT('') NOT NULL,
ContentLength					FLOAT DEFAULT(0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_UserPhoto_Id PRIMARY KEY CLUSTERED ( Id )
)

GO




CREATE TABLE Plato_UserSecret
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
Password						NVARCHAR(255) DEFAULT('') NOT NULL,
Salts							NVARCHAR(255) DEFAULT('') NOT NULL,
CONSTRAINT PK_Plato_UserSecret_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_UserDetail
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
EditionId						INT DEFAULT (0) NOT NULL,
RoleId							INT DEFAULT (0) NOT NULL,
TeamId							INT DEFAULT (0) NOT NULL,
TimeZoneOffset					FLOAT DEFAULT (0) NOT NULL,
ObserveDST						BIT DEFAULT (0) NOT NULL,
Culture							NVARCHAR(50) DEFAULT('') NOT NULL,
FirstName						NVARCHAR(100) DEFAULT('') NOT NULL,
LastName						NVARCHAR(100) DEFAULT('') NOT NULL,
WebSiteUrl						NVARCHAR(100) DEFAULT('') NOT NULL,
ApiKey							NVARCHAR(255) DEFAULT('') NOT NULL,
Visits							INT DEFAULT (0) NOT NULL,
Answers							INT DEFAULT (0) NOT NULL,
Entities						INT DEFAULT (0) NOT NULL,
Replies							INT DEFAULT (0) NOT NULL,
Reactions						INT DEFAULT (0) NOT NULL,
Mentions						INT DEFAULT (0) NOT NULL,
Follows							INT DEFAULT (0) NOT NULL,
Badges							INT DEFAULT (0) NOT NULL,
ReputationRank					INT DEFAULT (0) NOT NULL,
ReputationPoints				INT DEFAULT (0) NOT NULL,
Banner							IMAGE NULL,
ClientIpAddress					NVARCHAR(255) DEFAULT('') NOT NULL,
ClientName						NVARCHAR(255) DEFAULT('') NOT NULL,
EmailConfirmationCode			NVARCHAR(255) DEFAULT('') NOT NULL,
PasswordResetCode				NVARCHAR(255) DEFAULT('') NOT NULL,
IsEmailConfirmed				BIT DEFAULT(0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
IsBanned						BIT DEFAULT(0) NOT NULL,
BannedDate						DATETIME2 NULL,
BannedUserId					INT DEFAULT (0) NOT NULL,
IsLocked						BIT DEFAULT(0) NOT NULL,
LockedDate						DATETIME2 NULL,
LockedUserId					INT DEFAULT (0) NOT NULL,
UnLockDate						DATETIME2 NULL,
IsSpam							BIT DEFAULT(0) NOT NULL,
SpamDate						DATETIME2 NULL,
SpamUserId						INT DEFAULT (0) NOT NULL,
LastLoginDate					DATETIME2 NULL,
CONSTRAINT PK_Plato_UserDetail_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_UserFollows
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
IsStreamDisabled				BIT DEFAULT (0) NOT NULL,
IsEmailDisabled					BIT DEFAULT (0) NOT NULL,
OptOutCode						NVARCHAR(255) DEFAULT('') NOT NULL,
CONSTRAINT PK_Plato_UserFollows_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_TeamFollows
(
Id								INT IDENTITY(1,1) NOT NULL,
TeamId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
IsStreamDisabled				BIT DEFAULT (0) NOT NULL,
IsEmailDisabled					BIT DEFAULT (0) NOT NULL,
OptOutCode						NVARCHAR(255) DEFAULT('') NOT NULL,
CONSTRAINT PK_Plato_TeamFollows_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_ProductFollows
(
Id								INT IDENTITY(1,1) NOT NULL,
ProductId						INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
IsStreamDisabled				BIT DEFAULT (0) NOT NULL,
IsEmailDisabled					BIT DEFAULT (0) NOT NULL,
OptOutCode						NVARCHAR(255) DEFAULT('') NOT NULL,
CONSTRAINT PK_Plato_ProductFollows_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_UserFields
(
Id								INT IDENTITY(1,1) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
NameLocalizaed					NVARCHAR(100) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
DescriptionLocalizaed			NVARCHAR(100) DEFAULT('') NOT NULL,
[Values]						NVARCHAR(MAX) DEFAULT('') NOT NULL,
FieldTypeId						INT DEFAULT (0) NOT NULL,
SortOrder						INT DEFAULT (0) NOT NULL,
RegExValidation					NVARCHAR(255) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_UserFields_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_UserFieldValues
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
FieldId							INT DEFAULT (0) NOT NULL,
Value							NVARCHAR(255) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_UserFieldValues_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_UserSettings
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
[Key]							NVARCHAR(255) DEFAULT('') NOT NULL,
Value							NVARCHAR(255) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_UserSettings_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_Roles
(
Id								INT IDENTITY(1,1) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
PermissionId					INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
HtmlPrefix						NVARCHAR(255) DEFAULT('') NOT NULL,
HtmlSuffix						NVARCHAR(255) DEFAULT('') NOT NULL,
IsAdministrator					BIT DEFAULT(0) NOT NULL,
IsEmployee						BIT DEFAULT(0) NOT NULL,
IsAnonymous						BIT DEFAULT(0) NOT NULL,
IsMember						BIT DEFAULT(0) NOT NULL,
IsWaitingConfirmation			BIT DEFAULT(0) NOT NULL,
IsBanned						BIT DEFAULT(0) NOT NULL,
SortOrder						INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Roles_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_UserRoles
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
RoleId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_UserRoles_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_UserFavourites
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_UserFavourites_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_Entities
(
Id								INT IDENTITY(1,1) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
ParentId						INT DEFAULT (0) NOT NULL,
WorkFlowId						INT DEFAULT (0) NOT NULL,
Title							NVARCHAR(255) DEFAULT('') NOT NULL,
Body							NVARCHAR(MAX) DEFAULT('') NOT NULL,
Abstract						NVARCHAR(255) DEFAULT('') NOT NULL,
ImageUrl						NVARCHAR(255) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsQueued						BIT DEFAULT(0) NOT NULL,
QueuedDate						DATETIME2 NULL,
QueuedUserId					INT DEFAULT (0) NOT NULL,
IsSpam							BIT DEFAULT(0) NOT NULL,
SpamDate						DATETIME2 NULL,
SpamUserId						INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
IsAnswer						BIT DEFAULT(0) NOT NULL,
AnswerDate						DATETIME2 NULL,
AnswerUserId					INT DEFAULT (0) NOT NULL,
IsClosed						BIT DEFAULT(0) NOT NULL,
ClosedDate						DATETIME2 NULL,
ClosedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Entities_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_EntityDetails
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
Views							INT DEFAULT (0) NOT NULL,
Replies							INT DEFAULT (0) NOT NULL,
UpVoteReactions					INT DEFAULT (0) NOT NULL,
DownVoteReactions				INT DEFAULT (0) NOT NULL,
Mentions						INT DEFAULT (0) NOT NULL,
Follows							INT DEFAULT (0) NOT NULL,
Favourites						INT DEFAULT (0) NOT NULL,
Notes							INT DEFAULT (0) NOT NULL,
Assignees						INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_EntityDetails_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_EntityFollows
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
OptOutCode						NVARCHAR(255) DEFAULT('') NOT NULL,
CONSTRAINT PK_Plato_EntityFollows_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_EntityAssignees
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
UserId							INT DEFAULT(0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_EntityAssignees_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_EntityFields
(
Id								INT IDENTITY(1,1) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
NameLocalizaed					NVARCHAR(100) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
DescriptionLocalizaed			NVARCHAR(100) DEFAULT('') NOT NULL,
[Values]						NVARCHAR(MAX) DEFAULT('') NOT NULL,
FieldTypeId						INT DEFAULT (0) NOT NULL,
SortOrder						INT DEFAULT (0) NOT NULL,
RegExValidation					NVARCHAR(255) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_EntityFields_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_EntityFieldValues
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
FieldId							INT DEFAULT (0) NOT NULL,
Value							NVARCHAR(255) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_EntityFieldValues_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_EntityMentions
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_EntityMentions_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_EntityParticipants
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
Participations					INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_EntityParticipants_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_Labels
(
Id								INT IDENTITY(1,1) NOT NULL,
ParentId						INT DEFAULT (0) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
NameLocalizaed					NVARCHAR(100) DEFAULT('') NOT NULL,
Description						NVARCHAR(MAX) DEFAULT('') NOT NULL,
DescriptionLocalizaed			NVARCHAR(100) DEFAULT('') NOT NULL,
BackColor						NVARCHAR(20) DEFAULT('') NOT NULL,
ForeColor						NVARCHAR(20) DEFAULT('') NOT NULL,
IconClassName					NVARCHAR(MAX) DEFAULT('') NOT NULL,
Image							IMAGE NOT NULL,
SortOrder						INT DEFAULT (0) NOT NULL,
Follows							INT DEFAULT (0) NOT NULL,
Entities						INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Labels_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_LabelFollows
(
Id								INT IDENTITY(1,1) NOT NULL,
LabelId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
OptOutCode						NVARCHAR(255) DEFAULT('') NOT NULL,
CONSTRAINT PK_Plato_LabelFollows_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_EntityLabels
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
LabelId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_EntityLabels_Id PRIMARY KEY CLUSTERED ( Id )
)


GO


CREATE TABLE Plato_EntityProducts
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
ProductId						INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_EntityProducts_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_EntityViews
(
Id								INT IDENTITY(1,1) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ObayExpiration					BIT DEFAULT(0) NOT NULL,
CONSTRAINT PK_Plato_EntityViews_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_Reactions
(
Id								INT IDENTITY(1,1) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
NameLocalizaed					NVARCHAR(100) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
DescriptionLocalizaed			NVARCHAR(100) DEFAULT('') NOT NULL,
IconBlob						IMAGE NULL,
Points							INT DEFAULT (0) NOT NULL,
IsUpVote						BIT DEFAULT (0) NOT NULL,
IsDownVote						BIT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
IsDisabled						BIT DEFAULT(0) NOT NULL,
DisabledDate					DATETIME2 NULL,
DisabledUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Reactions_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_EntityReactions
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
ReactionId						INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_EntityReactions_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_EntityNotes
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
Body							NVARCHAR(MAX) DEFAULT('') NOT NULL,
Abstract						NVARCHAR(255) DEFAULT('') NOT NULL,
ImageUrl						NVARCHAR(255) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_EntityNotes_Id PRIMARY KEY CLUSTERED ( Id )
)

GO



CREATE TABLE Plato_Permissions
(
Id								INT IDENTITY(1,1) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
Claims							NVARCHAR(MAX) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
IsRequired						BIT DEFAULT(0) NOT NULL,
CONSTRAINT PK_Plato_Permissions_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_Attachments
(
Id								INT IDENTITY(1,1) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
ContentBlob						IMAGE NOT NULL,
ContentType						NVARCHAR(255) DEFAULT('') NOT NULL,
ContentLength					FLOAT DEFAULT(0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Attachments_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_EntityAttachments
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
AttachmentId					INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_EntityAttachments_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_WorkFlows
(
Id								INT IDENTITY(1,1) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
IsRequired						BIT DEFAULT(0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
IsDisabled						BIT DEFAULT(0) NOT NULL,
DisabledDate					DATETIME2 NULL,
DisabledUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_WorkFlows_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_WorkFlowTeams
(
Id								INT IDENTITY(1,1) NOT NULL,
WorkFlowId						INT DEFAULT (0) NOT NULL,
TeamId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_WorkFlowTeams_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_WorkFlowSteps
(
Id								INT IDENTITY(1,1) NOT NULL,
WorkFlowId						INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
Settings						NVARCHAR(MAX) DEFAULT('') NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
IsDisabled						BIT DEFAULT(0) NOT NULL,
DisabledDate					DATETIME2 NULL,
DisabledUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_WorkFlowSteps_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_WorkFlowNextSteps
(
Id								INT IDENTITY(1,1) NOT NULL,
StepId							INT DEFAULT (0) NOT NULL,
NextStepId						INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_WorkFlowNextSteps_Id PRIMARY KEY CLUSTERED ( Id )
)

GO



CREATE TABLE Plato_Badges
(
Id								INT IDENTITY(1,1) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
BadgeType						INT DEFAULT (0) NOT NULL,
Level							INT DEFAULT (0) NOT NULL,
Threshold						INT DEFAULT (0) NOT NULL,
Image							IMAGE NOT NULL,
ReputationPoints				INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
IsDisabled						BIT DEFAULT(0) NOT NULL,
DisabledDate					DATETIME2 NULL,
DisabledUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Badges_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_UserBadges
(
Id								INT IDENTITY(1,1) NOT NULL,
BadgeId							INT DEFAULT (0) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CONSTRAINT PK_Plato_UserBadges_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_Reputations
(
Id								INT IDENTITY(1,1) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
ReputationPoints				INT DEFAULT (0) NOT NULL,
Image							IMAGE NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
IsDeleted						BIT DEFAULT(0) NOT NULL,
DeletedDate						DATETIME2 NULL,
DeletedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_Reputations_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_UserReputations
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
Action							INT DEFAULT (0) NOT NULL,
ReputationPoints				INT DEFAULT (0) NOT NULL,
[Day]							INT DEFAULT (0) NOT NULL,
[Month]							INT DEFAULT (0) NOT NULL,
[Year]							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_UserReputations_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

---------------

GO


CREATE PROCEDURE [plato_sp_SelectUser] (
@Id int
) AS
SET NOCOUNT ON 

SELECT * FROM
Plato_Users WITH (nolock) 
WHERE (Id = @Id)
	
EXEC plato_sp_SelectUserSecretByUserId @Id

EXEC plato_sp_SelectUserDetailByUserId @Id

EXEC plato_sp_SelectUserPhotoByUserId @Id

RETURN









GO

------------------------

GO

CREATE PROCEDURE [plato_sp_SelectUserSecret] (
@Id int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_UserSecret WITH (nolock)
WHERE (Id = @Id)
	
RETURN

GO

-----------------------

GO


CREATE PROCEDURE [plato_sp_SelectUserSecretByUserId] (
@UserId int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_UserSecret WITH (nolock)
WHERE (UserId = @UserId)
	
RETURN


GO

------------------------

GO


CREATE PROCEDURE [plato_sp_SelectUserDetail] (
@Id int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_UserDetail WITH (nolock)
WHERE (Id = @Id)
	
RETURN

GO

------------------------

GO


CREATE PROCEDURE [plato_sp_SelectUserDetailByUserId] (
@UserId int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_UserDetail WITH (nolock)
WHERE (UserId = @UserId)
	
RETURN


GO

--------------------------

GO

CREATE PROCEDURE [plato_sp_SelectUserPhoto] (
@Id int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_UserPhoto WITH (nolock)
WHERE (Id = @Id)
	
RETURN

GO

------------------------

GO

CREATE PROCEDURE [plato_sp_SelectUserPhotoByUserId] (
@UserId int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_UserPhoto WITH (nolock)
WHERE (UserId = @UserId)
	
RETURN


GO

------------------------

GO


CREATE PROCEDURE [plato_sp_InsertUpdateUser] (
	@Id int,
	@SiteId int,
	@UserName nvarchar(255),
	@Email nvarchar(255),
	@DisplayName nvarchar(255),
	@SamAccountName nvarchar(255)
) AS

SET NOCOUNT ON 

DECLARE @intIdentity int;

IF EXISTS( 
	SELECT Id 
	FROM Plato_Users 
	WHERE (Id = @Id)
	)
BEGIN

	-- UPDATE
	UPDATE Plato_Users SET
		SiteId = @SiteId,
		UserName = @UserName,
		Email = @Email,
		DisplayName = @DisplayName,
		SamAccountName = @SamAccountName
	WHERE (Id = @Id);

	SET @intIdentity = @Id;

END
ELSE
BEGIN

	-- INSERT
	INSERT INTO Plato_Users (
		SiteId,
		UserName,
		Email,
		DisplayName,
		SamAccountName
	) VALUES (
		@SiteId,
		@UserName,
		@Email,
		@DisplayName,
		@SamAccountName 
	);

	SET @intIdentity = SCOPE_IDENTITY();

END

SELECT @intIdentity

RETURN



GO

------------------------

GO

CREATE PROCEDURE [plato_sp_InsertUpdateUserSecret] (
	@Id int,
	@UserId int,
	@Password nvarchar(255),
	@Salts nvarchar(255)
) AS

SET NOCOUNT ON 

DECLARE @intIdentity int;

IF EXISTS( 
	SELECT Id 
	FROM Plato_UserSecret 
	WHERE (Id = @Id)
	)
BEGIN

	-- UPDATE
	UPDATE Plato_UserSecret SET
		UserId = @UserId,
		[Password] = @Password,
		Salts = @Salts	
	WHERE (Id = @Id);

	SET @intIdentity = @Id;

END
ELSE
BEGIN

	-- INSERT
	INSERT INTO Plato_UserSecret (
		UserId,
		[Password],
		Salts		
	) VALUES (
		@UserId,
		@Password,
		@Salts	 
	);

	SET @intIdentity = SCOPE_IDENTITY();

END

SELECT @intIdentity;

RETURN


GO

-------------------

GO

CREATE PROCEDURE [plato_sp_InsertUpdateUserPhoto] (
	@Id int,
	@UserId int,
	@Name nvarchar(255),
	@BackColor nvarchar(20),
	@ForeColor nvarchar(20),
	@ContentBlob image,
	@ContentType nvarchar(75),
	@ContentLength float,
	@CreatedDate DateTime2,
	@CreatedUserId int,
	@ModifiedDate DateTime2,
	@ModifiedUserId int
) AS

SET NOCOUNT ON 

DECLARE @intIdentity int;

IF EXISTS( 
	SELECT Id 
	FROM Plato_UserPhoto
	WHERE (Id = @Id)
	)
BEGIN

	-- UPDATE
	UPDATE Plato_UserPhoto SET
		UserId = @UserId,
		Name = @Name,
		BackColor = @BackColor,
		ForeColor = ForeColor,
		ContentBlob = @ContentBlob,
		ContentType = @ContentType,
		ContentLength = @ContentLength,
		CreatedDate = @CreatedDate,
		CreatedUserId = @CreatedUserId,
		ModifiedDate = @ModifiedDate,
		ModifiedUserId = @ModifiedUserId
	WHERE (Id = @Id);

	SET @intIdentity = @Id;

END
ELSE
BEGIN

	-- INSERT
	INSERT INTO Plato_UserPhoto (
		UserId,
		Name,
		BackColor,
		ForeColor,
		ContentBlob,
		ContentType,
		ContentLength,
		CreatedDate,
		CreatedUserId,
		ModifiedDate,
		ModifiedUserId
	) VALUES (
		@UserId,
		@Name,
		@BackColor,
		@ForeColor,
		@ContentBlob,
		@ContentType,
		@ContentLength,
		@CreatedDate,
		@CreatedUserId,
		@ModifiedDate,
		@ModifiedUserId
	);

	SET @intIdentity = SCOPE_IDENTITY();

END

SELECT @intIdentity;

RETURN


GO

------------------------------

GO


CREATE PROCEDURE [plato_sp_InsertUpdateUserDetail] (
	@Id int,
	@UserId int,
	@EditionId int,
	@RoleId int,
	@TeamId int,
	@TimeZoneOffset float,
	@ObserveDST bit,
	@Culture nvarchar(50),
	@FirstName nvarchar(100),
	@LastName nvarchar(100),
	@WebSiteUrl nvarchar(100),
	@ApiKey nvarchar(255),
	@Visits int,
	@Answers int,
	@Entities int,
	@Replies int,
	@Reactions int,
	@Mentions int,
	@Follows int,
	@Badges int,
	@ReputationRank int,
	@ReputationPoints int,
	@Banner image,
	@ClientIpAddress  nvarchar(255),
	@ClientName  nvarchar(255),
	@EmailConfirmationCode  nvarchar(255),
	@PasswordResetCode  nvarchar(255),
	@IsEmailConfirmed bit,
	@CreatedDate datetime2,
	@CreatedUserId int,
	@ModifiedDate datetime2,
	@ModifiedUserId int,
	@IsDeleted bit,
	@DeletedDate datetime2,
	@DeletedUserId int,
	@IsBanned bit,
	@BannedDate datetime2,
	@BannedUserId int,
	@IsLocked bit,
	@LockedDate datetime2,
	@LockedUserId int,
	@UnLockDate datetime2,
	@IsSpam bit,
	@SpamDate datetime2,
	@SpamUserId int,
	@LastLoginDate datetime2
) AS

SET NOCOUNT ON 

DECLARE @intIdentity int;

IF EXISTS( 
	SELECT Id 
	FROM Plato_UserDetail 
	WHERE (Id = @Id)
	)
BEGIN

	-- UPDATE
	UPDATE Plato_UserDetail SET
		UserId = @UserId,
		EditionId = @EditionId,
		RoleId = @RoleId,
		TeamId = @TeamId,
		TimeZoneOffset = @TimeZoneOffset,
		ObserveDST = @ObserveDST,
		Culture = @Culture,
		FirstName = @FirstName,
		LastName = @LastName,
		WebSiteUrl = @WebSiteUrl,
		ApiKey = @ApiKey,
		Visits = @Visits,
		Answers = @Answers,
		Entities = @Entities,
		Replies = @Replies,
		Reactions = @Reactions,
		Mentions = @Mentions,
		Follows = @Follows,
		Badges = @Badges,
		ReputationRank = @ReputationRank,
		ReputationPoints = @ReputationPoints,
		Banner = @Banner,
		ClientIpAddress = @ClientIpAddress,
		ClientName = @ClientName,
		EmailConfirmationCode = @EmailConfirmationCode,
		PasswordResetCode = @PasswordResetCode,
		IsEmailConfirmed = @IsEmailConfirmed,
		CreatedDate = @CreatedDate,
		CreatedUserId = @CreatedUserId,
		ModifiedDate = @ModifiedDate,
		ModifiedUserId = @ModifiedUserId,
		IsDeleted = @IsDeleted,
		DeletedDate = @DeletedDate,
		DeletedUserId = @DeletedUserId,
		IsBanned = @IsBanned,
		BannedDate = @BannedDate,
		BannedUserId = @BannedUserId,
		IsLocked = @IsLocked,
		LockedDate = @LockedDate,
		LockedUserId = @LockedUserId,
		UnLockDate = @UnLockDate,
		IsSpam = @IsSpam,
		SpamDate = @SpamDate,
		SpamUserId = @SpamUserId,
		LastLoginDate = @LastLoginDate
	WHERE (Id = @Id);

	SET @intIdentity = @Id;

END
ELSE
BEGIN

	-- INSERT
	INSERT INTO Plato_UserDetail (
		UserId,
		EditionId,
		RoleId,
		TeamId,
		TimeZoneOffset,
		ObserveDST,
		Culture,
		FirstName,
		LastName,
		WebSiteUrl,
		ApiKey,
		Visits,
		Answers,
		Entities,
		Replies,
		Reactions,
		Mentions,
		Follows,
		Badges,
		ReputationRank,
		ReputationPoints,
		Banner,
		ClientIpAddress,
		ClientName,
		EmailConfirmationCode,
		PasswordResetCode,
		IsEmailConfirmed,
		CreatedDate,
		CreatedUserId,
		ModifiedDate,
		ModifiedUserId,
		IsDeleted,
		DeletedDate,
		DeletedUserId,
		IsBanned,
		BannedDate,
		BannedUserId,
		IsLocked,
		LockedDate,
		LockedUserId,
		UnLockDate,
		IsSpam,
		SpamDate,
		SpamUserId,
		LastLoginDate
	) VALUES (
		@UserId,
		@EditionId,
		@RoleId,
		@TeamId,
		@TimeZoneOffset,
		@ObserveDST,
		@Culture,
		@FirstName,
		@LastName,
		@WebSiteUrl,
		@ApiKey,
		@Visits,
		@Answers,
		@Entities,
		@Replies,
		@Reactions,
		@Mentions,
		@Follows,
		@Badges,
		@ReputationRank,
		@ReputationPoints,
		@Banner,
		@ClientIpAddress,
		@ClientName,
		@EmailConfirmationCode,
		@PasswordResetCode ,
		@IsEmailConfirmed,
		@CreatedDate,
		@CreatedUserId,
		@ModifiedDate,
		@ModifiedUserId,
		@IsDeleted,
		@DeletedDate,
		@DeletedUserId,
		@IsBanned,
		@BannedDate,
		@BannedUserId,
		@IsLocked,
		@LockedDate,
		@LockedUserId,
		@UnLockDate,
		@IsSpam,
		@SpamDate,
		@SpamUserId,
		@LastLoginDate
	);

	SET @intIdentity = SCOPE_IDENTITY();

END

SELECT @intIdentity;

RETURN


GO

--------------------------

GO



CREATE PROCEDURE [plato_sp_InsertUpdateSetting] (
	@Id int,
	@SiteId int,
	@SpaceId int,
	@Key nvarchar(255),
	@Value nvarchar(max),
	@CreatedDate datetime2,
	@CreatedUserId int,
	@ModifiedDate datetime2,
	@ModifiedUserId int
) AS

SET NOCOUNT ON 

DECLARE @intIdentity int;

IF EXISTS( 
	SELECT Id 
	FROM Plato_Settings 
	WHERE (Id = @Id)
	)
BEGIN

	-- UPDATE
	UPDATE Plato_Settings SET
		SiteId = @SiteId,
		SpaceId = @SpaceId,
		[Key] = @Key,
		Value = @Value,
		CreatedDate = @CreatedDate,
		CreatedUserId = @CreatedUserId,
		ModifiedDate = @ModifiedDate,
		ModifiedUserId = @ModifiedUserId
	WHERE (Id = @Id);

	SET @intIdentity = @Id;

END
ELSE
BEGIN

	-- INSERT
	INSERT INTO Plato_Settings (
		SiteId,
		SpaceId,
		[Key],
		Value,
		CreatedDate,
		CreatedUserId,
		ModifiedDate,
		ModifiedUserId
	) VALUES (
		@SiteId,
		@SpaceId,
		@Key,
		@Value,
		@CreatedDate,
		@CreatedUserId,
		@ModifiedDate,
		@ModifiedUserId
	);

	SET @intIdentity = SCOPE_IDENTITY();

END

SELECT @intIdentity;

RETURN




GO

------------------

GO


CREATE PROCEDURE [dbo].[plato_sp_SelectSetting] (
@Id int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_Settings WITH (nolock)
WHERE (Id = @Id)
	
RETURN

GO

---------------------

GO

CREATE PROCEDURE [plato_sp_SelectSettingsBySiteId] (
@SiteId int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_Settings WITH (nolock)
WHERE (SiteId = @SiteId)
	
RETURN

GO

-----------------

GO

CREATE PROCEDURE [plato_sp_SelectSettingsBySpaceId] (
@SpaceId int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_Settings WITH (nolock)
WHERE (SpaceId = @SpaceId)
	
RETURN



GO

---------------------

GO




-- ************************************
-- INSERT DEFAULT DATA
-- ************************************

DECLARE	@UserId int;

EXEC	@UserId = plato_sp_InsertUpdateUser
		@Id = 0,
		@SiteId = 1,
		@UserName = N'Admin',
		@Email = N'admin@admin.com',
		@DisplayName = N'',
		@SamAccountName = N''

DECLARE	@UserSecretId int;

EXEC	@UserSecretId = plato_sp_InsertUpdateUserSecret
		@Id = 0,
		@UserId = @UserId,
		@Password = N'admin123',
		@Salts = N''
		
DECLARE	@UserDetailId int;

EXEC	@UserDetailId = plato_sp_InsertUpdateUserDetail
		@Id = 0,
		@UserId = @UserId,
		@EditionId = 0,
		@RoleId = 5,
		@TeamId = 1,
		@TimeZoneOffSet = 0,
		@ObserveDST = 1,
		@Culture = '',
		@FirstName = '',
		@LastName = '',
		@WebSiteUrl = '',
		@ApiKey = '',
		@Visits = '',
		@Answers = 0,
		@Entities = 0,
		@Replies = 0,
		@Reactions = 0,
		@Mentions = 0,
		@Follows = 0,
		@Badges = 0,
		@ReputationRank = 0,
		@ReputationPoints = 0,
		@Banner = NULL,
		@ClientIpAddress = '',
		@ClientName = '',
		@EmailConfirmationCode = '',
		@PasswordResetCode = '',
		@IsEmailConfirmed = 1,		
		@CreatedDate = NULL,
		@CreatedUserId = 0,
		@ModifiedDate = NULL,
		@ModifiedUserId = 0,
		@IsDeleted = 0,
		@DeletedDate = NULL,
		@DeletedUserId = 0,
		@IsBanned = 0,
		@BannedDate = NULL,
		@BannedUserId = 0,
		@IsLocked = 0,
		@LockedDate = NULL,
		@LockedUserId = 0,
		@UnLockDate = NULL,
		@IsSpam = 0,
		@SpamDate = NULL,
		@SpamUserId = 0,
		@LastLoginDate = NULL
		

GO