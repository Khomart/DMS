ALTER TABLE [Programs] ADD [RowVersion] rowversion;

GO

ALTER TABLE [Semester] ADD [RowVersion] rowversion;

GO

ALTER TABLE [Faculty] ADD [RowVersion] rowversion;

GO

ALTER TABLE [Course] ADD [RowVersion] rowversion;

GO

ALTER TABLE [Committee] ADD [RowVersion] rowversion;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20170526024834_RowVersionAll', N'1.0.1');

GO

