CREATE VIEW LRIT_SIGO AS
SELECT [ID_Barco] as ID_BARCO
      ,[MMSINum] as MMSI
      ,[Matricula] as MATRICULA
      ,[IMONum] as NRO_OMI
      ,[Name] as NOMBRE
      ,'LRIT' as SISTEMA
      ,[Eslora] as ESLORA
      ,[Manga] as MANGA
      ,[Calado] as CALADO
      ,[Tipo_Navegacion] as TIPO_NAVEGACION
      ,[Tipo_Navegacion_Ingles] as TIPO_NAVEGACION_INGLES
      ,[Tipo_Buque] as TIPO_BUQUE
      ,[Tipo_Buque_Ingles] as TIPO_BUQUE_INGLES
      ,sp.TimeStamp as FECHA_REPORTE
      ,sp.Position
      ,sp.Rumbo as RUMBO
      ,sp.Velocidad as VELOCIDAD
      ,sp.Destino as DESTINO
  FROM [LRIT].[dbo].[Ship]
  INNER JOIN (SELECT * FROM ShipPosition WHERE ShipPosition.Id IN (SELECT MAX( ShipPosition.Id ) FROM ShipPosition GROUP BY ShipId)) AS sp
  on sp.ShipId = Ship.Id
