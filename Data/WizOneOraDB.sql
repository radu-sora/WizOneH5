CREATE VIEW "RapoarteGrupuriUtilizatori"
AS 
SELECT DISTINCT r."DynReportId" AS "IdRaport", gu."IdUser", gs."ExtensiiPermise", gs."MeniuRestrans"
FROM "DynReports" r
INNER JOIN "relGrupRaport" gr ON r."DynReportId" = gr."IdRaport"
INNER JOIN "relGrupUser" gu ON gr."IdGrup" = gu."IdGrup"
INNER JOIN "tblGrupUsers" gs ON gu."IdGrup" = gs."Id"
UNION ALL
SELECT "DynReportId" AS "IdRaport", "IdUser" AS "IdUser", NULL AS "ExtensiiPermise", NULL AS "MeniuRestrans"
FROM "relGrupUser", "DynReports"
WHERE "IdGrup" = 0 AND "DynReportId" NOT IN (SELECT "IdRaport" FROM "relGrupRaport" WHERE "IdGrup" = 0)
UNION ALL
SELECT B."IdRaport", A.F70102 AS "IdUser", NULL AS "ExtensiiPermise", NULL AS "MeniuRestrans"
FROM USERS A
INNER JOIN "relGrupRaport" B ON B."IdGrup"=-1;