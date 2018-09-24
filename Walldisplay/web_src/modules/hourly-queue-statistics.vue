<template>
    <div class="queue-statistics-hourly container-fluid">
        <div class="row header">
            <div class="col-xs-12">
                <h2>QUEUE {{ data.HourlyQueueKeys[0].GroupName }} STATISTICS - TODAY</h2>
            </div>
        </div>

        <div class="row content">
            <highcharts :options="chartOptions"></highcharts>
        </div>
    </div>
</template>
<script>
    import $ from 'jquery';
    import formatDuration from 'format-duration';

    let comp = {
        created() {
            $('body').on('updated', () => {
//                if (this.chartOptions) {
//                    this.chartOptions.series[0].data = this.getSeriesData('Received');
//                }
                this.updateSeries();
            });
            $('body').trigger('updated');
            setInterval(() => {
                if ($('.content').height() != this.chartOptions.chart.height) {
                    this.chartOptions.chart.height = $('.content').height();
                }
            }, 100);
        },
        data() {
            return {
                chartOptions: {
                    chartType: 'line',
                    chart: {
                        height: null
                    },
                    title: {
                        text: ''
                    },
                    xAxis: {
                        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
                    },
                    yAxis: {
                        title: {
                          text: ''
                        }
                    },
                    series: [{
                        name: 'RECEIVED',
                        data: [],
                        color: '#000'
                    }, {
                        name: 'ABANDONED',
                        data: [],
                        color: '#ff0000'
                    }, {
                        name: 'ANSWERED',
                        data: [],
                        color: '#00b050'
                    }],
                    plotOptions: {
                        line: {
                            dataLabels: {
                                enabled: true
                            },
                            enableMouseTracking: false
                        }
                    },

                },

                data: this.$parent.data,
            };
        },
        methods: {
            fmt(duration) {
                return formatDuration(duration * 1000);
            },
            getSeriesData(prop) {
                return this.$parent.data.HourlyQueueKeys[0].map(d => d[prop]);
            },
            updateSeries() {
                if (this.chartOptions) {
                    this.chartOptions.series[0].data = this.$parent.data.HourlyQueueKeys[0].map(d => d['Received']);
                    this.chartOptions.series[1].data = this.$parent.data.HourlyQueueKeys[0].map(d => d['Abandoned']);
                    this.chartOptions.series[2].data = this.$parent.data.HourlyQueueKeys[0].map(d => d['Answered']);
                    this.chartOptions.xAxis.categories = this.$parent.data.HourlyQueueKeys[0].map(d =>
                    `${d.Hour}:00-${d.Hour + 1}:00`
                    );
                }
            }

        }


    };


    export default comp;
</script>