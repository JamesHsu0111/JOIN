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



    //點擊鈴鐺後，數字通知消失
    // 使用事件委託，當點擊<i>元素時執行以下操作
    $(".notification").on("click", "i", function () {
        // 移除包含.notification--num的元素
        $(this).parent().find(".notification--num").remove();
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








// 取得下拉選單項目 
document.addEventListener('DOMContentLoaded', function () {
    const dropdownItems = document.querySelectorAll('dropdown ul li');

    dropdownItems.forEach(item => {
        item.addEventListener('click', function () {
            const selectedText = this.textContent;
            const correspondingLabel = this.closest('dropdown').querySelector('label');

            correspondingLabel.textContent = selectedText;
        });
    });
});


//   選取完或點擊空白處會自動縮回
document.addEventListener('click', function (event) {
    var dropdowns = document.querySelectorAll('dropdown');
    dropdowns.forEach(function (dropdown) {
        if (!dropdown.contains(event.target) || event.target.tagName === 'LI') {
            var input = dropdown.querySelector('input');
            if (input.checked) {
                input.checked = false;
            }
        }
    });
});

















// 取得輸入人數
$(document).ready(function () {
    // 取得下限
    const dropdownInputsA = $('dropdown input.NumberofpeopleA');

    dropdownInputsA.each(function () {
        $(this).on('input', function () {
            const inputValue = $(this).val();
            const numericValue = inputValue.replace(/[^0-9]/g, ''); // 过滤非数字字符

            const correspondingLabel = $(this).closest('dropdown').find('label.CC');

            // 顯示在上面後 顯示 下限_人 底線是輸入的數字
            if (numericValue) {
                correspondingLabel.text('下限 ' + numericValue + ' 人');
            } else {
                correspondingLabel.text('輸入人數下限');
            }
        });
    });

    // 取得上限
    const dropdownInputsB = $('dropdown input.NumberofpeopleB');

    dropdownInputsB.each(function () {
        $(this).on('input', function () {
            const inputValue = $(this).val();
            const numericValue = inputValue.replace(/[^0-9]/g, ''); // 过滤非数字字符

            const correspondingLabel = $(this).closest('dropdown').find('label.CC');

            // 顯示在上面後 顯示 下限_人 底線是輸入的數字
            if (numericValue) {
                correspondingLabel.text('上限 ' + numericValue + ' 人');
            } else {
                correspondingLabel.text('輸入人數上限');
            }
        });
    });



    // 選取完或點擊空白處會自動縮回
    $(document).click(function (event) {
        var dropdowns = $('dropdown');
        dropdowns.each(function () {
            if (!$(this).is(event.target) && $(this).has(event.target).length === 0) {
                var input = $(this).find('input');
                if (input.prop('checked')) {
                    input.prop('checked', false);
                }
            }
        });
    });

    // 只能輸入數字

    $('.NumberofpeopleA, .NumberofpeopleB').on('input', function () {
        const inputValue = $(this).val();
        const sanitizedValue = inputValue.replace(/[^0-9]/g, ''); // 去除非数字字符

        $(this).val(sanitizedValue); // 更新输入框的值为去除非数字字符后的值
    });

});













//日曆

$(document).ready(function () {
    function c(passed_month, passed_year, calNum) {
        var calendar = calNum == 0 ? calendars.cal1 : calendars.cal2;
        makeWeek(calendar.weekline);
        calendar.datesBody.empty();
        var calMonthArray = makeMonthArray(passed_month, passed_year);
        var r = 0;
        var u = false;
        while (!u) {
            if (daysArray[r] == calMonthArray[0].weekday) {
                u = true
            } else {
                calendar.datesBody.append('<div class="blank"></div>');
                r++;
            }
        }
        for (var cell = 0; cell < 42 - r; cell++) { // 42 date-cells in calendar
            if (cell >= calMonthArray.length) {
                calendar.datesBody.append('<div class="blank"></div>');
            } else {
                var shownDate = calMonthArray[cell].day;
                var iter_date = new Date(passed_year, passed_month, shownDate);
                if (
                    (
                        (shownDate != today.getDate() && passed_month == today.getMonth()) || passed_month != today.getMonth()) && iter_date < today) {
                    var m = '<div class="past-date">';
                } else {
                    var m = checkToday(iter_date) ? '<div class="today">' : "<div>";
                }
                calendar.datesBody.append(m + shownDate + "</div>");
            }
        }

        var color = "#444444";
        calendar.calHeader.find("h2").text(i[passed_month] + " " + passed_year);
        calendar.weekline.find("div").css("color", color);
        calendar.datesBody.find(".today").css("color", "#87b633");

        // find elements (dates) to be clicked on each time
        // the calendar is generated
        var clicked = false;
        selectDates(selected);

        clickedElement = calendar.datesBody.find('div');
        clickedElement.on("click", function () {
            clicked = $(this);
            var whichCalendar = calendar.name;

            if (firstClick && secondClick) {
                thirdClicked = getClickedInfo(clicked, calendar);
                var firstClickDateObj = new Date(firstClicked.year,
                    firstClicked.month,
                    firstClicked.date);
                var secondClickDateObj = new Date(secondClicked.year,
                    secondClicked.month,
                    secondClicked.date);
                var thirdClickDateObj = new Date(thirdClicked.year,
                    thirdClicked.month,
                    thirdClicked.date);
                if (secondClickDateObj > thirdClickDateObj && thirdClickDateObj > firstClickDateObj) {
                    secondClicked = thirdClicked;
                    // then choose dates again from the start :)
                    bothCals.find(".calendar_content").find("div").each(function () {
                        $(this).removeClass("selected");
                    });
                    selected = {};
                    selected[firstClicked.year] = {};
                    selected[firstClicked.year][firstClicked.month] = [firstClicked.date];
                    selected = addChosenDates(firstClicked, secondClicked, selected);
                } else { // reset clicks
                    selected = {};
                    firstClicked = [];
                    secondClicked = [];
                    firstClick = false;
                    secondClick = false;
                    bothCals.find(".calendar_content").find("div").each(function () {
                        $(this).removeClass("selected");
                    });
                }
            }
            if (!firstClick) {
                firstClick = true;
                firstClicked = getClickedInfo(clicked, calendar);
                selected[firstClicked.year] = {};
                selected[firstClicked.year][firstClicked.month] = [firstClicked.date];
            } else {
                secondClick = true;
                secondClicked = getClickedInfo(clicked, calendar);

                // what if second clicked date is before the first clicked?
                var firstClickDateObj = new Date(firstClicked.year,
                    firstClicked.month,
                    firstClicked.date);
                var secondClickDateObj = new Date(secondClicked.year,
                    secondClicked.month,
                    secondClicked.date);

                if (firstClickDateObj > secondClickDateObj) {

                    var cachedClickedInfo = secondClicked;
                    secondClicked = firstClicked;
                    firstClicked = cachedClickedInfo;
                    selected = {};
                    selected[firstClicked.year] = {};
                    selected[firstClicked.year][firstClicked.month] = [firstClicked.date];

                } else if (firstClickDateObj.getTime() == secondClickDateObj.getTime()) {
                    selected = {};
                    firstClicked = [];
                    secondClicked = [];
                    firstClick = false;
                    secondClick = false;
                    $(this).removeClass("selected");
                }


                // add between dates to [selected]
                selected = addChosenDates(firstClicked, secondClicked, selected);
            }
            selectDates(selected);
        });

    }

    function selectDates(selected) {
        if (!$.isEmptyObject(selected)) {
            var dateElements1 = datesBody1.find('div');
            var dateElements2 = datesBody2.find('div');

            function highlightDates(passed_year, passed_month, dateElements) {
                if (passed_year in selected && passed_month in selected[passed_year]) {
                    var daysToCompare = selected[passed_year][passed_month];
                    for (var d in daysToCompare) {
                        dateElements.each(function (index) {
                            if (parseInt($(this).text()) == daysToCompare[d]) {
                                $(this).addClass('selected');
                            }
                        });
                    }

                }
            }

            highlightDates(year, month, dateElements1);
            highlightDates(nextYear, nextMonth, dateElements2);
        }
    }

    function makeMonthArray(passed_month, passed_year) { // creates Array specifying dates and weekdays
        var e = [];
        for (var r = 1; r < getDaysInMonth(passed_year, passed_month) + 1; r++) {
            e.push({
                day: r,
                // Later refactor -- weekday needed only for first week
                weekday: daysArray[getWeekdayNum(passed_year, passed_month, r)]
            });
        }
        return e;
    }

    function makeWeek(week) {
        week.empty();
        for (var e = 0; e < 7; e++) {
            week.append("<div>" + daysArray[e].substring(0, 3) + "</div>")
        }
    }

    function getDaysInMonth(currentYear, currentMon) {
        return (new Date(currentYear, currentMon + 1, 0)).getDate();
    }

    function getWeekdayNum(e, t, n) {
        return (new Date(e, t, n)).getDay();
    }

    function checkToday(e) {
        var todayDate = today.getFullYear() + '/' + (today.getMonth() + 1) + '/' + today.getDate();
        var checkingDate = e.getFullYear() + '/' + (e.getMonth() + 1) + '/' + e.getDate();
        return todayDate == checkingDate;

    }

    function getAdjacentMonth(curr_month, curr_year, direction) {
        var theNextMonth;
        var theNextYear;
        if (direction == "next") {
            theNextMonth = (curr_month + 1) % 12;
            theNextYear = (curr_month == 11) ? curr_year + 1 : curr_year;
        } else {
            theNextMonth = (curr_month == 0) ? 11 : curr_month - 1;
            theNextYear = (curr_month == 0) ? curr_year - 1 : curr_year;
        }
        return [theNextMonth, theNextYear];
    }

    function b() {
        today = new Date;
        year = today.getFullYear();
        month = today.getMonth();
        var nextDates = getAdjacentMonth(month, year, "next");
        nextMonth = nextDates[0];
        nextYear = nextDates[1];
    }

    var e = 480;

    var today;
    var year,
        month,
        nextMonth,
        nextYear;

    var r = [];
    var i = [
        "JANUARY",
        "FEBRUARY",
        "MARCH",
        "APRIL",
        "MAY",
        "JUNE",
        "JULY",
        "AUGUST",
        "SEPTEMBER",
        "OCTOBER",
        "NOVEMBER",
        "DECEMBER"];
    var daysArray = [
        "Sunday",
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday"];

    var cal1 = $("#calendar_first");
    var calHeader1 = cal1.find(".calendar_header");
    var weekline1 = cal1.find(".calendar_weekdays");
    var datesBody1 = cal1.find(".calendar_content");

    var cal2 = $("#calendar_second");
    var calHeader2 = cal2.find(".calendar_header");
    var weekline2 = cal2.find(".calendar_weekdays");
    var datesBody2 = cal2.find(".calendar_content");

    var bothCals = $(".calendar");

    var switchButton = bothCals.find(".calendar_header").find('.switch-month');

    var calendars = {
        "cal1": {
            "name": "first",
            "calHeader": calHeader1,
            "weekline": weekline1,
            "datesBody": datesBody1
        },
        "cal2": {
            "name": "second",
            "calHeader": calHeader2,
            "weekline": weekline2,
            "datesBody": datesBody2
        }
    }


    var clickedElement;
    var firstClicked,
        secondClicked,
        thirdClicked;
    var firstClick = false;
    var secondClick = false;
    var selected = {};

    b();
    c(month, year, 0);
    c(nextMonth, nextYear, 1);
    switchButton.on("click", function () {
        var clicked = $(this);
        var generateCalendars = function (e) {
            var nextDatesFirst = getAdjacentMonth(month, year, e);
            var nextDatesSecond = getAdjacentMonth(nextMonth, nextYear, e);
            month = nextDatesFirst[0];
            year = nextDatesFirst[1];
            nextMonth = nextDatesSecond[0];
            nextYear = nextDatesSecond[1];

            c(month, year, 0);
            c(nextMonth, nextYear, 1);
        };
        if (clicked.attr("class").indexOf("left") != -1) {
            generateCalendars("previous");
        } else {
            generateCalendars("next");
        }
        clickedElement = bothCals.find(".calendar_content").find("div");
    });


    //  Click picking stuff
    function getClickedInfo(element, calendar) {
        var clickedInfo = {};
        var clickedCalendar,
            clickedMonth,
            clickedYear;
        clickedCalendar = calendar.name;
        clickedMonth = clickedCalendar == "first" ? month : nextMonth;
        clickedYear = clickedCalendar == "first" ? year : nextYear;
        clickedInfo = {
            "calNum": clickedCalendar,
            "date": parseInt(element.text()),
            "month": clickedMonth,
            "year": clickedYear
        }
        return clickedInfo;
    }


    // Finding between dates MADNESS. Needs refactoring and smartening up :)
    function addChosenDates(firstClicked, secondClicked, selected) {
        if (secondClicked.date > firstClicked.date || secondClicked.month > firstClicked.month || secondClicked.year > firstClicked.year) {

            var added_year = secondClicked.year;
            var added_month = secondClicked.month;
            var added_date = secondClicked.date;

            if (added_year > firstClicked.year) {
                // first add all dates from all months of Second-Clicked-Year
                selected[added_year] = {};
                selected[added_year][added_month] = [];
                for (var i = 1;
                    i <= secondClicked.date;
                    i++) {
                    selected[added_year][added_month].push(i);
                }

                added_month = added_month - 1;
                while (added_month >= 0) {
                    selected[added_year][added_month] = [];
                    for (var i = 1;
                        i <= getDaysInMonth(added_year, added_month);
                        i++) {
                        selected[added_year][added_month].push(i);
                    }
                    added_month = added_month - 1;
                }

                added_year = added_year - 1;
                added_month = 11; // reset month to Dec because we decreased year
                added_date = getDaysInMonth(added_year, added_month); // reset date as well

                // Now add all dates from all months of inbetween years
                while (added_year > firstClicked.year) {
                    selected[added_year] = {};
                    for (var i = 0; i < 12; i++) {
                        selected[added_year][i] = [];
                        for (var d = 1; d <= getDaysInMonth(added_year, i); d++) {
                            selected[added_year][i].push(d);
                        }
                    }
                    added_year = added_year - 1;
                }
            }

            if (added_month > firstClicked.month) {
                if (firstClicked.year == secondClicked.year) {
                    selected[added_year][added_month] = [];
                    for (var i = 1;
                        i <= secondClicked.date;
                        i++) {
                        selected[added_year][added_month].push(i);
                    }
                    added_month = added_month - 1;
                }
                while (added_month > firstClicked.month) {
                    selected[added_year][added_month] = [];
                    for (var i = 1;
                        i <= getDaysInMonth(added_year, added_month);
                        i++) {
                        selected[added_year][added_month].push(i);
                    }
                    added_month = added_month - 1;
                }
                added_date = getDaysInMonth(added_year, added_month);
            }

            for (var i = firstClicked.date + 1;
                i <= added_date;
                i++) {
                selected[added_year][added_month].push(i);
            }
        }
        return selected;
    }
});








// 選擇日歷顯示事件
$(document).ready(function () {
    var selectedStartDate = null;
    var selectedEndDate = null;

    $('#toggle3').on('click', function (e) {
        e.stopPropagation();
        $('.my-element .calendar').toggle();
    });


    $(".my-element .calendar .calendar_content").on("click", 'div', function () {
        var clicked = $(this);


        var selectedYear = parseInt(clicked.closest(".calendar").find(".calendar_header h2").text().split(" ")[1]);
        var selectedMonth = clicked.closest(".calendar").find(".calendar_header h2").text().split(" ")[0];
        var selectedDay = parseInt(clicked.text());


        var monthMapping = {
            "JANUARY": 0,
            "FEBRUARY": 1,
            "MARCH": 2,
            "APRIL": 3,
            "MAY": 4,
            "JUNE": 5,
            "JULY": 6,
            "AUGUST": 7,
            "SEPTEMBER": 8,
            "OCTOBER": 9,
            "NOVEMBER": 10,
            "DECEMBER": 11
        };

        selectedMonth = monthMapping[selectedMonth];


        var today = new Date();
        today.setHours(0, 0, 0, 0);


        var selectedDate = new Date(selectedYear, selectedMonth, selectedDay);


        if (selectedDate < today) {
            selectedStartDate = null;
            selectedEndDate = null;
        } else {
            if (selectedStartDate === null) {
                selectedStartDate = selectedDate;
                selectedEndDate = selectedStartDate;
            } else if (selectedStartDate.getTime() === selectedEndDate.getTime()) {
                selectedEndDate = selectedDate;
            } else {
                selectedStartDate = selectedDate;
                selectedEndDate = selectedStartDate;
            }
        }


        if (selectedStartDate === null) {
            $("#toggle3").next().text("");
        } else if (selectedStartDate.getTime() === selectedEndDate.getTime()) {
            $("#toggle3").next().text(formatDate(selectedStartDate));
        } else {
            $("#toggle3").next().text(formatDate(selectedStartDate) + " - " + formatDate(selectedEndDate));
        }
    });


    $(".my-element .calendar").on("click", function (e) {
        e.stopPropagation();
    });


    $(document).on('click', function () {
        $('.my-element .calendar').hide();
    });


    $(".my-element .calendar .calendar_content").on("click", function (e) {
        e.stopPropagation();
    });


    function formatDate(date) {
        if (date) {
            var month = (date.getMonth() + 1).toString().padStart(2, '0');
            var day = date.getDate().toString().padStart(2, '0');
            return month + '/' + day;
        }
        return "";
    }
});