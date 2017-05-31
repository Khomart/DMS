ALTER TABLE [Semester] ADD [EndingDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.000';

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20170515010216_semesterenddate', N'1.0.1');

GO

