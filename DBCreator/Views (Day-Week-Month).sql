CREATE VIEW DayStats
AS
SELECT     AVG(ABS(CAST(DATEDIFF(second, spr.TimeStamp2, spr.TimeStamp1) AS bigint))) AS AvgShipAsp, 
		   MAX(ABS(CAST(DATEDIFF(second, spr.TimeStamp2, spr.TimeStamp1) AS bigint))) AS MaxShipAsp, 
           MIN(ABS(CAST(DATEDIFF(second, spr.TimeStamp2, spr.TimeStamp1) AS bigint))) AS MinShipAsp, 
           
           AVG(ABS(CAST(DATEDIFF(second, spr.TimeStamp3,spr.TimeStamp2) AS bigint))) AS AvgAsp, 
           MAX(ABS(CAST(DATEDIFF(second, spr.TimeStamp3,spr.TimeStamp2) AS bigint))) AS MaxAsp, 
           MIN(ABS(CAST(DATEDIFF(second, spr.TimeStamp3,spr.TimeStamp2) AS bigint))) AS MinAsp, 
           
           AVG(ABS(CAST(DATEDIFF(second, spr.TimeStamp4, spr.TimeStamp3) AS bigint))) AS AvgAspDc, 
           MAX(ABS(CAST(DATEDIFF(second, spr.TimeStamp4, spr.TimeStamp3) AS bigint))) AS MaxAspDc, 
           MIN(ABS(CAST(DATEDIFF(second, spr.TimeStamp4, spr.TimeStamp3) AS bigint))) AS MinAspDc, 
           
           AVG(ABS(CAST(DATEDIFF(second, spr.TimeStamp5, spr.TimeStamp4) AS bigint))) AS AvgDc, 
           MAX(ABS(CAST(DATEDIFF(second, spr.TimeStamp5, spr.TimeStamp4) AS bigint))) AS MaxDc, 
           MIN(ABS(CAST(DATEDIFF(second, spr.TimeStamp5, spr.TimeStamp4) AS bigint))) AS MinDc, 
           
           DATEPART(DAYOFYEAR, spr.TimeStamp1) AS dia, 
           DATEPART(year,spr.TimeStamp1) AS ano

FROM         dbo.ShipPositionReport AS spr INNER JOIN
                      dbo.MsgInOut AS inout ON spr.MsgInOutId = inout.Id
WHERE     (inout.InOut = 1)
GROUP BY DATEPART(DAYOFYEAR, spr.TimeStamp1), DATEPART(year, spr.TimeStamp1)

CREATE VIEW WeekStats
AS
SELECT     AVG(ABS(CAST(DATEDIFF(second, spr.TimeStamp2, spr.TimeStamp1) AS bigint))) AS AvgShipAsp, 
		   MAX(ABS(CAST(DATEDIFF(second, spr.TimeStamp2, spr.TimeStamp1) AS bigint))) AS MaxShipAsp, 
           MIN(ABS(CAST(DATEDIFF(second, spr.TimeStamp2, spr.TimeStamp1) AS bigint))) AS MinShipAsp, 
           
           AVG(ABS(CAST(DATEDIFF(second, spr.TimeStamp3,spr.TimeStamp2) AS bigint))) AS AvgAsp, 
           MAX(ABS(CAST(DATEDIFF(second, spr.TimeStamp3,spr.TimeStamp2) AS bigint))) AS MaxAsp, 
           MIN(ABS(CAST(DATEDIFF(second, spr.TimeStamp3,spr.TimeStamp2) AS bigint))) AS MinAsp, 
           
           AVG(ABS(CAST(DATEDIFF(second, spr.TimeStamp4, spr.TimeStamp3) AS bigint))) AS AvgAspDc, 
           MAX(ABS(CAST(DATEDIFF(second, spr.TimeStamp4, spr.TimeStamp3) AS bigint))) AS MaxAspDc, 
           MIN(ABS(CAST(DATEDIFF(second, spr.TimeStamp4, spr.TimeStamp3) AS bigint))) AS MinAspDc, 
           
           AVG(ABS(CAST(DATEDIFF(second, spr.TimeStamp5, spr.TimeStamp4) AS bigint))) AS AvgDc, 
           MAX(ABS(CAST(DATEDIFF(second, spr.TimeStamp5, spr.TimeStamp4) AS bigint))) AS MaxDc, 
           MIN(ABS(CAST(DATEDIFF(second, spr.TimeStamp5, spr.TimeStamp4) AS bigint))) AS MinDc, 
           
           DATEPART(Week, spr.TimeStamp1) AS semana

FROM         dbo.ShipPositionReport AS spr INNER JOIN
                      dbo.MsgInOut AS inout ON spr.MsgInOutId = inout.Id
WHERE     (inout.InOut = 1)
GROUP BY DATEPART(Week, spr.TimeStamp1)


CREATE VIEW MonthStats
AS
SELECT     AVG(ABS(CAST(DATEDIFF(second, spr.TimeStamp2, spr.TimeStamp1) AS bigint))) AS AvgShipAsp, 
		   MAX(ABS(CAST(DATEDIFF(second, spr.TimeStamp2, spr.TimeStamp1) AS bigint))) AS MaxShipAsp, 
           MIN(ABS(CAST(DATEDIFF(second, spr.TimeStamp2, spr.TimeStamp1) AS bigint))) AS MinShipAsp, 
           
           AVG(ABS(CAST(DATEDIFF(second, spr.TimeStamp3,spr.TimeStamp2) AS bigint))) AS AvgAsp, 
           MAX(ABS(CAST(DATEDIFF(second, spr.TimeStamp3,spr.TimeStamp2) AS bigint))) AS MaxAsp, 
           MIN(ABS(CAST(DATEDIFF(second, spr.TimeStamp3,spr.TimeStamp2) AS bigint))) AS MinAsp, 
           
           AVG(ABS(CAST(DATEDIFF(second, spr.TimeStamp4, spr.TimeStamp3) AS bigint))) AS AvgAspDc, 
           MAX(ABS(CAST(DATEDIFF(second, spr.TimeStamp4, spr.TimeStamp3) AS bigint))) AS MaxAspDc, 
           MIN(ABS(CAST(DATEDIFF(second, spr.TimeStamp4, spr.TimeStamp3) AS bigint))) AS MinAspDc, 
           
           AVG(ABS(CAST(DATEDIFF(second, spr.TimeStamp5, spr.TimeStamp4) AS bigint))) AS AvgDc, 
           MAX(ABS(CAST(DATEDIFF(second, spr.TimeStamp5, spr.TimeStamp4) AS bigint))) AS MaxDc, 
           MIN(ABS(CAST(DATEDIFF(second, spr.TimeStamp5, spr.TimeStamp4) AS bigint))) AS MinDc, 
           
           DATEPART(MONTH, spr.TimeStamp1) AS mes,
           DATEPART(YEAR, spr.TimeStamp1) AS ano

FROM         dbo.ShipPositionReport AS spr INNER JOIN
                      dbo.MsgInOut AS inout ON spr.MsgInOutId = inout.Id
WHERE     (inout.InOut = 1)
GROUP BY DATEPART(MONTH, spr.TimeStamp1),DATEPART(YEAR, spr.TimeStamp1)
