CREATE VIEW LRIT_SIGO_PORT AS
select Place.PlaceStringId
,ContractingGoverment.DDPVersionId
,ContractingGoverment.Name as Name_ContractingGoverment
, ContractingGoverment.isoCode
, ContractingGoverment.DataCenterId
,ContractingGoverment.LRITId
, geography::STGeomFromWKB(place.Area, 4326).STAsText()as Poligon
, Place.AreaType
, Place.Name as Name_Place 
, Place.Area as Position
from ContractingGoverment, place
where ContractingGoverment.Id=Place.ContractingGovermentId
AND DDPVersionId=(select id from DDPVersion where DDPVersion=(select DDPVerision from Configuration))
AND Place.AreaType = 'port'