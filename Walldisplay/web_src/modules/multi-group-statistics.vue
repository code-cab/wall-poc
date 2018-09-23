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
                <div class="cell text-center">{{data.GroupKeys[0].LoggedOn}}</div>
                <div class="cell text-center">{{data.GroupKeys[0].OnRoutedCall}}</div>
                <div class="cell text-center">{{data.GroupKeys[0].OnDirectCall}}</div>
                <div class="cell text-center">{{data.GroupKeys[0].Idle}}</div>
                <div class="cell text-center">{{data.GroupKeys[0].Busy}}</div>
                <div class="cell text-center">{{data.GroupKeys[0].Away}}</div>
                <div class="cell text-center">{{data.GroupKeys[0].CallsWaiting}}</div>
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
                console.log(duration);
                return formatDuration(duration * 1000);
            },
        }
    }

//
//    let comp = {
//        data: () => ({stats: null}),
//        created: function() {
//            this.getData();
//        },
//        mounthed: function ()
//
//        },
//        methods: {
//            getData: function() {
//                $.getJSON('static/user-statistics.json', data => {
//                    console.log(JSON.stringify(data));
//                    this.stats = data;
//                });
//            }
//        }
//    };

//    export default comp;
</script>