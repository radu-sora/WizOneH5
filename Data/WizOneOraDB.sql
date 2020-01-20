CREATE TABLE "relGrupRaport" 
(
  "IdGrup" NUMBER(10, 0) NOT NULL 
, "IdRaport" NUMBER(10, 0) NOT NULL 
, "IdAuto" NUMBER(10, 0) NOT NULL 
, USER_NO NUMBER(10, 0) 
, TIME DATE 
, "AreParola" NUMBER(1, 0) DEFAULT 0 NOT NULL 
);

CREATE UNIQUE INDEX "relGrupRaport_PK" ON "relGrupRaport" ("IdAuto" ASC);

ALTER TABLE "relGrupRaport"
ADD CONSTRAINT "relGrupRaport_PK" PRIMARY KEY 
(
  "IdAuto" 
)
USING INDEX relGrupRaport_PK
ENABLE;

CREATE VIEW "RapoarteGrupuriUtilizatori"
AS 
SELECT DISTINCT r."DynReportId" AS "IdRaport", gu."IdUser", gs."ExtensiiPermise", gs."MeniuRestrans", gr."AreParola"
FROM "DynReports" r
INNER JOIN "relGrupRaport" gr ON r."DynReportId" = gr."IdRaport"
INNER JOIN "relGrupUser" gu ON gr."IdGrup" = gu."IdGrup"
INNER JOIN "tblGrupUsers" gs ON gu."IdGrup" = gs."Id"
UNION ALL
SELECT "DynReportId" AS "IdRaport", "IdUser" AS "IdUser", NULL AS "ExtensiiPermise", NULL AS "MeniuRestrans", 0 AS "AreParola"
FROM "relGrupUser", "DynReports"
WHERE "IdGrup" = 0 AND "DynReportId" NOT IN (SELECT "IdRaport" FROM "relGrupRaport" WHERE "IdGrup" = 0)
UNION ALL
SELECT B."IdRaport", A.F70102 AS "IdUser", NULL AS "ExtensiiPermise", NULL AS "MeniuRestrans", B."AreParola"
FROM USERS A
INNER JOIN "relGrupRaport" B ON B."IdGrup"=-1;