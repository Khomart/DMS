ALTER TABLE [Meeting] ADD [Archived] bit NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20170522224058_archMeeting', N'1.0.1');

GO

