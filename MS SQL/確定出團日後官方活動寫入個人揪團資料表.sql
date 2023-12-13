-- тщ布篒ゎらActivityID㎝癸莱程蔼布计VoteResult
WITH MaxVote AS (
    SELECT v.ActivityID, v.VoteResult, COUNT(*) AS VoteCount
    FROM VoteRecord v
    INNER JOIN MyActivity ma ON v.ActivityID = ma.ActivityID
    WHERE CAST(GETDATE() AS DATE) = DATEADD(DAY, 5, CAST(ma.VoteDate AS DATE))
    GROUP BY v.ActivityID, v.VoteResult
),
-- т–ActivityID癸莱程蔼布计
MaxVoteCount AS (
    SELECT ActivityID, MAX(VoteCount) AS MaxCount
    FROM MaxVote
    GROUP BY ActivityID
),
-- 程沧琩高т–ActivityID癸莱程蔼布计VoteResult
FinalResult AS (
    SELECT mv.ActivityID, mv.VoteResult
    FROM MaxVote mv
    INNER JOIN MaxVoteCount mvc ON mv.ActivityID = mvc.ActivityID AND mv.VoteCount = mvc.MaxCount
)
-- 础戈Group
INSERT INTO [Group] (GroupName, GroupCategory, GroupContent, MinAttendee, MaxAttendee, StartDate, EndDate, Organizer, OriginalActivityID)
SELECT ma.ActivityName, ma.Category, ma.ActivityContent, mvc.MaxCount, NULL, fr.VoteResult, DATEADD(DAY, 1, fr.VoteResult), 1, fr.ActivityID
FROM FinalResult fr
INNER JOIN MyActivity ma ON fr.ActivityID = ma.ActivityID
INNER JOIN MaxVoteCount mvc ON fr.ActivityID = mvc.ActivityID;