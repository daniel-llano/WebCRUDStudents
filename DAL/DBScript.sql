USE [master]
GO

IF EXISTS(select * from sys.databases where name='Students')
ALTER DATABASE Students SET OFFLINE WITH ROLLBACK IMMEDIATE
IF EXISTS(select * from sys.databases where name='Students')
ALTER DATABASE Students SET ONLINE
IF EXISTS(select * from sys.databases where name='Students')
DROP DATABASE [Students]


CREATE DATABASE [Students]
GO

USE [Students]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[student](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](500) NOT NULL,
	[type] [varchar](10) NOT NULL,
	[gender] [char](1) NOT NULL,
	[enabled] [bit] NOT NULL,
	[updated_on] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('Anthony "Tony" Edward Stark','University','M',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('Thor Odinson','University','M',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('Dr. Henry Jonathan "Hank" Pym','University','M',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('Janet van Dyne','University','F',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('Dr. Robert Bruce Banner','University','M',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('Steven "Steve" Rogers','High','M',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('Clinton Francis Barton','Elementary','M',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('Pietro Maximoff','Kinder','M',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('Wanda Maximoff','Kinder','F',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('T''Challa','University','M',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('Natasha Alianovna Romanoff','Elementary','F',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('Samuel "Snap" Thomas Wilson','Elementary','M',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES ('James Rupert "Rhodey" Rhodes','Kinder','M',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES('Miles Morales','Kinder','M',1,GETDATE())
GO
INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) VALUES('Wade Wilson','Kinder','M',1,GETDATE())
GO


CREATE PROC [dbo].[addStudent]  
	@name varchar(500) = null,
	@type varchar(10) = null,
	@gender char(1) = null
AS  
	SET NOCOUNT ON  
	SET XACT_ABORT ON   
 
	BEGIN TRAN 
 
	INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) 
	SELECT @name, @type, @gender, 1, GETDATE()  
 
	COMMIT 

	RETURN SCOPE_IDENTITY() 
GO

CREATE PROC [dbo].[addFullStudent]  
	@name varchar(500) = null,
	@type varchar(10) = null,
	@gender char(1) = null,
	@enabled bit = null,
	@updated_on datetime = null
AS  
	SET NOCOUNT ON  
	SET XACT_ABORT ON   
 
	BEGIN TRAN 
 
	INSERT INTO [dbo].[student] ([name],[type],[gender],[enabled],[updated_on]) 
	SELECT @name, @type, @gender, @enabled, @updated_on 
 
	COMMIT 

	RETURN SCOPE_IDENTITY() 
GO


CREATE PROC [dbo].[delStudent]  
	@id bigint 
AS  
	SET NOCOUNT ON  
	SET XACT_ABORT ON   
 
	BEGIN TRAN 
 
	DELETE 
	FROM   [student] 
	WHERE  [id] = @id 
 
	COMMIT 
GO


CREATE PROC [dbo].[hideStudent]  
	@id bigint
AS  
	SET NOCOUNT ON  
	SET XACT_ABORT ON   
 
	BEGIN TRAN 
 
	UPDATE [student] 
	SET    [enabled] = 0, [updated_on] = GETDATE() 
	WHERE  [id] = @id 
 
	COMMIT TRAN 

	SELECT [id],[name],[type],[gender],[enabled],[updated_on] 
	FROM   [student] 
	WHERE  [id] = @id 
GO 


CREATE PROC [dbo].[updStudent]  
	@id bigint = null,
	@name varchar(500) = null,
	@type varchar(10) = null,
	@gender char(1) = null,
	@enabled bit = null
AS  
	SET NOCOUNT ON  
	SET XACT_ABORT ON   
 
	BEGIN TRAN 
 
	UPDATE [student] 
	SET  [name] = @name,[type]=@type,[gender]=@gender,[enabled]=@enabled,[updated_on]=GETDATE() 
	WHERE  [id] = @id 
 
	COMMIT TRAN 

	SELECT [id],[name],[type],[gender],[enabled],[updated_on] 
	FROM   [student] 
	WHERE  [id] = @id 
GO


CREATE PROC [dbo].[selStudentById]
	@id bigint  
AS  
	SET NOCOUNT ON  
	SET XACT_ABORT ON   
 
	BEGIN TRAN 
 
	SELECT [id],[name],[type],[gender],[enabled],[updated_on]  
	FROM   [student]
	WHERE  [id] = @id 
  
	COMMIT 
GO


CREATE PROC [dbo].[selPaginatedStudentsWhere] 
	@OFFSET BIGINT,
	@LIMIT BIGINT,
	@COUNT BIGINT OUTPUT,
	@TPAGS BIGINT OUTPUT, 
	@field varchar(10),
	@cond varchar(2000),
	@sort varchar(4),
	@enabled bit
AS  
	SET NOCOUNT ON  
	SET XACT_ABORT ON   

	DECLARE @SQLStatement nvarchar(max)
	DECLARE @outPut NVARCHAR(50)
	
	SET @OFFSET = (@OFFSET - 1) * @LIMIT;

	IF @cond <> '' 
		SET @cond = ' AND ' + @cond; 
	ELSE
		SET @cond = '' 
		
	SET @SQLStatement = 
	'select top ' + CAST(@LIMIT as varchar(20)) + ' * from (
		select [id],[name],[type],[gender],[enabled],[updated_on], ROW_NUMBER() over (order by ' + @field + ' ' + @sort + ') as row_number  
		from [student] where [enabled] = ' + CAST(@enabled AS CHAR(1)) + @cond + 
	') resultSet where row_number > ' + CAST(@OFFSET as varchar(20))
	
	EXEC sp_executesql @SQLStatement

	SET @outPut = N'@ICOUNT int OUTPUT'
	SET @SQLStatement = 'SELECT @ICOUNT = COUNT(*) FROM [student] where [enabled] = ' + CAST(@enabled AS CHAR(1)) + @cond

	EXEC sp_executesql @SQLStatement,@outPut,@ICOUNT=@COUNT OUTPUT;

	IF @COUNT % @LIMIT > 0 BEGIN
		SET @TPAGS = 1;
	END
	ELSE
	BEGIN
		SET @TPAGS = 0;
	END

	SET @TPAGS = @TPAGS + ROUND(@COUNT / @LIMIT, 2, 1);
GO


USE [master]
GO
ALTER DATABASE [Students] SET READ_WRITE 
GO