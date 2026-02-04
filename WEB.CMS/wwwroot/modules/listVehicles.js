$(document).ready(function () {
    _listVehicles.init();
    var input_chua_xu_ly = document.getElementById("input_chua_xu_ly");
    input_chua_xu_ly.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _listVehicles.ListCartoFactory();
        }
    });
    input_chua_xu_ly.addEventListener("keyup", function (event) {
        _listVehicles.ListCartoFactory();
    });
    var input_da_xu_ly = document.getElementById("input_da_xu_ly");
    input_da_xu_ly.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            _listVehicles.ListCartoFactory_Da_SL();
        }
    });
    input_da_xu_ly.addEventListener("keyup", function (event) {
        _listVehicles.ListCartoFactory_Da_SL();
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

        // Tính toán vị trí

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


                $currentBtn
                    .text(text)
                    .removeClass(function (_, old) {
                        return (old.match(/(^|\s)status-\S+/g) || []).join(' ');
                    }) // xoá các class status- cũ
                    .addClass(cls); // gắn class mới (status-arrived, status-blank…)

                _listVehicles.UpdateStatus(id_row, val_TT, 7);
                if (val_TT == 0) {
                    $('#dataBody-0').find('.CartoFactory_' + id_row).remove();

                } else {
                    $('#dataBody-1').find('.CartoFactory_' + id_row).remove();
                    $('#dataBody-0').find('.CartoFactory_' + id_row).remove();
                }
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
        { Description: "Blank", CodeValue: "2" },
       
        { Description: "Đã cân ra", CodeValue: "0" },
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
        var date = new Date(item.vehicleWeighingTimeComplete);
        let formatted =
            String(date.getHours()).padStart(2, '0') + ":" +
            String(date.getMinutes()).padStart(2, '0') + " " +
            String(date.getDate()).padStart(2, '0') + "/" +
            String(date.getMonth() + 1).padStart(2, '0') + "/" +
            date.getFullYear();
        var date2 = new Date(item.vehicleTroughTimeComeOut);
        let formatted2 =
            String(date2.getHours()).padStart(2, '0') + ":" +
            String(date2.getMinutes()).padStart(2, '0') + " " +
            String(date2.getDate()).padStart(2, '0') + "/" +
            String(date2.getMonth() + 1).padStart(2, '0') + "/" +
            date2.getFullYear()  ;
        return `
        <tr class="CartoFactory_${item.id}" data-queue="${formatted2}" >
            <td>${item.recordNumber}</td>
            <td>${item.vehicleNumber}</td>
            <td>${item.customerName}</td>
            <td>${item.driverName}</td>    
            <td>${formatted}</td>
            <td>
                <div class="status-dropdown">
                    <button class="dropdown-toggle " data-options='${jsonString}'>
                        ${item.vehicleWeighingStatusName}
                    </button>
                </div>

            </td>

        </tr>`;
    }
    function renderRow2(item) {
        var date = new Date(item.vehicleTroughTimeComeOut);
        let formatted =
            String(date.getHours()).padStart(2, '0') + ":" +
            String(date.getMinutes()).padStart(2, '0') + " " +
            String(date.getDate()).padStart(2, '0') + "/" +
            String(date.getMonth() + 1).padStart(2, '0') + "/" +
            date.getFullYear();
        return `
        <tr class="CartoFactory_${item.id}" data-queue="${formatted}" >
            <td>${item.recordNumber}</td>
            <td>${item.vehicleNumber}</td>
            <td>${item.customerName}</td>
            <td>${item.driverName}</td>
            <td></td>
            <td>
                <div class="status-dropdown">
                    <button class="dropdown-toggle " data-options='${jsonString}'>
                        ${item.vehicleWeighingStatusName}
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
    function sortTable() {
        const tbody = document.getElementById("dataBody-0");
        const rows = Array.from(tbody.querySelectorAll("tr"));
        rows.sort((a, b) => {
            const timeA = parseDateTime(a.dataset.queue);
            const timeB = parseDateTime(b.dataset.queue);
            return timeA - timeB; // tăng dần
        });

        tbody.innerHTML = "";
        rows.forEach(r => tbody.appendChild(r));
    }
 
    // Nhận data mới từ server
    connection.off("ListVehicles_Da_SL");
    connection.on("ListVehicles_Da_SL", function (item) {
        $('.CartoFactory_' + item.id).remove();
        const tbody = document.getElementById("dataBody-1");
        tbody.insertAdjacentHTML("beforeend", renderRow(item));
        sortTable_Da_SL(); // sắp xếp lại ngay khi thêm
    });
    connection.off("ListVehicles");
    connection.on("ListVehicles", function (item) {
        $('.CartoFactory_' + item.id).remove();
        const tbody = document.getElementById("dataBody-0");
        tbody.insertAdjacentHTML("beforeend", renderRow(item));
        sortTable(); // sắp xếp lại ngay khi thêm
    });
    // Nhận data mới từ gọi xe cân đầu vào
    connection.off("ListCarCall_Da_SL");
    connection.on("ListCarCall_Da_SL", function (item) {
        $('.CartoFactory_' + item.id).remove();
        const tbody = document.getElementById("dataBody-0");
        tbody.insertAdjacentHTML("beforeend", renderRow2(item));
        sortTable();
    });
    connection.off("ListCarCall");
    connection.on("ListCarCall", function (item) {
        $('#dataBody-0').find('.CartoFactory_' + item.id).remove();

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
var _listVehicles = {
    init: function () {
        _listVehicles.ListCartoFactory();
        _listVehicles.ListCartoFactory_Da_SL();
    },
    ListCartoFactory: function () {
        var model = {
            VehicleNumber: $('#input_chua_xu_ly').val(),
            PhoneNumber: $('#input_chua_xu_ly').val(), 
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: 0,
            VehicleTroughStatus: 0,
            TroughType: null,
            VehicleWeighingStatus: null,
            LoadingStatus: 0,
            VehicleWeighedstatus: 0,
            type:0,
        }
        $.ajax({
            url: "/ListCar/ListVehiclesisLoading",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#data_chua_xu_ly').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    ListCartoFactory_Da_SL: function () {
        var model = {
            VehicleNumber: $('#input_da_xu_ly').val(),
            PhoneNumber: $('#input_da_xu_ly').val(),
            VehicleStatus: 0,
            LoadType: null,
            VehicleWeighingType: 0,
            VehicleTroughStatus: 0,
            TroughType: null,
            VehicleWeighingStatus: 0,
            LoadingStatus: 0,
            VehicleWeighedstatus: 0,
            type: 1,
           
        }
        $.ajax({
            url: "/ListCar/ListVehiclesisLoading",
            type: "post",
            data: { SearchModel: model },
            success: function (result) {
                $('#imgLoading').hide();
                $('#data_da_xu_ly').html(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    UpdateStatus: function (id, status, type) {
        $.ajax({
            url: "/Car/UpdateStatus",
            type: "post",
            data: { id: id, status: status, type: type },
            success: function (result) {
                if (result.status == 0) {
                    _msgalert.success(result.msg)

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