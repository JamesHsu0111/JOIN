-- 處理在資料庫設計時，把ActivityName設定為Unique，現在要移除這個設定
-- 1. 查詢所有約束
-- SELECT name AS ConstraintName
-- FROM sys.objects
-- WHERE type_desc LIKE '%CONSTRAINT%'
--     AND parent_object_id = OBJECT_ID('MyActivity');

-- 2. 從執行後的訊息找出正確的"約束名稱"並刪除唯一性約束
-- 把下面的程式碼CONSTRAINT之後的部分改成步驟1查找到的約束名稱
-- ALTER TABLE MyActivity
-- DROP CONSTRAINT UQ__MyActivi__1DB4FDB3C70A73C1;

-- 當投票時間等於系統時間，新增一筆資料並把VoteDate & ExpectedDepartureMonth +1個月
-- 創建新資料表以判斷活動是否已處理 
-- 檢查表格是否存在於資料庫中
IF OBJECT_ID('ProcessedActivities') IS NULL
BEGIN
    -- 如果表格不存在，則建立表格
    CREATE TABLE ProcessedActivities (
        ActivityID INT PRIMARY KEY,
        Processed BIT
    );
END

-- 創建一個臨時表格來保存符合條件的活動ID
CREATE TABLE #TempActivitiesToProcess (
    ActivityID INT
);

-- 插入符合條件的活動ID到臨時表格
INSERT INTO #TempActivitiesToProcess (ActivityID)
SELECT ActivityID
FROM MyActivity
WHERE 
    VoteDate = CAST(GETDATE() AS DATE) -- 投票日期等於今天的日期
    AND ActivityID NOT IN (SELECT ActivityID FROM ProcessedActivities WHERE Processed = 1);

-- 創建變數來存儲當前處理的活動ID
DECLARE @CurrentActivityID INT;

-- 使用游標遍歷待處理的活動
DECLARE ActivityCursor CURSOR FOR
SELECT ActivityID
FROM #TempActivitiesToProcess;

-- 開始游標迴圈
OPEN ActivityCursor;

FETCH NEXT FROM ActivityCursor INTO @CurrentActivityID;

-- 開始迴圈處理
WHILE @@FETCH_STATUS = 0
BEGIN
    -- 將符合條件的活動複製並插入新的資料列
    -- 新增活動到 MyActivity 資料表，取得新增活動的 ActivityID
    DECLARE @NewActivityID INT;

    INSERT INTO MyActivity (
        ActivityName,
        Category,
        SuggestedAmount,
        ActivityContent,
        MinAttendee,
        VoteDate,
        ExpectedDepartureMonth
    )
    SELECT
        ActivityName,
        Category,
        SuggestedAmount,
        ActivityContent,
        MinAttendee,
        DATEADD(MONTH, 1, VoteDate) AS VoteDate, -- 投票日期加1個月
        DATEADD(MONTH, 1, ExpectedDepartureMonth) AS ExpectedDepartureMonth -- 預計出發月份加1個月
    FROM MyActivity
    WHERE ActivityID = @CurrentActivityID;

    -- 取得剛新增活動的 ActivityID
    SET @NewActivityID = SCOPE_IDENTITY();

    -- 新增活動到 OfficialPhoto 資料表，複製所有原資料
    INSERT INTO OfficialPhoto (ActivityID, PhotoPath)
    SELECT @NewActivityID, PhotoPath
    FROM OfficialPhoto
    WHERE ActivityID = @CurrentActivityID;

    -- 更新Processed標記，表示這個活動已被處理
    INSERT INTO ProcessedActivities (ActivityID, Processed)
    VALUES (@CurrentActivityID, 1);

    -- 繼續處理下一個活動
    FETCH NEXT FROM ActivityCursor INTO @CurrentActivityID;
END;

-- 關閉游標
CLOSE ActivityCursor;

-- 釋放游標資源
DEALLOCATE ActivityCursor;

-- 刪除臨時表格
DROP TABLE #TempActivitiesToProcess;
