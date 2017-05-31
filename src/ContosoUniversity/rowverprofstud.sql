ALTER TABLE [Users] ADD [RowVersion] rowversion;

GO

ALTER TABLE [MeetingComment] ADD [RowVersion] rowversion;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20170530230144_rowVerProfStud', N'1.0.1');

GO

