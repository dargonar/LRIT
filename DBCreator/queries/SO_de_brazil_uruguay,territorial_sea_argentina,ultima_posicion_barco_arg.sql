SELECT [Id]
      ,[ContractingGovermentId]
      ,[PlaceStringId]
      ,[Name]
      ,[AreaType]
      ,geography::STGeomFromWKB([Area],4326)
  FROM [LRIT].[dbo].[Place] where ContractingGovermentId=618
  and AreaType='territorialsea'

union all

select so.PlaceId, 0, '', '', '', geography::STGeomFromWKB(p.Area, 4326) 
from StandingOrder as so left join Place as p on so.PlaceId=p.Id
left join ContractingGoverment as cg on cg.Id=p.ContractingGovermentId 

where cg.Name='URUGUAY' or cg.Name='BRAZIL'

union all 

select top 1 0,0,'','','', geography::STGeomFromWKB(Position, 4326) from ShipPosition where TimeStamp=(select MAX(TimeStamp) from ShipPosition where ShipId=1)
