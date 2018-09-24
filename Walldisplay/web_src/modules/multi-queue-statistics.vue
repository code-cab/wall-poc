<template>
    <div class="multi-queue-statistics container-fluid">
        <div class="row header">
            <div class="col-xs-12">
                <h2>MULTI QUEUE STATISTICS - TODAY</h2>
            </div>
        </div>

        <div class="entries row content">
            <div class="grid entry col-xs-12">
                <div class="">&nbsp;</div>
                <div class="cell text-center">RECEIVED</div>
                <div class="cell text-center">ABANDONED</div>
                <div class="cell text-center">ANSWERED</div>
                <div class="cell text-center">AVG WAIT</div>
                <div class="cell text-center">MAX WAIT</div>
                <div class="cell text-center">SL %</div>
            </div>
            <div v-for="group in data.QueueKeys" class="grid entry col-xs-12">
                <div class="cell">{{group.GroupName}}</div>
                <div class="cell text-center">{{group.Received}}</div>
                <div class="cell text-center">{{group.Abandoned}}</div>
                <div class="cell text-center">{{group.Answered}}</div>
                <div class="cell text-center avg-waiting-time" v-bind:class="{warn: group.AvgWaitingTimeSec > data.AvgWaitingTimeLimit}">{{fmt(group.AvgWaitingTimeSec)}}</div>
                <div class="cell text-center">{{fmt(group.MaxWaitingTimeSec)}}</div>
                <div class="cell text-center calls-waiting" v-bind:class="{warn: group.ServiceLevelPerc < data.ServiceLevelWarnLimit}">
                    {{group.ServiceLevelPerc}}
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import $ from 'jquery';
    import formatDuration from 'format-duration';

    export default {
        data() {
            return {
                data: this.$parent.data
            };
        },
        methods: {
            fmt(duration) {
                return formatDuration(duration * 1000);
            },
        }
    }
</script>