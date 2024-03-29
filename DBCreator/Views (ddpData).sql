CREATE VIEW DDPData AS 
SELECT dv.Id, 
       cg.DataCenterId,
       cg.LRITId,
       cg.Name as Name,
       p.PlaceStringId,
       p.Name as PlaceName,
       p.AreaType,
       ISNUMERIC(so.PlaceId) as PlaceId,
      (dv.regularVer +':'+ dv.inmediateVer) as DDPVersion
       
FROM DDPVersion as dv inner join ContractingGoverment as cg on dv.Id = cg.DDPVersionId
inner join Place as p on p.ContractingGovermentId = cg.Id
left join StandingOrder as so on so.PlaceId = p.Id