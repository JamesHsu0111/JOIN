$(document).ready(function () {
    $('#chatBtn').on('click', function () {
        $("#dataTable").empty();
        console.log("OK");
        $.ajax({
            url: '/api/getChatData', // 替换为实际的控制器和路由
            method: 'GET',
            success: function (data) {
                var count = 0;
                var dataHTML = `<thead>
                                    <tr>
                                        <th scope="col">#</th>
                                        <th scope="col">活動ID</th>
                                        <th scope="col">使用者ID</th>
                                        <th scope="col">留言</th>
                                    </tr>
                                </thead>
                                <tbody>`
                data.forEach(function (data) {
                    var id = data.ActivityID;
                    count++;
                    var dataInfo = `<tr>
                                    <th scope="row">${count}</th>
                                        <td><a href="/groupPage/groupPage/${id}">${id}</a></td>
                                        <td>${data.UserID}</td>
                                        <td>${data.ChatContent}</td>
                                    </tr>`;
                    dataHTML = dataHTML + dataInfo;
                });
                dataHTML = dataHTML + `</tbody>`;
                $("#dataTable").append(dataHTML); // 使用正确的jQuery选择器
            },
            error: function (error) {
                // 处理请求错误
            }
        });
    });
});


$(document).ready(function () {
    $('#groupBtn').on('click', function () {
        $("#dataTable").empty();
        console.log("OK");
        $.ajax({
            url: '/api/getGroupData', // 替换为实际的控制器和路由
            method: 'GET',
            success: function (data) {
                var count = 0;
                var dataHTML = `<thead>
                                    <tr>
                                        <th scope="col">#</th>
                                        <th scope="col">活動編號</th>
                                        <th scope="col">活動名稱</th>
                                        <th scope="col">活動分類</th>
                                    </tr>
                                </thead>
                                <tbody>`
                data.forEach(function (data) {
                    var id = data.GroupID;
                    count++;
                    var dataInfo = `<tr>
                                    <th scope="row">${count}</th>
                                        <td><a href="/groupPage/groupPage/${id}">${id}</a></td>
                                        <td>${data.GroupName}</td>
                                        <td>${data.GroupCategory}</td>
                                    </tr>`;
                    dataHTML = dataHTML + dataInfo;
                });
                dataHTML = dataHTML + `</tbody>`;
                $("#dataTable").append(dataHTML); // 使用正确的jQuery选择器
            },
            error: function (error) {
                // 处理请求错误
            }
        });
    });
});

$(document).ready(function () {
    $('#activityBtn').on('click', function () {
        $("#dataTable").empty();
        console.log("OK");
        $.ajax({
            url: '/api/getActivityData', // 替换为实际的控制器和路由
            method: 'GET',
            success: function (data) {
                var count = 0;
                var dataHTML = `<thead>
                                    <tr>
                                        <th scope="col">#</th>
                                        <th scope="col">活動編號</th>
                                        <th scope="col">活動名稱</th>
                                        <th scope="col">活動分類</th>
                                    </tr>
                                </thead>
                                <tbody>`
                data.forEach(function (data) {
                    var id = data.ActivityID;
                    count++;
                    var dataInfo = `<tr>
                                    <th scope="row">${count}</th>
                                        <td><a href="/Activity/Index/${id}">${id}</a></td>
                                        <td>${data.ActivityName}</td>
                                        <td>${data.Category}</td>
                                    </tr>`;
                    dataHTML = dataHTML + dataInfo;
                });
                dataHTML = dataHTML + `</tbody>`;
                $("#dataTable").append(dataHTML); // 使用正确的jQuery选择器
            },
            error: function (error) {
                // 处理请求错误
            }
        });
    });
});
   