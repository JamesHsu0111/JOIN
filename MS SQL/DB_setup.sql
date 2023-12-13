create database ProjectX
go

use ProjectX
go


CREATE TABLE [dbo].[MyActivity](
	[ActivityID] [int] IDENTITY(1,1) NOT NULL,
	[ActivityName] [nvarchar](255) NULL,
	[Category] [nvarchar](255) NULL,
	[SuggestedAmount] [money] NULL,
	[ActivityContent] [nvarchar](max) NULL,
	[MinAttendee] [int] NULL,
	[VoteDate] [smalldatetime] NULL,
	[ExpectedDepartureMonth] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[ActivityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ActivityName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



--�|����ƪ�

CREATE TABLE [dbo].[Member](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Nickname] [nvarchar](50) NULL,
	[Account] [nvarchar](50) NULL,
	[Password] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
	[Phone] [nvarchar](20) NULL,
	[Intro] [nvarchar](max) NULL,
	[UserPhoto] [varbinary](max) NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Phone] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Account] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Nickname] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Member] ADD  DEFAULT ((0)) FOR [IsActive]
GO



--���θ�ƪ�

CREATE TABLE [dbo].[Group](
	[GroupID] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [nvarchar](255) NULL,
	[GroupCategory] [nvarchar](50) NULL,
	[GroupContent] [nvarchar](max) NULL,
	[MinAttendee] [int] NULL,
	[MaxAttendee] [int] NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[Organizer] [int] NULL,
	[OriginalActivityID] [int] NULL,
	[HasSent] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[GroupName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Group] ADD  DEFAULT ((0)) FOR [HasSent]
GO

ALTER TABLE [dbo].[Group]  WITH CHECK ADD  CONSTRAINT [FK_Organizer] FOREIGN KEY([Organizer])
REFERENCES [dbo].[Member] ([UserID])
GO

ALTER TABLE [dbo].[Group] CHECK CONSTRAINT [FK_Organizer]
GO

ALTER TABLE [dbo].[Group]  WITH CHECK ADD  CONSTRAINT [FK_OriginalActivity] FOREIGN KEY([OriginalActivityID])
REFERENCES [dbo].[MyActivity] ([ActivityID])
GO

ALTER TABLE [dbo].[Group] CHECK CONSTRAINT [FK_OriginalActivity]
GO


--�벼�ɶ���ƪ�

CREATE TABLE [dbo].[VoteTime](
	[VoteID] [int] IDENTITY(1,1) NOT NULL,
	[ActivityID] [int] NULL,
	[StartDate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[VoteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VoteTime]  WITH CHECK ADD  CONSTRAINT [FK_VoteTime_Activity] FOREIGN KEY([ActivityID])
REFERENCES [dbo].[MyActivity] ([ActivityID])
GO

ALTER TABLE [dbo].[VoteTime] CHECK CONSTRAINT [FK_VoteTime_Activity]
GO



--�벼������ƪ�

CREATE TABLE [dbo].[VoteRecord](
	[RecordID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[ActivityID] [int] NULL,
	[VoteResult] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[RecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VoteRecord]  WITH CHECK ADD  CONSTRAINT [FK_VoteRecord_ActivityID] FOREIGN KEY([ActivityID])
REFERENCES [dbo].[MyActivity] ([ActivityID])
GO

ALTER TABLE [dbo].[VoteRecord] CHECK CONSTRAINT [FK_VoteRecord_ActivityID]
GO

ALTER TABLE [dbo].[VoteRecord]  WITH CHECK ADD  CONSTRAINT [FK_VoteRecord_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Member] ([UserID])
GO

ALTER TABLE [dbo].[VoteRecord] CHECK CONSTRAINT [FK_VoteRecord_UserID]
GO



--���W������ƪ�

CREATE TABLE [dbo].[Registration](
	[RegistrationID] [int] IDENTITY(1,1) NOT NULL,
	[GroupID] [int] NULL,
	[ParticipantID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[RegistrationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Registration]  WITH CHECK ADD  CONSTRAINT [FK_Registration_Group] FOREIGN KEY([GroupID])
REFERENCES [dbo].[Group] ([GroupID])
GO

ALTER TABLE [dbo].[Registration] CHECK CONSTRAINT [FK_Registration_Group]
GO

ALTER TABLE [dbo].[Registration]  WITH CHECK ADD  CONSTRAINT [FK_Registration_User] FOREIGN KEY([ParticipantID])
REFERENCES [dbo].[Member] ([UserID])
GO

ALTER TABLE [dbo].[Registration] CHECK CONSTRAINT [FK_Registration_User]
GO



--���ʦ��ø�ƪ�

CREATE TABLE [dbo].[LikeRecord](
	[LikeRecordID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[ActivityID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[LikeRecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_UserActivityLike] UNIQUE NONCLUSTERED 
(
	[UserID] ASC,
	[ActivityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LikeRecord]  WITH CHECK ADD  CONSTRAINT [FK_ActivityID] FOREIGN KEY([ActivityID])
REFERENCES [dbo].[MyActivity] ([ActivityID])
GO

ALTER TABLE [dbo].[LikeRecord] CHECK CONSTRAINT [FK_ActivityID]
GO

ALTER TABLE [dbo].[LikeRecord]  WITH CHECK ADD  CONSTRAINT [FK_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Member] ([UserID])
GO

ALTER TABLE [dbo].[LikeRecord] CHECK CONSTRAINT [FK_UserID]
GO



-- �q����ƪ�

CREATE TABLE [dbo].[Notification](
	[NotificationID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[NotificationContent] [nvarchar](max) NOT NULL,
	[IsRead] [bit] NOT NULL,
	[NotificationDate] [datetime] NULL,
	[NotificationType] [nvarchar](25) NULL,
	[NotificationToWhichActivityID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[NotificationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Notification] ADD  DEFAULT ((0)) FOR [IsRead]
GO

ALTER TABLE [dbo].[Notification] ADD  DEFAULT (sysdatetime()) FOR [NotificationDate]
GO

ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_User_Notification] FOREIGN KEY([UserID])
REFERENCES [dbo].[Member] ([UserID])
GO

ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_User_Notification]
GO

ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [CK_IsRead] CHECK  (([IsRead]=(1) OR [IsRead]=(0)))
GO

ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [CK_IsRead]
GO



--�x�謡�ʷӤ���ƪ�

CREATE TABLE [dbo].[OfficialPhoto](
	[OfficialPhotoID] [int] IDENTITY(1,1) NOT NULL,
	[ActivityID] [int] NULL,
	[PhotoPath] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OfficialPhotoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[OfficialPhoto]  WITH CHECK ADD  CONSTRAINT [FK_Activity_Photo] FOREIGN KEY([ActivityID])
REFERENCES [dbo].[MyActivity] ([ActivityID])
GO

ALTER TABLE [dbo].[OfficialPhoto] CHECK CONSTRAINT [FK_Activity_Photo]
GO



--�ӤH�}�ά��ʷӤ���ƪ�

CREATE TABLE [dbo].[PersonalPhoto](
	[PersonalPhotoID] [int] IDENTITY(1,1) NOT NULL,
	[GroupID] [int] NULL,
	[PhotoData] [varbinary](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PersonalPhotoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[PersonalPhoto]  WITH CHECK ADD  CONSTRAINT [FK_PersonalPhoto_Group] FOREIGN KEY([GroupID])
REFERENCES [dbo].[Group] ([GroupID])
GO

ALTER TABLE [dbo].[PersonalPhoto] CHECK CONSTRAINT [FK_PersonalPhoto_Group]
GO



--�d���O��ƪ�

CREATE TABLE [dbo].[Chat](
	[ChatID] [int] IDENTITY(1,1) NOT NULL,
	[ActivityID] [int] NULL,
	[UserID] [int] NULL,
	[ChatContent] [nvarchar](max) NOT NULL,
	[ToWhom] [int] NULL,
	[ChatTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ChatID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Chat] ADD  DEFAULT (sysdatetime()) FOR [ChatTime]
GO

ALTER TABLE [dbo].[Chat]  WITH CHECK ADD FOREIGN KEY([ActivityID])
REFERENCES [dbo].[Group] ([GroupID])
GO

ALTER TABLE [dbo].[Chat]  WITH CHECK ADD FOREIGN KEY([ToWhom])
REFERENCES [dbo].[Chat] ([ChatID])
GO

ALTER TABLE [dbo].[Chat]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Member] ([UserID])
GO



--�p���ڭ̪��

CREATE TABLE [dbo].[Contact](
	[FormID] [int] IDENTITY(1,1) NOT NULL,
	[SenderName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[EmailTitle] [nvarchar](255) NOT NULL,
	[FormContent] [nvarchar](max) NOT NULL,
	[SendTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[FormID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Contact] ADD  DEFAULT (sysdatetime()) FOR [SendTime]
GO

--����s�W�bMember��
EXEC sp_rename 'Member.Password', 'PasswordHash', 'COLUMN';

ALTER TABLE [Member] ADD Salt VARBINARY(128);
go

ALTER TABLE [Member]
ADD ResetToken NVARCHAR(255),
    ResetTokenExpiration DATETIME;