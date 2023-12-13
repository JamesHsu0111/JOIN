-- ���b�벼�I���᪺ActivityID�M�������̰����ƪ�VoteResult
WITH MaxVote AS (
    SELECT v.ActivityID, v.VoteResult, COUNT(*) AS VoteCount
    FROM VoteRecord v
    INNER JOIN MyActivity ma ON v.ActivityID = ma.ActivityID
    WHERE CAST(GETDATE() AS DATE) = DATEADD(DAY, 5, CAST(ma.VoteDate AS DATE))
    GROUP BY v.ActivityID, v.VoteResult
),
-- ���C��ActivityID�������̰�����
MaxVoteCount AS (
    SELECT ActivityID, MAX(VoteCount) AS MaxCount
    FROM MaxVote
    GROUP BY ActivityID
),
-- �̲׬d�ߡA���C��ActivityID�������̰����ƪ�VoteResult
FinalResult AS (
    SELECT mv.ActivityID, mv.VoteResult
    FROM MaxVote mv
    INNER JOIN MaxVoteCount mvc ON mv.ActivityID = mvc.ActivityID AND mv.VoteCount = mvc.MaxCount
)
-- ���J��ƨ�Group��
INSERT INTO [Group] (GroupName, GroupCategory, GroupContent, MinAttendee, MaxAttendee, StartDate, EndDate, Organizer, OriginalActivityID)
SELECT ma.ActivityName, ma.Category, ma.ActivityContent, mvc.MaxCount, NULL, fr.VoteResult, DATEADD(DAY, 1, fr.VoteResult), 1, fr.ActivityID
FROM FinalResult fr
INNER JOIN MyActivity ma ON fr.ActivityID = ma.ActivityID
INNER JOIN MaxVoteCount mvc ON fr.ActivityID = mvc.ActivityID;