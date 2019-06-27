// <reference path="/wwwroot/js/app.js" />

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

$(function(win, doc, $) {

    "use strict";

    // --------

    var app = win.$.Plato,
        featureId = "Plato.Reports";

    /* reporting */
    var reports = {
        init: function() {
            app.logger.logInfo(featureId + " initializing");
            this.bind();
            app.logger.logInfo(featureId + " initialized");
        },
        bind: function() {

        }
    };

    /* charts */
    var charts = {
        colors: {
            red: 'rgb(255, 99, 132)',
            orange: 'rgb(255, 159, 64)',
            yellow: 'rgb(255, 205, 86)',
            green: 'rgb(75, 192, 192)',
            blue: 'rgb(54, 162, 235)',
            purple: 'rgb(153, 102, 255)',
            grey: 'rgb(201, 203, 207)',
            white: 'rgb(255, 255, 255)'
        },
        randomColors: function(length) {

            var graphColors = [], graphOutlines = [], hoverColor = [], i = 0;

            while (i <= length) {

                var randomR = Math.floor(Math.random() * 100 + Math.random() * 100);
                var randomG = Math.floor(Math.random() * 100 + Math.random() * 100);
                var randomB = Math.floor(Math.random() * 255 + Math.random() * 255);

                var graphBackground = "rgb(" + randomR + ", " + randomG + ", " + randomB + ")";
                graphColors.push(win.Chart.helpers.color(graphBackground).alpha(0.4).rgbString());

                var graphOutline = "rgb(" + randomR + ", " + randomG + ", " + randomB + ")";
                graphOutlines.push(graphOutline);

                var hoverColors = "rgb(" + randomR + ", " + randomG + ", " + randomB + ")";
                hoverColor.push(win.Chart.helpers.color(hoverColors).alpha(1).rgbString());

                i++;
            }

            return {
                backgroundColors: graphColors,
                borderColors: graphOutlines,
                hoverBackgroundColors: hoverColor
            };
        },
        init: function(id, config) {
            var ctx = doc.getElementById(id).getContext('2d');
            return new win.Chart(ctx, config);
        },
        initLine: function(id, data, opts) {
            this.init(id, {
                type: 'line',
                data: data,
                options: $.extend({
                    responsive: true,
                    maintainAspectRatio: false,
                    legend:
                    {
                        display: false,
                        position: 'bottom'
                    },
                    title:
                    {
                        display: false
                    },
                    tooltips: {
                        position: 'nearest',
                        mode: 'index',
                        intersect: false
                    },
                    scales: {
                        xAxes: [
                            {
                                display: false,
                                gridLines: {
                                    drawBorder: false,
                                    drawOnChartArea: true,
                                    drawTicks: false,
                                    color: win.Chart.helpers.color(charts.colors.grey).alpha(0.5).rgbString()
                                },
                                ticks: {
                                    stepSize: 10,
                                    callback: function (dataLabel, index) {
                                        return index % 3 === 0 ? dataLabel : '';
                                    }
                                }
                            }
                        ],
                        yAxes: [
                            {
                                display: false,
                                gridLines: {
                                    drawBorder: false,
                                    drawOnChartArea: false,
                                    drawTicks: false,
                                    color: win.Chart.helpers.color(charts.colors.grey).alpha(0.1).rgbString()
                                },
                                ticks: {
                                    stepSize: 10,
                                    callback: function (dataLabel, index) {
                                        return index % 3 === 0 ? dataLabel : '';
                                    }
                                }
                            }
                        ]
                    }
                }, opts)
            });
        },
        initBar: function (id, data, opts) {
            this.init(id, {
                type: 'bar',
                data: data,
                options: $.extend({
                    responsive: true,
                    maintainAspectRatio: false,
                    legend:
                    {
                        display: false,
                        position: 'bottom'
                    },
                    title:
                    {
                        display: false
                    },
                    tooltips: {
                        position: 'nearest',
                        mode: 'index',
                        intersect: false
                    },
                    scales: {
                        xAxes: [
                            {
                                display: false
                            }
                        ],
                        yAxes: [
                            {
                                display: false
                            }
                        ]
                    }
                }, opts)
            });
        },
        initDoughnut: function(id, data, opts) {
            this.init(id,
                {
                    type: 'doughnut',
                    data: data,
                    options: $.extend({
                        responsive: true,
                        maintainAspectRatio: false,
                        cutoutPercentage: 90,
                        legend:
                        {
                            display: false,
                            position: 'right'
                        },
                        title:
                        {
                            display: false
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        scales: {
                            xAxes: [
                                {
                                    display: false,
                                    exploded: true
                                }
                            ],
                            yAxes: [
                                {
                                    display: false,
                                    exploded: true
                                }
                            ]
                        }
                    }, opts)
                });
        }
    };
    
    // Extend $.Plato

    win.$.Plato.Reports = reports;
    win.$.Plato.Charts = charts;

    // --------

    // app ready
    app.ready(function () {
        reports.init();
    });

}(window, document, jQuery));
