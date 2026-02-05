
var _menu = {

    Init: function () {
        this.modal_element = $('#global_modal_popup');
        this.OnSearch();
        this.InitYearDropdown();
        // đổi năm là search luôn
        $('#search_year').on('change', () => {
            this.OnSearch();
        });


    },
    InitYearDropdown: function () {

        const currentYear = new Date().getFullYear();
        const $year = $('#search_year');

        $year.empty();

        // 10 năm gần nhất (current → -9)
        for (let y = currentYear; y >= currentYear - 9; y--) {
            $year.append(`<option value="${y}">${y}</option>`);
        }

    },


    GetSearchParam: function () {
        return {
            year: parseInt($('#search_year').val())
        };
    },


    GetFormData: function ($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};
        $.map(unindexed_array, function (n, i) {
            indexed_array[n['name']] = n['value'];
        });
        return indexed_array;
    },

    Search: function (input) {

        window.scrollTo(0, 0);

        _ajax_caller.post('/plan/search', input, function (result) {
            $('#grid_data').html(result);
        });

    },



    OnSearch: function () {
        let objSearch = this.GetSearchParam();
        this.Search(objSearch);
    },


    ReLoad: function () {
        this.Search(this.SearchParam);
    },

    OnChangeFullNameSearch: function (value) {
        var searchobj = this.SearchParam;
        searchobj.name = value;
        this.SearchParam = searchobj;
        this.Search(searchobj);
    },

    ShowAddOrUpdate: function (id, parent_id = 0) {
        let title = `${id > 0 ? "Cập nhật" : "Thêm mới"} Plan`;
        let url = '/Plan/AddOrUpdate';
        let param = { id: id, parent_id: parent_id };
        _magnific.OpenSmallPopup(title, url, param);
    },
    OnDeleteWeight: function (weightId) {
        if (!weightId || weightId <= 0) return;

        if (!confirm("Bạn chắc chắn muốn xóa dòng chi nhánh này không?")) return;

        _ajax_caller.post('/plan/delete-weight', { id: weightId }, function (res) {
            if (res && res.isSuccess) {
                alert(res.message || "Xóa thành công");
                // reload lại grid theo năm đang chọn
                _menu.OnSearch();
            } else {
                alert(res.message || "Xóa thất bại");
            }
        });
    },


    

    //OnSave: function () {
    //    let Form = $('#form_plan');


    //    if (!Form.valid()) { return; }


    //    let formData = this.GetFormData(Form);

    //    let url = "/plan/AddOrUpdateMenu";
    //    $.ajax({
    //        url: url,
    //        type: "POST",
    //        data: { model: formData },
    //        success: function (result) {
    //            if (result.isSuccess) {
    //                _msgalert.success(result.message);
    //                setTimeout(function () {
    //                    window.location.reload()
    //                }, 1000);
    //            } else {
    //                _msgalert.error(result.message);
    //            }
    //        }
    //    });

    //},
    OnSave: function () {
        $.ajax({
            url: '/Plan/AddOrUpdatePlan',
            type: 'POST',
            data: $('#form_plan').serialize(),
            success: function (res) {
                if (res.isSuccess) {
                    _msgalert.success(res.message);
                    setTimeout(function () {
                                        window.location.reload()
                                    }, 1000);
                } else {
                    _msgalert.error(res.message);
                }
            },
            error: function () {
                _msgalert.error(res.message);
            }
        });
    },

  

    OnChangeStatus: function (id, status) {
        let title = `Xác nhận ${status == 0 ? "hiển thị" : "ẩn"} menu`;
        let description = `${status == 1 ? "Khi ẩn Menu cha, các menu con cũng sẽ bị ẩn. " : ""}Bạn xác nhận muốn ${status == 0 ? "hiển thị" : "ẩn"} menu này?`;

        _msgconfirm.openDialog(title, description, function () {
            let url = "/plan/ChangeStatus";
            _ajax_caller.get(url, { Id: id, Status: status }, function (result) {
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    setTimeout(function () {
                        window.location.reload()
                    }, 1000);
                } else {
                    _msgalert.error(result.message);
                }
            });
        });
    },

}

var _changeInterval = null;

$("#ip_search_name").keyup(function (e) {
    if (e.which === 13) {
        _menu.OnChangeFullNameSearch(e.target.value);
    } else {
        clearInterval(_changeInterval);
        _changeInterval = setInterval(function () {
            _menu.OnChangeFullNameSearch(e.target.value);
            clearInterval(_changeInterval);
        }, 1000);
    }
});

$(document).ready(function () {
    $('input').attr('autocomplete', 'off');
    _menu.Init();

});

