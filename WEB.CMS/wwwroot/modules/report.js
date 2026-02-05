$(document).ready(function () {
    _report.init();
});
var _report = {
    init: function () {
        var newdate = new Date();
        newdate.setDate(01);
        var FromDateStr = newdate.toLocaleDateString("en-GB");
        var ToDateStr = new Date().toLocaleDateString("en-GB");
        _report.GetTotalVehicleInspection(FromDateStr, ToDateStr);
        _report.GetSummaryVehicleBySite(FromDateStr, ToDateStr);
    },
    GetTotalVehicleInspection: function (fromdate, todate) {
        $.ajax({
            url: '/Report/TotalVehicleInspection',
            type: 'POST',
            data: {
                fromdate: fromdate,
                todate: todate
            },
            success: function (result) {
                if (result.status == 0) {
                    var html = `<div class="summary-card">
                                    <div class="card-label">Tổng Sản lượng (Tấn)</div>
                                    <div class="card-value">
                                     ${_global_function.Comma(result.data.totalWeightTroughType)}
                                    </div>
                                </div>
                                <div class="summary-card">
                                    <div class="card-label">Tổng số Chuyến</div>
                                    <div class="card-value">
                                     ${_global_function.Comma(result.data.totalCar)}
                                    </div></div>`;
                    $('#summary-grid').html(html)

                } 
            }
        });
    },
    GetSummaryVehicleBySite: function (fromdate, todate) {
        $.ajax({
            url: '/Report/GetSummaryVehicleBySite',
            type: 'POST',
            data: {
                fromdate: fromdate,
                todate: todate
            },
            success: function (result) {
           
                $('#table-data').html(result)

                
            }
        });
    },
};