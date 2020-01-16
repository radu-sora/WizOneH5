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
/****** Object:  View [dbo].[RapoarteGrupuriUtilizatori]    Script Date: 4/9/2019 5:56:12 PM ******/
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
INNER JOIN tblGrupUsers AS gs ON gu.IdGrup = gs.Id
GO
