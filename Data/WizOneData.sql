INSERT [dbo].[DynReportTypes] ([DynReportTypeId], [Name]) VALUES (1, N'Raport')
GO
INSERT [dbo].[DynReportTypes] ([DynReportTypeId], [Name]) VALUES (2, N'Document')
GO
INSERT [dbo].[DynReportTypes] ([DynReportTypeId], [Name]) VALUES (3, N'Cub')
GO
INSERT [dbo].[DynReportTypes] ([DynReportTypeId], [Name]) VALUES (4, N'Tabel')
GO

UPDATE [dbo].[tblMeniuri] SET [Pagina] = 'Generatoare\Reports\Pages\Manage' WHERE [Id] = 24
GO
