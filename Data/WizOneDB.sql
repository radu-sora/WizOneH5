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
/****** Object:  View [dbo].[Schedule]    Script Date: 11/12/2020 1:57:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[Schedule]
AS
SELECT 'CR' + CAST(ce.IdAuto AS VARCHAR) AS Id, 1 AS Type, ce.Id AS InternalId, ce.F10003 AS EmployeeId, 
		ab.Denumire AS Name, ce.Observatii AS Remarks, ce.IdAbsenta AS LabelId, ce.IdStare AS StatusId,
		ce.DataInceput AS StartDate, DATEADD(DD, 1, ce.DataSfarsit) AS EndDate
FROM Ptj_Cereri AS ce
INNER JOIN Ptj_tblAbsente AS ab ON ce.IdAbsenta = ab.Id
INNER JOIN Ptj_tblStari AS st ON ce.IdStare = st.Id
UNION
SELECT 'CC' + CAST(cc.IdAuto AS VARCHAR) AS Id, 2 AS Type, cc.IdAuto AS InternalId, cc.F10003 AS EmployeeId, 
		fcc.F06205 AS Name, cc.Observatii AS Remarks, 0 AS LabelId, cc.IdStare AS StatusId,		
		ISNULL(cc.De, DATEADD(HH, 9, CAST(cc.Ziua AS DATETIME))) AS StartDate,
		ISNULL(cc.La, IIF(cc.IdStare = 2,			
			IIF(ISNULL(cc.De, DATEADD(HH, 9, CAST(cc.Ziua AS DATETIME))) > GETDATE(), ISNULL(cc.De, DATEADD(HH, 9, CAST(cc.Ziua AS DATETIME))), GETDATE()),
			DATEADD(MI, (9 * 60) + ISNULL(cc.NrOre1, 0) + ISNULL(cc.NrOre2, 0) + ISNULL(cc.NrOre3, 0), CAST(cc.Ziua AS DATETIME)))) AS EndDate
FROM Ptj_CC AS cc
INNER JOIN F062 AS fcc ON cc.F06204 = fcc.F06204
INNER JOIN Ptj_tblStari AS st ON cc.IdStare = st.Id;
GO