



// $('#myCarousel').carousel({
//     interval: false
//   });
//   $('#carousel-thumbs').carousel({
//     interval: false
//   });

// handles the carousel thumbnails
// https://stackoverflow.com/questions/25752187/bootstrap-carousel-with-thumbnails-multiple-carousel
//$('[id^=carousel-selector-]').click(function () {
//    var id_selector = $(this).attr('id');
//    var id = parseInt(id_selector.substr(id_selector.lastIndexOf('-') + 1));
//    $('#myCarousel').carousel(id);
//    console.log("id_selector:" +id_selector);
//    console.log("id:" +id);
//});
$(document).on('click', '[id^=carousel-selector-]', function () {
    var id_selector = $(this).attr('id');
    var id = parseInt(id_selector.substr(id_selector.lastIndexOf('-') + 1));
    $('#myCarousel').carousel(id);
    console.log("id_selector:" + id_selector);
    console.log("id:" + id);
});
// Only display 3 items in nav on mobile.
if ($(window).width() < 575) {
    $('#carousel-thumbs .row div:nth-child(4)').each(function () {
        var rowBoundary = $(this);
        $('<div class="row mx-0">').insertAfter(rowBoundary.parent()).append(rowBoundary.nextAll().addBack());
    });
    $('#carousel-thumbs .carousel-item .row:nth-child(even)').each(function () {
        var boundary = $(this);
        $('<div class="carousel-item">').insertAfter(boundary.parent()).append(boundary.nextAll().addBack());
    });
}

$('#myCarousel').on('slide.bs.carousel', function (e) {
    console.log('slide.bs.carousel');
    var id = parseInt($(e.relatedTarget).attr('data-slide-number'));
    $('[id^=carousel-selector-]').removeClass('selected');
    $('[id=carousel-selector-' + id + ']').addClass('selected');
    console.log(id)
});



$('#myCarousel .carousel-item img').on('click', function (e) {
    var src = $(e.target).attr('data-remote');
    if (src) $(this).ekkoLightbox();
});


$(document).on('toggle.bs.modal', '.modal fade', function () {
    $('.modal:visible').length && $(document.body).addClass('modal-open');
    getChatData();
});



//聊天室登入按鈕
$(document).ready(function () {
    $(document).on('click', '#signBtn', function () {
        window.location.href = '/Home/Login';
    });
    $(document).on('click', '#heartSignBtn', function () {
        window.location.href = '/Home/Login';
    });
});

// 聊天室
$(document).ready(function () {
    $(document).on('click', '#discussBtn', function () {
        $("#discussInput").toggle();
    });
});


$(document).on("click", ".replyBtn", function () {
    $($(this).parent().parent().children(".replyTextDiv")).toggle();
});




$(document).ready(function () {
    // 監聽 #discussTextArea 元素的 keydown 事件
    $(document).on("keydown", "#discussTextArea", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();  // 阻止預設的 Enter 鍵行為
            simulateClick();  // 呼叫模擬 click 函式
        }
    });

    $(document).on("click", ".publishBtn", simulateClick);  // 點擊 publishBtn 也呼叫模擬 click 函式

    function simulateClick() {
        var temp = $("#discussTextArea").val();
        if (temp == "") {
            Swal.fire('請輸入文字');


        } else {
            Swal.fire({

                title: '提交留言',
                text: "確定提交嗎?",
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                cancelButtonText: '取消',
                confirmButtonText: '確定',
                reverseButtons: true

            }).then((result) => {

                if (result.isConfirmed) {

                    discussUpdate();

                    Swal.fire({

                        icon: 'success',

                        title: '成功',

                        showConfirmButton: false,

                        timer: 1000

                    })

                }

            })
        }

    }
});


$(document).on("click", ".messageBtn", function () {
    var chatId = $(this).closest(".replyTextDiv").find("#replyTextDivId").val();

    var temp = $(this).parent().children("textarea[name='ChatContent']").val();
    if (temp == "") {
        Swal.fire('請輸入文字');
    } else {
        Swal.fire({

            title: '提交留言?',

            text: "確定提交嗎?",

            icon: 'question',

            showCancelButton: true,

            confirmButtonColor: '#3085d6',

            cancelButtonColor: '#d33',

            confirmButtonText: '確定',

            cancelButtonText: '取消'

        }).then((result) => {

            if (result.isConfirmed) {

                replyUpdate(chatId);

                Swal.fire({

                    icon: 'success',

                    title: '成功',

                    showConfirmButton: false,

                    timer: 1000

                })

            }

        })
    }
});


// 在 #replyTextArea 上監聽 keydown 事件
$(document).on("keydown", "#replyTextArea", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();  // 阻止預設的 Enter 鍵行為
        $(this).siblings(".messageBtn").click();  // 模擬點擊 .messageBtn
    }
});



/*收藏API (James 10/21改)*/
const VoteIcons = document.querySelectorAll('.vote-icon');

VoteIcons.forEach(function (voteIcon) {
    voteIcon.addEventListener('click', function () {
        let currentUserId = $('#currentUserId').val();
        if (currentUserId != 0) {
            const activityId = voteIcon.getAttribute('data-activityid');
            if (voteIcon.classList.contains('fa-envelope-open-text')) {
                $.ajax({
                    type: 'POST',
                    url: '/MyActivity/LikeActivity',
                    data: {
                        activityId: activityId,
                        userId: currentUserId
                    },
                    success: function (data) {
                        voteIcon.classList.remove('fa-envelope-open-text');
                        voteIcon.classList.add('fa-envelope-circle-check');
                        // 添加 padding-left 屬性
                        voteIcon.setAttribute('style', 'padding-left: 3px');
                        // 找到 custom-title 元素並更改其內容
                        var customTitle = document.querySelector('.custom-title');
                        customTitle.textContent = '已取得投票資格';
                    },
                    error: function (error) {
                        // 處理錯誤，如果有錯誤發生
                    }
                });
            } else {
                // 在此處執行AJAX DELETE請求，從"LikeRecord"資料表中刪除對應的記錄
                $.ajax({
                    type: 'DELETE',
                    url: '/Activity/UnlikeActivity',
                    data: {
                        activityId: activityId,
                        userId: currentUserId
                    },
                    success: function (data) {
                        voteIcon.classList.remove('fa-envelope-circle-check');
                        voteIcon.classList.add('fa-envelope-open-text');
                        // 移除 padding-left 屬性
                        voteIcon.removeAttribute('style');
                        // 找到 custom-title 元素並更改其內容
                        var customTitle = document.querySelector('.custom-title');
                        customTitle.textContent = '已取得投票資格';
                    },
                    error: function (error) {
                        // 處理錯誤，如果有錯誤發生
                    }
                });
            }
        }
    });
});


//group page取消報名 (James 10/21新增)
$(document).ready(function () {
    $('#registerLink').on('click', function () {
        // 檢查按鈕是否包含fa-user-check類別(已報名狀態)
        if ($(this).find('.fa-user-check').length > 0) {
            // 收集groupId和userId
            var groupId = $('input[name="groupId"]').val();
            var userId = $('input[name="userId"]').val();

            // 執行刪除操作
            $.ajax({
                url: '/groupPage/DeleteRegistration',
                type: 'POST',
                data: {
                    groupId: groupId,
                    userId: userId
                },
                success: function (response) {
                    if (response.success) {
                        var customTitle = document.querySelector('.custom-title');
                        customTitle.textContent = '';
                        // 修改i元素的圖示
                        $('#registerLink').removeClass('fa-user-check').addClass('fa-user-plus');
                        // 修改表單的 action
                        $('#registerForm').attr('asp-action', '/grouppage/Register');

                    } else {
                    }
                }
            });
        }
    });
});

//主揪資訊

$(document).ready(function () {

    Organizer();


});
function Organizer() {
    const id = getIdFromUrl();

    console.log(`/api/Organizer/${id}`);
    $.ajax({
        url: `/api/Organizer/${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log(data[0].Nickname);
            $('.organizer-info').text(data[0].Nickname);
        },
        error: function (error) {
            console.error('Error:', error);

        }
    });

}

/*抓取groupid*/
function getIdFromUrl() {
    const groupidElement = document.querySelector('.groupid');  // 選取具有 class 為 groupid 的元素
    if (groupidElement) {
        return groupidElement.getAttribute('data-groupid');
    }
    return null;
}
/*抓取groupid*/
function getChatData() {
    const id = getIdFromUrl();  // 取得 id
    if (!id) {
        console.log('ID not found in the URL.');
        return;
    }

    $.ajax({
        url: `/api/chatData/${id}`,  // 使用 id 構建 URL
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            updateChatInModal(data);
            console.log("updateChatInModal(data);");
            // Do something with the data, e.g., update the UI
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}

const id = getIdFromUrl();
if (id) {
    console.log('ID:', id);
    getChatData(id);  // 呼叫 getChatData 函式並傳遞 id
}


function updateChatInModal(chatList) {
    // 清空原有的討論區內容
    $("#dialogDiv").empty();

    var replyList = chatList.slice().reverse();
    // 將新的聊天資料插入到討論區
    var chatBoardHTML = `<div>
                    <div id="messageBoardTitle">
                        <span class="h1">留言板</span>
                        <div id="discussBtn">
                        <i class="fa-regular fa-message"></i>  留言
                        </div>
                    </div>

                    <!-- 討論輸入區 -->
                    
                    <div class="commentDiv" id="discussInput">
                            <div class="userCommentDiv">
                                <img class="profile" src="" />
                                <div class="userCommentDivRight">
                                    <p class="h3 align-self-center"></p>
                                    <div class="discuss-box align-self-start" name="ChatContent">
                                        <textarea name="ChatContent" id="discussTextArea" class="col-auto myTextarea" rows="3" placeholder="留言....."></textarea>
                                    </div>
                                </div>
                                <div class="commentBtnDiv">
                                <button class="publishBtn" type="submit">
                                    <i class="fa-regular fa-paper-plane"></i>
                                </button>
                            </div>
                            </div>
                            
                    </div>
                </div>`;
    $('#dialogDiv').append(chatBoardHTML);
    replyList.forEach(function (chat) {
        if (chat.ToWhom === null) {
            var chatId = chat.ChatID;  // 注意這裡要使用 ChatID，而不是 chatId
            var userPhoto = chat.UserPhoto ? `<img src="data:image/png;base64,${chat.UserPhoto}" class="profile" />` : '';
            var chatContent = chat.ChatContent ? `<div class="comment-box align-self-start">${chat.ChatContent}</div>` : '';

            // 計算留言時間與系統時間的差距
            var chatTime = new Date(chat.ChatTime);
            var currentTime = new Date();
            var timeDiff = currentTime - chatTime;
            var timeText = '';

            if (timeDiff < 60000) {
                // 小於1分鐘
                timeText = '現在';
            } else if (timeDiff < 3600000) {
                // 小於60分鐘
                var minutes = Math.floor(timeDiff / 60000);
                timeText = minutes + ' 分鐘前';
            } else if (timeDiff < 86400000) {
                // 小於24小時
                var hours = Math.floor(timeDiff / 3600000);
                timeText = hours + ' 小時前';
            } else {
                // 大於24小時
                var days = Math.floor(timeDiff / 86400000);
                timeText = days + ' 天前';
            }

            var chatCommentDiv = `
            <div class="commentDiv">
                <div class="userCommentDiv">
                    ${userPhoto}
                    <div class="userCommentDivRight">
                        <p class="ChatTitle align-self-center">${chat.Nickname}</p>
                        ${chatContent}
                </div>
            </div>
            <div class="editDeleteTime-div">
                <a id="editA" class="editA" value="${chat.UserID}" ChatID = "${chat.ChatID}" chatToWhom = "${chat.ToWhom}">編輯</a>
                <a id="deleteA" class="deleteA" value="${chat.UserID}" ChatID = "${chat.ChatID}">刪除</a>
                <p class="commentDatetime">${timeText}</p>

                <div class="commentBtnDiv replyBtn" id =${chatId}>
                    <div id="replyBtn">
                        <span>回覆</span>
                </div>
            </div>
            </div>
            `;
            chatList.forEach(function (reply) {
                if (reply.ToWhom !== null && reply.ToWhom === chatId) {
                    //var replyTime = new Date(reply.ChatTime).toLocaleString('en-US', {
                    //    year: 'numeric',
                    //    month: '2-digit',
                    //    day: '2-digit',
                    //    hour: '2-digit',
                    //    minute: '2-digit'
                    //});
                    // 計算留言時間與系統時間的差距
                    var replyTime = new Date(reply.ChatTime);
                    var currentTime = new Date();
                    var timeDiff = currentTime - replyTime;
                    var timeText = '';

                    if (timeDiff < 60000) {
                        // 小於1分鐘
                        timeText = '現在';
                    } else if (timeDiff < 3600000) {
                        // 小於60分鐘
                        var minutes = Math.floor(timeDiff / 60000);
                        timeText = minutes + ' 分鐘前';
                    } else if (timeDiff < 86400000) {
                        // 小於24小時
                        var hours = Math.floor(timeDiff / 3600000);
                        timeText = hours + ' 小時前';
                    } else {
                        // 大於24小時
                        var days = Math.floor(timeDiff / 86400000);
                        timeText = days + ' 天前';
                    }


                    // 建立回覆的 HTML
                    var replyHtml = `
                    <div class="replyDiv">
                        <div class="userCommentDiv">
                            <img src="data:image/png;base64,${reply.UserPhoto}" class="profile-toReply" />
                            <div class="userCommentDivRight">
                                <p class="ChatTitle align-self-center">${reply.Nickname}</p>
                                ${reply.ChatContent}
                        </div>
                    </div>
                    <div class="editDeleteTime-div">
                        <a id="editA" class="editA"  value="${reply.UserID}" ChatID = "${reply.ChatID}" chatToWhom = "${reply.ToWhom}">編輯</a>
                        <a id="deleteA" class="deleteA" value="${reply.UserID}" ChatID = "${reply.ChatID}">刪除</a>
                        <p class="commentDatetime">${timeText}</p>
                    </div>
                </div>`;

                    chatCommentDiv = chatCommentDiv + replyHtml;
                }
            });
            var replyTextDiv = `
                <div class="replyTextDiv" id="replyTextDiv${chatId}">
                    <input type="hidden" id="replyTextDivId" value="${chatId}"/>
                    <div class="userCommentDiv">
                        <img class="profile" src=""/>
                        <div class="userCommentDivRight">
                            <p class="h3 align-self-center" id="userInfoNickname"></p>
                            <textarea name="ChatContent" id="replyTextArea" class="col-auto myTextarea" rows="1" placeholder="留言....."></textarea>
                        </div>
                        <div class="commentBtnDiv">
                            <button class="messageBtn" type="submit">
                                <i class="fa-regular fa-paper-plane"></i>    
                            </button>
                        </div>
                    </div>
                </div>
            </div>`;

            chatCommentDiv = chatCommentDiv + replyTextDiv;
            // 插入到討論區
            $('#dialogDiv').append(chatCommentDiv);
        }
    });
    getUserInfo();
    userEditDelete();
}

//會員讀取自訂函式
function userEditDelete() {
    const currentUserId = $('#currentUserId').val(); // 使用者id

    $('.editA, .deleteA').each(function () {
        const editLink = $(this);
        const chatUserId = editLink.attr('value');

        // 检查条件，如果当前用户的ID等于聊天用户的ID，则显示链接
        if (currentUserId === chatUserId || currentUserId == 7) {
            editLink.css('display', 'inline-block');
        } else {
            editLink.css('display', 'none');
        }
    });
}

function getUserInfo() {
    let currentUserId = $('#currentUserId').val();

    console.log(`/api/getUserInfo/${currentUserId}`);
    $.ajax({
        url: `/api/getUserInfo/${currentUserId}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log('userInfo', data);
            $('#discussInput .userCommentDiv p').text(data[0].Nickname);
            $('#discussInput .userCommentDiv .profile').attr('src', 'data:image/png;base64,' + data[0].UserPhoto);
            $('.replyTextDiv .userCommentDiv #userInfoNickname').text(data[0].Nickname);
            $('.replyTextDiv .userCommentDiv .profile').attr('src', 'data:image/png;base64,' + data[0].UserPhoto);
            getUserIngroup();
        },
        error: function (error) {
            console.error('Error:', error);
            $('#replyTextDiv').remove();
            $('.commentDiv').remove();
            $('#discussInput').remove();
            $('#discussBtn').replaceWith(`
                <span class="registertodiscuss"><i class="fa-solid fa-triangle-exclamation fa-lg" style="color: #eed21b;"></i> 報名後顯示內容</span>
            `);
        }
    });
}
//會員留言權限
function getUserIngroup() {
    const id = getIdFromUrl();
    console.log(`/api/getUserIngroup/${id}`);
    $.ajax({
        url: `/api/getUserIngroup/${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            if (data == false ) {
                $('#replyTextDiv').remove();
                console.log("$(`#replyTextDiv`).empty();")
                $('.commentBtnDiv').remove();
                $('#discussInput').remove();
                $('#discussBtn').replaceWith(`
                <span class="registertodiscuss"><i class="fa-solid fa-triangle-exclamation fa-lg" style="color: #eed21b;"></i> 報名後開放留言功能</span>
            `);
            }

            console.log(currentUserId)
            console.log(data);
        },
        error: function (error) {

        }
    });
}

//刪除留言請求
$(document).on('click', '.deleteA', function () {
    Swal.fire({
        title: '確定要刪除留言嗎?',
        text: "刪除後無法回復",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: '確認',
        cancelButtonText: '取消',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            const currentUserId = $('#currentUserId').val(); // 当前用户的 ID
            const replyUserId = $(this).attr('value'); // 获取要删除的留言的用户 ID
            if (currentUserId === replyUserId || currentUserId == 7) {
                const ChatID = $(this).attr('ChatID'); // 获取要删除的留言的 ID
                console.log(ChatID);

                // 发送 DELETE 请求到后端 API 来删除留言
                fetch(`/groupPage/Delete/${ChatID}`, {
                    method: 'Post',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                })
                    .then(response => {
                        Swal.fire({
                            title: '刪除留言',
                            text: '刪除成功',
                            icon: 'success',
                            showConfirmButton: false,
                            timer: 1000
                        })
                        getChatData();
                    })
                    .catch(error => {
                        console.error('An error occurred:', error);
                    });
            } else {
                console.log('You are not allowed to delete this reply.');
            }

        }
    })
});


//編輯留言請求

$(document).on('click', '.editA', function () {
    console.log("OK");
    Swal.fire({
        title: "編輯",
        html:
            '<b>更改留言：</b>',
        input: 'text',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        reverseButtons: true
    }).then((result) => {
        if (result.value) {
            const id = getIdFromUrl();
            const currentUserId = $('#currentUserId').val(); // 当前用户的 ID
            const ChatID = $(this).attr('ChatID');
            const Content = result.value;
            const chatToWhom = $(this).attr('chatToWhom');;
            var editData = {
                ChatID: ChatID,
                ActivityID: id,
                UserID: currentUserId,
                ChatContent: Content,

            };
            console.log(editData)
            $.ajax({

                url: '/groupPage/EditChat',

                type: 'POST',

                data: JSON.stringify(editData),

                contentType: 'application/json; charset=utf-8',

                success: function (response) {
                    Swal.fire({
                        title: '編輯成功',
                        icon: 'success',
                        showConfirmButton: false,
                        timer: 1000
                    });
                    getChatData();

                },
                error: function (error) {

                }
            });
        } else {
            Swal.fire({
                title: '未更改',
                icon: 'info',
                showConfirmButton: false,
                timer: 1000
            });
        }

    })
});






//討論上傳自訂函數
function discussUpdate() {
    let discussContent = $('#discussTextArea').val();
    const id = getIdFromUrl();  // 取得 id
    let currentUserId = $('#currentUserId').val();

    let updateData = {
        ActivityID: id,
        UserID: currentUserId,
        ChatContent: discussContent,
        ToWhom: null,
        // 可以添加其他需要上传的数据字段
    };

    // 发起AJAX请求
    $.ajax({
        url: '/api/discussUpdate', // 后端API的URL
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify(updateData), // 将数据转为JSON字符串
        contentType: 'application/json', // 设置Content-Type为JSON
        success: function (response) {
            console.log('数据成功上传:', response);
            getChatData();
            // 在这里处理成功响应
        },
        error: function (error) {
            console.error('上传数据时出错:', error);
            console.log(JSON.stringify(updateData));
            // 在这里处理错误响应
        }
    });
}

//留言上傳
function replyUpdate(chatId) {
    const id = getIdFromUrl();  // 取得 id
    const replyContent = $(`#replyTextDiv${chatId}`).find('textarea[name="ChatContent"]').val();  // 根據 chatId 取得回覆內容
    const currentUserId = $('#currentUserId').val();

    let replyData = {
        ActivityID: id,
        UserID: currentUserId,
        ChatContent: replyContent,
        ToWhom: chatId,  // 設定回覆的對象為原始留言的 chatId
    };

    // 發起 AJAX 請求
    $.ajax({
        url: '/api/replyUpdate',  // 後端 API 的 URL
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify(replyData),  // 將資料轉為 JSON 字串
        contentType: 'application/json',  // 設定 Content-Type 為 JSON
        success: function (response) {
            console.log('回覆成功:', response);
            getChatData();  // 更新聊天資料
            // 在這裡處理成功回應
        },
        error: function (error) {
            console.error('回覆時發生錯誤:', error);
            console.log(JSON.stringify(replyData));
            // 在這裡處理錯誤回應
        }
    });
}


$(document).ready(function () {
    photoGet();
    console.log("getphoto");
    });
//圖片API
function photoGet() {
    const id = getIdFromUrl();
    $.ajax({
        url: `/api/photoGet/${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log(data);
            $(".carouselDiv").empty();
            var carouselHTML = `
                <div class="carousel-container position-relative  col-lg-8" id="pictureDiv">
                <div id="myCarousel" class="carousel slide" data-ride="carousel">
                    <div class="carousel-inner">
                        <div class="carousel-item active" data-slide-number="0">
                            <img src="${data[0].PhotoPath}" class="d-block w-100" alt="..." data-type="image"
                                 data-toggle="lightbox" data-gallery="example-gallery">
                        </div>
                        <div class="carousel-item" data-slide-number="1">
                            <img src="${data[1].PhotoPath}" class="d-block w-100" alt="..." data-type="image"
                                 data-toggle="lightbox" data-gallery="example-gallery">
                        </div>
                        <div class="carousel-item" data-slide-number="2">
                            <img src="${data[2].PhotoPath}" class="d-block w-100" alt="..." data-type="image"
                                 data-toggle="lightbox" data-gallery="example-gallery">
                        </div>
                        <div class="carousel-item" data-slide-number="3">
                            <img src="${data[3].PhotoPath}" class="d-block w-100" alt="..." data-type="image"
                                 data-toggle="lightbox" data-gallery="example-gallery">
                        </div>
                    </div>
                </div>

                <!-- Carousel Navigation -->
                <div id="carousel-thumbs" class="carousel slide " data-ride="carousel">
                    <div class="carousel-inner">
                        <div class="carousel-item active">
                            <div class="row mx-0">
                                <div id="carousel-selector-0" class="thumb col-sm-1 col-lg-3 px-1 py-2 selected"
                                     data-target="#myCarousel" data-slide-to="0">
                                    <img src="${data[0].PhotoPath}" class="img-fluid" alt="...">
                                </div>
                                <div id="carousel-selector-1" class="thumb col-sm-1 col-lg-3 px-1 py-2" data-target="#myCarousel"
                                     data-slide-to="1">
                                    <img src="${data[1].PhotoPath}" class="img-fluid" alt="...">
                                </div>
                                <div id="carousel-selector-2" class="thumb col-sm-1 col-lg-3 px-1 py-2" data-target="#myCarousel"
                                     data-slide-to="2">
                                    <img src="${data[2].PhotoPath}" class="img-fluid" alt="...">
                                </div>
                                <div id="carousel-selector-3" class="thumb col-sm-1 col-lg-3 px-1 py-2" data-target="#myCarousel"
                                     data-slide-to="3">
                                    <img src="${data[3].PhotoPath}" class="img-fluid" alt="...">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            `
            $(".carouselDiv").append(carouselHTML);
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}




$(document).ready(function () {
    $(".organizer-info").click(function () {
        var organizerId = $(this).data('organizer-id');

        $.ajax({
            url: '/Member/GetOrganizerInfo',  // 請更換成你的Controller和Action名稱
            method: 'GET',
            data: { userId: organizerId },
            success: function (response) {
                // 顯示彈出窗，並在裡面填充主揪的資訊
                $('#organizerNickname').text(response.Nickname);
                $('#organizerIntro').text(response.Intro);
                $('#organizerImage').attr('src', response.imageUrl);
                $('#organizerModal').modal('show');
            }
        });
    });
});

//-------James加的--------------
//麵包屑判斷導向哪個活動類別
$(document).ready(function () {
    $("nav ol li a").click(function (e) {
        e.preventDefault(); // 阻止默認連結行為

        var linkText = $(this).text();

        // 使用switch語句根據不同的類別內容執行不同的操作
        switch (linkText) {
            case "登山":
                // 更新href屬性
                var newHref = "/MyActivity/ACT?page=0&category=登山";
                $(this).attr("href", newHref);
                // 使用window.location進行重定向
                window.location.href = newHref;
                break;
            case "溯溪":
                // 更新href屬性
                var newHref = "/MyActivity/ACT?page=0&category=溯溪";
                $(this).attr("href", newHref);
                // 使用window.location進行重定向
                window.location.href = newHref;
                break;
            case "潛水":
                // 更新href屬性
                var newHref = "/MyActivity/ACT?page=0&category=潛水";
                $(this).attr("href", newHref);
                // 使用window.location進行重定向
                window.location.href = newHref;
                break;
            case "露營":
                // 更新href屬性
                var newHref = "/MyActivity/ACT?page=0&category=露營";
                $(this).attr("href", newHref);
                // 使用window.location進行重定向
                window.location.href = newHref;
                break;
            case "其他":
                // 更新href屬性
                var newHref = "/MyActivity/ACT?page=0&category=其他";
                $(this).attr("href", newHref);
                // 使用window.location進行重定向
                window.location.href = newHref;
                break;
            default:
                // 更新href屬性
                var newHref = "/MyActivity/ACT";
                $(this).attr("href", newHref);
                // 使用window.location進行重定向
                window.location.href = newHref;
                break;
        }
    });
});

$(document).ready(function () {
    const id = getIdFromUrl();
    $.ajax({
        url: `/api/registration/${id}`,  
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            var RegistrationHTML = "";
            var count = 0;
            response.forEach(function (responseList) {
                count ++ ;
                var tr = `<tr>
                            <th scope="row">${count}</th>
                            <td>${responseList.Nickname}</td>
                            </tr>`
                RegistrationHTML = RegistrationHTML + tr;
            });
            $("#RegistrationName").append(RegistrationHTML);
        }
    });
    
    $(".progress").click(function () {
        console.log("showmodal");
        $("#registrationModal").modal("show");
    });
});
