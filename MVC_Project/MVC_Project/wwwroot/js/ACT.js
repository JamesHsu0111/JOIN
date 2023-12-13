//--------James加的-----------
//麵包屑設計
$(document).ready(function () {
    // 獲取當前URL
    var encodedText = window.location.href;
    var currentURL = decodeURIComponent(encodedText);
    console.log(currentURL);

    // 檢查URL的分類
    switch (true) {
        case currentURL.endsWith("登山"):
            updateBreadcrumb("登山");
            break;
        case currentURL.endsWith("溯溪"):
            updateBreadcrumb("溯溪");
            break;
        case currentURL.endsWith("潛水"):
            updateBreadcrumb("潛水");
            break;
        case currentURL.endsWith("露營"):
            updateBreadcrumb("露營");
            break;
        case currentURL.endsWith("其他"):
            updateBreadcrumb("其他");
            break;
        default:
            // 如果URL不符合任何分類，保持原有麵包屑
            break;
    }
});

function updateBreadcrumb(category) {
    // 更新麵包屑內容
    $(".breadcrumb").append('<li class="separator">&nbsp;<i class="fa-solid fa-chevron-right"></i>&nbsp;</li>');
    $(".breadcrumb").append('<li class="breadcrumb-item">' + category + '</li>');
}

var checkbox = document.getElementById("cktoggle_id2");

// 監聽 checkbox 變化事件
checkbox.addEventListener("change", function () {
    // 檢查 checkbox 的狀態
    if (checkbox.checked) {
        // checkbox 被選中
        console.log("Checkbox 已選中");
        $.ajax({
            url: '/api/loadData', // 控制器的路徑
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                console.log(data);
                $('.cards').empty();
                data.forEach(function (item) {
                    var StartDate = new Date(item.StartDate).toLocaleString('en-US', {
                        year: 'numeric',
                        month: '2-digit',
                        day: '2-digit'

                    });
                    var EndDate = new Date(item.EndDate).toLocaleString('en-US', {
                        year: 'numeric',
                        month: '2-digit',
                        day: '2-digit'

                    });
                    /*console.log(item.PhotoData[0].PhotoData);*/
                    var photo = `<img class='card__img--hover' src = 'data: image/png;base64,${item.PhotoData[0].PhotoData}'></img>`

                    var cardHtml = `
            <article class="card card--${item.GroupID}">
                <div class="card__info-hover">
                
                    <div class="card__clock-info">
                               
                    </div>
                </div>
                <div class="card__img"></div>
                <a href="/groupPage/groupPage/${item.GroupID}" class="card_link">
                    ${photo}
                </a>
                <div class="card__info">
                    <span class="card__category">${item.GroupCategory}</span>
                    <h3 class="card__title">${item.GroupName}</h3>
                    <p class="card__period" style="font-size: 1.25rem;font-weight: bold;">${StartDate} ~ ${EndDate}</p>
                    <span class="card__by">主揪<a href="#" class="card__author" title="author"> ${item.Nickname}</a></span>
                </div>  
            </article>
        `;
                    $('.cards').append(cardHtml); // 將生成的卡片添加到.cards元素中
                });
                

            },
            error: function (error) {
                console.error(error);
            }
        });

    } else {
        // checkbox 未被選中
        console.log("Checkbox 未選中");
        $.ajax({
            url: '/api/loadActivityData', // 控制器的路徑
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                console.log(data);
                $('.cards').empty();
                data.forEach(function (item) {
                    var photo = item.OfficialPhoto[0].PhotoPath;
                    console.log(photo);
                    var VoteMonth = new Date(item.VoteDate).toLocaleString('en-US', {
                        month: '2-digit'
                    });
                    var VoteDay = new Date(item.VoteDate).toLocaleString('en-US', {
                        day: '2-digit'

                    });
                    var ExpectedYear = new Date(item.ExpectedDepartureMonth).toLocaleString('en-US', {
                        year: 'numeric',
                    });

                    var ExpectedMonth = new Date(item.ExpectedDepartureMonth).toLocaleString('en-US', {
                        month: '2-digit'
                    });
                    

                                        var cardHtml = `
            <article class="card card--${item.ActivityID}">
          <div class="card__info-hover">
            <i class="fa-solid fa-envelope-open-text fa-2xl vote-icon"></i>
            <div class="card__clock-info">
              <div class="card__img">${item.OfficialPhotoID}</div>
              </div>
          </div>

          <div class="card__img" style="background-image: url(${photo})"></div>
          <a href="/Activity/Index/${item.ActivityID}" class="card_link">
            <div class="card__img--hover" style="background-image: url(${photo})"></div>
          </a>
          <span class="card__VoteDate-text">投票日：<span style="color: var(--brown)">${VoteMonth}月${VoteDay}日</span></span>
          <div class="card__info">
            <span class="card__category">${item.Category}</span>
            <h3 class="card__title">${item.ActivityName}</h3>
            <span class="card__ExpectedMonth">${ExpectedYear}-${ExpectedMonth} 月活動</span>
          </div>
        </article>
`;
                    $('.cards').append(cardHtml); // 將生成的卡片添加到.cards元素中
                });


            },
            error: function (error) {
                console.error(error);
            }
        });

    }
});

