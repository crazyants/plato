
IF NOT EXISTS(SELECT 1 FROM sys.Objects WHERE  Object_id = OBJECT_ID(N'Plato_AuditLog') AND Type = N'U')
BEGIN

	CREATE TABLE Plato_AuditLog
	(
	Id								INT IDENTITY(1,1) NOT NULL,
	EntityId						INT NOT NULL,
	TableName						NVARCHAR(75) DEFAULT('') NOT NULL,
	ColumnName						NVARCHAR(75) DEFAULT('') NOT NULL,
	[Values]						NVARCHAR(75) DEFAULT('') NOT NULL,
	CreatedDate						DATETIME2 NULL,
	CreatedUserId					INT DEFAULT (0) NOT NULL,
	CONSTRAINT PK_Plato_AuditLog_Id PRIMARY KEY CLUSTERED ( Id )
	)

END

GO


IF NOT EXISTS(SELECT 1 FROM sys.Objects WHERE  Object_id = OBJECT_ID(N'Plato_LoginAttempts') AND Type = N'U')
BEGIN

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

END

GO

IF NOT EXISTS(SELECT 1 FROM sys.Objects WHERE  Object_id = OBJECT_ID(N'Plato_Groups') AND Type = N'U')
BEGIN

	CREATE TABLE Plato_Groups
	(
	Id								INT IDENTITY(1,1) NOT NULL,	
	[Name]							NVARCHAR(255) DEFAULT('') NOT NULL,
	[Description]					NVARCHAR(255) DEFAULT('') NOT NULL,
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

END

GO


CREATE TABLE Plato_Settings
(
Id								INT IDENTITY(1,1) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
[Key]							NVARCHAR(255) DEFAULT('') NOT NULL,
[Value]							NVARCHAR(MAX) DEFAULT('') NOT NULL,
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
UserName						NVARCHAR(255) DEFAULT('') NOT NULL,
NormalizedUserName				NVARCHAR(255) DEFAULT('') NOT NULL,
Email							NVARCHAR(255) DEFAULT('') NOT NULL,
NormalizedEmail					NVARCHAR(255) DEFAULT('') NOT NULL,
EmailConfirmed					BIT DEFAULT(0) NOT NULL,
DisplayName						NVARCHAR(255) DEFAULT('') NOT NULL,
SamAccountName					NVARCHAR(255) DEFAULT('') NOT NULL,
PasswordHash					NVARCHAR(255) DEFAULT('') NOT NULL,
SecurityStamp					NVARCHAR(255) DEFAULT('') NOT NULL,
PhoneNumber						NVARCHAR(255) DEFAULT('') NOT NULL,
PhoneNumberConfirmed			BIT DEFAULT(0) NOT NULL,
TwoFactorEnabled				BIT DEFAULT(0) NOT NULL,
LockoutEnd						DATETIME2,
LockoutEnabled					BIT DEFAULT(0) NOT NULL,
AccessFailedCount				INT DEFAULT(0) NOT NULL,
CONSTRAINT PK_Plato_Users_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_UserPhoto
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
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

CREATE TABLE Plato_UserBanner
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
ContentBlob						IMAGE NOT NULL,
ContentType						NVARCHAR(75) DEFAULT('') NOT NULL,
ContentLength					FLOAT DEFAULT(0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
ModifiedDate					DATETIME2 NULL,
ModifiedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_UserBanner_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

CREATE TABLE Plato_UserSecret
(
Id								INT IDENTITY(1,1) NOT NULL,
UserId							INT DEFAULT (0) NOT NULL,
Secret							NVARCHAR(255) DEFAULT('') NOT NULL,
Salts							NVARCHAR(255) DEFAULT('') NOT NULL,
SecurityStamp					NVARCHAR(255) DEFAULT('') NOT NULL,
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
PhotoId							TINYINT DEFAULT(0) NOT NULL,
BannerId						TINYINT DEFAULT(0) NOT NULL,
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

CREATE TABLE Plato_GroupFollows
(
Id								INT IDENTITY(1,1) NOT NULL,
GroupId							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
IsStreamDisabled				BIT DEFAULT (0) NOT NULL,
IsEmailDisabled					BIT DEFAULT (0) NOT NULL,
OptOutCode						NVARCHAR(255) DEFAULT('') NOT NULL,
CONSTRAINT PK_Plato_TeamFollows_Id PRIMARY KEY CLUSTERED ( Id )
)

GO


CREATE TABLE Plato_UserFields
(
Id								INT IDENTITY(1,1) NOT NULL,
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
PermissionId					INT DEFAULT (0) NOT NULL,
Name							NVARCHAR(255) DEFAULT('') NOT NULL,
NormalizedName					NVARCHAR(255) DEFAULT('') NOT NULL,
Description						NVARCHAR(255) DEFAULT('') NOT NULL,
HtmlPrefix						NVARCHAR(100) DEFAULT('') NOT NULL,
HtmlSuffix						NVARCHAR(100) DEFAULT('') NOT NULL,
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
ConcurrencyStamp				NVARCHAR(100) DEFAULT('') NOT NULL,
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
ConcurrencyStamp				NVARCHAR(100) DEFAULT('') NOT NULL,
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
ParentId						INT DEFAULT (0) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
GroupId							INT DEFAULT (0) NOT NULL,
WorkFlowId						INT DEFAULT (0) NOT NULL,
Title							NVARCHAR(255) DEFAULT('') NOT NULL,
TitleNormalized					NVARCHAR(255) DEFAULT('') NOT NULL,
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
TotalViews						INT DEFAULT (0) NOT NULL,
TotalReplies					INT DEFAULT (0) NOT NULL,
TotalPositiveReactions			INT DEFAULT (0) NOT NULL,
TotalNeutralReactions			INT DEFAULT (0) NOT NULL,
TotalNegativeReactions			INT DEFAULT (0) NOT NULL,
TotalMentions					INT DEFAULT (0) NOT NULL,
TotalFollows					INT DEFAULT (0) NOT NULL,
TotalFavourites					INT DEFAULT (0) NOT NULL,
TotalNotes						INT DEFAULT (0) NOT NULL,
TotalAssignees					INT DEFAULT (0) NOT NULL,

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
[Name]							NVARCHAR(255) DEFAULT('') NOT NULL,
NameLocalizaed					NVARCHAR(100) DEFAULT('') NOT NULL,
[Description]					NVARCHAR(255) DEFAULT('') NOT NULL,
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
[Name]							NVARCHAR(255) DEFAULT('') NOT NULL,
NameLocalized					NVARCHAR(100) DEFAULT('') NOT NULL,
NameNormalized					NVARCHAR(100) DEFAULT('') NOT NULL,
[Description]					NVARCHAR(MAX) DEFAULT('') NOT NULL,
DescriptionLocalizaed			NVARCHAR(100) DEFAULT('') NOT NULL,
BackColor						NVARCHAR(20) DEFAULT('') NOT NULL,
ForeColor						NVARCHAR(20) DEFAULT('') NOT NULL,
IconClassName					NVARCHAR(MAX) DEFAULT('') NOT NULL,
ImageBlob						IMAGE NOT NULL,
SortOrder						INT DEFAULT (0) NOT NULL,
TotalFollows					INT DEFAULT (0) NOT NULL,
TotalEntities					INT DEFAULT (0) NOT NULL,
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

CREATE TABLE Plato_Reactions
(
Id								INT IDENTITY(1,1) NOT NULL,
SpaceId							INT DEFAULT (0) NOT NULL,
[Name]							NVARCHAR(255) DEFAULT('') NOT NULL,
[Description]					NVARCHAR(255) DEFAULT('') NOT NULL,
ImageBlob						IMAGE NULL,
Points							INT DEFAULT (0) NOT NULL,
IsPositive						BIT DEFAULT (0) NOT NULL,
IsNeutral						BIT DEFAULT (0) NOT NULL,
IsNegative						BIT DEFAULT (0) NOT NULL,
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

GO

CREATE TABLE Plato_Permissions
(
Id								INT IDENTITY(1,1) NOT NULL,
[Name]							NVARCHAR(255) DEFAULT('') NOT NULL,
[Description]					NVARCHAR(255) DEFAULT('') NOT NULL,
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
[Name]							NVARCHAR(255) DEFAULT('') NOT NULL,
[Description]					NVARCHAR(255) DEFAULT('') NOT NULL,
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
[Action]						INT DEFAULT (0) NOT NULL,
ReputationPoints				INT DEFAULT (0) NOT NULL,
[Day]							INT DEFAULT (0) NOT NULL,
[Month]							INT DEFAULT (0) NOT NULL,
[Year]							INT DEFAULT (0) NOT NULL,
CreatedDate						DATETIME2 NULL,
CreatedUserId					INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_Plato_UserReputations_Id PRIMARY KEY CLUSTERED ( Id )
)

GO

-- ************************************
-- Stored Procedures
-- ************************************

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

EXEC plato_sp_SelectRolesByUserId @Id

RETURN

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

CREATE PROCEDURE [plato_sp_SelectUserSecretByUserId] (
@UserId int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_UserSecret WITH (nolock)
WHERE (UserId = @UserId)
	
RETURN

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

CREATE PROCEDURE [plato_sp_SelectUserDetailByUserId] (
@UserId int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_UserDetail WITH (nolock)
WHERE (UserId = @UserId)
	
RETURN

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


CREATE PROCEDURE [plato_sp_SelectUserBanner] (
@Id int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_UserBanner WITH (nolock)
WHERE (Id = @Id)
	
RETURN

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

CREATE PROCEDURE [plato_sp_SelectUserBannerByUserId] (
@UserId int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_UserBanner WITH (nolock)
WHERE (UserId = @UserId)
	
RETURN

GO

CREATE PROCEDURE [plato_sp_InsertUpdateUser] (
	@Id int,
	@UserName nvarchar(255),
	@NormalizedUserName nvarchar(255),
	@Email nvarchar(255),
	@NormalizedEmail nvarchar(255),	
	@EmailConfirmed bit,
	@DisplayName nvarchar(255),
	@SamAccountName nvarchar(255),
	@PasswordHash nvarchar(255),
	@SecurityStamp nvarchar(255),
	@PhoneNumber nvarchar(255),
	@PhoneNumberConfirmed bit,
	@TwoFactorEnabled bit,
	@LockoutEnd datetime2,
	@LockoutEnabled bit,
	@AccessFailedCount int
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
		UserName = @UserName,
		NormalizedUserName = @NormalizedUserName,
		Email = @Email,
		NormalizedEmail = @NormalizedEmail,
		EmailConfirmed = @EmailConfirmed,
		DisplayName = @DisplayName,
		SamAccountName = @SamAccountName,
		PasswordHash = @PasswordHash,
		SecurityStamp = @SecurityStamp,
		PhoneNumber = @PhoneNumber,
		PhoneNumberConfirmed = @PhoneNumberConfirmed,
		TwoFactorEnabled = @TwoFactorEnabled,
		LockoutEnd = @LockoutEnd,
		LockoutEnabled = @LockoutEnabled,
		AccessFailedCount = @AccessFailedCount
	WHERE (Id = @Id);

	SET @intIdentity = @Id;

END
ELSE
BEGIN

	-- INSERT
	INSERT INTO Plato_Users (
		UserName,
		NormalizedUserName,
		Email,
		NormalizedEmail,
		EmailConfirmed,
		DisplayName,
		SamAccountName,
		PasswordHash,
		SecurityStamp,
		PhoneNumber,
		PhoneNumberConfirmed,
		TwoFactorEnabled,
		LockoutEnd,
		LockoutEnabled,
		AccessFailedCount
	) VALUES (
		@UserName,
		@NormalizedUserName,
		@Email,
		@NormalizedEmail,
		@EmailConfirmed,
		@DisplayName,
		@SamAccountName,
		@PasswordHash,
		@SecurityStamp,
		@PhoneNumber,
		@PhoneNumberConfirmed,
		@TwoFactorEnabled,
		@LockoutEnd,
		@LockoutEnabled,
		@AccessFailedCount
	);

	SET @intIdentity = SCOPE_IDENTITY();

END

SELECT @intIdentity

RETURN

GO

CREATE PROCEDURE [plato_sp_InsertUpdateUserSecret] (
	@Id int,
	@UserId int,
	@Secret nvarchar(255),
	@Salts nvarchar(255),
	@SecurityStamp nvarchar(255)
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
		[Secret] = @Secret,
		Salts = @Salts,
		SecurityStamp = @SecurityStamp
	WHERE (Id = @Id);

	SET @intIdentity = @Id;

END
ELSE
BEGIN

	-- INSERT
	INSERT INTO Plato_UserSecret (
		UserId,
		[Secret],
		Salts,
		SecurityStamp
	) VALUES (
		@UserId,
		@Secret,
		@Salts,
		@SecurityStamp
	);

	SET @intIdentity = SCOPE_IDENTITY();

END

SELECT @intIdentity;

RETURN

GO

CREATE PROCEDURE [plato_sp_InsertUpdateUserBanner] (
	@Id int,
	@UserId int,
	@Name nvarchar(255),
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
	FROM Plato_UserBanner
	WHERE (Id = @Id)
	)
BEGIN

	-- UPDATE
	UPDATE Plato_UserBanner SET
		UserId = @UserId,
		Name = @Name,
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
	INSERT INTO Plato_UserBanner (
		UserId,
		Name,
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

CREATE PROCEDURE [plato_sp_InsertUpdateUserPhoto] (
	@Id int,
	@UserId int,
	@Name nvarchar(255),
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

CREATE PROCEDURE [plato_sp_SelectUserByUserName] (
@UserName nvarchar(255)
) AS
SET NOCOUNT ON 

DECLARE @Id int;
SELECT Id FROM
Plato_Users WITH (nolock) 
WHERE (UserName = @UserName)
	
EXEC plato_sp_SelectUser @Id;

RETURN

GO

CREATE PROCEDURE [plato_sp_DeleteUserRole] (
@Id int
) AS
SET NOCOUNT ON 


DECLARE @Success bit;
SET @Success = 0;	

IF (EXISTS(	
	SELECT Id FROM Plato_UserRoles 
	WHERE (Id = @Id)
		)
	)
BEGIN
	DELETE FROM 
	Plato_UserRoles
	WHERE (Id = @Id)
	SET @Success = 1;	
END

SELECT @Success;

RETURN

GO

CREATE PROCEDURE [plato_sp_DeleteUserRoles] (
@UserId int
) AS
SET NOCOUNT ON 

DECLARE @Success bit;
SET @Success = 0;	

IF (EXISTS(	
	SELECT Id FROM Plato_UserRoles 
	WHERE (UserId = @UserId)
		)
	)
BEGIN
	DELETE FROM 
	Plato_UserRoles
	WHERE (UserId = @UserId)
	SET @Success = 1;	
END

SELECT @Success;

RETURN

GO

CREATE PROCEDURE [plato_sp_SelectUserByUserNameNormalized] (
@NormalizedUserName nvarchar(255)
) AS
SET NOCOUNT ON 

DECLARE @Id int;
SELECT Id FROM
Plato_Users WITH (nolock) 
WHERE (NormalizedUserName = @NormalizedUserName)
	
EXEC plato_sp_SelectUser @Id;

RETURN

GO

CREATE PROCEDURE [plato_sp_SelectUserByApiKey] (
@ApiKey nvarchar(255)
) AS
SET NOCOUNT ON 

DECLARE @Id int;
SELECT u.Id FROM
Plato_Users u WITH (nolock) 
INNER JOIN Plato_UserDetail ud ON ud.UserId = u.Id
WHERE (ud.ApiKey = @ApiKey)
	
EXEC plato_sp_SelectUser @Id;

RETURN

GO

CREATE PROCEDURE [plato_sp_SelectUserByUserName] (
@UserName nvarchar(255)
) AS
SET NOCOUNT ON 

DECLARE @Id int;
SET @Id = (
	SELECT Id FROM
	Plato_Users WITH (nolock) 
	WHERE (UserName = @UserName)
)
	
IF (@Id > 0)
BEGIN
	EXEC plato_sp_SelectUser @Id;
END

RETURN

GO


CREATE PROCEDURE [plato_sp_SelectUserByEmailAndPassword] (
@Email nvarchar(255),
@PasswordHash nvarchar(255)
) AS
SET NOCOUNT ON 

DECLARE @Id int;
SELECT u.Id FROM
Plato_Users u WITH (nolock) 
INNER JOIN Plato_UserSecret us ON us.UserId = u.Id
WHERE (u.Email = @Email AND us.PasswordHash = @PasswordHash)
	
EXEC plato_sp_SelectUser @Id;

RETURN

GO

CREATE PROCEDURE [plato_sp_SelectUserByEmail] (
@Email nvarchar(255)
) AS
SET NOCOUNT ON 

DECLARE @Id int;
SET @Id = (
	SELECT u.Id FROM
	Plato_Users u WITH (nolock) 
	WHERE (u.Email = @Email)
)

IF (@Id > 0)
BEGIN
	EXEC plato_sp_SelectUser @Id;
END

RETURN

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
	@PhotoId tinyint,
	@BannerId tinyint,	
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
		PhotoId = @PhotoId,
		BannerId = @BannerId,
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
		PhotoId,
		BannerId,
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
		@PhotoId,
		@BannerId,
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


CREATE PROCEDURE [plato_sp_InsertUpdateSetting] (
	@Id int,
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
		SpaceId,
		[Key],
		Value,
		CreatedDate,
		CreatedUserId,
		ModifiedDate,
		ModifiedUserId
	) VALUES (
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

CREATE PROCEDURE [dbo].[plato_sp_SelectSetting] (
@Id int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_Settings WITH (nolock)
WHERE (Id = @Id)
	
RETURN


GO

CREATE PROCEDURE [plato_sp_SelectSettings] 
AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_Settings WITH (nolock)
	
RETURN

GO

CREATE PROCEDURE [plato_sp_InsertUpdateRole] (
	@Id int,
	@PermissionId int,
	@Name nvarchar(255),
	@NormalizedName nvarchar(255),
	@Description nvarchar(255),
	@HtmlPrefix nvarchar(100),	
	@HtmlSuffix nvarchar(100),
	@IsAdministrator bit,
	@IsEmployee bit,
	@IsAnonymous bit,
	@IsMember bit,
	@IsWaitingConfirmation bit,
	@IsBanned bit,
	@SortOrder int,
	@CreatedDate datetime2,
	@CreatedUserId int,
	@ModifiedDate datetime2,
	@ModifiedUserId int,
	@IsDeleted bit,
	@DeletedDate datetime2,
	@DeletedUserId int,
	@ConcurrencyStamp nvarchar(255)
) AS

SET NOCOUNT ON 

DECLARE @intIdentity int;

IF EXISTS( 
	SELECT Id 
	FROM Plato_Roles
	WHERE (Id = @Id)
	)
BEGIN

	-- UPDATE
	UPDATE Plato_Roles SET
		PermissionId = @PermissionId,
		Name = @Name,
		NormalizedName = @NormalizedName,
		[Description] = @Description,
		HtmlPrefix = @HtmlPrefix,
		HtmlSuffix = @HtmlSuffix,
		IsAdministrator = @IsAdministrator,
		IsEmployee = @IsEmployee,
		IsAnonymous = @IsAnonymous,
		IsMember = @IsMember,
		IsWaitingConfirmation = @IsWaitingConfirmation,
		IsBanned = @IsBanned,
		SortOrder = @SortOrder,
		CreatedDate = @CreatedDate,
		CreatedUserId = @CreatedUserId,
		ModifiedDate = @ModifiedDate,
		ModifiedUserId = @ModifiedUserId,
		IsDeleted = @IsDeleted,
		DeletedDate = @DeletedDate,
		DeletedUserId = @DeletedUserId,
		ConcurrencyStamp = @ConcurrencyStamp		
	WHERE (Id = @Id);

	SET @intIdentity = @Id;

END
ELSE
BEGIN

	-- INSERT
	INSERT INTO Plato_Roles (
		PermissionId,
		Name,
		NormalizedName,
		[Description],
		HtmlPrefix,
		HtmlSuffix,
		IsAdministrator,
		IsEmployee,
		IsAnonymous,
		IsMember,
		IsWaitingConfirmation,
		IsBanned,
		SortOrder,
		CreatedDate,
		CreatedUserId,
		ModifiedDate,
		ModifiedUserId,
		IsDeleted,
		DeletedDate,
		DeletedUserId,
		ConcurrencyStamp
	) VALUES (
		@PermissionId,
		@Name,
		@NormalizedName,
		@Description,
		@HtmlPrefix,
		@HtmlSuffix,
		@IsAdministrator,
		@IsEmployee,
		@IsAnonymous,
		@IsMember,
		@IsWaitingConfirmation,
		@IsBanned,
		@SortOrder,
		@CreatedDate,
		@CreatedUserId,
		@ModifiedDate,
		@ModifiedUserId,
		@IsDeleted,
		@DeletedDate,
		@DeletedUserId,
		@ConcurrencyStamp
	);

	SET @intIdentity = SCOPE_IDENTITY();

END

SELECT @intIdentity

RETURN

GO

CREATE PROCEDURE [plato_sp_SelectRoleByNameNormalized] (
@NormalizedName nvarchar(255)
) AS
SET NOCOUNT ON 

DECLARE @Id int;
SET @Id = (SELECT Id FROM 
Plato_Roles WITH (nolock)
WHERE (NormalizedName = @NormalizedName))

EXEC plato_sp_SelectRole @id;
	
RETURN

GO

CREATE PROCEDURE [plato_sp_InsertUpdateUserRole] (
	@Id int,
	@UserId int,
	@RoleId int,
	@CreatedDate datetime2,
	@CreatedUserId int,
	@ModifiedDate datetime2,
	@ModifiedUserId int,
	@ConcurrencyStamp nvarchar(50)
) AS

SET NOCOUNT ON 

DECLARE @intIdentity int;

IF EXISTS( 
	SELECT Id 
	FROM Plato_UserRoles
	WHERE (Id = @Id)
	)
BEGIN

	-- UPDATE
	UPDATE Plato_UserRoles SET	
		UserId = @UserId,
		RoleId = @RoleId,
		CreatedDate = @CreatedDate,
		CreatedUserId = @CreatedUserId,
		ModifiedDate = @ModifiedDate,
		ModifiedUserId = @ModifiedUserId,
		ConcurrencyStamp = @ConcurrencyStamp
	WHERE (Id = @Id);

	SET @intIdentity = @Id;

END
ELSE
BEGIN

	-- INSERT
	INSERT INTO Plato_UserRoles (
		UserId,
		RoleId,
		CreatedDate,
		CreatedUserId,
		ModifiedDate,
		ModifiedUserId,
		ConcurrencyStamp
	) VALUES (
		@UserId,
		@RoleId,
		@CreatedDate,
		@CreatedUserId,
		@ModifiedDate,
		@ModifiedUserId,
		@ConcurrencyStamp
	);

	SET @intIdentity = SCOPE_IDENTITY();

END

SELECT @intIdentity;

RETURN

GO

CREATE PROCEDURE [plato_sp_SelectRole] (
@Id int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_Roles WITH (nolock)
WHERE (Id = @Id)
	
RETURN

GO

CREATE PROCEDURE [plato_sp_SelectRoleByName] (
@Name nvarchar(255)
) AS
SET NOCOUNT ON 

DECLARE @Id int;
SET @Id = (SELECT Id FROM 
Plato_Roles WITH (nolock)
WHERE (Name = @Name))

EXEC plato_sp_SelectRole @id;
	
RETURN

GO

CREATE PROCEDURE [plato_sp_SelectRolesByUserId] (
@UserId int
) AS
SET NOCOUNT ON 

DECLARE @Id int;
SELECT r.* FROM 
Plato_Roles r WITH (nolock)
INNER JOIN Plato_UserRoles ur WITH (nolock) ON ur.RoleId = r.Id
WHERE (ur.UserId = @UserId)
	
RETURN

GO

CREATE PROCEDURE [plato_sp_SelectUsersPaged] (
@PageIndex int,
@PageSize int,
@SQLPopulate nvarchar(max),
@SQLCount nvarchar(max),
@SQLMaxRank nvarchar(max),
@ReturnCount bit
)
AS

SET NOCOUNT ON

IF (@SQLPopulate <> '')
BEGIN

	DECLARE @temp TABLE
	(
		IndexID int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
		UserId int, 
		[Rank] int,
		MaxRank int
	)

	INSERT INTO @temp (UserId, [Rank]) 
	EXECUTE sp_executesql @SQLPopulate 

	-- return our full text max rank
	DECLARE @intMaxRank int
	IF (@SQLMaxRank <> '')
	BEGIN

		-- insert full text max rank
		INSERT INTO @temp (MaxRank)
		EXECUTE sp_executesql @SQLMaxRank
	
		-- set max rank
		SET @intMaxRank = (SELECT IsNull(MaxRank, 0) FROM @temp WHERE IndexID = @@IDENTITY)
		DELETE FROM @temp WHERE IndexID = @@IDENTITY
		UPDATE @temp SET MaxRank = @intMaxRank

	END

	SELECT 
	u.*,
	IPI.[Rank], 
	IPI.MaxRank	
	FROM @temp AS IPI
		INNER JOIN Plato_Users u WITH (nolock) 
		ON u.Id = IPI.UserId  		
	ORDER BY IPI.IndexID

END

-- return rcord count?
IF (@ReturnCount = 1)
BEGIN
	EXECUTE sp_executesql @SQLCount
END

GO

CREATE  PROCEDURE plato_sp_SelectUsersPaged
(    
	@PageIndex int = 1,
    @PageSize int = 10,
	@SqlStartId nvarchar(max) = 'SELECT @start_id_out = Id FROM Plato_Users ORDER BY Id',
	@SqlPopulate nvarchar(max) = 'SELECT * FROM Plato_Users WHERE Id >= @start_id_in ORDER BY Id',
	@SqlCount nvarchar(max) = 'SELECT COUNT(Id) FROM Plato_Users',
	@SqlParams nvarchar(max) = '@Id int, @UserName nvarchar(255), @Email nvarchar(255)',
	@Id int = 0,
	@UserName nvarchar(255) = '',
	@Email nvarchar(255) = ''
)
AS

DECLARE @first_id int, 
	@start_row int

-- set start row pageSize * pageIndex - pageSize - 1
-- 1 * 5  = 1, 2 * 5 = 6, 3 * 5 = 11
-- 1 * 10  = 1, 2 * 10 = 11, 3 * 10 = 21
SET @start_row = 1;
IF (@PageIndex > 1)
	SET @start_row = (
		(@PageIndex * @PageSize) - ( @PageSize - 1 )
	)

-- Get the first row for our page of records
SET ROWCOUNT @start_row
DECLARE @parms nvarchar(100);

-- get the first Id
SET @parms = '@start_id_out int OUTPUT,' + + @SqlParams;  
EXECUTE sp_executesql  @SqlStartId, @parms, 
	@start_id_out = @first_id OUTPUT,
	@Id = 1,
	@UserName = '',
	@Email = ''

-- set to our page size
SET ROWCOUNT @PageSize

-- add our start parameter to the start
SET @SqlParams = '@start_id_in int,' + @SqlParams;

-- get all records >= @first_id
EXECUTE sp_executesql @SqlPopulate, @SqlParams, 
	@start_id_in = @first_id,
	@Id = 1,
	@UserName = '',
	@Email = ''

SET ROWCOUNT 0;

-- total count
IF (@SqlCount <> '')
	EXECUTE sp_executesql @SqlCount, @SqlParams, 
		@start_id_in = @first_id,
		@Id = 1,
		@UserName = '',
		@Email = ''

GO

CREATE PROCEDURE [plato_sp_SelectUserRole] (
@Id int
) AS
SET NOCOUNT ON 

SELECT * FROM 
Plato_UserRoles WITH (nolock)
WHERE (Id = @Id)
	
RETURN

GO

-- ************************************
-- Functions
-- ************************************

GO

CREATE FUNCTION plato_fn_ListToTable
(
	@Delimiter              char(1),          
	@String                 varchar(8000)    
)
RETURNS
@temptable table
(
    Items varchar(500)
)
AS
BEGIN

	DECLARE @idx int       
	DECLARE @slice varchar(8000)       

    SELECT @idx = 1       
        IF len(@String)<1 or @String is null  return       

    WHILE @idx!= 0       
    BEGIN       
        SET @idx = charindex(@Delimiter,@String)       
        IF @idx != 0       
            SET @slice = left(@String,@idx - 1)       
        ELSE       
            SET @slice = @String       

        IF(len(@slice)>0)  
            INSERT INTO @temptable(Items) VALUES (@slice)       

        SET @String = right(@String,len(@String) - @idx)       
        IF len(@String) = 0 break       
    END   

	RETURN

END 

GO

-- ************************************
-- INSERT DEFAULT DATA
-- ************************************

GO

DECLARE	@UserId int;

EXEC	@UserId = plato_sp_InsertUpdateUser
		@Id = 0,
		@UserName = N'Admin',
		@NormalizedUserName = '',
		@Email = N'admin@admin.com',
		@NormalizedEmail = '',
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