// 獲取該會員在此活動的投票選項
$(document).ready(function () {

    
    // 檢查網址中是否包含'activityId'參數
    if (window.location.search.includes('activityId')) {
        // 使用方法B從URL參數中獲取activityId
        var urlParams = new URLSearchParams(window.location.search);
        var activityId = urlParams.get('activityId');
    } else {
        // 使用方法A從網址路徑中獲取activityId
        var activityId = window.location.pathname.split('/').pop();
    }

    // 現在你可以使用activityId進行後續操作
    console.log('獲取的activityId為：' + activityId);

    $.ajax({
        url: '/VoteRecords/GetMemberVote',
        type: 'GET',
        data: {
            activityId: activityId
        },
        success: function (lastVoteStr) {
            $('#memberVote').val(lastVoteStr);

            // 如果result不為null，表示用戶已經投票，設置相應的選項為checked並禁用
            if (lastVoteStr != null) {
                $('input[type=radio]').each(function () {
                    if ($(this).val() === lastVoteStr) {
                        $(this).prop('checked', true);
                        $(this).prop('disabled', true);
                    }
                });

                 /*禁用"提交"按鈕*/
                $('button[type="submit"]').prop('disabled', true);
            } else {
                // 禁用"提交"按鈕
                $('button[type="submit"]').prop('disabled', true);
            }
        }
    });

    //當投票選項更改時啟動"提交"按鈕
    $('input[type=radio]').change(function () {
        // 啟用提交按鈕
        $('button[type="submit"]').prop('disabled', false);
    });
});

//寫入投票選項到VoteRecord資料表

// 當提交按鈕被點擊時
$('button[type="submit"]').click(function (e) {
    e.preventDefault(); // 阻止表單默認的提交行為

    // 獲取選中的投票選項的值
    var selectedVote = $('input[type=radio]:checked').val();

    // 檢查網址中是否包含'activityId'參數
    if (window.location.search.includes('activityId')) {
        // 使用方法B從URL參數中獲取activityId
        var urlParams = new URLSearchParams(window.location.search);
        var activityId = urlParams.get('activityId');
    } else {
        // 使用方法A從網址路徑中獲取activityId
        var activityId = window.location.pathname.split('/').pop();
    }

    // 現在你可以使用activityId進行後續操作
    console.log('獲取的activityId為：' + activityId);

    // 檢查是否有選擇投票選項
    if (selectedVote) {
        $.ajax({
            url: '/VoteRecords/SaveVote',
            type: 'POST',
            data: {
                activityId: activityId,
                voteResult: selectedVote
            },
            success: function (UpdatedVoteStr) {
                console.log('Server Response:', UpdatedVoteStr);  // 除錯語句
                // 取消所有選項的選中狀態和禁用
                $('input[type=radio]').prop('checked', false);
                $('input[type=radio]').prop('disabled', false);

                $('#memberVote').val(UpdatedVoteStr);

                // 如果result不為null，表示用戶已經投票，設置相應的選項為checked並禁用
                if (UpdatedVoteStr != null) {
                    $('input[type=radio]').each(function () {
                        if ($(this).val() === UpdatedVoteStr) {
                            $(this).prop('checked', true);
                            $(this).prop('disabled', true);
                        }
                    });

                    // 禁用"提交"按鈕
                    $('button[type="submit"]').prop('disabled', true);
                }

                //顯示成功訊息
                Swal.fire({
                    title: '提交成功!',
                    text: '您已投票',
                    icon: 'success',
                    confirmButtonText: 'Ok!',
                    confirmButtonColor: 'var(--deepBlue)',
                })
            },
            error: function (error) {
                console.log('Error:', error);
                // 在這里處理保存失敗的情況
            }
        });
    }
    // 如果沒有選擇投票選項，你可以顯示一個錯誤消息或者執行其他操作






});


