IF OBJECT_ID(N'__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Semester] (
    [ID] int NOT NULL IDENTITY,
    [Archived] bit NOT NULL,
    [Current] bit NOT NULL,
    [Open] bit NOT NULL,
    [Season] int NOT NULL,
    [StartYear] int NOT NULL,
    [StartingDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Semester] PRIMARY KEY ([ID])
);

GO

CREATE TABLE [Role] (
    [Id] int NOT NULL IDENTITY,
    [ConcurrencyStamp] nvarchar(max),
    [Name] nvarchar(max),
    [NormalizedName] nvarchar(max),
    CONSTRAINT [PK_Role] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [UserToken] (
    [UserId] int NOT NULL IDENTITY,
    [LoginProvider] nvarchar(max),
    [Name] nvarchar(max),
    [Value] nvarchar(max),
    CONSTRAINT [PK_UserToken] PRIMARY KEY ([UserId])
);

GO

CREATE TABLE [RoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [ClaimType] nvarchar(max),
    [ClaimValue] nvarchar(max),
    [IdentityRole<int>Id] int,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RoleClaims_Role_IdentityRole<int>Id] FOREIGN KEY ([IdentityRole<int>Id]) REFERENCES [Role] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [AccessFailedCount] int NOT NULL,
    [ConcurrencyStamp] nvarchar(max),
    [Discriminator] nvarchar(max) NOT NULL,
    [Email] nvarchar(max),
    [EmailConfirmed] bit NOT NULL,
    [LockoutEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset,
    [NormalizedEmail] nvarchar(max),
    [NormalizedUserName] nvarchar(max),
    [PasswordHash] nvarchar(max),
    [PhoneNumber] nvarchar(max),
    [PhoneNumberConfirmed] bit NOT NULL,
    [SecurityStamp] nvarchar(max),
    [TwoFactorEnabled] bit NOT NULL,
    [UserName] nvarchar(max),
    [Archived] bit,
    [DatesSuggestionSuggestionID] int,
    [DepartmentID] int,
    [FirstName] nvarchar(50),
    [HireDate] datetime2,
    [LastName] nvarchar(50),
    [Approved] bit,
    [ProgramID] int,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Faculty] (
    [FacultyID] int NOT NULL IDENTITY,
    [Archived] bit NOT NULL,
    [Name] nvarchar(50),
    [ProfessorID] int,
    [StartDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Faculty] PRIMARY KEY ([FacultyID]),
    CONSTRAINT [FK_Faculty_Users_ProfessorID] FOREIGN KEY ([ProfessorID]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [OfficeAssignment] (
    [ProfessorID] int NOT NULL,
    [Location] nvarchar(50),
    CONSTRAINT [PK_OfficeAssignment] PRIMARY KEY ([ProfessorID]),
    CONSTRAINT [FK_OfficeAssignment_Users_ProfessorID] FOREIGN KEY ([ProfessorID]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [TeachingRequest] (
    [RequestID] int NOT NULL IDENTITY,
    [Annotation] nvarchar(max),
    [Approved] bit NOT NULL,
    [ProfessorID] int NOT NULL,
    [SemesterID] int NOT NULL,
    CONSTRAINT [PK_TeachingRequest] PRIMARY KEY ([RequestID]),
    CONSTRAINT [FK_TeachingRequest_Users_ProfessorID] FOREIGN KEY ([ProfessorID]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeachingRequest_Semester_SemesterID] FOREIGN KEY ([SemesterID]) REFERENCES [Semester] ([ID]) ON DELETE CASCADE
);

GO

CREATE TABLE [Workloads] (
    [WorkloadID] int NOT NULL IDENTITY,
    [AcBooksAndMono] int NOT NULL,
    [AccJourArt] int NOT NULL,
    [ChaptInBooks] int NOT NULL,
    [CompButUnpubPapers] int NOT NULL,
    [DepCommittees] int NOT NULL,
    [DiscAndTrChairAndOther] int NOT NULL,
    [Duties] int NOT NULL,
    [EditedBooks] int NOT NULL,
    [ExtResGrantApplications] int NOT NULL,
    [ExtResGrantReceived] int NOT NULL,
    [FacCommittees] int NOT NULL,
    [IntAndNatConferences] int NOT NULL,
    [IntResGrantReceived] int NOT NULL,
    [MBA] int NOT NULL,
    [MSc] int NOT NULL,
    [NonRefJourArtic] int NOT NULL,
    [Notes] nvarchar(200),
    [OtherPresentations] int NOT NULL,
    [OtherPublications] int NOT NULL,
    [PHDPhase2] int NOT NULL,
    [PHDPhase3] int NOT NULL,
    [ProfAssociatons] int NOT NULL,
    [ProfessorID] int NOT NULL,
    [ProjInProgress] int NOT NULL,
    [PubBooksRevs] int NOT NULL,
    [RefConPro] int NOT NULL,
    [RefJourArt] int NOT NULL,
    [RegConferences] int NOT NULL,
    [Reviewed] bit NOT NULL,
    [SemesterID] int NOT NULL,
    [SympWorkshops] int NOT NULL,
    [Textbooks] int NOT NULL,
    [UnivCommittees] int NOT NULL,
    [Year] int NOT NULL,
    CONSTRAINT [PK_Workloads] PRIMARY KEY ([WorkloadID]),
    CONSTRAINT [FK_Workloads_Users_ProfessorID] FOREIGN KEY ([ProfessorID]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Workloads_Semester_SemesterID] FOREIGN KEY ([SemesterID]) REFERENCES [Semester] ([ID]) ON DELETE CASCADE
);

GO

CREATE TABLE [UserClaims] (
    [Id] int NOT NULL IDENTITY,
    [ClaimType] nvarchar(max),
    [ClaimValue] nvarchar(max),
    [IdentityUser<int>Id] int,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserClaims_Users_IdentityUser<int>Id] FOREIGN KEY ([IdentityUser<int>Id]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [UserLogin] (
    [ProviderKey] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [IdentityUser<int>Id] int,
    [ProviderDisplayName] nvarchar(max),
    [UserId] int NOT NULL,
    CONSTRAINT [PK_UserLogin] PRIMARY KEY ([ProviderKey], [LoginProvider]),
    CONSTRAINT [FK_UserLogin_Users_IdentityUser<int>Id] FOREIGN KEY ([IdentityUser<int>Id]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [UserRole] (
    [RoleId] int NOT NULL,
    [UserId] int NOT NULL,
    [IdentityRole<int>Id] int,
    [IdentityUser<int>Id] int,
    CONSTRAINT [PK_UserRole] PRIMARY KEY ([RoleId], [UserId]),
    CONSTRAINT [FK_UserRole_Role_IdentityRole<int>Id] FOREIGN KEY ([IdentityRole<int>Id]) REFERENCES [Role] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UserRole_Users_IdentityUser<int>Id] FOREIGN KEY ([IdentityUser<int>Id]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Department] (
    [DepartmentID] int NOT NULL IDENTITY,
    [Archived] bit NOT NULL,
    [FacultyID] int NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [ProfessorID] int,
    [RowVersion] rowversion,
    CONSTRAINT [PK_Department] PRIMARY KEY ([DepartmentID]),
    CONSTRAINT [FK_Department_Faculty_FacultyID] FOREIGN KEY ([FacultyID]) REFERENCES [Faculty] ([FacultyID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Department_Users_ProfessorID] FOREIGN KEY ([ProfessorID]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Committee] (
    [CommitteeID] int NOT NULL IDENTITY,
    [Archived] bit NOT NULL,
    [DepartmentID] int,
    [FacultyID] int,
    [Level] int NOT NULL,
    [ProfessorID] int,
    [Title] nvarchar(120) NOT NULL,
    CONSTRAINT [PK_Committee] PRIMARY KEY ([CommitteeID]),
    CONSTRAINT [FK_Committee_Department_DepartmentID] FOREIGN KEY ([DepartmentID]) REFERENCES [Department] ([DepartmentID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Committee_Faculty_FacultyID] FOREIGN KEY ([FacultyID]) REFERENCES [Faculty] ([FacultyID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Committee_Users_ProfessorID] FOREIGN KEY ([ProfessorID]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Course] (
    [CourseID] int NOT NULL IDENTITY,
    [Active] bit NOT NULL,
    [Archived] bit NOT NULL,
    [Credits] int NOT NULL,
    [DepartmentID] int NOT NULL,
    [ShortTitle] nvarchar(20) NOT NULL,
    [TeachingRequestRequestID] int,
    [Title] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Course] PRIMARY KEY ([CourseID]),
    CONSTRAINT [FK_Course_Department_DepartmentID] FOREIGN KEY ([DepartmentID]) REFERENCES [Department] ([DepartmentID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Course_TeachingRequest_TeachingRequestRequestID] FOREIGN KEY ([TeachingRequestRequestID]) REFERENCES [TeachingRequest] ([RequestID]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Programs] (
    [ProgramID] int NOT NULL IDENTITY,
    [Archived] bit NOT NULL,
    [DepartmentID] int NOT NULL,
    [Short] nvarchar(20),
    [Title] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Programs] PRIMARY KEY ([ProgramID]),
    CONSTRAINT [FK_Programs_Department_DepartmentID] FOREIGN KEY ([DepartmentID]) REFERENCES [Department] ([DepartmentID]) ON DELETE NO ACTION
);

GO

CREATE TABLE [CommitieMembership] (
    [CommitteeID] int NOT NULL,
    [ProfessorID] int NOT NULL,
    [DateOfEnrollment] datetime2 NOT NULL,
    [Chair] bit NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [EstimatedEndDate] datetime2 NOT NULL,
    [FinishedWork] bit NOT NULL,
    CONSTRAINT [PK_CommitieMembership] PRIMARY KEY ([CommitteeID], [ProfessorID], [DateOfEnrollment]),
    CONSTRAINT [FK_CommitieMembership_Committee_CommitteeID] FOREIGN KEY ([CommitteeID]) REFERENCES [Committee] ([CommitteeID]) ON DELETE CASCADE,
    CONSTRAINT [FK_CommitieMembership_Users_ProfessorID] FOREIGN KEY ([ProfessorID]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Meeting] (
    [MeetingID] int NOT NULL IDENTITY,
    [CommitteeID] int NOT NULL,
    [Date] datetime2 NOT NULL,
    [FinalDate] bit NOT NULL,
    [Location] nvarchar(50),
    [OpenDate] datetime2 NOT NULL,
    [Title] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Meeting] PRIMARY KEY ([MeetingID], [CommitteeID]),
    CONSTRAINT [FK_Meeting_Committee_CommitteeID] FOREIGN KEY ([CommitteeID]) REFERENCES [Committee] ([CommitteeID]) ON DELETE CASCADE
);

GO

CREATE TABLE [CourseAssignment] (
    [AssignmentID] int NOT NULL IDENTITY,
    [AssignmentDate] datetime2 NOT NULL,
    [CourseDescription] nvarchar(50),
    [CourseID] int NOT NULL,
    [CurrentlyTought] bit NOT NULL,
    [ProfessorID] int NOT NULL,
    [SemesterID] int NOT NULL,
    CONSTRAINT [PK_CourseAssignment] PRIMARY KEY ([AssignmentID]),
    CONSTRAINT [FK_CourseAssignment_Course_CourseID] FOREIGN KEY ([CourseID]) REFERENCES [Course] ([CourseID]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseAssignment_Users_ProfessorID] FOREIGN KEY ([ProfessorID]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseAssignment_Semester_SemesterID] FOREIGN KEY ([SemesterID]) REFERENCES [Semester] ([ID]) ON DELETE CASCADE
);

GO

CREATE TABLE [RequestedCourse] (
    [CourseID] int NOT NULL,
    [RequestID] int NOT NULL,
    [Choice] int NOT NULL,
    CONSTRAINT [PK_RequestedCourse] PRIMARY KEY ([CourseID], [RequestID]),
    CONSTRAINT [FK_RequestedCourse_Course_CourseID] FOREIGN KEY ([CourseID]) REFERENCES [Course] ([CourseID]) ON DELETE CASCADE,
    CONSTRAINT [FK_RequestedCourse_TeachingRequest_RequestID] FOREIGN KEY ([RequestID]) REFERENCES [TeachingRequest] ([RequestID]) ON DELETE CASCADE
);

GO

CREATE TABLE [Enrollment] (
    [EnrollmentID] int NOT NULL IDENTITY,
    [CourseID] int NOT NULL,
    [Grade] int,
    [Notes] nvarchar(max),
    [SemesterID] int,
    [SmID] int NOT NULL,
    CONSTRAINT [PK_Enrollment] PRIMARY KEY ([EnrollmentID]),
    CONSTRAINT [FK_Enrollment_Course_CourseID] FOREIGN KEY ([CourseID]) REFERENCES [Course] ([CourseID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Enrollment_Semester_SemesterID] FOREIGN KEY ([SemesterID]) REFERENCES [Semester] ([ID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Enrollment_Users_SmID] FOREIGN KEY ([SmID]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [DateSuggestion] (
    [SuggestionID] int NOT NULL IDENTITY,
    [CommitteeID] int NOT NULL,
    [MeetingID] int NOT NULL,
    [Value] datetime2 NOT NULL,
    CONSTRAINT [PK_DateSuggestion] PRIMARY KEY ([SuggestionID]),
    CONSTRAINT [FK_DateSuggestion_Meeting_MeetingID_CommitteeID] FOREIGN KEY ([MeetingID], [CommitteeID]) REFERENCES [Meeting] ([MeetingID], [CommitteeID]) ON DELETE CASCADE
);

GO

CREATE TABLE [MeetingComment] (
    [CommentID] int NOT NULL IDENTITY,
    [Comment] nvarchar(350),
    [CommitteeID] int NOT NULL,
    [DateStamp] datetime2 NOT NULL,
    [MeetingID] int NOT NULL,
    [Private] bit NOT NULL,
    [Professor] int NOT NULL,
    [ProfessorID] int NOT NULL,
    [ProfessorName] nvarchar(max),
    CONSTRAINT [PK_MeetingComment] PRIMARY KEY ([CommentID]),
    CONSTRAINT [FK_MeetingComment_Meeting_MeetingID_CommitteeID] FOREIGN KEY ([MeetingID], [CommitteeID]) REFERENCES [Meeting] ([MeetingID], [CommitteeID]) ON DELETE CASCADE
);

GO

CREATE TABLE [FileBase] (
    [FileBaseID] int NOT NULL IDENTITY,
    [Added] datetime2 NOT NULL,
    [CommitteeID] int,
    [CourseID] int,
    [Location] nvarchar(max) NOT NULL,
    [MeetingCommentCommentID] int,
    [MeetingsCommitteeID] int,
    [MeetingsMeetingID] int,
    [Modified] datetime2 NOT NULL,
    [Owned] int NOT NULL,
    [OwnerID] int NOT NULL,
    [Size] int NOT NULL,
    [Type] int NOT NULL,
    [ViewTitle] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_FileBase] PRIMARY KEY ([FileBaseID]),
    CONSTRAINT [FK_FileBase_Committee_CommitteeID] FOREIGN KEY ([CommitteeID]) REFERENCES [Committee] ([CommitteeID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FileBase_Course_CourseID] FOREIGN KEY ([CourseID]) REFERENCES [Course] ([CourseID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FileBase_MeetingComment_MeetingCommentCommentID] FOREIGN KEY ([MeetingCommentCommentID]) REFERENCES [MeetingComment] ([CommentID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FileBase_Meeting_MeetingsMeetingID_MeetingsCommitteeID] FOREIGN KEY ([MeetingsMeetingID], [MeetingsCommitteeID]) REFERENCES [Meeting] ([MeetingID], [CommitteeID]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_CommitieMembership_CommitteeID] ON [CommitieMembership] ([CommitteeID]);

GO

CREATE INDEX [IX_CommitieMembership_ProfessorID] ON [CommitieMembership] ([ProfessorID]);

GO

CREATE INDEX [IX_CourseAssignment_CourseID] ON [CourseAssignment] ([CourseID]);

GO

CREATE INDEX [IX_CourseAssignment_ProfessorID] ON [CourseAssignment] ([ProfessorID]);

GO

CREATE INDEX [IX_CourseAssignment_SemesterID] ON [CourseAssignment] ([SemesterID]);

GO

CREATE INDEX [IX_RequestedCourse_CourseID] ON [RequestedCourse] ([CourseID]);

GO

CREATE INDEX [IX_RequestedCourse_RequestID] ON [RequestedCourse] ([RequestID]);

GO

CREATE INDEX [IX_DateSuggestion_MeetingID_CommitteeID] ON [DateSuggestion] ([MeetingID], [CommitteeID]);

GO

CREATE INDEX [IX_Enrollment_CourseID] ON [Enrollment] ([CourseID]);

GO

CREATE INDEX [IX_Enrollment_SemesterID] ON [Enrollment] ([SemesterID]);

GO

CREATE INDEX [IX_Enrollment_SmID] ON [Enrollment] ([SmID]);

GO

CREATE INDEX [IX_Committee_DepartmentID] ON [Committee] ([DepartmentID]);

GO

CREATE INDEX [IX_Committee_FacultyID] ON [Committee] ([FacultyID]);

GO

CREATE INDEX [IX_Committee_ProfessorID] ON [Committee] ([ProfessorID]);

GO

CREATE INDEX [IX_Course_DepartmentID] ON [Course] ([DepartmentID]);

GO

CREATE INDEX [IX_Course_TeachingRequestRequestID] ON [Course] ([TeachingRequestRequestID]);

GO

CREATE INDEX [IX_Department_FacultyID] ON [Department] ([FacultyID]);

GO

CREATE INDEX [IX_Department_ProfessorID] ON [Department] ([ProfessorID]);

GO

CREATE INDEX [IX_Faculty_ProfessorID] ON [Faculty] ([ProfessorID]);

GO

CREATE INDEX [IX_Programs_DepartmentID] ON [Programs] ([DepartmentID]);

GO

CREATE INDEX [IX_FileBase_CommitteeID] ON [FileBase] ([CommitteeID]);

GO

CREATE INDEX [IX_FileBase_CourseID] ON [FileBase] ([CourseID]);

GO

CREATE INDEX [IX_FileBase_MeetingCommentCommentID] ON [FileBase] ([MeetingCommentCommentID]);

GO

CREATE INDEX [IX_FileBase_MeetingsMeetingID_MeetingsCommitteeID] ON [FileBase] ([MeetingsMeetingID], [MeetingsCommitteeID]);

GO

CREATE INDEX [IX_MeetingComment_MeetingID_CommitteeID] ON [MeetingComment] ([MeetingID], [CommitteeID]);

GO

CREATE INDEX [IX_Meeting_CommitteeID] ON [Meeting] ([CommitteeID]);

GO

CREATE UNIQUE INDEX [IX_OfficeAssignment_ProfessorID] ON [OfficeAssignment] ([ProfessorID]) WHERE [ProfessorID] IS NOT NULL;

GO

CREATE INDEX [IX_TeachingRequest_ProfessorID] ON [TeachingRequest] ([ProfessorID]);

GO

CREATE INDEX [IX_TeachingRequest_SemesterID] ON [TeachingRequest] ([SemesterID]);

GO

CREATE INDEX [IX_Workloads_ProfessorID] ON [Workloads] ([ProfessorID]);

GO

CREATE INDEX [IX_Workloads_SemesterID] ON [Workloads] ([SemesterID]);

GO

CREATE INDEX [IX_RoleClaims_IdentityRole<int>Id] ON [RoleClaims] ([IdentityRole<int>Id]);

GO

CREATE INDEX [IX_Users_DatesSuggestionSuggestionID] ON [Users] ([DatesSuggestionSuggestionID]);

GO

CREATE INDEX [IX_Users_DepartmentID] ON [Users] ([DepartmentID]);

GO

CREATE INDEX [IX_Users_ProgramID] ON [Users] ([ProgramID]);

GO

CREATE INDEX [IX_UserClaims_IdentityUser<int>Id] ON [UserClaims] ([IdentityUser<int>Id]);

GO

CREATE INDEX [IX_UserLogin_IdentityUser<int>Id] ON [UserLogin] ([IdentityUser<int>Id]);

GO

CREATE INDEX [IX_UserRole_IdentityRole<int>Id] ON [UserRole] ([IdentityRole<int>Id]);

GO

CREATE INDEX [IX_UserRole_IdentityUser<int>Id] ON [UserRole] ([IdentityUser<int>Id]);

GO

ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Department_DepartmentID] FOREIGN KEY ([DepartmentID]) REFERENCES [Department] ([DepartmentID]) ON DELETE NO ACTION;

GO

ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_DateSuggestion_DatesSuggestionSuggestionID] FOREIGN KEY ([DatesSuggestionSuggestionID]) REFERENCES [DateSuggestion] ([SuggestionID]) ON DELETE NO ACTION;

GO

ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Programs_ProgramID] FOREIGN KEY ([ProgramID]) REFERENCES [Programs] ([ProgramID]) ON DELETE CASCADE;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20170415185520_init', N'1.0.1');

GO

ALTER TABLE [Workloads] ADD [Finished] bit NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20170415215634_WorklFinished', N'1.0.1');

GO

