ALTER TABLE [Committee] ADD [Commentary] nvarchar(300) NOT NULL DEFAULT N'';

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20170518235640_committeeCommentary', N'1.0.1');

GO

