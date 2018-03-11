/****** Object:  Table [dbo].[Address]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HouseId] [int] NULL,
	[FollowerId] [int] NULL,
 CONSTRAINT [PK_Adress] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Admin]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FollowerId] [int] NULL,
	[DateAdd] [datetime] NULL,
	[AdminKeyId] [int] NULL,
	[Enable] [bit] NULL,
	[NotyfiActive] [bit] NULL,
 CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdminKey]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AdminKey](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KeyValue] [varchar](256) NULL,
	[DateAdd] [datetime] NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_AdminKey] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AttachmentFs]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttachmentFs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GuId] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Fs] [varbinary](max) FILESTREAM  NULL,
	[Caption] [varchar](200) NULL,
	[AttachmentTypeId] [int] NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_AttachmentFs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] FILESTREAM_ON [BotDbFs],
 CONSTRAINT [UQ__Attachme__3214EC06D860A4AD] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Attachme__A2B66B0514E78B0A] UNIQUE NONCLUSTERED 
(
	[GuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] FILESTREAM_ON [BotDbFs]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AttachmentTelegram]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttachmentTelegram](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileId] [varchar](100) NULL,
	[AttachmentFsId] [int] NULL,
	[BotInfoId] [int] NULL,
 CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AttachmentType]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttachmentType](
	[Id] [int] NOT NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_AttachmentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Available�ities]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Available�ities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Timestamp] [datetime] NULL,
	[CityName] [varchar](200) NULL,
 CONSTRAINT [PK_Available�ities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Basket]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Basket](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FollowerId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Amount] [int] NULL,
	[DateAdd] [datetime] NULL,
	[Enable] [bit] NOT NULL,
	[BotInfoId] [int] NULL,
 CONSTRAINT [PK_Cart_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BotInfo]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BotInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[ChatId] [int] NULL,
	[Token] [varchar](500) NULL,
	[Timestamp] [datetime] NULL,
	[OwnerChatId] [int] NULL,
	[WebHookUrl] [varchar](100) NULL,
	[ServerVersion] [bit] NULL,
	[HomeVersion] [bit] NULL,
 CONSTRAINT [PK_BotInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Category]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[City]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[City](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[RegionId] [int] NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Company]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Company](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NULL,
	[Text] [varchar](1000) NULL,
	[City] [varchar](100) NULL,
	[Vk] [varchar](100) NULL,
	[Instagram] [varchar](100) NULL,
	[Chanel] [varchar](100) NULL,
	[Telephone] [varchar](20) NULL,
	[Chat] [varchar](100) NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Configuration]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Configuration](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExampleCsvFileId] [varchar](100) NULL,
	[TemplateCsvFileId] [varchar](100) NULL,
	[BotBlocked] [bit] NULL,
	[ManualFileId] [varchar](100) NULL,
	[PrivateGroupChatId] [varchar](50) NULL,
	[BotInfoId] [int] NULL,
	[VerifyTelephone] [bit] NULL,
	[OwnerPrivateNotify] [bit] NULL,
	[UserNameFaqFileId] [varchar](100) NULL,
	[StartTime] [time](7) NULL,
	[EndTime] [time](7) NULL,
	[Delivery] [bit] NULL,
	[Pickup] [bit] NULL,
	[ShipPrice] [float] NULL,
	[FreeShipPrice] [float] NULL,
	[CurrencyId] [int] NULL,
 CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Configuration_BotInfoId] UNIQUE NONCLUSTERED 
(
	[BotInfoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Currency]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Currency](
	[Id] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[ShortName] [varchar](10) NULL,
	[Code] [varchar](50) NULL,
 CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FeedBack]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FeedBack](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [varchar](2500) NULL,
	[DateAdd] [datetime] NULL,
	[OrderId] [int] NULL,
	[RaitingId] [int] NULL,
	[RaitingValue] [int] NULL,
	[ProductId] [int] NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_Feedback] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FeedBackAttachmentFs]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FeedBackAttachmentFs](
	[FeedBackId] [int] NOT NULL,
	[AttachmentFsId] [int] NOT NULL,
 CONSTRAINT [PK_FeedBackAttachmentFs] PRIMARY KEY CLUSTERED 
(
	[FeedBackId] ASC,
	[AttachmentFsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Follower]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Follower](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FIrstName] [varchar](500) NULL,
	[LastName] [varchar](500) NULL,
	[UserName] [varchar](500) NULL,
	[ChatType] [int] NULL,
	[Blocked] [bit] NULL,
	[Telephone] [varchar](50) NULL,
	[ChatId] [int] NULL,
	[DateAdd] [datetime] NULL,
 CONSTRAINT [PK_followers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HelpDesk]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HelpDesk](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [int] NULL,
	[Timestamp] [datetime] NULL,
	[FollowerId] [int] NULL,
	[Text] [varchar](1000) NULL,
	[Send] [bit] NULL,
	[Closed] [bit] NULL,
	[InWork] [bit] NULL,
	[BotInfoId] [int] NULL,
 CONSTRAINT [PK_HelpDesk] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HelpDeskAnswer]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HelpDeskAnswer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HelpDeskId] [int] NULL,
	[Timestamp] [datetime] NULL,
	[FollowerId] [int] NULL,
	[Text] [varchar](1000) NULL,
	[Closed] [bit] NULL,
	[ClosedTimestamp] [datetime] NULL,
 CONSTRAINT [PK_HelpDeskAnswer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HelpDeskAnswerAttachment]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HelpDeskAnswerAttachment](
	[HelpDeskAnswerId] [int] NOT NULL,
	[AttachmentFsId] [int] NOT NULL,
 CONSTRAINT [PK_HelpDeskAnswerAttachment] PRIMARY KEY CLUSTERED 
(
	[HelpDeskAnswerId] ASC,
	[AttachmentFsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HelpDeskAttachment]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HelpDeskAttachment](
	[HelpDeskId] [int] NOT NULL,
	[AttachmentFsId] [int] NOT NULL,
 CONSTRAINT [PK_HelpDeskAttachment] PRIMARY KEY CLUSTERED 
(
	[HelpDeskId] ASC,
	[AttachmentFsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HelpDeskInWork]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HelpDeskInWork](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HelpDeskId] [int] NULL,
	[FollowerId] [int] NULL,
	[Timestamp] [datetime] NULL,
	[InWork] [bit] NULL,
 CONSTRAINT [PK_HelpDeskInWork] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[House]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[House](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [varchar](10) NULL,
	[StreetId] [int] NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[ZipCode] [int] NULL,
	[Apartment] [varchar](10) NULL,
 CONSTRAINT [PK_House] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Invoice]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Invoice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreateTimestamp] [datetime] NULL,
	[LifeTimeDuration] [time](7) NULL,
	[PaymentTypeId] [int] NULL,
	[AccountNumber] [varchar](500) NULL,
	[Comment] [varchar](500) NULL,
	[Value] [float] NULL,
	[InvoiceNumber] [int] NULL,
	[Paid] [bit] NULL,
 CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Notification](
	[Id] [int] NOT NULL,
	[FollowerId] [int] NULL,
	[DateAdd] [datetime] NULL,
	[Text] [varchar](5000) NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderAddress]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderAddress](
	[AdressId] [int] NOT NULL,
	[OrderId] [int] NOT NULL,
	[ShipPriceValue] [float] NULL,
 CONSTRAINT [PK_OrderAdress] PRIMARY KEY CLUSTERED 
(
	[AdressId] ASC,
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderProduct]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductId] [int] NULL,
	[Count] [int] NULL,
	[PriceId] [int] NULL,
	[DateAdd] [datetime] NULL,
 CONSTRAINT [PK_OrderStr] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Orders]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [int] NULL,
	[FollowerId] [int] NOT NULL,
	[Text] [varchar](500) NULL,
	[DateAdd] [datetime] NULL,
	[Paid] [bit] NULL,
	[BotInfoId] [int] NULL,
	[InvoiceId] [int] NULL,
	[PickupPointId] [int] NULL,
	[CurrentStatus] [int] NULL,
	[StockUpdate] [bit] NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrdersInWork]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrdersInWork](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[Timestamp] [datetime] NULL,
	[FollowerId] [int] NULL,
	[InWork] [bit] NULL,
 CONSTRAINT [PK_OrdersInWork] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderStatus]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[StatusId] [int] NULL,
	[FollowerId] [int] NULL,
	[Timestamp] [datetime] NULL,
	[Text] [varchar](500) NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_OrderStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderTemp]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderTemp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [varchar](500) NULL,
	[AddressId] [int] NULL,
	[FollowerId] [int] NULL,
	[PaymentTypeId] [int] NULL,
	[BotInfoId] [int] NULL,
	[PickupPointId] [int] NULL,
 CONSTRAINT [PK_OrderTemp] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Payment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TxId] [varchar](200) NULL,
	[TimestampDataAdd] [datetime] NULL,
	[Comment] [varchar](500) NULL,
	[Summ] [float] NULL,
	[InvoiceId] [int] NULL,
	[TimestampTx] [datetime] NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PaymentType]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PaymentType](
	[Id] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[Enable] [bit] NULL,
	[Code] [varchar](10) NULL,
 CONSTRAINT [PK_PaymentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PaymentTypeConfig]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PaymentTypeConfig](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PaymentId] [int] NULL,
	[Enable] [bit] NULL,
	[Host] [varchar](100) NULL,
	[Port] [varchar](10) NULL,
	[Login] [varchar](100) NULL,
	[Pass] [varchar](100) NULL,
	[TimeStamp] [datetime] NULL,
 CONSTRAINT [PK_PaymentTypeConfig] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PickupPoint]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PickupPoint](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_PickupPoint] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Product]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Text] [varchar](500) NULL,
	[PhotoId] [varchar](500) NULL,
	[CategoryId] [int] NULL,
	[Enable] [bit] NULL,
	[TelegraphUrl] [varchar](200) NULL,
	[DateAdd] [datetime] NULL,
	[PhotoUrl] [varchar](500) NULL,
	[UnitId] [int] NULL,
	[Code] [varchar](50) NULL,
	[MainPhoto] [int] NULL,
	[CurrentPriceId] [int] NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProductPhoto]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductPhoto](
	[ProductId] [int] NOT NULL,
	[AttachmentFsId] [int] NOT NULL,
	[MainPhoto] [bit] NULL,
 CONSTRAINT [PK_ProductPhoto] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC,
	[AttachmentFsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductPrice]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductPrice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Value] [float] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[DateAdd] [datetime] NULL,
	[Volume] [int] NULL,
	[CurrencyId] [int] NULL,
 CONSTRAINT [PK_Price] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Raiting]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Raiting](
	[Id] [int] NOT NULL,
	[Value] [smallint] NULL,
 CONSTRAINT [PK_Raiting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Region]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Region](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NULL,
 CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReportsRequestLog]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportsRequestLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateAdd] [datetime] NULL,
	[FollowerId] [int] NULL,
 CONSTRAINT [PK_ReportsRequestLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Status]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Status](
	[Id] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Stock]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Stock](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Balance] [int] NULL,
	[DateAdd] [datetime] NULL,
	[Text] [varchar](500) NULL,
 CONSTRAINT [PK_Stock] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Street]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Street](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NULL,
	[CityId] [int] NULL,
 CONSTRAINT [PK_Street] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TelegramMessage]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TelegramMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UpdateId] [int] NULL,
	[MessageId] [varchar](200) NULL,
	[FollowerId] [int] NULL,
	[UpdateJson] [varchar](max) NULL,
	[DateAdd] [datetime] NULL,
	[BotInfoId] [int] NULL,
 CONSTRAINT [PK_TelegramMessage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Units]    Script Date: 11.03.2018 19:50:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Units](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[ShortName] [varchar](5) NULL,
 CONSTRAINT [PK_Units] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_House] FOREIGN KEY([HouseId])
REFERENCES [dbo].[House] ([Id])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_House]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Adress_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Adress_Follower]
GO
ALTER TABLE [dbo].[Admin]  WITH CHECK ADD  CONSTRAINT [FK_Admin_AdminKey] FOREIGN KEY([AdminKeyId])
REFERENCES [dbo].[AdminKey] ([Id])
GO
ALTER TABLE [dbo].[Admin] CHECK CONSTRAINT [FK_Admin_AdminKey]
GO
ALTER TABLE [dbo].[Admin]  WITH CHECK ADD  CONSTRAINT [FK_Admin_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[Admin] CHECK CONSTRAINT [FK_Admin_Follower]
GO
ALTER TABLE [dbo].[AttachmentFs]  WITH CHECK ADD  CONSTRAINT [FK_AttachmentFs_AttachmentType] FOREIGN KEY([AttachmentTypeId])
REFERENCES [dbo].[AttachmentType] ([Id])
GO
ALTER TABLE [dbo].[AttachmentFs] CHECK CONSTRAINT [FK_AttachmentFs_AttachmentType]
GO
ALTER TABLE [dbo].[AttachmentTelegram]  WITH CHECK ADD  CONSTRAINT [FK_Attachment_AttachmentFs] FOREIGN KEY([AttachmentFsId])
REFERENCES [dbo].[AttachmentFs] ([Id])
GO
ALTER TABLE [dbo].[AttachmentTelegram] CHECK CONSTRAINT [FK_Attachment_AttachmentFs]
GO
ALTER TABLE [dbo].[AttachmentTelegram]  WITH CHECK ADD  CONSTRAINT [FK_Attachment_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[AttachmentTelegram] CHECK CONSTRAINT [FK_Attachment_BotInfo]
GO
ALTER TABLE [dbo].[Basket]  WITH CHECK ADD  CONSTRAINT [FK_Basket_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[Basket] CHECK CONSTRAINT [FK_Basket_BotInfo]
GO
ALTER TABLE [dbo].[Basket]  WITH CHECK ADD  CONSTRAINT [FK_Cart_Follower1] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[Basket] CHECK CONSTRAINT [FK_Cart_Follower1]
GO
ALTER TABLE [dbo].[Basket]  WITH CHECK ADD  CONSTRAINT [FK_Cart_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[Basket] CHECK CONSTRAINT [FK_Cart_Product]
GO
ALTER TABLE [dbo].[City]  WITH CHECK ADD  CONSTRAINT [FK_City_Region] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([Id])
GO
ALTER TABLE [dbo].[City] CHECK CONSTRAINT [FK_City_Region]
GO
ALTER TABLE [dbo].[Configuration]  WITH CHECK ADD  CONSTRAINT [FK_Configuration_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[Configuration] CHECK CONSTRAINT [FK_Configuration_BotInfo]
GO
ALTER TABLE [dbo].[Configuration]  WITH CHECK ADD  CONSTRAINT [FK_Configuration_Currency] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([Id])
GO
ALTER TABLE [dbo].[Configuration] CHECK CONSTRAINT [FK_Configuration_Currency]
GO
ALTER TABLE [dbo].[FeedBack]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[FeedBack] CHECK CONSTRAINT [FK_Feedback_Orders]
GO
ALTER TABLE [dbo].[FeedBack]  WITH CHECK ADD  CONSTRAINT [FK_FeedBack_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[FeedBack] CHECK CONSTRAINT [FK_FeedBack_Product]
GO
ALTER TABLE [dbo].[FeedBack]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Raiting] FOREIGN KEY([RaitingId])
REFERENCES [dbo].[Raiting] ([Id])
GO
ALTER TABLE [dbo].[FeedBack] CHECK CONSTRAINT [FK_Feedback_Raiting]
GO
ALTER TABLE [dbo].[FeedBackAttachmentFs]  WITH CHECK ADD  CONSTRAINT [FK_FeedBackAttachmentFs_AttachmentFs] FOREIGN KEY([AttachmentFsId])
REFERENCES [dbo].[AttachmentFs] ([Id])
GO
ALTER TABLE [dbo].[FeedBackAttachmentFs] CHECK CONSTRAINT [FK_FeedBackAttachmentFs_AttachmentFs]
GO
ALTER TABLE [dbo].[FeedBackAttachmentFs]  WITH CHECK ADD  CONSTRAINT [FK_FeedBackAttachmentFs_FeedBack] FOREIGN KEY([FeedBackId])
REFERENCES [dbo].[FeedBack] ([Id])
GO
ALTER TABLE [dbo].[FeedBackAttachmentFs] CHECK CONSTRAINT [FK_FeedBackAttachmentFs_FeedBack]
GO
ALTER TABLE [dbo].[HelpDesk]  WITH CHECK ADD  CONSTRAINT [FK_HelpDesk_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[HelpDesk] CHECK CONSTRAINT [FK_HelpDesk_BotInfo]
GO
ALTER TABLE [dbo].[HelpDesk]  WITH CHECK ADD  CONSTRAINT [FK_HelpDesk_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[HelpDesk] CHECK CONSTRAINT [FK_HelpDesk_Follower]
GO
ALTER TABLE [dbo].[HelpDeskAnswer]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskAnswer_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskAnswer] CHECK CONSTRAINT [FK_HelpDeskAnswer_Follower]
GO
ALTER TABLE [dbo].[HelpDeskAnswer]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskAnswer_HelpDesk] FOREIGN KEY([HelpDeskId])
REFERENCES [dbo].[HelpDesk] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskAnswer] CHECK CONSTRAINT [FK_HelpDeskAnswer_HelpDesk]
GO
ALTER TABLE [dbo].[HelpDeskAnswerAttachment]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskAnswerAttachment_Attachment] FOREIGN KEY([AttachmentFsId])
REFERENCES [dbo].[AttachmentFs] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskAnswerAttachment] CHECK CONSTRAINT [FK_HelpDeskAnswerAttachment_Attachment]
GO
ALTER TABLE [dbo].[HelpDeskAnswerAttachment]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskAnswerAttachment_HelpDesk] FOREIGN KEY([HelpDeskAnswerId])
REFERENCES [dbo].[HelpDeskAnswer] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskAnswerAttachment] CHECK CONSTRAINT [FK_HelpDeskAnswerAttachment_HelpDesk]
GO
ALTER TABLE [dbo].[HelpDeskAttachment]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskAttachment_Attachment] FOREIGN KEY([AttachmentFsId])
REFERENCES [dbo].[AttachmentFs] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskAttachment] CHECK CONSTRAINT [FK_HelpDeskAttachment_Attachment]
GO
ALTER TABLE [dbo].[HelpDeskAttachment]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskAttachment_HelpDesk] FOREIGN KEY([HelpDeskId])
REFERENCES [dbo].[HelpDesk] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskAttachment] CHECK CONSTRAINT [FK_HelpDeskAttachment_HelpDesk]
GO
ALTER TABLE [dbo].[HelpDeskInWork]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskInWork_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskInWork] CHECK CONSTRAINT [FK_HelpDeskInWork_Follower]
GO
ALTER TABLE [dbo].[HelpDeskInWork]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskInWork_HelpDesk] FOREIGN KEY([HelpDeskId])
REFERENCES [dbo].[HelpDesk] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskInWork] CHECK CONSTRAINT [FK_HelpDeskInWork_HelpDesk]
GO
ALTER TABLE [dbo].[House]  WITH CHECK ADD  CONSTRAINT [FK_House_Street] FOREIGN KEY([StreetId])
REFERENCES [dbo].[Street] ([Id])
GO
ALTER TABLE [dbo].[House] CHECK CONSTRAINT [FK_House_Street]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_PaymentType] FOREIGN KEY([PaymentTypeId])
REFERENCES [dbo].[PaymentType] ([Id])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_PaymentType]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_Follower]
GO
ALTER TABLE [dbo].[OrderAddress]  WITH CHECK ADD  CONSTRAINT [FK_OrderAdress_Adress] FOREIGN KEY([AdressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[OrderAddress] CHECK CONSTRAINT [FK_OrderAdress_Adress]
GO
ALTER TABLE [dbo].[OrderAddress]  WITH CHECK ADD  CONSTRAINT [FK_OrderAdress_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderAddress] CHECK CONSTRAINT [FK_OrderAdress_Order]
GO
ALTER TABLE [dbo].[OrderProduct]  WITH CHECK ADD  CONSTRAINT [FK_OrdersStr_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[OrderProduct] CHECK CONSTRAINT [FK_OrdersStr_Product]
GO
ALTER TABLE [dbo].[OrderProduct]  WITH CHECK ADD  CONSTRAINT [FK_OrderStr_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderProduct] CHECK CONSTRAINT [FK_OrderStr_Order]
GO
ALTER TABLE [dbo].[OrderProduct]  WITH CHECK ADD  CONSTRAINT [FK_OrderStr_Price] FOREIGN KEY([PriceId])
REFERENCES [dbo].[ProductPrice] ([Id])
GO
ALTER TABLE [dbo].[OrderProduct] CHECK CONSTRAINT [FK_OrderStr_Price]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Order_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Order_Follower]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_BotInfo]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Invoice]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_OrderStatus] FOREIGN KEY([CurrentStatus])
REFERENCES [dbo].[OrderStatus] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_OrderStatus]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_PickupPoint] FOREIGN KEY([PickupPointId])
REFERENCES [dbo].[PickupPoint] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_PickupPoint]
GO
ALTER TABLE [dbo].[OrdersInWork]  WITH CHECK ADD  CONSTRAINT [FK_OrdersInWork_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[OrdersInWork] CHECK CONSTRAINT [FK_OrdersInWork_Follower]
GO
ALTER TABLE [dbo].[OrdersInWork]  WITH CHECK ADD  CONSTRAINT [FK_OrdersInWork_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrdersInWork] CHECK CONSTRAINT [FK_OrdersInWork_Orders]
GO
ALTER TABLE [dbo].[OrderStatus]  WITH CHECK ADD  CONSTRAINT [FK_OrderStatus_Status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([Id])
GO
ALTER TABLE [dbo].[OrderStatus] CHECK CONSTRAINT [FK_OrderStatus_Status]
GO
ALTER TABLE [dbo].[OrderTemp]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemp_Address] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[OrderTemp] CHECK CONSTRAINT [FK_OrderTemp_Address]
GO
ALTER TABLE [dbo].[OrderTemp]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemp_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[OrderTemp] CHECK CONSTRAINT [FK_OrderTemp_BotInfo]
GO
ALTER TABLE [dbo].[OrderTemp]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemp_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[OrderTemp] CHECK CONSTRAINT [FK_OrderTemp_Follower]
GO
ALTER TABLE [dbo].[OrderTemp]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemp_PaymentType] FOREIGN KEY([PaymentTypeId])
REFERENCES [dbo].[PaymentType] ([Id])
GO
ALTER TABLE [dbo].[OrderTemp] CHECK CONSTRAINT [FK_OrderTemp_PaymentType]
GO
ALTER TABLE [dbo].[OrderTemp]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemp_PickupPoint] FOREIGN KEY([PickupPointId])
REFERENCES [dbo].[PickupPoint] ([Id])
GO
ALTER TABLE [dbo].[OrderTemp] CHECK CONSTRAINT [FK_OrderTemp_PickupPoint]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([Id])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_Invoice]
GO
ALTER TABLE [dbo].[PaymentTypeConfig]  WITH CHECK ADD  CONSTRAINT [FK_PaymentTypeConfig_PaymentType] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[PaymentType] ([Id])
GO
ALTER TABLE [dbo].[PaymentTypeConfig] CHECK CONSTRAINT [FK_PaymentTypeConfig_PaymentType]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_AttachmentFs] FOREIGN KEY([MainPhoto])
REFERENCES [dbo].[AttachmentFs] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_AttachmentFs]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Category]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductPrice] FOREIGN KEY([CurrentPriceId])
REFERENCES [dbo].[ProductPrice] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductPrice]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Units] FOREIGN KEY([UnitId])
REFERENCES [dbo].[Units] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Units]
GO
ALTER TABLE [dbo].[ProductPhoto]  WITH CHECK ADD  CONSTRAINT [FK_ProductPhoto_AttachmentFs] FOREIGN KEY([AttachmentFsId])
REFERENCES [dbo].[AttachmentFs] ([Id])
GO
ALTER TABLE [dbo].[ProductPhoto] CHECK CONSTRAINT [FK_ProductPhoto_AttachmentFs]
GO
ALTER TABLE [dbo].[ProductPhoto]  WITH CHECK ADD  CONSTRAINT [FK_ProductPhoto_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[ProductPhoto] CHECK CONSTRAINT [FK_ProductPhoto_Product]
GO
ALTER TABLE [dbo].[ProductPrice]  WITH CHECK ADD  CONSTRAINT [FK_Price_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[ProductPrice] CHECK CONSTRAINT [FK_Price_Product]
GO
ALTER TABLE [dbo].[ProductPrice]  WITH CHECK ADD  CONSTRAINT [FK_ProductPrice_Currency] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([Id])
GO
ALTER TABLE [dbo].[ProductPrice] CHECK CONSTRAINT [FK_ProductPrice_Currency]
GO
ALTER TABLE [dbo].[ReportsRequestLog]  WITH CHECK ADD  CONSTRAINT [FK_ReportsRequestLog_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[ReportsRequestLog] CHECK CONSTRAINT [FK_ReportsRequestLog_Follower]
GO
ALTER TABLE [dbo].[Stock]  WITH CHECK ADD  CONSTRAINT [FK_Stock_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[Stock] CHECK CONSTRAINT [FK_Stock_Product]
GO
ALTER TABLE [dbo].[Street]  WITH CHECK ADD  CONSTRAINT [FK_Street_City] FOREIGN KEY([CityId])
REFERENCES [dbo].[City] ([Id])
GO
ALTER TABLE [dbo].[Street] CHECK CONSTRAINT [FK_Street_City]
GO
ALTER TABLE [dbo].[TelegramMessage]  WITH CHECK ADD  CONSTRAINT [FK_TelegramMessage_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[TelegramMessage] CHECK CONSTRAINT [FK_TelegramMessage_BotInfo]
GO
ALTER TABLE [dbo].[TelegramMessage]  WITH CHECK ADD  CONSTRAINT [FK_TelegramMessage_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[TelegramMessage] CHECK CONSTRAINT [FK_TelegramMessage_Follower]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'������� ���������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Units'
GO
