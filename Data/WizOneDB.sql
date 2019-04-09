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
