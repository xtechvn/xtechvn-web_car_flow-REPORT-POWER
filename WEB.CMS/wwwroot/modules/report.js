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
        _report.GetBieudo(FromDateStr, ToDateStr);
        _report.GetBieudo_phantram(FromDateStr, ToDateStr);
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
    GetBieudo: function (fromdate, todate) {
        $.ajax({
            url: "/Report/GetDataBieudo",
            type: "post",
            data: {
                fromdate: fromdate,
                todate: todate
            },
            success: function (result) {
                if (result.isSuccess != false) {
                    _report.Bieudo(result.data.labels, result.data.weightValue, result.data.totalWeightValue);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    GetBieudo_phantram: function (fromdate, todate) {
        $.ajax({
            url: "/Report/GetDataBieudo",
            type: "post",
            data: {
                fromdate: fromdate,
                todate: todate
            },
            success: function (result) {
                if (result.isSuccess != false) {
                    var html = '';
                    if (result.data && result.data.labels && result.data.weightValue) {
                        // Mock percentages based on data since API doesn't return explicit percentages in this structure
                        // Assuming weightValue is Actual and totalWeightValue is Plan (or vice versa based on earlier charts)
                        // But for this specific UI "Region Completion Rate", let's calculate simplistic percentages or mock them if data isn't sufficient.
                        // Result data structure seems to be labels[], weightValue[], totalWeightValue[]

                        for (var i = 0; i < result.data.labels.length; i++) {
                            var region = result.data.labels[i];
                            var actual = result.data.totalWeightValue[i] || 0; // Using totalWeightValue as Actual based on Bieudo function colors (yellow=Actual)
                            var plan = result.data.weightValue[i] || 1; // Avoid divide by zero
                            var percent = Math.round((actual / plan) * 100);

                            var barColor = 'var(--success)';
                            if (percent < 50) barColor = 'var(--danger)';
                            else if (percent < 90) barColor = 'var(--accent-yellow)';

                            html += `
                                <div class="region-item">
                                    <div class="region-header">
                                        <span>${region}</span>
                                        <strong>${percent}%</strong>
                                    </div>
                                    <div class="region-bar-bg">
                                        <div class="region-bar-fill" style="width: ${Math.min(percent, 100)}%; background-color: ${barColor}"></div>
                                    </div>
                                </div>`;
                        }
                        $('#region-progress-list').html(html);
                    }
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("Status: " + textStatus);
            }
        });
    },
    Bieudo: function (listLabel, weightValue, totalWeightValue) {

        if (window.revenuChartInstance) {
            window.revenuChartInstance.destroy();
        }

        var canvas = document.getElementById("revenuChart");
        var ctx = canvas.getContext('2d');

        var barChartData = {
            labels: listLabel,
            datasets: [
                {
                    label: 'Kế hoạch',
                    data: weightValue,
                    backgroundColor: '#00e6ff',
                    borderColor: '#00e6ff',
                    borderWidth: 1,
                    barPercentage: 0.25
                },
                {
                    label: 'Thực tế',
                    data: totalWeightValue,
                    backgroundColor: '#ffff00',
                    borderColor: '#ffff00',
                    borderWidth: 1,
                    barPercentage: 0.25
                },

            ]
        };

        var ObjectChart = {
            type: 'bar',
            data: barChartData,
            options: {
                responsive: true,
                maintainAspectRatio: false,
                tooltips: { enabled: false },
                hover: { mode: null, onHover: null },
                events: [],

                legend: {
                    labels: { fontColor: '#000' },
                    onClick: function () { }
                },

                title: {
                    display: true,
                    text: 'Sản lượng theo nhà máy',
                    fontColor: '#000',
                    fontSize: 16
                },

                scales: {
                    xAxes: [{
                        display: true,
                        ticks: { fontColor: '#000' },
                        scaleLabel: {
                            display: true,
                            labelString: '',
                            fontColor: '#000'
                        },
                        gridLines: { display: false },
                        barPercentage: 0.9,
                        categoryPercentage: 0.7
                    }],
                    yAxes: [{
                        display: false, // 💥 Ẩn hoàn toàn trục Y
                        gridLines: { display: false },
                        ticks: { display: false }
                    }]
                },

                animation: {
                    duration: 0,
                    onComplete: function () {
                        var chartInstance = this.chart;
                        var ctx = chartInstance.ctx;
                        ctx.textAlign = 'center';
                        ctx.textBaseline = 'middle';
                        ctx.font = 'bold 11px Arial';
                        ctx.fillStyle = '#000';

                        this.data.datasets.forEach(function (dataset, i) {
                            var meta = chartInstance.controller.getDatasetMeta(i);
                            meta.data.forEach(function (bar, index) {
                                var value = dataset.data[index];
                                if (value != null) {
                                    ctx.fillText(value, bar._model.x, bar._model.y - 10);
                                }
                            });
                        });
                    }
                }
            }
        };

        window.revenuChartInstance = new Chart(ctx, ObjectChart);
        if (window.revenuChartInstance) window.revenuChartInstance.update();
    }
};