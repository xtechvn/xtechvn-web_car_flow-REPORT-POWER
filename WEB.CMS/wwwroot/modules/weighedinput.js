$(document).ready(function () {
    _Weighed_Input.init();
    var input_Weighed_Input_Chua_SL = document.getElementById("input_Weighed_Input_Chua_SL");
    input_Weighed_Input_Chua_SL.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _Weighed_Input.ListWeighedInput();
        }
    });
    input_Weighed_Input_Chua_SL.addEventListener("keyup", function (event) {
        _Weighed_Input.ListWeighedInput();
    });
    var input_Weighed_Input_Da_SL = document.getElementById("input_Weighed_Input_Da_SL");
    input_Weighed_Input_Da_SL.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _Weighed_Input.ListWeighedInput_Da_SL();
        }
    });
    input_Weighed_Input_Da_SL.addEventListener("keyup", function (event) {
        _Weighed_Input.ListWeighedInput_Da_SL();
    });
    const container = $('<div id="dropdown-container"></div>').appendTo('body');
    let $menu = null;
    let $currentBtn = null;

    $(document).on('click', '.status-dropdown .dropdown-toggle', function (e) {
        e.stopPropagation();
        const $btn = $(this);
        const optsData = $btn.data('options'); // mảng [{text, class}]
        const options = Array.isArray(optsData) ? optsData : JSON.parse(optsData);
        const currentText = $.trim($btn.text());

        // Đóng menu cũ (nếu có)
        if ($menu) {
            $menu.remove();
            $menu = null;
        }

        $currentBtn = $btn;

        // Tạo menu + danh sách li
        $menu = $('<div class="dropdown-menu"><ul></ul></div>');
        const $ul = $menu.find('ul');

        options.forEach(opt => {
            $('<li>')
                .text(opt.text)
                .addClass('status-option')
                .attr('data-value', opt.value) // Corrected from opt.valuse
                .toggleClass('active', opt.text === currentText)
                .appendTo($ul);
        });

        const $actions = $('<div class="actions"></div>');
        $('<button class="cancel">Bỏ qua</button>').appendTo($actions);
        $('<button class="confirm">Xác nhận</button>').appendTo($actions);
        $menu.append($actions);
        container.append($menu);


        // --- 🔧 Tính toán vị trí dropdown (dùng viewport coords) ---
        const rect = $btn[0].getBoundingClientRect(); // viewport coordinates
        const btnHeight = rect.height;
        const winWidth = $(window).width();
        const winHeight = $(window).height();
        const paddingScreen = 15; // chừa khoảng 15px mỗi bên
        $menu.css({
            position: 'absolute',
            left: 0,
            top: 0,
            display: 'block',
            visibility: 'hidden'
        });

        const menuWidth = $menu.outerWidth();
        const menuHeight = $menu.outerHeight();

        // Vị trí mặc định: bên dưới button (viewport coords)
        let left = rect.left;
        let top = rect.top + btnHeight;

        // Nếu dropdown tràn phải -> dịch sang trái
        if (left + menuWidth + paddingScreen > winWidth) {
            left = winWidth - menuWidth - paddingScreen;
        }

        // Nếu tràn trái -> giữ cách paddingScreen
        if (left < paddingScreen) {
            left = paddingScreen;
        }

        // Nếu tràn dưới -> bật drop-up (hiển thị phía trên button)
        if (top + menuHeight > winHeight) {
            top = rect.top - menuHeight;
            $menu.addClass('drop-up');
        } else {
            $menu.removeClass('drop-up');
        }

        // Áp vị trí cuối cùng và hiển thị menu
        $menu.css({
            left: left,
            top: top,
            visibility: 'visible' // hiện lên
        });
    });

    // Click chọn item
    $(document).on('click', '#dropdown-container .dropdown-menu li', function (e) {
        e.stopPropagation();
        $('#dropdown-container .dropdown-menu li').removeClass('active');
        $(this).addClass('active');
    });

    // Bỏ qua
    $(document).on('click', '#dropdown-container .actions .cancel', function (e) {
        e.stopPropagation();
        closeMenu();
    });

    // ✅ Xác nhận – đổi text + class cho button
    $(document).on('click', '#dropdown-container .actions .confirm', function (e) {
        e.stopPropagation();
        if ($menu && $currentBtn) {
            const $active = $menu.find('li.active');
            if ($active.length) {
                const text = $active.text();
                const val_TT = $active.attr('data-value');
                const $row = $currentBtn.closest('tr');
                let id_row = 0;
                if ($row.length) {
                    const classAttr = $row.attr('class');
                    const match = classAttr.match(/CartoFactory_(\d+)/);
                    if (match && match[1]) {
                        id_row = match[1];
                    }
                }

                const cls = $active.attr('class').split(/\s+/)
                    .filter(c => c !== 'active')[0] || '';
                const $btn = $currentBtn; // copy lại trước khi gọi Ajax
                $.ajax({
                    url: "/Car/UpdateStatus",
                    type: "post",
                    data: { id: id_row, status: val_TT, type: 9 },
                    success: function (result) {
                        status_type = result.status;
                        if (result.status == 0) {
                            _msgalert.success(result.msg)
                            $btn
                                .text(text)
                                .removeClass(function (_, old) {
                                    return (old.match(/(^|\s)status-\S+/g) || []).join(' ');
                                }) // xoá các class status- cũ
                                .addClass(cls); // gắn class mới (status-arrived, status-blank…)\
                            if (val_TT == 1) {
                                $('#dataBody-1').find('.CartoFactory_' + id_row).remove();
                               

                            } else {
                                $('#dataBody-0').find('.CartoFactory_' + id_row).remove();
                            }
                        } else {
                            _msgalert.error(result.msg)
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log("Status: " + textStatus);
                    }
                });

            }
        }
        closeMenu();
    });

    // Đóng menu khi click ra ngoài
    $(document).on('click', function () {
        closeMenu();
    });

    function closeMenu() {
        if ($menu) {
            $menu.remove();
            $menu = null;
            $currentBtn = null;
        }
    }
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/CarHub", { transport: signalR.HttpTransportType.WebSockets, skipNegotiation: true })
        .withAutomaticReconnect([ 2000, 5000, 10000])
        .build();
    connection.start()
        .then(() => console.log("✅ SignalR connected"))
        .catch(err => console.error(err));
    const AllCode = [
        { Description: "Blank", CodeValue: "1" },
        { Description: "Đã cân xong đầu vào", CodeValue: "0" },
        // Add more objects as needed
    ];
    // Create a new array of objects in the desired format
    const options = AllCode.map(allcode => ({
        text: allcode.Description,
        value: allcode.CodeValue
    }));
    const jsonString = JSON.stringify(options);
    // Hàm render row
    function renderRow(item) {
        var date = new Date(item.registerDateOnline);
        let formatted =
            String(date.getHours()).padStart(2, '0') + ":" +
            String(date.getMinutes()).padStart(2, '0') + " " +
            String(date.getDate()).padStart(2, '0') + "/" +
            String(date.getMonth() + 1).padStart(2, '0') + "/" +
            date.getFullYear();
        var date2 = new Date(item.vehicleWeighingTimeComeIn);
        let formatted2 =
            String(date2.getHours()).padStart(2, '0') + ":" +
            String(date2.getMinutes()).padStart(2, '0') + " " +
            String(date2.getDate()).padStart(2, '0') + "/" +
            String(date2.getMonth() + 1).padStart(2, '0') + "/" +
            date2.getFullYear();
        return `
        <tr class="CartoFactory_${item.id}" data-queue="${formatted2}" data-LoadType="${item.loadType}" >
            <td>${item.recordNumber}</td>
            <td>${formatted}</td>
            <td>${item.vehicleNumber}</td>
            <td>${item.customerName}</td>
            <td>${item.driverName}</td>
            <td>${item.phoneNumber}</td>
            <td>${item.loadTypeName}</td>
            <td>
                <div class="status-dropdown">
                    <button class="dropdown-toggle " data-options='${jsonString}'>
                        ${item.vehicleWeighedstatusName}
                    </button>
                </div>

            </td>

        </tr>`;
    }

    // Hàm sắp xếp lại tbody theo QueueNumber tăng dần
    function sortTable_Da_SL() {
        const tbody = document.getElementById("dataBody-1");
        const rows = Array.from(tbody.querySelectorAll("tr"));

        rows.sort((a, b) => {
            const timeA = parseDateTime(a.dataset.queue);
            const timeB = parseDateTime(b.dataset.queue);
            return timeA - timeB; // tăng dần
        });

        tbody.innerHTML = "";
        rows.forEach(r => tbody.appendChild(r));
    }
    function sortTable_Da_SL2() {
        const tbody = document.getElementById("dataBody-1");
        const rows = Array.from(tbody.querySelectorAll("tr"));

        rows.sort((a, b) => {
            const timeA = parseDateTime(a.dataset.queue);
            const timeB = parseDateTime(b.dataset.queue);
            return timeA - timeB; // tăng dần
        });

        tbody.innerHTML = "";
        rows.forEach(r => tbody.appendChild(r));
    }
    function sortTable() {
        const tbody = document.getElementById("dataBody-0");
        const rows = Array.from(tbody.querySelectorAll("tr"));

        rows.sort((a, b) => {
            const qa = parseInt(a.getAttribute("data-queue") || 0);
            const qb = parseInt(b.getAttribute("data-queue") || 0);
            return qa - qb;
        });

        tbody.innerHTML = "";
        rows.forEach(r => tbody.appendChild(r));
    }
    function sortTable2() {
        const tbody = document.getElementById("dataBody-0");
        const rows = Array.from(tbody.querySelectorAll("tr"));

        rows.sort((a, b) => {
            const qa = parseInt(a.getAttribute("data-LoadType") || 0);
            const qb = parseInt(b.getAttribute("data-LoadType") || 0);
            return qa - qb;
        });

        tbody.innerHTML = "";
        rows.forEach(r => tbody.appendChild(r));
    }

    // Nhận data mới từ server
    connection.off("ListWeighedInput_Da_SL");
    connection.on("ListWeighedInput_Da_SL", function (item) {
        $('.CartoFactory_' + item.id).remove();
        const tbody = document.getElementById("dataBody-1");
        tbody.insertAdjacentHTML("beforeend", renderRow(item));
        sortTable_Da_SL(); // sắp xếp lại ngay khi thêm
        sortTable_Da_SL2(); // sắp xếp lại ngay khi thêm
    });
    connection.off("ListWeighedInput");
    connection.on("ListWeighedInput", function (item) {
        $('.CartoFactory_' + item.id).remove();
        const tbody = document.getElementById("dataBody-0");
        tbody.insertAdjacentHTML("beforeend", renderRow(item));
        sortTable(); // sắp xếp lại ngay khi thêm
        sortTable2(); // sắp xếp lại ngay khi thêm
    });
    // Nhận data mới từ gọi xe cân đầu vào
    connection.off("ListCallTheScale_Da_SL");
    connection.on("ListCallTheScale_Da_SL", function (item) {
        const tbody = document.getElementById("dataBody-0");
        tbody.insertAdjacentHTML("beforeend", renderRow(item));
        sortTable();
    });
    connection.off("ListCallTheScale_0");
    connection.on("ListCallTheScale_0", function (item) {
        $('.CartoFactory_' + item.id).remove();

    });
    connection.off("ListCallTheScale_1");
    connection.on("ListCallTheScale_1", function (item) {
        $('.CartoFactory_' + item.id).remove();

    });
    connection.off("ListCarCall_Da_SL");
    connection.on("ListCarCall_Da_SL", function (item) {
        $('.CartoFactory_' + item.id).remove();

    });
    connection.off("ListCarCall");
    connection.on("ListCarCall", function (item) {
        const tbody = document.getElementById("dataBody-1");
        tbody.insertAdjacentHTML("beforeend", renderRow(item));
        sortTable_Da_SL(); // sắp xếp lại ngay khi thêm
        sortTable_Da_SL2(); // sắp xếp lại ngay khi thêm

    });
    connection.onreconnecting(error => {
        console.warn("🔄 Đang reconnect...", error);
    });

    connection.onreconnected(connectionId => {
        console.log("✅ Đã reconnect. Connection ID:", connectionId);
    });

    connection.onclose(error => {
        console.error("❌ Kết nối bị đóng.", error);
    });
    function parseDateTime(str) {
        // "11:33 17/12/2025"
        const [time, date] = str.split(" ");
        const [hour, minute] = time.split(":").map(Number);
        const [day, month, year] = date.split("/").map(Number);

        return new Date(year, month - 1, day, hour, minute).getTime();
    }
});
var _Weighed_Input = {
    init: function () {
        _Weighed_Input.ListWeighedInput();
        _Weighed_Input.ListWeighedInput_Da_SL();
    },
    ListWeighedInput: function () {
        var model = {
            VehicleNumber: $('#input_Weighed_Input_Chua_SL').val() != undefined && $('#input_Weighed_Input_Chua_SL').val() != "" ? $('#input_Weighed_Input_Chua_SL').val().trim() : "",
            PhoneNumber: $('#input_Weighed_Input_Chua_SL').val() != undefined && $('#input_Weighed_Input_Chua_SL').val() != "" ? $('#input_Weighed_Input_Chua_SL').val().trim() : "", 
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: 0,
            VehicleTroughStatus: null,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: 0,
            VehicleWeighedstatus: null,
            type: 0,
        }
        $.ajax({
            url: "/Car/ListWeighedInput",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#Weighed_Input_Chua_SL').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    ListWeighedInput_Da_SL: function () {
        var model = {
            VehicleNumber: $('#input_Weighed_Input_Da_SL').val() != undefined && $('#input_Weighed_Input_Da_SL').val() != "" ? $('#input_Weighed_Input_Da_SL').val().trim() : "",
            PhoneNumber: $('#input_Weighed_Input_Da_SL').val() != undefined && $('#input_Weighed_Input_Da_SL').val() != "" ? $('#input_Weighed_Input_Da_SL').val().trim() : "",
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: 0,
            VehicleTroughStatus: null,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: 0,
            VehicleWeighedstatus: 0,
            type: 1,
        }
        $.ajax({
            url: "/Car/ListWeighedInput",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#Weighed_Input_Da_SL').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    UpdateStatus: function (id, status, type) {
        var status_type = 0
        $.ajax({
            url: "/Car/UpdateStatus",
            type: "post",
            data: { id: id, status: status, type: type },
            success: function (result) {
                status_type = result.status;
                if (result.status == 0) {
                    _msgalert.success(result.msg)
                    $.magnificPopup.close();
                } else {
                    _msgalert.error(result.msg)
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
        return status_type;
    },
}