
$(document).ready(function () {
    // 假設你已經取得了currentUserId
    let currentUserId = $('#currentUserId').val();

    $.get('/Member/GetUserPhoto', { userId: currentUserId }, function (data) {
        if (data.imageUrl) {
            console.log(currentUserId);
            $('#item-img-output').attr('src', data.imageUrl);
            console.log(currentUserId);
        }
    });

    //$.get('/Member/GetMemberInfo', { userId: currentUserId }, function (data) {
    //    if (data.Nickname) {
    //        $('#nickname').val(data.Nickname);
    //    }
    //    if (data.Intro) {
    //        $('#introduceyrself').val(data.Intro);
    //    }
    //});






// Start upload preview image

$(".gambar").attr("src", "https://user.gadjian.com/static/images/personnel_boy.png");
var $uploadCrop,
    tempFilename,
    rawImg,
    imageId;
function readFile(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('.upload-demo').addClass('ready');
            $('#cropImagePop').modal('show');
            rawImg = e.target.result;
        }
        reader.readAsDataURL(input.files[0]);
    }
    else {
        swal("Sorry - you're browser doesn't support the FileReader API");
    }
}

$uploadCrop = $('#upload-demo').croppie({
    viewport: {
        width: 300,
        height: 300,
    },
    enforceBoundary: false,
    enableExif: true
});
$('#cropImagePop').on('shown.bs.modal', function () {
    // alert('Shown pop');
    $uploadCrop.croppie('bind', {
        url: rawImg
    }).then(function () {
        console.log('jQuery bind complete');
    });
});

$('.item-img').on('change', function () {
    imageId = $(this).data('id'); tempFilename = $(this).val();
    $('#cancelCropBtn').data('id', imageId); readFile(this);
});
$('#cropImageBtn').on('click', function (ev) {
    $uploadCrop.croppie('result', {
        type: 'base64',
        format: 'jpeg',
        size: { width: 300, height: 300 }
    }).then(function (resp) {
        $('#item-img-output').attr('src', resp);
        $('#cropImagePop').modal('hide');

        // AJAX 請求傳送照片到後端
        $.ajax({
            url: '/Member/UpdateUserPhoto',
            method: 'POST',
            data: {
                userId: currentUserId, // 使用從隱藏輸入框取得的當前用戶ID
                imageBase64: resp
            },
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        title: '編輯成功',
                        icon: 'success',
                        showConfirmButton: false,
                        timer: 1000
                    });
                } else {
                    alert('Failed to update photo: ' + response.message);
                }
            }
        });
    });
});

 
    
});


