IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'00000000000000_CreateIdentitySchema', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [SchoolId] int NULL;
GO

CREATE TABLE [Attendances] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] nvarchar(450) NOT NULL,
    [Date] datetime2 NOT NULL,
    [CheckedInAt] datetime2 NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Attendances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Attendances_AspNetUsers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [LeaveRequests] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] nvarchar(450) NOT NULL,
    [TargetDate] datetime2 NOT NULL,
    [SubmittedAt] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [Reason] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_LeaveRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LeaveRequests_AspNetUsers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [LessonPlans] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] nvarchar(450) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    [IsLate] bit NOT NULL,
    [JustificationText] nvarchar(max) NULL,
    [HasJustificationAttachment] bit NOT NULL,
    [Status] int NOT NULL,
    [Feedback] nvarchar(max) NULL,
    CONSTRAINT [PK_LessonPlans] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LessonPlans_AspNetUsers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [LessonPlanSettings] (
    [Id] int NOT NULL IDENTITY,
    [DailyDeadline] time NOT NULL,
    CONSTRAINT [PK_LessonPlanSettings] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Schools] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Schools] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_AspNetUsers_SchoolId] ON [AspNetUsers] ([SchoolId]);
GO

CREATE INDEX [IX_Attendances_TeacherId] ON [Attendances] ([TeacherId]);
GO

CREATE INDEX [IX_LeaveRequests_TeacherId] ON [LeaveRequests] ([TeacherId]);
GO

CREATE INDEX [IX_LessonPlans_TeacherId] ON [LessonPlans] ([TeacherId]);
GO

ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_Schools_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [Schools] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260628092044_AddCustomSchoolTables', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [FullName] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260628094615_AddUserFullName', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'AccessFailedCount');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [AccessFailedCount];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'LockoutEnabled');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [LockoutEnabled];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'LockoutEnd');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [LockoutEnd];
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'PhoneNumber');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [PhoneNumber];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'PhoneNumberConfirmed');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [PhoneNumberConfirmed];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'TwoFactorEnabled');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [TwoFactorEnabled];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260628113447_RemoveUnusedIdentityColumns', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'EmailConfirmed');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [EmailConfirmed];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260628113703_RemoveEmailConfirmedColumn', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'FullName');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [FullName];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260629054602_RemoveUserFullNameColumn', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Departments] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [SchoolId] int NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Departments_Schools_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [Schools] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [SchoolClasses] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [GradeLevel] nvarchar(max) NOT NULL,
    [SchoolId] int NOT NULL,
    CONSTRAINT [PK_SchoolClasses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SchoolClasses_Schools_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [Schools] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Subjects] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [DepartmentId] int NOT NULL,
    CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Subjects_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [TeacherAssignments] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] nvarchar(450) NOT NULL,
    [ClassId] int NOT NULL,
    [SubjectId] int NOT NULL,
    CONSTRAINT [PK_TeacherAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherAssignments_AspNetUsers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeacherAssignments_SchoolClasses_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [SchoolClasses] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherAssignments_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Departments_SchoolId] ON [Departments] ([SchoolId]);
GO

CREATE INDEX [IX_SchoolClasses_SchoolId] ON [SchoolClasses] ([SchoolId]);
GO

CREATE INDEX [IX_Subjects_DepartmentId] ON [Subjects] ([DepartmentId]);
GO

CREATE INDEX [IX_TeacherAssignments_ClassId] ON [TeacherAssignments] ([ClassId]);
GO

CREATE INDEX [IX_TeacherAssignments_SubjectId] ON [TeacherAssignments] ([SubjectId]);
GO

CREATE INDEX [IX_TeacherAssignments_TeacherId] ON [TeacherAssignments] ([TeacherId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260629072250_AddAcademicTables', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [FullName] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260629173243_AddFullNameToUser', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP TABLE [LessonPlanSettings];
GO

CREATE TABLE [SystemSettings] (
    [Id] int NOT NULL IDENTITY,
    [DailyDeadline] time NOT NULL,
    CONSTRAINT [PK_SystemSettings] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260629174251_RenameLessonPlanSettingsToSystemSetting', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [DepartmentId] int NULL;
GO

CREATE INDEX [IX_AspNetUsers_DepartmentId] ON [AspNetUsers] ([DepartmentId]);
GO

ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260629180103_AddDepartmentToUser', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [SystemSettings] ADD [MaintenanceMode] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260629182208_AddMaintenanceModeToSystemSetting', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [PhoneNumber] nvarchar(max) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [ProfilePicturePath] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260629234432_AddProfilePicturePathAndRestorePhoneNumber', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [FeedPosts] (
    [Id] int NOT NULL IDENTITY,
    [AuthorId] nvarchar(450) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_FeedPosts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FeedPosts_AspNetUsers_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [FeedPostComments] (
    [Id] int NOT NULL IDENTITY,
    [FeedPostId] int NOT NULL,
    [AuthorId] nvarchar(450) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_FeedPostComments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FeedPostComments_AspNetUsers_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FeedPostComments_FeedPosts_FeedPostId] FOREIGN KEY ([FeedPostId]) REFERENCES [FeedPosts] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [FeedPostMedias] (
    [Id] int NOT NULL IDENTITY,
    [FeedPostId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [StoredFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FileSize] bigint NOT NULL,
    [SortOrder] int NOT NULL,
    CONSTRAINT [PK_FeedPostMedias] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FeedPostMedias_FeedPosts_FeedPostId] FOREIGN KEY ([FeedPostId]) REFERENCES [FeedPosts] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [FeedPostReactions] (
    [Id] int NOT NULL IDENTITY,
    [FeedPostId] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_FeedPostReactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FeedPostReactions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FeedPostReactions_FeedPosts_FeedPostId] FOREIGN KEY ([FeedPostId]) REFERENCES [FeedPosts] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_FeedPostComments_AuthorId] ON [FeedPostComments] ([AuthorId]);
GO

CREATE INDEX [IX_FeedPostComments_FeedPostId] ON [FeedPostComments] ([FeedPostId]);
GO

CREATE INDEX [IX_FeedPostMedias_FeedPostId] ON [FeedPostMedias] ([FeedPostId]);
GO

CREATE UNIQUE INDEX [IX_FeedPostReactions_FeedPostId_UserId] ON [FeedPostReactions] ([FeedPostId], [UserId]);
GO

CREATE INDEX [IX_FeedPostReactions_UserId] ON [FeedPostReactions] ([UserId]);
GO

CREATE INDEX [IX_FeedPosts_AuthorId] ON [FeedPosts] ([AuthorId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260630002747_AddFeedFeature', N'8.0.28');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Attendances] DROP CONSTRAINT [FK_Attendances_AspNetUsers_TeacherId];
GO

ALTER TABLE [LeaveRequests] DROP CONSTRAINT [FK_LeaveRequests_AspNetUsers_TeacherId];
GO

EXEC sp_rename N'[LeaveRequests].[TeacherId]', N'UserId', N'COLUMN';
GO

EXEC sp_rename N'[LeaveRequests].[IX_LeaveRequests_TeacherId]', N'IX_LeaveRequests_UserId', N'INDEX';
GO

EXEC sp_rename N'[Attendances].[TeacherId]', N'UserId', N'COLUMN';
GO

EXEC sp_rename N'[Attendances].[IX_Attendances_TeacherId]', N'IX_Attendances_UserId', N'INDEX';
GO

ALTER TABLE [Attendances] ADD CONSTRAINT [FK_Attendances_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [LeaveRequests] ADD CONSTRAINT [FK_LeaveRequests_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260630030533_RenameTeacherIdToUserId_Attendance_LeaveRequest', N'8.0.28');
GO

COMMIT;
GO

