CREATE VIEW LRIT_SIGO_INTERNAL_WATERS AS
select  place.Id
,Place.PlaceStringId
,ContractingGoverment.DDPVersionId
,ContractingGoverment.Name as Name_ContractingGoverment
, ContractingGoverment.isoCode
, ContractingGoverment.DataCenterId
,ContractingGoverment.LRITId
, geography::STGeomFromWKB(place.Area, 4326).STAsText()as Polygon
, Place.AreaType
, Place.Name as Name_Place 
from ContractingGoverment, place
where ContractingGoverment.Id=Place.ContractingGovermentId
and place.Area is not null
AND DDPVersionId=(select id from DDPVersion where DDPVersion=(select DDPVerision from Configuration))
AND Place.AreaType = 'internalwaters'
