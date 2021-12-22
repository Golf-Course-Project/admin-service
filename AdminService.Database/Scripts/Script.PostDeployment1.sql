

DECLARE @CustomerId AS NVARCHAR(64) = 'b27cee43-4bd4-4c1d-923f-24f455729565';

IF NOT EXISTS(SELECT Id FROM [Groups] WHERE Id = 1)
BEGIN
    INSERT INTO Groups ([Id], [Type], [Name], [IsActive])
    VALUES (1, 'settings', 'smtp', 1)
END

IF NOT EXISTS (SELECT Id FROM [Settings] WHERE Id = 100)
BEGIN
    INSERT INTO Settings ([Id], [CustomerId], [GroupId], [Key], [Value], [IsEncrypted])
    VALUES(10, @CustomerId, 1, 'smtp_port', '587', 0)
END

IF NOT EXISTS (SELECT Id FROM [Settings] WHERE Id = 101)
BEGIN
    INSERT INTO Settings ([Id], [CustomerId], [GroupId], [Key], [Value], [IsEncrypted])
    VALUES(11, @CustomerId, 1, 'smtp_from', NULL, 1)
END

IF NOT EXISTS (SELECT Id FROM [Settings] WHERE Id = 102)
BEGIN
    INSERT INTO Settings ([Id], [CustomerId], [GroupId], [Key], [Value], [IsEncrypted])
    VALUES(12, @CustomerId, 1, 'smtp_host', 'smtp.gmail.com', 0)
END

IF NOT EXISTS (SELECT Id FROM [Settings] WHERE Id = 103)
BEGIN
    INSERT INTO Settings ([Id], [CustomerId], [GroupId], [Key], [Value], [IsEncrypted])
    VALUES(13, @CustomerId, 1, 'smtp_username', NULL, 1)
END

IF NOT EXISTS (SELECT Id FROM [Settings] WHERE Id = 104)
BEGIN
    INSERT INTO Settings ([Id], [CustomerId], [GroupId], [Key], [Value], [IsEncrypted])
    VALUES(14, @CustomerId, 1, 'smtp_password', NULL, 1)
END
