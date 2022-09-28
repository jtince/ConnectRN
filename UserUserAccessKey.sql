/****** Object:  Table [dbo].[ContentType]    Script Date: 9/25/2022 10:41:26 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserInfo](
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
	[FirstName] [varchar](255) NULL,
	[LastName] [varchar](255) NULL,
	[City] [varchar](1000) NULL,
	[ZipCode] [varchar](10) NOT NULL
)
GO



CREATE TABLE [dbo].[UserAccess](
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[AccessKey] [varchar](255) NULL,
	[ModifiedDate] [datetimeoffset](7) NULL,
	[IsActive] [bit] NULL
	CONSTRAINT [FK_UserPassword_UserInfo] FOREIGN KEY (UserId) REFERENCES [dbo].[UserInfo]([Id]) ON DELETE CASCADE
)
GO

CREATE CERTIFICATE UserEncryption  
   WITH SUBJECT = 'User password encryption';  
GO  

CREATE SYMMETRIC KEY PW_Key_01  
    WITH ALGORITHM = AES_256  
    ENCRYPTION BY CERTIFICATE UserEcryption;  
GO  

USE [dbo];  
GO  

-- Create a column in which to store the encrypted data.  
ALTER TABLE dbo.UserAccess  
    ADD EncryptedAcess varbinary(255);   
GO  

-- Open the symmetric key with which to encrypt the data.  
OPEN SYMMETRIC KEY PW_Key_01  
   DECRYPTION BY CERTIFICATE UserEncryption;  

-- Encrypt the value in column NationalIDNumber with symmetric   
-- key SSN_Key_01. Save the result in column EncryptedNationalIDNumber.  
UPDATE [dbo].[UserAccess]  
SET EncryptedAcess = EncryptByKey(Key_GUID('PW_Key_01'), AccessKey);  
GO  

-- Verify the encryption.  
-- First, open the symmetric key with which to decrypt the data.  
OPEN SYMMETRIC KEY PW_Key_01  
   DECRYPTION BY CERTIFICATE UserEncryption;  
GO  

-- Now list the original ID, the encrypted ID, and the   
-- decrypted ciphertext. If the decryption worked, the original  
-- and the decrypted ID will match.  
SELECT AccessKey, EncryptedAccess   
    AS 'Encrypted Access Key',  
    CONVERT(nvarchar, DecryptByKey(EncryptedAccess))   
    AS 'Decrypted Access Key'  
    FROM dbo.UserAccess;  
GO  

SELECT Id, UserId, AccessKey FROM dbo.UserAccess WHERE IsActive = 1
GO

CREATE PROCEDURE [dbo].[usp_update_useraccess] (@userid uniqueidentifier,@accessKey nvarchar(255))
AS
BEGIN
	UPDATE dbo.UserAccess SET IsActive = 0 WHERE UserId = @userid and IsActive = 1
		  
	INSERT INTO dbo.UserAccess (UserId, AccessKey, ModifiedDate, IsActive)
	VALUES
	(@userid, @accessKey, GETDATE(), 1)
END
GO


