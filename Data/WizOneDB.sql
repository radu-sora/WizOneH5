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
ALTER TABLE [dbo].[tblGrupUsers] ADD  CONSTRAINT [DF__tmp_ms_xx___TIME__78359834]  DEFAULT (getdate()) FOR [TIME]
GO
/****** Object:  Table [dbo].[relGrupRaport2]    Script Date: 1/17/2020 4:35:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[relGrupRaport2](
	[IdGrup] [int] NOT NULL,
	[IdRaport] [int] NOT NULL,
	[IdAuto] [int] IDENTITY(1,1) NOT NULL,
	[USER_NO] [int] NULL,
	[TIME] [datetime] NULL,
	[AreParola] [bit] NOT NULL,
 CONSTRAINT [relGrupRaport_PK] PRIMARY KEY CLUSTERED 
(
	[IdAuto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[relGrupRaport2] ADD  CONSTRAINT [DF__tmp_ms_xx___TIME__4C5715F6]  DEFAULT (getdate()) FOR [TIME]
GO
ALTER TABLE [dbo].[relGrupRaport2] ADD  CONSTRAINT [DF_relGrupRaport2_AreParola]  DEFAULT ((0)) FOR [AreParola]
GO
/****** Object:  View [dbo].[RapoarteGrupuriUtilizatori]    Script Date: 1/17/2020 12:41:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER VIEW [dbo].[RapoarteGrupuriUtilizatori]
AS
SELECT DISTINCT r.DynReportId AS IdRaport, gu.IdUser, gs.ExtensiiPermise, gs.MeniuRestrans, gr.AreParola
FROM DynReports AS r
INNER JOIN relGrupRaport2 AS gr ON r.DynReportId = gr.IdRaport 
INNER JOIN relGrupUser AS gu ON gr.IdGrup = gu.IdGrup
INNER JOIN tblGrupUsers AS gs ON gu.IdGrup = gs.Id;
GO
