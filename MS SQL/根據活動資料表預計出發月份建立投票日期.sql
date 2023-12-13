-- �t�η�e�ɶ�
DECLARE @CurrentTime DATETIME = GETDATE()

-- ���ݭn�إߧ벼�ﶵ������
;WITH RankedActivities AS (
    SELECT
        MyActivity.ActivityID,
        DateGenerated.StartDate,
        ROW_NUMBER() OVER (PARTITION BY MyActivity.ActivityID ORDER BY DateGenerated.StartDate) AS RowNum
    FROM MyActivity
	--�ͦ����
    CROSS APPLY (
        SELECT 
            StartDate
        FROM (
            SELECT DATEADD(DAY, Number - 1, DATEFROMPARTS(YEAR(ExpectedDepartureMonth), MONTH(ExpectedDepartureMonth), 1)) AS StartDate
            FROM master.dbo.spt_values
            WHERE Type = 'P' AND DATEPART(WEEKDAY, DATEADD(DAY, Number - 1, DATEFROMPARTS(YEAR(ExpectedDepartureMonth), MONTH(ExpectedDepartureMonth), 1))) = 7
        ) AS Dates
        WHERE DATEPART(MONTH, StartDate) = DATEPART(MONTH, ExpectedDepartureMonth)  -- �T�O�b�P�@�Ӥ����
		AND DATEPART(YEAR, StartDate) = DATEPART(YEAR, ExpectedDepartureMonth)  -- �T�O�b�P�@�Ӧ~����
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
  
