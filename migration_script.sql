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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] uniqueidentifier NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] uniqueidentifier NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] uniqueidentifier NULL,
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
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] uniqueidentifier NOT NULL,
        [ActorUserId] uniqueidentifier NULL,
        [ActorRole] nvarchar(50) NOT NULL,
        [Action] nvarchar(100) NOT NULL,
        [EntityType] nvarchar(100) NOT NULL,
        [EntityId] nvarchar(50) NOT NULL,
        [OldValue] nvarchar(max) NOT NULL,
        [NewValue] nvarchar(max) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [IpAddress] nvarchar(50) NOT NULL,
        [TraceId] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [FeatureFlags] (
        [Id] uniqueidentifier NOT NULL,
        [Key] nvarchar(100) NOT NULL,
        [IsEnabled] bit NOT NULL,
        [Description] nvarchar(500) NULL,
        [Group] nvarchar(100) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_FeatureFlags] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [Locations] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Slug] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_Locations] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Channel] nvarchar(50) NOT NULL,
        [Type] nvarchar(100) NOT NULL,
        [Subject] nvarchar(255) NOT NULL,
        [Body] nvarchar(max) NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [ErrorMessage] nvarchar(max) NULL,
        [SentAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [NotificationTemplates] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(100) NOT NULL,
        [Channel] nvarchar(50) NOT NULL,
        [SubjectTemplate] nvarchar(255) NOT NULL,
        [BodyTemplate] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_NotificationTemplates] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [Permissions] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(100) NOT NULL,
        [Description] nvarchar(255) NOT NULL,
        [Group] nvarchar(100) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [StoredFiles] (
        [Id] uniqueidentifier NOT NULL,
        [OwnerUserId] uniqueidentifier NOT NULL,
        [RelatedEntityType] nvarchar(100) NOT NULL,
        [RelatedEntityId] nvarchar(50) NOT NULL,
        [FileType] nvarchar(50) NOT NULL,
        [OriginalFileName] nvarchar(255) NOT NULL,
        [StoredFileName] nvarchar(255) NOT NULL,
        [MimeType] nvarchar(100) NOT NULL,
        [Extension] nvarchar(20) NOT NULL,
        [SizeInBytes] bigint NOT NULL,
        [StorageProvider] nvarchar(50) NOT NULL,
        [StoragePath] nvarchar(500) NOT NULL,
        [PublicUrl] nvarchar(500) NOT NULL,
        [IsPublic] bit NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_StoredFiles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [SystemSettings] (
        [Id] uniqueidentifier NOT NULL,
        [Key] nvarchar(100) NOT NULL,
        [Value] nvarchar(max) NOT NULL,
        [ValueType] nvarchar(50) NOT NULL,
        [Group] nvarchar(100) NOT NULL,
        [IsSensitive] bit NOT NULL,
        [IsEditable] bit NOT NULL,
        [Description] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_SystemSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [Captains] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [IdentityNumber] nvarchar(max) NOT NULL,
        [Bio] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_Captains] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Captains_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [RolePermissions] (
        [RoleId] uniqueidentifier NOT NULL,
        [PermissionId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([RoleId], [PermissionId]),
        CONSTRAINT [FK_RolePermissions_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [Boats] (
        [Id] uniqueidentifier NOT NULL,
        [CaptainId] uniqueidentifier NOT NULL,
        [LocationId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Slug] nvarchar(max) NOT NULL,
        [Capacity] int NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_Boats] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Boats_Captains_CaptainId] FOREIGN KEY ([CaptainId]) REFERENCES [Captains] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Boats_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [TourPackages] (
        [Id] uniqueidentifier NOT NULL,
        [BoatId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [MinCapacity] int NOT NULL,
        [MaxCapacity] int NOT NULL,
        [IsActive] bit NOT NULL,
        [ApprovalType] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_TourPackages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TourPackages_Boats_BoatId] FOREIGN KEY ([BoatId]) REFERENCES [Boats] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE TABLE [Reservations] (
        [Id] uniqueidentifier NOT NULL,
        [CustomerId] uniqueidentifier NOT NULL,
        [BoatId] uniqueidentifier NOT NULL,
        [TourPackageId] uniqueidentifier NOT NULL,
        [StartDateTime] datetime2 NOT NULL,
        [EndDateTime] datetime2 NOT NULL,
        [GuestCount] int NOT NULL,
        [TotalPrice] decimal(18,2) NOT NULL,
        [PackageNameSnapshot] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_Reservations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reservations_AspNetUsers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reservations_Boats_BoatId] FOREIGN KEY ([BoatId]) REFERENCES [Boats] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reservations_TourPackages_TourPackageId] FOREIGN KEY ([TourPackageId]) REFERENCES [TourPackages] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Action] ON [AuditLogs] ([Action]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_CreatedAt] ON [AuditLogs] ([CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_EntityType] ON [AuditLogs] ([EntityType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Boats_CaptainId] ON [Boats] ([CaptainId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Boats_LocationId] ON [Boats] ([LocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Captains_UserId] ON [Captains] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_FeatureFlags_Key] ON [FeatureFlags] ([Key]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Notifications_Status] ON [Notifications] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_NotificationTemplates_Code_Channel] ON [NotificationTemplates] ([Code], [Channel]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Permissions_Code] ON [Permissions] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Reservations_BoatId] ON [Reservations] ([BoatId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Reservations_CustomerId] ON [Reservations] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Reservations_TourPackageId] ON [Reservations] ([TourPackageId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_RolePermissions_PermissionId] ON [RolePermissions] ([PermissionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_StoredFiles_OwnerUserId] ON [StoredFiles] ([OwnerUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_StoredFiles_RelatedEntityType_RelatedEntityId] ON [StoredFiles] ([RelatedEntityType], [RelatedEntityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SystemSettings_Key] ON [SystemSettings] ([Key]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    CREATE INDEX [IX_TourPackages_BoatId] ON [TourPackages] ([BoatId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527124628_CoreInfrastructure'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260527124628_CoreInfrastructure', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527130744_AddBusinessEntities'
)
BEGIN
    ALTER TABLE [Captains] ADD [AdminNote] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527130744_AddBusinessEntities'
)
BEGIN
    ALTER TABLE [Captains] ADD [LicenseNumber] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527130744_AddBusinessEntities'
)
BEGIN
    ALTER TABLE [Captains] ADD [Status] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527130744_AddBusinessEntities'
)
BEGIN
    CREATE TABLE [AvailabilitySlots] (
        [Id] uniqueidentifier NOT NULL,
        [BoatId] uniqueidentifier NOT NULL,
        [StartDateTime] datetime2 NOT NULL,
        [EndDateTime] datetime2 NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [Reason] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_AvailabilitySlots] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AvailabilitySlots_Boats_BoatId] FOREIGN KEY ([BoatId]) REFERENCES [Boats] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527130744_AddBusinessEntities'
)
BEGIN
    CREATE INDEX [IX_AvailabilitySlots_BoatId] ON [AvailabilitySlots] ([BoatId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527130744_AddBusinessEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260527130744_AddBusinessEntities', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609213822_UpdateSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609213822_UpdateSchema', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [AccountStatus] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [ActiveSessionCount] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [LastLoginAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [LastLoginIp] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [LastPasswordChangedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [RegistrationSource] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [RiskStatus] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [UserNo] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [VerificationStatus] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] uniqueidentifier NOT NULL,
        [PaymentNo] nvarchar(50) NOT NULL,
        [ReservationId] uniqueidentifier NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [PlatformCommission] decimal(18,2) NOT NULL,
        [CaptainEarnings] decimal(18,2) NOT NULL,
        [PaymentMethod] int NOT NULL,
        [Status] int NOT NULL,
        [PaidAt] datetime2 NULL,
        [ProviderTransactionId] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_Reservations_ReservationId] FOREIGN KEY ([ReservationId]) REFERENCES [Reservations] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    CREATE TABLE [Reviews] (
        [Id] uniqueidentifier NOT NULL,
        [ReviewNo] nvarchar(50) NOT NULL,
        [CustomerId] uniqueidentifier NOT NULL,
        [BoatId] uniqueidentifier NOT NULL,
        [TourPackageId] uniqueidentifier NOT NULL,
        [Rating] int NOT NULL,
        [Comment] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reviews_AspNetUsers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reviews_Boats_BoatId] FOREIGN KEY ([BoatId]) REFERENCES [Boats] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reviews_TourPackages_TourPackageId] FOREIGN KEY ([TourPackageId]) REFERENCES [TourPackages] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    CREATE TABLE [SupportTickets] (
        [Id] uniqueidentifier NOT NULL,
        [TicketNo] nvarchar(50) NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Subject] nvarchar(max) NOT NULL,
        [Category] int NOT NULL,
        [Priority] int NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_SupportTickets] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SupportTickets_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    CREATE TABLE [UserAdminNotes] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [NoteType] int NOT NULL,
        [Note] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_UserAdminNotes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserAdminNotes_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    CREATE INDEX [IX_Payments_ReservationId] ON [Payments] ([ReservationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    CREATE INDEX [IX_Reviews_BoatId] ON [Reviews] ([BoatId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    CREATE INDEX [IX_Reviews_CustomerId] ON [Reviews] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    CREATE INDEX [IX_Reviews_TourPackageId] ON [Reviews] ([TourPackageId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    CREATE INDEX [IX_SupportTickets_UserId] ON [SupportTickets] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    CREATE INDEX [IX_UserAdminNotes_UserId] ON [UserAdminNotes] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609214838_AddUserModuleEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609214838_AddUserModuleEntities', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609215128_AddUserManagementUpdates'
)
BEGIN
    EXEC sp_rename N'[AspNetUsers].[AccountStatus]', N'Status', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609215128_AddUserManagementUpdates'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [UserAgent] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609215128_AddUserManagementUpdates'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [EmailVerifiedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609215128_AddUserManagementUpdates'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [PhoneVerifiedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609215128_AddUserManagementUpdates'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [ProfileImageUrl] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609215128_AddUserManagementUpdates'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609215128_AddUserManagementUpdates', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Locations]') AND [c].[name] = N'Slug');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Locations] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Locations] ALTER COLUMN [Slug] nvarchar(150) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Locations]') AND [c].[name] = N'Name');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Locations] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Locations] ALTER COLUMN [Name] nvarchar(150) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Locations]') AND [c].[name] = N'Description');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Locations] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Locations] ALTER COLUMN [Description] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [Address] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [CanonicalUrl] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [CoordinateStatus] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [CoverImageUrl] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [ImageAltText] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [IsPopular] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [Latitude] float NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [Longitude] float NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [MetaDescription] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [OgDescription] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [OgImageUrl] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [OgTitle] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [ParentLocationId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [Region] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [SeoStatus] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [SeoTitle] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [ShortDescription] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [SortOrder] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [Status] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD [Type] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Boats] ADD [HarborId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    CREATE TABLE [Harbors] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [LocationId] uniqueidentifier NOT NULL,
        [Type] int NOT NULL,
        [Status] int NOT NULL,
        [IsMainDeparturePoint] bit NOT NULL,
        [Latitude] float NULL,
        [Longitude] float NULL,
        [CoordinateStatus] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_Harbors] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Harbors_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    CREATE INDEX [IX_Locations_ParentLocationId] ON [Locations] ([ParentLocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Locations_Slug] ON [Locations] ([Slug]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    CREATE INDEX [IX_Boats_HarborId] ON [Boats] ([HarborId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    CREATE INDEX [IX_Harbors_LocationId] ON [Harbors] ([LocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Boats] ADD CONSTRAINT [FK_Boats_Harbors_HarborId] FOREIGN KEY ([HarborId]) REFERENCES [Harbors] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    ALTER TABLE [Locations] ADD CONSTRAINT [FK_Locations_Locations_ParentLocationId] FOREIGN KEY ([ParentLocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613203955_AddLocationsAndHarbors'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260613203955_AddLocationsAndHarbors', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614204148_AddBoatFeatures'
)
BEGIN
    CREATE TABLE [BoatFeatures] (
        [Id] uniqueidentifier NOT NULL,
        [BoatId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Category] nvarchar(50) NOT NULL,
        [IsAvailable] bit NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_BoatFeatures] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BoatFeatures_Boats_BoatId] FOREIGN KEY ([BoatId]) REFERENCES [Boats] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614204148_AddBoatFeatures'
)
BEGIN
    CREATE INDEX [IX_BoatFeatures_BoatId] ON [BoatFeatures] ([BoatId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614204148_AddBoatFeatures'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260614204148_AddBoatFeatures', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [CancellationPolicyType] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [CaptainCancellationNote] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [Category] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [Currency] nvarchar(10) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [DurationHours] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [EndTime] time NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [FreeCancellationHours] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [IsChildFriendly] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [PrepaymentPercentage] decimal(5,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [RefundPolicyNote] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [ServiceFee] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [StartTime] time NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [Status] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [TimeLabel] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [TourType] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    ALTER TABLE [TourPackages] ADD [WeatherPostponeNote] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    CREATE TABLE [PackageIncludes] (
        [Id] uniqueidentifier NOT NULL,
        [TourPackageId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [IsIncluded] bit NOT NULL,
        [Description] nvarchar(500) NULL,
        [Status] nvarchar(50) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_PackageIncludes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PackageIncludes_TourPackages_TourPackageId] FOREIGN KEY ([TourPackageId]) REFERENCES [TourPackages] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    CREATE INDEX [IX_PackageIncludes_TourPackageId] ON [PackageIncludes] ([TourPackageId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614205903_AddPackageFeatures'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260614205903_AddPackageFeatures', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614211548_AddCompanyAndCaptainDetails'
)
BEGIN
    ALTER TABLE [Captains] ADD [AccountStatus] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614211548_AddCompanyAndCaptainDetails'
)
BEGIN
    ALTER TABLE [Captains] ADD [Address] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614211548_AddCompanyAndCaptainDetails'
)
BEGIN
    ALTER TABLE [Captains] ADD [ApplicationNo] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614211548_AddCompanyAndCaptainDetails'
)
BEGIN
    ALTER TABLE [Captains] ADD [ApplicationType] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614211548_AddCompanyAndCaptainDetails'
)
BEGIN
    ALTER TABLE [Captains] ADD [Harbor] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614211548_AddCompanyAndCaptainDetails'
)
BEGIN
    ALTER TABLE [Captains] ADD [Iban] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614211548_AddCompanyAndCaptainDetails'
)
BEGIN
    ALTER TABLE [Captains] ADD [Location] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614211548_AddCompanyAndCaptainDetails'
)
BEGIN
    CREATE TABLE [Companies] (
        [Id] uniqueidentifier NOT NULL,
        [CaptainId] uniqueidentifier NOT NULL,
        [CompanyName] nvarchar(max) NOT NULL,
        [AuthorizedPersonName] nvarchar(max) NOT NULL,
        [TaxOffice] nvarchar(max) NOT NULL,
        [TaxNumber] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NOT NULL,
        [Iban] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_Companies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Companies_Captains_CaptainId] FOREIGN KEY ([CaptainId]) REFERENCES [Captains] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614211548_AddCompanyAndCaptainDetails'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Companies_CaptainId] ON [Companies] ([CaptainId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614211548_AddCompanyAndCaptainDetails'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260614211548_AddCompanyAndCaptainDetails', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614213136_UpdatePaymentAndReservation'
)
BEGIN
    ALTER TABLE [Reservations] ADD [ReservationNo] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614213136_UpdatePaymentAndReservation'
)
BEGIN
    ALTER TABLE [Payments] ADD [Currency] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614213136_UpdatePaymentAndReservation'
)
BEGIN
    ALTER TABLE [Payments] ADD [DepositAmount] decimal(18,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614213136_UpdatePaymentAndReservation'
)
BEGIN
    ALTER TABLE [Payments] ADD [GrossTourAmount] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614213136_UpdatePaymentAndReservation'
)
BEGIN
    ALTER TABLE [Payments] ADD [PaymentProvider] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614213136_UpdatePaymentAndReservation'
)
BEGIN
    ALTER TABLE [Payments] ADD [PayoutStatus] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614213136_UpdatePaymentAndReservation'
)
BEGIN
    ALTER TABLE [Payments] ADD [ProviderReferenceNo] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614213136_UpdatePaymentAndReservation'
)
BEGIN
    ALTER TABLE [Payments] ADD [RefundStatus] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614213136_UpdatePaymentAndReservation'
)
BEGIN
    ALTER TABLE [Payments] ADD [RefundedAmount] decimal(18,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614213136_UpdatePaymentAndReservation'
)
BEGIN
    ALTER TABLE [Payments] ADD [RemainingAmount] decimal(18,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614213136_UpdatePaymentAndReservation'
)
BEGIN
    ALTER TABLE [Payments] ADD [ServiceFeeAmount] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614213136_UpdatePaymentAndReservation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260614213136_UpdatePaymentAndReservation', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619213811_AddPackageToAvailabilitySlot'
)
BEGIN
    ALTER TABLE [AvailabilitySlots] ADD [Capacity] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619213811_AddPackageToAvailabilitySlot'
)
BEGIN
    ALTER TABLE [AvailabilitySlots] ADD [TourPackageId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619213811_AddPackageToAvailabilitySlot'
)
BEGIN
    CREATE INDEX [IX_AvailabilitySlots_TourPackageId] ON [AvailabilitySlots] ([TourPackageId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619213811_AddPackageToAvailabilitySlot'
)
BEGIN
    ALTER TABLE [AvailabilitySlots] ADD CONSTRAINT [FK_AvailabilitySlots_TourPackages_TourPackageId] FOREIGN KEY ([TourPackageId]) REFERENCES [TourPackages] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619213811_AddPackageToAvailabilitySlot'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260619213811_AddPackageToAvailabilitySlot', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619214847_AddCaptainNoteToReservation'
)
BEGIN
    ALTER TABLE [Reservations] ADD [CaptainNote] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619214847_AddCaptainNoteToReservation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260619214847_AddCaptainNoteToReservation', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619215828_AddReviewReplyAndReport'
)
BEGIN
    ALTER TABLE [Reviews] ADD [ReservationId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619215828_AddReviewReplyAndReport'
)
BEGIN
    CREATE TABLE [ReviewReplies] (
        [Id] uniqueidentifier NOT NULL,
        [ReviewId] uniqueidentifier NOT NULL,
        [ReplyText] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_ReviewReplies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ReviewReplies_Reviews_ReviewId] FOREIGN KEY ([ReviewId]) REFERENCES [Reviews] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619215828_AddReviewReplyAndReport'
)
BEGIN
    CREATE TABLE [ReviewReports] (
        [Id] uniqueidentifier NOT NULL,
        [ReviewId] uniqueidentifier NOT NULL,
        [Reason] nvarchar(max) NOT NULL,
        [Message] nvarchar(max) NULL,
        [IsResolved] bit NOT NULL,
        [AdminNote] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_ReviewReports] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ReviewReports_Reviews_ReviewId] FOREIGN KEY ([ReviewId]) REFERENCES [Reviews] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619215828_AddReviewReplyAndReport'
)
BEGIN
    CREATE INDEX [IX_Reviews_ReservationId] ON [Reviews] ([ReservationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619215828_AddReviewReplyAndReport'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ReviewReplies_ReviewId] ON [ReviewReplies] ([ReviewId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619215828_AddReviewReplyAndReport'
)
BEGIN
    CREATE INDEX [IX_ReviewReports_ReviewId] ON [ReviewReports] ([ReviewId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619215828_AddReviewReplyAndReport'
)
BEGIN
    ALTER TABLE [Reviews] ADD CONSTRAINT [FK_Reviews_Reservations_ReservationId] FOREIGN KEY ([ReservationId]) REFERENCES [Reservations] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619215828_AddReviewReplyAndReport'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260619215828_AddReviewReplyAndReport', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619220507_AddPayoutEntity'
)
BEGIN
    CREATE TABLE [Payouts] (
        [Id] uniqueidentifier NOT NULL,
        [PayoutNo] nvarchar(max) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [IbanMasked] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [ScheduledDate] datetime2 NULL,
        [PaidDate] datetime2 NULL,
        [CaptainId] uniqueidentifier NOT NULL,
        [RelatedReservationCount] int NOT NULL,
        [Description] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_Payouts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payouts_Captains_CaptainId] FOREIGN KEY ([CaptainId]) REFERENCES [Captains] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619220507_AddPayoutEntity'
)
BEGIN
    CREATE INDEX [IX_Payouts_CaptainId] ON [Payouts] ([CaptainId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619220507_AddPayoutEntity'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260619220507_AddPayoutEntity', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620225142_AddUserSettingsEntities'
)
BEGIN
    CREATE TABLE [UserActiveSessions] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Device] nvarchar(max) NOT NULL,
        [Location] nvarchar(max) NOT NULL,
        [IpAddress] nvarchar(max) NOT NULL,
        [LastAccess] datetime2 NOT NULL,
        [IsCurrent] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_UserActiveSessions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserActiveSessions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620225142_AddUserSettingsEntities'
)
BEGIN
    CREATE TABLE [UserLegalAgreements] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [AgreementName] nvarchar(max) NOT NULL,
        [Version] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_UserLegalAgreements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserLegalAgreements_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620225142_AddUserSettingsEntities'
)
BEGIN
    CREATE TABLE [UserNotificationPreferences] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Category] nvarchar(max) NOT NULL,
        [Email] bit NOT NULL,
        [Sms] bit NOT NULL,
        [Whatsapp] bit NOT NULL,
        [InApp] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_UserNotificationPreferences] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserNotificationPreferences_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620225142_AddUserSettingsEntities'
)
BEGIN
    CREATE TABLE [UserSecurityEvents] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [IpAddress] nvarchar(max) NOT NULL,
        [UserAgent] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_UserSecurityEvents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserSecurityEvents_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620225142_AddUserSettingsEntities'
)
BEGIN
    CREATE INDEX [IX_UserActiveSessions_UserId] ON [UserActiveSessions] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620225142_AddUserSettingsEntities'
)
BEGIN
    CREATE INDEX [IX_UserLegalAgreements_UserId] ON [UserLegalAgreements] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620225142_AddUserSettingsEntities'
)
BEGIN
    CREATE INDEX [IX_UserNotificationPreferences_UserId] ON [UserNotificationPreferences] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620225142_AddUserSettingsEntities'
)
BEGIN
    CREATE INDEX [IX_UserSecurityEvents_UserId] ON [UserSecurityEvents] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620225142_AddUserSettingsEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260620225142_AddUserSettingsEntities', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622211954_AddBoatDetails'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260622211954_AddBoatDetails', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622212900_AddBoatDetails2'
)
BEGIN
    ALTER TABLE [Boats] ADD [BoatType] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622212900_AddBoatDetails2'
)
BEGIN
    ALTER TABLE [Boats] ADD [BrandModel] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622212900_AddBoatDetails2'
)
BEGIN
    ALTER TABLE [Boats] ADD [Description] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622212900_AddBoatDetails2'
)
BEGIN
    ALTER TABLE [Boats] ADD [Length] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622212900_AddBoatDetails2'
)
BEGIN
    ALTER TABLE [Boats] ADD [ProductionYear] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622212900_AddBoatDetails2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260622212900_AddBoatDetails2', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    ALTER TABLE [AvailabilitySlots] DROP CONSTRAINT [FK_AvailabilitySlots_TourPackages_TourPackageId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    ALTER TABLE [Payouts] DROP CONSTRAINT [FK_Payouts_Captains_CaptainId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DROP INDEX [IX_UserNotificationPreferences_UserId] ON [UserNotificationPreferences];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DROP INDEX [IX_Payouts_CaptainId] ON [Payouts];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DROP INDEX [IX_AvailabilitySlots_BoatId] ON [AvailabilitySlots];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserSecurityEvents]') AND [c].[name] = N'UserAgent');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [UserSecurityEvents] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [UserSecurityEvents] ALTER COLUMN [UserAgent] nvarchar(500) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserSecurityEvents]') AND [c].[name] = N'Title');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [UserSecurityEvents] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [UserSecurityEvents] ALTER COLUMN [Title] nvarchar(200) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserSecurityEvents]') AND [c].[name] = N'IpAddress');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [UserSecurityEvents] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [UserSecurityEvents] ALTER COLUMN [IpAddress] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserSecurityEvents]') AND [c].[name] = N'Description');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [UserSecurityEvents] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [UserSecurityEvents] ALTER COLUMN [Description] nvarchar(1000) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserNotificationPreferences]') AND [c].[name] = N'Category');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [UserNotificationPreferences] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [UserNotificationPreferences] ALTER COLUMN [Category] nvarchar(100) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserLegalAgreements]') AND [c].[name] = N'Version');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [UserLegalAgreements] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [UserLegalAgreements] ALTER COLUMN [Version] nvarchar(20) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserLegalAgreements]') AND [c].[name] = N'Status');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [UserLegalAgreements] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [UserLegalAgreements] ALTER COLUMN [Status] nvarchar(20) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserLegalAgreements]') AND [c].[name] = N'AgreementName');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [UserLegalAgreements] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [UserLegalAgreements] ALTER COLUMN [AgreementName] nvarchar(100) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserActiveSessions]') AND [c].[name] = N'Location');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [UserActiveSessions] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [UserActiveSessions] ALTER COLUMN [Location] nvarchar(200) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserActiveSessions]') AND [c].[name] = N'IpAddress');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [UserActiveSessions] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [UserActiveSessions] ALTER COLUMN [IpAddress] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserActiveSessions]') AND [c].[name] = N'Device');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [UserActiveSessions] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [UserActiveSessions] ALTER COLUMN [Device] nvarchar(200) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ReviewReports]') AND [c].[name] = N'Reason');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [ReviewReports] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [ReviewReports] ALTER COLUMN [Reason] nvarchar(100) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ReviewReports]') AND [c].[name] = N'Message');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [ReviewReports] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [ReviewReports] ALTER COLUMN [Message] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ReviewReports]') AND [c].[name] = N'AdminNote');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [ReviewReports] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [ReviewReports] ALTER COLUMN [AdminNote] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ReviewReplies]') AND [c].[name] = N'ReplyText');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [ReviewReplies] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [ReviewReplies] ALTER COLUMN [ReplyText] nvarchar(1000) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var18 sysname;
    SELECT @var18 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payouts]') AND [c].[name] = N'PayoutNo');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [Payouts] DROP CONSTRAINT [' + @var18 + '];');
    ALTER TABLE [Payouts] ALTER COLUMN [PayoutNo] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var19 sysname;
    SELECT @var19 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payouts]') AND [c].[name] = N'IbanMasked');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [Payouts] DROP CONSTRAINT [' + @var19 + '];');
    ALTER TABLE [Payouts] ALTER COLUMN [IbanMasked] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var20 sysname;
    SELECT @var20 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payouts]') AND [c].[name] = N'Description');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [Payouts] DROP CONSTRAINT [' + @var20 + '];');
    ALTER TABLE [Payouts] ALTER COLUMN [Description] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var21 sysname;
    SELECT @var21 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'TaxOffice');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var21 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [TaxOffice] nvarchar(100) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var22 sysname;
    SELECT @var22 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'TaxNumber');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var22 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [TaxNumber] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var23 sysname;
    SELECT @var23 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'Iban');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var23 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [Iban] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var24 sysname;
    SELECT @var24 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'CompanyName');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var24 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [CompanyName] nvarchar(200) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var25 sysname;
    SELECT @var25 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'AuthorizedPersonName');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var25 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [AuthorizedPersonName] nvarchar(100) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var26 sysname;
    SELECT @var26 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'Address');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var26 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [Address] nvarchar(500) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var27 sysname;
    SELECT @var27 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Captains]') AND [c].[name] = N'Status');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [Captains] DROP CONSTRAINT [' + @var27 + '];');
    ALTER TABLE [Captains] ALTER COLUMN [Status] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var28 sysname;
    SELECT @var28 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Captains]') AND [c].[name] = N'ApplicationType');
    IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [Captains] DROP CONSTRAINT [' + @var28 + '];');
    ALTER TABLE [Captains] ALTER COLUMN [ApplicationType] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var29 sysname;
    SELECT @var29 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Captains]') AND [c].[name] = N'AccountStatus');
    IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [Captains] DROP CONSTRAINT [' + @var29 + '];');
    ALTER TABLE [Captains] ALTER COLUMN [AccountStatus] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var30 sysname;
    SELECT @var30 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AvailabilitySlots]') AND [c].[name] = N'Status');
    IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [AvailabilitySlots] DROP CONSTRAINT [' + @var30 + '];');
    ALTER TABLE [AvailabilitySlots] ALTER COLUMN [Status] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var31 sysname;
    SELECT @var31 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AvailabilitySlots]') AND [c].[name] = N'Reason');
    IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [AvailabilitySlots] DROP CONSTRAINT [' + @var31 + '];');
    ALTER TABLE [AvailabilitySlots] ALTER COLUMN [Reason] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    ALTER TABLE [AvailabilitySlots] ADD [BoatId1] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    DECLARE @var32 sysname;
    SELECT @var32 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetRoles]') AND [c].[name] = N'Description');
    IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [AspNetRoles] DROP CONSTRAINT [' + @var32 + '];');
    ALTER TABLE [AspNetRoles] ALTER COLUMN [Description] nvarchar(500) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserNotificationPreferences_UserId_Category] ON [UserNotificationPreferences] ([UserId], [Category]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    CREATE INDEX [IX_Payouts_CaptainId_Status] ON [Payouts] ([CaptainId], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Payouts_PayoutNo] ON [Payouts] ([PayoutNo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    CREATE INDEX [IX_AvailabilitySlots_BoatId_StartDateTime_EndDateTime] ON [AvailabilitySlots] ([BoatId], [StartDateTime], [EndDateTime]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    CREATE INDEX [IX_AvailabilitySlots_BoatId1] ON [AvailabilitySlots] ([BoatId1]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    ALTER TABLE [AvailabilitySlots] ADD CONSTRAINT [FK_AvailabilitySlots_Boats_BoatId1] FOREIGN KEY ([BoatId1]) REFERENCES [Boats] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    ALTER TABLE [AvailabilitySlots] ADD CONSTRAINT [FK_AvailabilitySlots_TourPackages_TourPackageId] FOREIGN KEY ([TourPackageId]) REFERENCES [TourPackages] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    ALTER TABLE [Payouts] ADD CONSTRAINT [FK_Payouts_Captains_CaptainId] FOREIGN KEY ([CaptainId]) REFERENCES [Captains] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623221748_AddDecimalPrecisions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260623221748_AddDecimalPrecisions', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628215345_AddValidUntilToStoredFile'
)
BEGIN
    ALTER TABLE [StoredFiles] ADD [ValidUntil] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628215345_AddValidUntilToStoredFile'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260628215345_AddValidUntilToStoredFile', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628225729_FixBoatFeatureStatus'
)
BEGIN
    UPDATE BoatFeatures SET Status = 'PendingReview' WHERE Status = 'Kontrol Bekliyor' OR Status = '0'
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628225729_FixBoatFeatureStatus'
)
BEGIN
    UPDATE BoatFeatures SET Status = 'Approved' WHERE Status = 'Onaylandı' OR Status = '1'
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628225729_FixBoatFeatureStatus'
)
BEGIN
    UPDATE BoatFeatures SET Status = 'Missing' WHERE Status = 'Eksik' OR Status = '2'
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628225729_FixBoatFeatureStatus'
)
BEGIN
    UPDATE BoatFeatures SET Status = 'Risky' WHERE Status = 'Riskli' OR Status = '3'
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628225729_FixBoatFeatureStatus'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260628225729_FixBoatFeatureStatus', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705211219_AddCaptainSettingsFields'
)
BEGIN
    ALTER TABLE [Companies] ADD [TradeRegistryNumber] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705211219_AddCaptainSettingsFields'
)
BEGIN
    ALTER TABLE [Captains] ADD [City] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705211219_AddCaptainSettingsFields'
)
BEGIN
    ALTER TABLE [Captains] ADD [District] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705211219_AddCaptainSettingsFields'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [BirthDate] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705211219_AddCaptainSettingsFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260705211219_AddCaptainSettingsFields', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705212430_AddCaptainExperienceAndLanguages'
)
BEGIN
    ALTER TABLE [Captains] ADD [ExperienceYears] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705212430_AddCaptainExperienceAndLanguages'
)
BEGIN
    ALTER TABLE [Captains] ADD [Languages] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705212430_AddCaptainExperienceAndLanguages'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260705212430_AddCaptainExperienceAndLanguages', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705215357_AddCaptainFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260705215357_AddCaptainFields', N'9.0.2');
END;

COMMIT;
GO

