-- 系統當前時間
DECLARE @CurrentTime DATETIME = GETDATE()

-- 找到需要建立投票選項的活動
;WITH RankedActivities AS (
    SELECT
        MyActivity.ActivityID,
        DateGenerated.StartDate,
        ROW_NUMBER() OVER (PARTITION BY MyActivity.ActivityID ORDER BY DateGenerated.StartDate) AS RowNum
    FROM MyActivity
	--生成日期
    CROSS APPLY (
        SELECT 
            StartDate
        FROM (
            SELECT DATEADD(DAY, Number - 1, DATEFROMPARTS(YEAR(ExpectedDepartureMonth), MONTH(ExpectedDepartureMonth), 1)) AS StartDate
            FROM master.dbo.spt_values
            WHERE Type = 'P' AND DATEPART(WEEKDAY, DATEADD(DAY, Number - 1, DATEFROMPARTS(YEAR(ExpectedDepartureMonth), MONTH(ExpectedDepartureMonth), 1))) = 7
        ) AS Dates
        WHERE DATEPART(MONTH, StartDate) = DATEPART(MONTH, ExpectedDepartureMonth)  -- 確保在同一個月份內
		AND DATEPART(YEAR, StartDate) = DATEPART(YEAR, ExpectedDepartureMonth)  -- 確保在同一個年份內
    ) AS DateGenerated
    WHERE CONVERT(DATE, VoteDate) = CONVERT(DATE, @CurrentTime)
          AND ExpectedDepartureMonth >= @CurrentTime
)
INSERT INTO VoteTime (ActivityID, StartDate)
SELECT ActivityID, StartDate
FROM RankedActivities
WHERE RowNum <= 5
      AND ActivityID NOT IN (
          SELECT DISTINCT ActivityID
          FROM VoteTime
      );
  
