// <reference path="/wwwroot/js/app.js" />

if (typeof $().modal === 'undefined') {
    throw new Error("BootStrap 4.1.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

$(function(win, doc, $) {

    "use strict";

    // --------

    var app = win.$.Plato,
        featureId = "Plato.Reporting";

    /* reporting */
    var reporting = {
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
                var randomR = Math.floor(Math.random() * 130 + 100);
                var randomG = Math.floor(Math.random() * 130 + 100);
                var randomB = Math.floor(Math.random() * 130 + 100);

                var graphBackground = "rgb(" + randomR + ", " + randomG + ", " + randomB + ")";
                graphColors.push(win.Chart.helpers.color(graphBackground).alpha(0.5).rgbString());

                var graphOutline = "rgb(" + (randomR - 80) + ", " + (randomG - 80) + ", " + (randomB - 80) + ")";
                graphOutlines.push(graphOutline);

                var hoverColors = "rgb(" + (randomR + 25) + ", " + (randomG + 25) + ", " + (randomB + 25) + ")";
                hoverColor.push(hoverColors);

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
        initLine: function(id, data) {
            this.init(id, {
                type: 'line',
                data: data,
                options: {
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
                    animation: false,
                    scales: {
                        xAxes: [
                            {
                                display: false,
                                gridLines: {
                                    drawBorder: false,
                                    drawOnChartArea: false,
                                    drawTicks: false,
                                    color: win.Chart.helpers.color(app.ui.chartColors.white).alpha(0.5).rgbString()
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
                                display: false
                            }
                        ]
                    }
                }
            });
        },
        initDoughnut: function(id, data) {
            this.init(id,
                {
                    type: 'doughnut',
                    data: data,
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        legend:
                        {
                            display: true,
                            position: 'right'
                        },
                        title:
                        {
                            display: false
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
                    }
                });
        }
    };
    
    // Extend $.Plato

    win.$.Plato.Reporting = reporting;
    win.$.Plato.Charts = charts;

    // --------

    // app ready
    app.ready(function () {
        reporting.init();
    });

}(window, document, jQuery));
