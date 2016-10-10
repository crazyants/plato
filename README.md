

CREATE TABLE Plato_AuditLog
(
Id								INT IDENTITY(1,1) NOT NULL,
EntityId						INT NOT NULL,
SiteId							INT NOT NULL,
TableName						NVARCHAR(75) DEFAULT('') NOT NULL,
ColumnName						NVARCHAR(75) DEFAULT('') NOT NULL,
Values							NVARCHAR(75) DEFAULT('') NOT NULL,
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

CREATE TABLE Plato_Tennets
(
Id								INT IDENTITY(1,1) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
ConnectionString				NVARCHAR(255) DEFAULT('') NOT NULL,
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
CONSTRAINT PK_Plato_Tennets_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_Sites
(
Id								INT IDENTITY(1,1) NOT NULL,
TennetId						INT DEFAULT (0) NOT NULL,
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
CONSTRAINT PK_Plato_Sites_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_LoginAttempts
(
Id								INT IDENTITY(1,1) NOT NULL,
UserNameOrEmailAddress			NVARCHAR(255) DEFAULT('') NOT NULL,
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
Key								NVARCHAR(255) DEFAULT('') NOT NULL,
Value							NVARCHAR(255) DEFAULT('') NOT NULL,
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
TennetId						INT DEFAULT (0) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
EmailAddress					NVARCHAR(255) DEFAULT('') NOT NULL,
DisplayName						NVARCHAR(255) DEFAULT('') NOT NULL,
SamAccountName					NVARCHAR(255) DEFAULT('') NOT NULL,
PhotoId							INT DEFAULT (0) NOT NULL,
PhotoBlob						IMAGE NULL,
CONSTRAINT PK_Plato_Users_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_UserSecrets
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
Password						NVARCHAR(255) DEFAULT('') NOT NULL,
Salts							NVARCHAR(255) DEFAULT('') NOT NULL,
CONSTRAINT PK_Plato_UserSecrets_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_UserDetails
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
EditionId						INT DEFAULT (0) NOT NULL,
RoleId							INT DEFAULT (0) NOT NULL,
TeamId							INT DEFAULT (0) NOT NULL,
Culture							NVARCHAR(50) DEFAULT('') NOT NULL,
FirstName						NVARCHAR(100) DEFAULT('') NOT NULL,
LastName						NVARCHAR(100) DEFAULT('') NOT NULL,
WebSiteUrl						NVARCHAR(100) DEFAULT('') NOT NULL,
ApiKey							NVARCHAR(255) DEFAULT('') NOT NULL,
Visits							INT DEFAULT (0) NOT NULL,
Answers							INT DEFAULT (0) NOT NULL,
Entities						INT DEFAULT (0) NOT NULL,
EntityReplies					INT DEFAULT (0) NOT NULL,
Reactions						INT DEFAULT (0) NOT NULL,
Mentions						INT DEFAULT (0) NOT NULL,
Mentions						INT DEFAULT (0) NOT NULL,
ReputationRank					INT DEFAULT (0) NOT NULL,
ReputationPoints				INT DEFAULT (0) NOT NULL,
Badges							INT DEFAULT (0) NOT NULL,
BannerBlob						IMAGE NULL,
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
IsSpam							BIT DEFAULT(0) NOT NULL,
SpamDate						DATETIME2 NULL,
SpamUserId						INT DEFAULT (0) NOT NULL,
LastLoginDate					DATETIME2 NULL,
CONSTRAINT PK_Plato_UserDetails_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_UserFollows
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
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
OptOutCode						NVARCHAR(255) DEFAULT('') NOT NULL,
CONSTRAINT PK_Plato_UserFollows_Id PRIMARY KEY CLUSTERED ( Id )
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
Values							NVARCHAR(MAX) DEFAULT('') NOT NULL,
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
Key								NVARCHAR(255) DEFAULT('') NOT NULL,
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
UserId							INT IDENTITY(1,1) NOT NULL,
EntityId						INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_EntityAssignees_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_SpaceFields
(
Id								INT IDENTITY(1,1) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
NameLocalizaed					NVARCHAR(100) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
DescriptionLocalizaed			NVARCHAR(100) DEFAULT('') NOT NULL,
Values							NVARCHAR(MAX) DEFAULT('') NOT NULL,
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


/* values populated by fields defined within Plato_SpaceFields */
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
Blob							IMAGE NOT NULL,
Type							NVARCHAR(255) DEFAULT('') NOT NULL,
Length							FLOAT DEFAULT(0) NOT NULL,
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
CONSTRAINT PK_Plato_WorkFlowSteps_Id PRIMARY KEY CLUSTERED ( Id )
)

GO



CREATE TABLE Plato_Badges
(
Id								INT IDENTITY(1,1) NOT NULL,
SiteId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
Type							INT DEFAULT (0) NOT NULL,
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
Day								INT DEFAULT (0) NOT NULL,
Month							INT DEFAULT (0) NOT NULL,
Year							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_UserReputations_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

