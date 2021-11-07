class Chart {
    constructor() {
        this.title;
        this.seriesName;
        this.data;
        if (this.constructor === Chart) {
            throw new Error('Abstract class');
        }
    }
    set(title, data, seriesName) {
        throw new Error('Abstract method');
    }
    getOptions() {
        throw new Error('Abstract method');
    }
}
// --------------------------------------------------------------------------------------------------------------------
class BarChart extends Chart {
    constructor() {
        super();
        this.label;
    }
    set(title, data, seriesName) {
        this.title = title;
        this.seriesName = seriesName;
        this.label = [];
        this.data = [];
        for (var key in data) {
            this.label.push(key);
            this.data.push(data[key]);
        }
    }
    getOptions() {
        return {
            title: {
                text: this.title,
                subtext: this.seriesName,
            },
            tooltip: {},
            legend: {
                show: false,
            },
            xAxis: {
                type: 'value',
                show: true,
            },
            yAxis: {
                type: 'category',
                show: true,
                data: this.label
            },
            series: [{
                name: this.seriesName,
                colorBy: 'data',
                type: 'bar',
                data: this.data
            }]
        };;
    }
}
// --------------------------------------------------------------------------------------------------------------------
class PieChart extends Chart {
    constructor() {
        super();
    }
    set(title, data, seriesName) {
        this.title = title;
        this.seriesName = seriesName;
        var parsedResult = [];
        for (var key in data) {
            parsedResult.push({ 'value': data[key], 'name': key });
        }
        this.data = parsedResult;
    }
    getOptions() {
        return {
            title: {
                text: this.title,
                subtext: this.seriesName
            },
            tooltip: {},
            legend: {
                show: false,
            },
            xAxis: {
                show: false,
            },
            yAxis: {
                show: false,
            },
            series: [{
                name: this.seriesName,
                type: 'pie',
                radius: '60%',
                data: this.data
            }]
        };
    }
}
// --------------------------------------------------------------------------------------------------------------------
class ChartDirector { // parecido com padrão mediator
    constructor(element, title, model) {
        this.element = element;
        this.title = title;
        this.activeChartOption = 'pieChart_' + element.id;
        this.activeDataOption = 'durationInMinutes_' + element.id;
        this.chart = new PieChart();
        this.seriesName = "Dura\u00e7\u00e3o em minutos";
        this.data = model;
        this.currentDrawChart = echarts.init(this.element);
    }
    updateChart(chart, elementID) {
        document.getElementById(this.activeChartOption).className = '';
        this.activeChartOption = elementID;
        document.getElementById(this.activeChartOption).className = 'active';
        this.chart = chart;
        this.draw();
    }
    updateData(data, seriesName, elementID) {
        document.getElementById(this.activeDataOption).className = '';
        this.activeDataOption = elementID;
        document.getElementById(this.activeDataOption).className = 'active';
        this.data = data;
        this.seriesName = seriesName;
        this.draw();
    }
    draw() {
        this.chart.set(this.title, this.data, this.seriesName);
        this.currentDrawChart.setOption(this.chart.getOptions());
    }
    undraw() {
        var fakeoptions = this.chart.getOptions();
        fakeoptions["series"][0]["data"] = [];
        this.currentDrawChart.setOption(fakeoptions);
    }
}
// --------------------------------------------------------------------------------------------------------------------
function parseRawData(model, extract) {
    var result = {};
    for (var i = 0; i < model.length; i++) {
        if (model[i][extract] in result) {
            result[model[i][extract]]++;
        } else {
            result[model[i][extract]] = 1;
        }
    }
    return result;
}
function parseTableRawData(model, table, extract) {
    var result = {};
    for (var i = 0; i < model.length; i++) {
        if (model[i][table][extract] in result) {
            result[model[i][table][extract]]++;
        } else {
            result[model[i][table][extract]] = 1;
        }
    }
    return result;
}
// --------------------------------------------------------------------------------------------------------------------
function toList() {
    document.getElementById("list").className = "active";
    document.getElementById("chart").className = "";
    document.getElementById("listView").style.display = "block";
    document.getElementById("chartView").style.display = "none";
    for (var i in charts) {
        charts[i].undraw();
    }
}
function toChart() {
    document.getElementById("list").className = "";
    document.getElementById("chart").className = "active";
    document.getElementById("listView").style.display = "none";
    document.getElementById("chartView").style.display = "block";
    for (var i in charts) {
        charts[i].draw();
    }
}
// --------------------------------------------------------------------------------------------------------------------
var charts = {};
var parsedModel = {};

function createSimpleChart(name, title) {
    var element = document.getElementById(name);
    parsedModel[name] = {};
    parsedModel[name]['durationInMinutes'] = parseRawData(model, 'durationInMinutes');
    parsedModel[name]['appointmentType'] = parseRawData(model, 'appointmentType');
    parsedModel[name]['dentist'] = parseTableRawData(model, 'dentist', 'name');
    parsedModel[name]['patient'] = parseRawData(model, 'patient');
    charts[name] = new ChartDirector(element, title, parsedModel[name]['durationInMinutes']);
}

function createGroupingChart(name, title, iteration) {
    var element = document.getElementById(name);
    parsedModel[name] = {};
    parsedModel[name]['durationInMinutes'] = parseRawData(model[iteration], 'durationInMinutes');
    parsedModel[name]['appointmentType'] = parseRawData(model[iteration], 'appointmentType');
    parsedModel[name]['patient'] = parseRawData(model[iteration], 'patient');
    charts[name] = new ChartDirector(element, title, parsedModel[name]['durationInMinutes']);
}
// --------------------------------------------------------------------------------------------------------------------

document.getElementById('main-container').style.opacity = '0';
document.addEventListener("DOMContentLoaded", function () {
    toList();
    document.getElementById('main-container').style.opacity = '1';
});