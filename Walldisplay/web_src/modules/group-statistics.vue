<template>
    <div class="group-statistics container-fluid">
        <div class="row header">
            <div class="col-xs-12">
                <h2>GROUP {{ data.GroupKeys[0].GroupName }} STATISTICS - REALTIME</h2>
            </div>
        </div>
        <ul class="entries row content">
            <div class="grid entry col-xs-12 col-md-6 woman">
                <div class="img"></div>
                <div class="cell text-center">LOGGED ON</div>
                <div class="cell text-center">{{data.GroupKeys[0].LoggedOn}}</div>
            </div>
            <div class="grid entry col-xs-12 col-md-6 man">
                <div class="img"></div>
                <div class="cell text-center">IDLE</div>
                <div class="cell text-center">{{data.GroupKeys[0].Idle}}</div>
            </div>
            <div class="grid entry col-xs-12 col-md-6 man">
                <div class="img"></div>
                <div class="cell text-center">ON ROUTED CALL</div>
                <div class="cell text-center">{{data.GroupKeys[0].OnRoutedCall}}</div>
            </div>
            <div class="grid entry col-xs-12 col-md-6 woman">
                <div class="img"></div>
                <div class="cell text-center">BUSY</div>
                <div class="cell text-center">{{data.GroupKeys[0].Busy}}</div>
            </div>
            <div class="grid entry col-xs-12 col-md-6 woman">
                <div class="img"></div>
                <div class="cell text-center">ON DIRECT CALL</div>
                <div class="cell text-center">{{data.GroupKeys[0].OnDirectCall}}</div>
            </div>
            <div class="grid entry col-xs-12 col-md-6 man">
                <div class="img"></div>
                <div class="cell text-center">AWAY</div>
                <div class="cell text-center">{{data.GroupKeys[0].Away}}</div>
            </div>
            <div class="grid entry col-xs-12 col-md-offset-3 col-md-6 calls-waiting"
                 v-bind:class="{warn: data.GroupKeys[0].CallsWaiting > data.WaitingWarnLimit}">
                <div class="img"></div>
                <div class="cell text-center">CALLS WAITING</div>
                <div class="cell text-center">{{data.GroupKeys[0].CallsWaiting}}</div>
            </div>
        </ul>
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