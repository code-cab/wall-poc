<template>
    <div class="user-statistics container-fluid">
        <div class="row">
            <div class="col-xs-12">
                <h2>GROUP {{ data.GroupKeys[0].GroupName }} USER STATISTICS - REALTIME</h2>
            </div>
        </div>
        <ul class="entries row">
            <li v-for="user in data.UserKeys" class="col-xs-12 col-md-6">
                <div class="entry" v-bind:class="stateClass(user.State)">
                    <span class="col-xs-8">{{user.Name}}</span>
                    <span class="col-xs-2">{{user.State}}</span>
                    <span class="col-xs-2 text-right">{{fmt(user.DurationSec)}}</span>
                </div>
            </li>
        </ul>
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
            stateClass(state) {
                return {
                    "ACTIVE": "active"
                }[state];
            }
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