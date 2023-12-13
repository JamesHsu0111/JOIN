-- �B�z�b��Ʈw�]�p�ɡA��ActivityName�]�w��Unique�A�{�b�n�����o�ӳ]�w
-- 1. �d�ߩҦ�����
-- SELECT name AS ConstraintName
-- FROM sys.objects
-- WHERE type_desc LIKE '%CONSTRAINT%'
--     AND parent_object_id = OBJECT_ID('MyActivity');

-- 2. �q����᪺�T����X���T��"�����W��"�çR���ߤ@�ʬ���
-- ��U�����{���XCONSTRAINT���᪺�����令�B�J1�d��쪺�����W��
-- ALTER TABLE MyActivity
-- DROP CONSTRAINT UQ__MyActivi__1DB4FDB3C70A73C1;

-- ��벼�ɶ�����t�ήɶ��A�s�W�@����ƨç�VoteDate & ExpectedDepartureMonth +1�Ӥ�
-- �Ыطs��ƪ�H�P�_���ʬO�_�w�B�z 
-- �ˬd���O�_�s�b���Ʈw��
IF OBJECT_ID('ProcessedActivities') IS NULL
BEGIN
    -- �p�G��椣�s�b�A�h�إߪ��
    CREATE TABLE ProcessedActivities (
        ActivityID INT PRIMARY KEY,
        Processed BIT
    );
END

-- �Ыؤ@���{�ɪ��ӫO�s�ŦX���󪺬���ID
CREATE TABLE #TempActivitiesToProcess (
    ActivityID INT
);

-- ���J�ŦX���󪺬���ID���{�ɪ��
INSERT INTO #TempActivitiesToProcess (ActivityID)
SELECT ActivityID
FROM MyActivity
WHERE 
    VoteDate = CAST(GETDATE() AS DATE) -- �벼������󤵤Ѫ����
    AND ActivityID NOT IN (SELECT ActivityID FROM ProcessedActivities WHERE Processed = 1);

-- �Ы��ܼƨӦs�x��e�B�z������ID
DECLARE @CurrentActivityID INT;

-- �ϥδ�йM���ݳB�z������
DECLARE ActivityCursor CURSOR FOR
SELECT ActivityID
FROM #TempActivitiesToProcess;

-- �}�l��аj��
OPEN ActivityCursor;

FETCH NEXT FROM ActivityCursor INTO @CurrentActivityID;

-- �}�l�j��B�z
WHILE @@FETCH_STATUS = 0
BEGIN
    -- �N�ŦX���󪺬��ʽƻs�ô��J�s����ƦC
    -- �s�W���ʨ� MyActivity ��ƪ�A���o�s�W���ʪ� ActivityID
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
        DATEADD(MONTH, 1, VoteDate) AS VoteDate, -- �벼����[1�Ӥ�
        DATEADD(MONTH, 1, ExpectedDepartureMonth) AS ExpectedDepartureMonth -- �w�p�X�o����[1�Ӥ�
    FROM MyActivity
    WHERE ActivityID = @CurrentActivityID;

    -- ���o��s�W���ʪ� ActivityID
    SET @NewActivityID = SCOPE_IDENTITY();

    -- �s�W���ʨ� OfficialPhoto ��ƪ�A�ƻs�Ҧ�����
    INSERT INTO OfficialPhoto (ActivityID, PhotoPath)
    SELECT @NewActivityID, PhotoPath
    FROM OfficialPhoto
    WHERE ActivityID = @CurrentActivityID;

    -- ��sProcessed�аO�A��ܳo�Ӭ��ʤw�Q�B�z
    INSERT INTO ProcessedActivities (ActivityID, Processed)
    VALUES (@CurrentActivityID, 1);

    -- �~��B�z�U�@�Ӭ���
    FETCH NEXT FROM ActivityCursor INTO @CurrentActivityID;
END;

-- �������
CLOSE ActivityCursor;

-- �����и귽
DEALLOCATE ActivityCursor;

-- �R���{�ɪ��
DROP TABLE #TempActivitiesToProcess;
