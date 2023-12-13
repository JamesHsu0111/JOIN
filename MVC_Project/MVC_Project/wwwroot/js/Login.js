


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

    exploreLink.hover(
        function () {
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
    groupBtn.hover(
        function () {
            exploreDropdown.css("display", "none")
        }
    )
});

//網站介紹
$(".option").click(function () {
    $(".option").removeClass("active");
    $(this).addClass("active");
});


//活動方塊-愛心設計
const heartIcons = document.querySelectorAll('.heart-icon');

heartIcons.forEach(function (heartIcon) {
    heartIcon.addEventListener('click', function () {
        if (heartIcon.classList.contains('fa-regular')) {
            heartIcon.classList.remove('fa-regular');
            heartIcon.classList.add('fa-solid');
            heartIcon.style.color = "#B44163";

            const cardInfo = heartIcon.closest('.card').querySelector('.card__info');

            const likedText = document.createElement('span');
            likedText.textContent = '已收藏';
            likedText.classList.add('liked-text');

            cardInfo.appendChild(likedText);
        } else {
            heartIcon.classList.remove('fa-solid');
            heartIcon.classList.add('fa-regular');
            heartIcon.style.color = "#1E3050";
            // 移除已經存在的 "Liked" 文字
            const likedText = heartIcon.closest('.card').querySelector('.liked-text');
            if (likedText) {
                likedText.remove();
            }
        }
    });
});

// 聯絡我們-表單
// 當按鈕點擊時，顯示 dialog
document.getElementById("showContactButton").addEventListener("click", function () {
    var dialog = document.getElementById("contactDialog");
    dialog.showModal();


});

// 當關閉按鈕被點擊時，關閉 dialog
document.getElementById("closeDialogButton").addEventListener("click", function () {
    var dialog = document.getElementById("contactDialog");
    dialog.close();
});

// 當 dialog 關閉時，重置表單
document.getElementById("contactDialog").addEventListener("close", function () {
    document.getElementById("myForm").reset();
});




//響應式表單
$(document).ready(function () {
    $("#contactBox form input").on("input", function () {
        if (this.checkValidity()) {
            $(this).css("border", "0.2rem green solid");
        } else {
            $(this).css("border", "0.2rem red solid");
        }
    });
});

//alert表單內容
document.getElementById("myForm").onsubmit = function (event) {
    // 獲取表單元素
    var form = event.target;
    // 獲取各input元素的值
    var name = form.elements["name"].value;
    var email = form.elements["email"].value;
    var emailSubject = form.elements["emailSubject"].value;
    var mobileNumber = form.elements["mobileNumber"].value;
    var message = form.elements["message"].value;


};




































//BUG修正
// 獲取註冊鏈接元素
const registerLink = document.querySelector('.register-link');
const loginLink = document.querySelector('.login-link'); // 添加這一行來獲取登入鏈接

// 添加點擊事件監聽器
registerLink.addEventListener('click', function () {
    // 獲取登入表單和註冊表單元素
    const loginForm = document.getElementById('container1');
    const signupForm = document.getElementById('container2');

    // 切換表單的顯示狀態
    loginForm.style.display = "none";
    signupForm.style.display = "block";
    signupForm.style.visibility = "visible";
});

// 添加從註冊切換回登入的事件監聽器
loginLink.addEventListener('click', function () {
    // 獲取登入表單和註冊表單元素
    const loginForm = document.getElementById('container1');
    const signupForm = document.getElementById('container2');

    // 切換表單的顯示狀態
    loginForm.style.display = "block";
    signupForm.style.display = "none";
});

// 在頁面加載時，確保一個表單可見，一個表單不可見
document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('container1');
    const signupForm = document.getElementById('container2');
    loginForm.style.display = "block";
    signupForm.style.display = "none";
});