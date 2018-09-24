<template>
    <div class="multi-group-statistics container-fluid">
        <div class="row header">
            <div class="col-xs-12">
                <h2>MULTI GROUP STATISTICS - REALTIME</h2>
            </div>
        </div>

        <div class="entries row content">
            <div class="grid entry col-xs-12">
                <div class="">&nbsp;</div>
                <div class="cell text-center">LOGGED ON</div>
                <div class="cell text-center">ROUTED</div>
                <div class="cell text-center">DIRECT</div>
                <div class="cell text-center">IDLE</div>
                <div class="cell text-center">BUSY</div>
                <div class="cell text-center">AWAY</div>
                <div class="cell text-center">WAITING</div>
            </div>
            <div v-for="group in data.GroupKeys" class="grid entry col-xs-12">
                <div class="cell">{{group.GroupName}}</div>
                <div class="cell text-center">{{group.LoggedOn}}</div>
                <div class="cell text-center">{{group.OnRoutedCall}}</div>
                <div class="cell text-center">{{group.OnDirectCall}}</div>
                <div class="cell text-center">{{group.Idle}}</div>
                <div class="cell text-center">{{group.Busy}}</div>
                <div class="cell text-center">{{group.Away}}</div>
                <div class="cell text-center calls-waiting" v-bind:class="{warn: group.CallsWaiting >= data.WaitingWarnLimit}">
                    {{group.CallsWaiting}}
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