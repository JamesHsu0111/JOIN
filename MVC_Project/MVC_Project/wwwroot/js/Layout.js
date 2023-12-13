//回到網頁頂端
const scrollButton = document.getElementById("scroll-to-top");

window.addEventListener("scroll", () => {
    const scrollY = window.scrollY;

    // 檢查滾動位置是否超過1vh
    if (scrollY > window.innerHeight * 0.3) {
        scrollButton.classList.remove("hidden");
        scrollButton.style.opacity = 1;
        scrollButton.style.pointerEvents = "auto";
    } else {
        scrollButton.classList.add("hidden");
        scrollButton.style.opacity = 0;
        scrollButton.style.pointerEvents = "none";
    }
});

// 導覽列
$(document).ready(function () {
    // 選取探索按鈕
    var exploreLink = $("#explore-link");
    // 選取下拉內容
    var exploreDropdown = $("#explore-dropdown");
    //選取揪團按鈕
    var groupBtn = $("#group-link");

    exploreLink.hover(function () {
        // 滑鼠進入時顯示下拉內容
        exploreDropdown.css("display", "block");
    });

    exploreDropdown.hover(
        function () {
            // 滑鼠進入下拉內容時保持顯示
            exploreDropdown.css("display", "block");
        },
        function () {
            // 滑鼠離開下拉內容時隱藏
            exploreDropdown.css("display", "none");
        }
    );
    //滑鼠移動到揪團也會讓下拉消失
    groupBtn.hover(function () {
        exploreDropdown.css("display", "none");
    });
});


//鈴鐺+下拉顯示
$(".notification").on("click", function () {
    const ele = $(".notification-popup");
    if (ele.hasClass("active")) {
        console.log("remove active");
        ele.removeClass("active");
    } else {
        console.log("add active");
        ele.addClass("active");
    }
});

//關閉通知下拉
document.addEventListener(
    "click",
    function (event) {
        if (
            event.target.closest(".notification") == null &&
            event.target.closest(".notification-popup") == null
        ) {
            console.log("remove active click outside");
            $(".notification-popup").removeClass("active");
        }
    },
);

// 在頁面載入時，向後端發送請求以獲取通知數目和通知內容
$(document).ready(function () {
    //從body抓現在的使用者是誰(<input>裡面)
    let userId = $("#currentUserId").val();
    $.ajax({
        type: 'GET',
        url: '/MyActivity/GetNotifications',
        data: {
            userId: userId
        },
        success: function (notificationData) {
            ////Json資料
            //console.log(notificationData); // 輸出 JSON 數據到控制台
            //console.log(JSON.stringify(notificationData, null, 2)); // 格式化 JSON 數據

            // 更新通知數字
            var notificationNum = $(".notification--num");
            if (notificationData.notificationCount > 0) {
                // 如果還有未讀訊息，顯示數字並更新內容
                notificationNum.text(notificationData.notificationCount);
                notificationNum.show();
            } else {
                // 如果沒有未讀訊息，隱藏數字
                notificationNum.hide();
            }
            // 清空通知下拉框
            var notificationPopup = $(".notification-popup");
            // 保存通知下拉框的Header
            var notificationHeader = notificationPopup.find(".notification-popup__header").clone();
            var notificationEmpty = notificationPopup.find(".notification-empty").clone();

            notificationPopup.empty();
            // 清空通知下拉框，但保留Header
            notificationPopup.append(notificationHeader);


            // 生成通知內容
            if (notificationData.notifications.length > 0) {
                for (var i = 0; i < notificationData.notifications.length; i++) {
                    var notification = notificationData.notifications[i];
                    var isReadClass = notification.IsRead ? 'read' : 'unread'; // 判斷已讀或未讀
                    var notificationContent = '<div class="popup-content row ' + isReadClass + '" data-notificationid="' + notification.NotificationID + '" data-notificationtype="' + notification.NotificationType + '" data-notificationtoactivityid="' + notification.NotificationToWhichActivityID + '">';

                    notificationContent += '<i class="fa-solid fa-circle col-1 notification-dot ' + isReadClass + '"></i>';
                    notificationContent += '<i class="fa-regular fa-newspaper col-2"></i>';
                    notificationContent += '<div class="notification-message col-9">';
                    notificationContent += '<h4>' + notification.NotificationContent + '</h4>';
                    notificationContent += '<span class="notified-date">' + formatNotificationDate(notification.NotificationDate) + '</span>';
                    notificationContent += '</div>';
                    notificationContent += '</div>';

                    notificationPopup.append(notificationContent);
                }
            } else {
                // 如果通知數為0，顯示"暫無通知"提示
                notificationPopup.append(notificationEmpty);
            }
        },
        error: function () {
            console.log('無法獲取通知數據。');
        }
    });
});

// 點擊通知項目後更改已讀||未讀狀態
$(".notification-popup").on("click", ".popup-content", function () {
    var notificationId = $(this).data("notificationid");
    //從body抓現在的使用者是誰(<input>裡面)
    let userId = $("#currentUserId").val();

    // 使用AJAX向後端發送標記為已讀的請求
    $.ajax({
        type: 'POST',
        url: '/MyActivity/ChangeNotificationReadState',
        data: {
            userId: userId,
            notificationId: notificationId
        },
        success: function (notificationData) {
            // 更新通知下拉框
            var notificationPopup = $(".notification-popup");
            // 保存通知下拉框的Header
            var notificationHeader = notificationPopup.find(".notification-popup__header").clone();

            notificationPopup.empty();
            // 清空通知下拉框，但保留Header
            notificationPopup.append(notificationHeader);

            // 生成通知內容
            for (var i = 0; i < notificationData.notifications.length; i++) {
                var notification = notificationData.notifications[i];
                var isReadClass = notification.IsRead ? 'read' : 'unread'; // 判斷已讀或未讀
                var notificationContent = '<div class="popup-content row ' + isReadClass + '" data-notificationid="' + notification.NotificationID + '" data-notificationtype="' + notification.NotificationType + '" data-notificationtoactivityid="' + notification.NotificationToWhichActivityID + '">';

                notificationContent += '<i class="fa-solid fa-circle col-1 notification-dot ' + isReadClass + '"></i>';
                notificationContent += '<i class="fa-regular fa-newspaper col-2"></i>';
                notificationContent += '<div class="notification-message col-9">';
                notificationContent += '<h4>' + notification.NotificationContent + '</h4>';
                notificationContent += '<span class="notified-date">' + formatNotificationDate(notification.NotificationDate) + '</span>';
                notificationContent += '</div>';
                notificationContent += '</div>';

                notificationPopup.append(notificationContent);
            }

            // 更新通知數字
            var notificationNum = $(".notification--num");
            if (notificationData.notificationCount > 0) {
                // 如果還有未讀訊息，顯示數字並更新內容
                notificationNum.text(notificationData.notificationCount);
                notificationNum.show();
            } else {
                // 如果沒有未讀訊息，隱藏數字
                notificationNum.hide();
            }
        },
        error: function () {
            console.log('無法標記通知為已讀。');
        }
    });
});

// 點擊"全部已讀"選項
$(".notification-popup").on("click", ".read-all", function () {
    //從body抓現在的使用者是誰(<input>裡面)
    let userId = $("#currentUserId").val();

    $.ajax({
        type: 'POST',
        url: '/MyActivity/MarkAllNotificationsAsRead',
        data: {
            userId: userId,
        },
        success: function (notificationData) {
            // 更新通知下拉框
            var notificationPopup = $(".notification-popup");
            // 保存通知下拉框的Header
            var notificationHeader = notificationPopup.find(".notification-popup__header").clone();

            notificationPopup.empty();
            // 清空通知下拉框，但保留Header
            notificationPopup.append(notificationHeader);

            // 生成通知內容
            for (var i = 0; i < notificationData.notifications.length; i++) {
                var notification = notificationData.notifications[i];
                var isReadClass = notification.IsRead ? 'read' : 'unread'; // 判斷已讀或未讀
                var notificationContent = '<div class="popup-content row ' + isReadClass + '" data-notificationid="' + notification.NotificationID + '" data-notificationtype="' + notification.NotificationType + '" data-notificationtoactivityid="' + notification.NotificationToWhichActivityID + '">';

                notificationContent += '<i class="fa-solid fa-circle col-1 notification-dot ' + isReadClass + '"></i>';
                notificationContent += '<i class="fa-regular fa-newspaper col-2"></i>';
                notificationContent += '<div class="notification-message col-9">';
                notificationContent += '<h4>' + notification.NotificationContent + '</h4>';
                notificationContent += '<span class="notified-date">' + formatNotificationDate(notification.NotificationDate) + '</span>';
                notificationContent += '</div>';
                notificationContent += '</div>';

                notificationPopup.append(notificationContent);
            }

            // 更新通知數字
            var notificationNum = $(".notification--num");
            if (notificationData.notificationCount > 0) {
                // 如果還有未讀訊息，顯示數字並更新內容
                notificationNum.text(notificationData.notificationCount);
                notificationNum.show();
            } else {
                // 如果沒有未讀訊息，隱藏數字
                notificationNum.hide();
            }
        },
        error: function () {
            console.log('無法標記所有通知為已讀。');
        }
    });
});

// 處理通知日期顯示
function formatNotificationDate(notificationDate) {
    const now = new Date();
    const notificationDateTime = new Date(notificationDate);
    const timeDiff = now - notificationDateTime;
    const seconds = Math.floor(timeDiff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (seconds < 60) {
        return "現在";
    } else if (minutes < 60) {
        return `${minutes}分鐘前`;
    } else if (hours < 24) {
        return `${hours}小時前`;
    } else {
        return `${days}天前`;
    }
}

//判斷通知導向
$(".notification-popup").on("click", ".popup-content", function () {
    var notificationId = $(this).data("notificationid"); //可能需要用來判斷是哪個通知? 目前沒用到

    var notificationType = $(this).data("notificationtype"); // 讀取通知類型
    var isUnread = $(this).hasClass("unread"); // 檢查通知是否為未讀
    var notificationToActivityID = $(this).data("notificationtoactivityid"); //讀取要導向的活動的ID


    if (notificationType && isUnread) {
        // 根據通知類型執行不同的操作
        if (notificationType === "Vote") {
            window.location.href = `/VoteRecords/selectDate/${notificationToActivityID}`; // 導向投票頁面
        }
        else if (notificationType === "Reply" || notificationType === "ActivityToGroup") {
            window.location.href = `/grouppage/grouppage/${notificationToActivityID}`; // 導向活動頁面
        }
        else if (notificationType === "Cancelled") {
            window.location.href = "/Member/LikeRecord/"  //導向會員中心>已收藏活動
        }

        else {
            //應該沒有其他判斷了吧?
        }
    }
});


//SearchBar@@@@@@@@@@@@@@@@@@@@@@@@測試中@@@@@@@@@@@@@@@@@@@@@
// 透過按Enter鍵觸發搜索
$(".searchInputWrapper").on("keydown", "input[type='text']", function (e) {
    if (e.key === "Enter") {
        $("form").submit();
        console.log(finalResults); // 輸出 JSON 數據到控制台
        console.log(JSON.stringify(finalResults, null, 2)); // 格式化 JSON 數據
    }
});

// 透過點擊按鈕觸發搜索
$(".searchInputWrapper").on("click", "#search-icon", function () {
    $("form").submit();
    console.log(finalResults); // 輸出 JSON 數據到控制台
    console.log(JSON.stringify(finalResults, null, 2)); // 格式化 JSON 數據
});











// 聯絡我們-表單
// 當按鈕點擊時，顯示 dialog
$(document).ready(function () {
    // 表單驗證的代碼
    $("#contactBox form input").on("input", function () {
        if (this.checkValidity()) {
            $(this).css("border", "0.2rem green solid");
        } else {
            $(this).css("border", "0.2rem red solid");
        }
    });

     /*表單提交的代碼*/
    $("#myFormforContact").on("submit", function (event) {
        event.preventDefault();

        var formData = $(this).serialize();

        $.post("/Account/newContact", formData, function (response) {
            Swal.fire(
                '成功！',
                '您的資料已成功送出。',
                'success'
            );
            var dialog = document.getElementById("contactDialog");
            dialog.close();
            document.getElementById("myFormforContact").reset();
        }).fail(function () {
            Swal.fire(
                '失敗！',
                '出現了一些問題。',
                'error'
            );
        });
    });
});

document.getElementById("showContactButton").addEventListener("click", function () {
    var dialog = document.getElementById("contactDialog");
    dialog.showModal();
});
document.getElementById("closeDialogButton").addEventListener("click", function () {
    var dialog = document.getElementById("contactDialog");
    dialog.close();
});
document.getElementById("contactDialog").addEventListener("close", function () {
    document.getElementById("myFormforContact").reset();
});
