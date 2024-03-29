
CREATE VIEW LRIT_SIGO_SO AS 
SELECT [IMONum]
	,[ShipName]
      ,[Latitude]
      ,[Longitude]
      ,[MMSINum]
      ,[TimeStamp1]
      ,[TimeStamp2]
      ,[TimeStamp3]
      ,[TimeStamp4]
      ,[TimeStamp5]
FROM ShipPositionReport as spr
join MsgInOut as m on m.Id=spr.MsgInOutId and m.InOut=0
WHERE spr.ID in (Select MAX(Id) from ShipPositionReport group by IMONum)
