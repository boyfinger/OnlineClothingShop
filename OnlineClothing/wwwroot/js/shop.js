
    $(document).ready(function() {
        // ==================================================
        // Lọc trạng thái cửa hàng bằng AJAX
        // ==================================================
        $('#statusFilter').change(function () {
            var status = $(this).val(); // Lấy giá trị trạng thái
            var search = '@ViewBag.Search'; // Lấy giá trị tìm kiếm (nếu có)
            var page = '@ViewBag.CurrentPage'; // Lấy trang hiện tại

            // Gửi yêu cầu AJAX để lọc cửa hàng
            $.ajax({
                url: '/Admin/Shop/Manage',
                type: 'GET',
                data: {
                    status: status,
                    search: search,
                    page: page
                },
                success: function (response) {
                    // Cập nhật lại danh sách cửa hàng
                    $('#shopContainer').html($(response).find('#shopContainer').html());
                    $('.pagination').html($(response).find('.pagination').html());

                    // Hiển thị thông báo nếu không có dữ liệu
                    if ($('#shopContainer').children().length === 0) {
                        $('#noDataMessage').show();
                    } else {
                        $('#noDataMessage').hide();
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi!',
                        text: 'Đã xảy ra lỗi khi lọc dữ liệu.'
                    });
                }
            });
        });

    // ==================================================
    // Thay đổi trạng thái cửa hàng bằng AJAX
    // ==================================================
        $(document).on('click', '.btn-status', function () {
            var id = $(this).data('id'); // Lấy ID của cửa hàng
            var action = $(this).data('action'); // Lấy hành động (approve, reject, ban, unban)
            var button = $(this); // Lưu lại nút được nhấn

            // Hiển thị hộp thoại xác nhận bằng SweetAlert2
            Swal.fire({
                title: 'Bạn có chắc chắn không?',
                text: "Bạn sắp thay đổi trạng thái của cửa hàng này!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Đúng, hãy thay đổi!'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Gửi yêu cầu AJAX để thay đổi trạng thái
                    $.ajax({
                        url: '/Admin/Shop/ChangeStatus/' + id,
                        type: 'POST',
                        data: { action: action },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Thành công!',
                                    text: response.message
                                }).then(() => {
                                    // Cập nhật giao diện ngay lập tức
                                    updateUI(button, action);
                                });
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Lỗi!',
                                    text: response.message
                                });
                            }
                        },
                        error: function () {
                            Swal.fire({
                                icon: 'error',
                                title: 'Lỗi!',
                                text: 'Đã xảy ra lỗi khi thay đổi trạng thái.'
                            });
                        }
                    });
                }
            });
        });

        function updateUI(button, action) {
            var card = button.closest('.card'); 
            var statusBadge = card.find('.badge-status');
            var actionButtonsContainer = card.find('.ms-3');

            switch (action) {
                case 'approve':
                    statusBadge.text('Hoạt động').removeClass('bg-warning bg-danger').addClass('bg-success');
                    break;
                case 'reject':
                    card.remove();
                    return; 
                case 'ban':
                    statusBadge.text('Bị cấm').removeClass('bg-warning bg-success').addClass('bg-danger');
                    break;
                case 'unban':
                    statusBadge.text('Hoạt động').removeClass('bg-danger').addClass('bg-success');
                    break;
            }

            var newButtons = '';
            if (action === 'approve' || action === 'unban') {
                newButtons = `
            <button class="btn btn-danger btn-sm btn-status fw-bold" data-id="${button.data('id')}" data-action="ban">
                <i class="fas fa-ban"></i> Cấm
            </button>
        `;
            } else if (action === 'ban') {
                newButtons = `
            <button class="btn btn-warning btn-sm btn-status fw-bold" data-id="${button.data('id')}" data-action="unban">
                <i class="fas fa-unlock"></i> Bỏ cấm
            </button>
        `;
            }

            actionButtonsContainer.html(newButtons);
        }
    });
