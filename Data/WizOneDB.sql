/****** Object:  Table [dbo].[tblGrupUsers]    Script Date: 4/10/2019 7:53:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblGrupUsers](
	[Id] [int] NOT NULL,
	[Denumire] [nvarchar](250) NULL,
	[IdAuto] [int] IDENTITY(1,1) NOT NULL,
	[USER_NO] [int] NULL,
	[TIME] [datetime] NULL,
	[Pozitie] [int] NULL,
	[Ordine] [int] NULL,
	[Prioritate] [int] NULL,
	[ExtensiiPermise] [nvarchar](100) NULL,
	[MeniuRestrans] [smallint] NULL,
 CONSTRAINT [tblGrupUsers_PK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tblGrupUsers] ADD  CONSTRAINT [DF_tblGrupUsers_TIME]  DEFAULT (getdate()) FOR [TIME]
GO
/****** Object:  View [dbo].[RapoarteGrupuriUtilizatori]    Script Date: 1/17/2020 12:41:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[RapoarteGrupuriUtilizatori]
AS
SELECT DISTINCT r.DynReportId AS IdRaport, gu.IdUser, gs.ExtensiiPermise, gs.MeniuRestrans
FROM DynReports AS r
INNER JOIN relGrupRaport AS gr ON r.DynReportId = gr.IdRaport 
INNER JOIN relGrupUser AS gu ON gr.IdGrup = gu.IdGrup
INNER JOIN tblGrupUsers AS gs ON gu.IdGrup = gs.Id;
GO
/****** Object:  Table [dbo].[MeniuLinii]    Script Date: 8/7/2020 3:25:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeniuLinii](
	[Id] [int] NOT NULL,
	[IdMeniu] [int] NOT NULL,
	[Parinte] [int] NOT NULL,
	[Ordine] [int] NULL,
	[Stare] [int] NULL,
	[Nume] [nvarchar](150) NOT NULL,
	[Descriere] [nvarchar](250) NULL,
	[Imagine] [nvarchar](150) NULL,
	[IdNomen] [int] NULL,
	[USER_NO] [int] NULL,
	[TIME] [datetime] NULL,
	[StareMobil] [int] NULL,
	[NumeMobil] [nvarchar](20) NULL,
	[OrdineMobil] [int] NULL,
 CONSTRAINT [PK_MeniuLinii] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[IdMeniu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[MeniuLinii] ADD  CONSTRAINT [DF_MeniuLinii_Activ]  DEFAULT ((1)) FOR [Stare]
GO
ALTER TABLE [dbo].[MeniuLinii] ADD  CONSTRAINT [DF_MeniuLinii_TIME]  DEFAULT (getdate()) FOR [TIME]
GO