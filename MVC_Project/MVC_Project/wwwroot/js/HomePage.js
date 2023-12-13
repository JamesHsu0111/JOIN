
//網站介紹
$(".option").click(function () {
    $(".option").removeClass("active");
    $(this).addClass("active");
});

//活動方塊-愛心設計
const VoteIcons = document.querySelectorAll('.vote-icon');

//從body抓現在的使用者是誰(<input>裡面)
let userId = $("#currentUserId").val();

VoteIcons.forEach(function (voteIcon) {
    voteIcon.addEventListener('click', function () {
        if (userId != 0) {
            const activityId = voteIcon.getAttribute('data-activityid');
            if (voteIcon.classList.contains('fa-envelope-open-text')) {
                $.ajax({
                    type: 'POST',
                    url: '/MyActivity/LikeActivity',
                    data: {
                        activityId: activityId,
                        userId: userId
                    },
                    success: function (data) {
                        // 處理成功的回應，可以更新UI或執行其他操作
                        voteIcon.classList.remove('fa-envelope-open-text');
                        voteIcon.classList.add('fa-envelope-circle-check');

                        const cardInfo = voteIcon.closest('.card').querySelector('.card__info');

                        const likedText = document.createElement('span');
                        likedText.textContent = '已取得投票資格';
                        likedText.classList.add('card__liked-text');

                        cardInfo.appendChild(likedText);
                    },
                    error: function (error) {
                        // 處理錯誤，如果有錯誤發生
                    }
                });
            } else {
                // 在此處執行AJAX DELETE請求，從"LikeRecord"資料表中刪除對應的記錄
                $.ajax({
                    type: 'DELETE',
                    url: '/MyActivity/UnlikeActivity',
                    data: {
                        activityId: activityId,
                        userId: userId
                    },
                    success: function (data) {
                        // 處理成功的回應，可以更新UI或執行其他操作
                        voteIcon.classList.remove('fa-envelope-circle-check');
                        voteIcon.classList.add('fa-envelope-open-text');

                        // 移除已經存在的 "Liked" 文字
                        const likedText = voteIcon.closest('.card').querySelector('.card__liked-text');
                        if (likedText) {
                            likedText.remove();
                        }
                    },
                    error: function (error) {
                        // 處理錯誤，如果有錯誤發生
                    }
                });
            }
        }
    });
});
