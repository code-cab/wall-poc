<template>
    <div class="aggregate-statistics container-fluid">
        <div class="row header">
            <div class="col-xs-12">
                <h2>AGGREGATE {{ data.AggregateKeys[0].GroupName }} STATISTICS - TODAY</h2>
            </div>
        </div>

        <div class="entries row content">
            <div class="grid entry col-xs-4 received">
                <div class="cell text-center">RECEIVED</div>
                <div class="img"></div>
                <div class="cell text-center">{{data.AggregateKeys[0].Received}}</div>
            </div>
            <div class="grid entry col-xs-4 abandoned">
                <div class="cell text-center">ABANDONED</div>
                <div class="img"></div>
                <div class="cell text-center">{{data.AggregateKeys[0].Abandoned}}</div>
            </div>
            <div class="grid entry col-xs-4 answered">
                <div class="cell text-center">ANSWERED</div>
                <div class="img"></div>
                <div class="cell text-center">{{data.AggregateKeys[0].Answered}}</div>
            </div>
            <div class="grid entry col-xs-4 avg-waiting-time">
                <div class="cell text-center">AVERAGE WAITING TIME</div>
                <div class="img"></div>
                <div class="cell text-center">{{fmt(data.AggregateKeys[0].AvgWaitingTimeSec)}}</div>
            </div>
            <div class="grid entry col-xs-4 max-waiting-time">
                <div class="cell text-center">MAXIMUM WAITING TIME</div>
                <div class="img"></div>
                <div class="cell text-center">{{fmt(data.AggregateKeys[0].MaxWaitingTimeSec)}}</div>
            </div>
            <div class="grid entry col-xs-4 service-level" v-bind:class="{warn: data.AggregateKeys[0].ServiceLevelPerc < data.ServiceLevelWarnLimit}">
                <div class="cell text-center">SERVICE LEVEL %</div>
                <div class="img"></div>
                <div class="cell text-center">{{data.AggregateKeys[0].ServiceLevelPerc}} %</div>
            </div>
        </div>
    </div>
</template>
<script>
    import $ from 'jquery';
    import formatDuration from 'format-duration';

    export default {
        computed: {
            data: function() {
                return this.$parent.data
            }
        },
        methods: {
            fmt(duration) {
                return formatDuration(duration * 1000);
            },
        }
    }
</script>