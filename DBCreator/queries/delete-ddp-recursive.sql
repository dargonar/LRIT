delete from StandingOrder where PlaceId in (select id from Place where ContractingGovermentId in (select id from ContractingGoverment where DDPVersionId< 66))
delete from Place where ContractingGovermentId in (select id from ContractingGoverment where DDPVersionId< 66)
delete from Exclusion where ContractingGoverment in (select id from ContractingGoverment where DDPVersionId< 66)
delete from SARService where ContractingGovermentId in (select id from ContractingGoverment where DDPVersionId< 66)
delete from ContractingGoverment where DDPVersionId< 66
delete from DDPVersion where Id < 66