CREATE TABLE [dbo].[Settings]
(
	[Id] INT NOT NULL PRIMARY KEY , 
    [CustomerId] NVARCHAR(64) NOT NULL,
    [GroupId] INT NOT NULL, 
    [Key] NVARCHAR(128) NOT NULL, 
    [Value] NVARCHAR(256) NULL, 
    [IsEncrypted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Settings_ToGroups] FOREIGN KEY ([GroupId]) REFERENCES [Groups]([Id])
)

GO

CREATE INDEX [IX_Settings_GroupId] ON [dbo].[Settings] ([GroupId])
