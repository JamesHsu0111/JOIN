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


//鈴鐺+下拉
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

document.addEventListener(
    "click",
    function (event) {
        // If user either clicks X button OR clicks outside the modal window, then close modal by calling closeModal()
        if (
            event.target.closest(".notification") == null &&
            event.target.closest(".notification-popup") == null
        ) {
            console.log("remove active click outside");
            $(".notification-popup").removeClass("active");
        }
    },
    false
);

// 初始化目前的未讀訊息數量，這裡假設有3條未讀訊息

let unreadMessagesCount = 3;

// 點擊某一個通知後，圓點顯示灰色，表示已讀
$(document).ready(function () {
    $(".popup-content").click(function () {
        const notificationDot = $(this).find(".notification-dot");
        if (notificationDot.css("color") === "rgb(153, 155, 159)") {
            // 如果圓點是灰色，表示已讀，將其設置為未讀（藍色）
            notificationDot.css("color", "#0860f7");
            // 增加未讀訊息數量
            unreadMessagesCount++;
            // 更新通知鈴鐺上的數字
            updateNotificationCount();
        } else {
            // 如果圓點是藍色，表示未讀，將其設置為已讀（灰色）
            notificationDot.css("color", "#999b9f");
            // 更新未讀訊息數量，減少1
            unreadMessagesCount--;
            // 更新通知鈴鐺上的數字
            updateNotificationCount();
        }

    });

    // 監聽read-all按鈕的點擊事件
    $(".read-all").click(function () {
        // 選擇所有的notification-dot元素並改變顏色
        $(".notification-dot").css("color", "#999b9f");
        // 將未讀訊息數量設置為0
        unreadMessagesCount = 0;
        // 更新通知鈴鐺上的數字
        updateNotificationCount();
    });
});


//更新通知數函式
function updateNotificationCount() {
    // 選擇通知鈴鐺上的數字元素
    const notificationNum = $(".notification--num");
    if (unreadMessagesCount > 0) {
        // 如果還有未讀訊息，顯示數字並更新內容
        notificationNum.text(unreadMessagesCount);
        notificationNum.show();
    } else {
        // 如果沒有未讀訊息，隱藏數字
        notificationNum.hide();
    }
}
