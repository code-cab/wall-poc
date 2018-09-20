<template>
    <div v-if="stats" class="user-statistics container-fluid">
        <div class="row">
            <div class="col-xs-12">
                <h2>GROUP {{ stats.group }} USER STATISTICS - REALTIME</h2>
            </div>
        </div>
        <ul class="entries row">
            <li v-for="user in stats.users" class="col-xs-12 col-md-6">
                <div class="entry">
                    <span class="col-xs-8">{{user.name}}</span>
                    <span class="col-xs-2">{{user.state}}</span>
                    <span class="col-xs-2 text-right">{{fmt(user.duration)}}</span>
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
                stats: null
            };
        },
        created() {
            this.$watch('stats', function() {}, {deep: true});
            this.updateData();
        },
        mounted() {
            if (this.timer) clearInterval(this.timer);
            this.timer = setInterval(() => this.updateData(), 1000);
        },
        destroyed() {
            if (this.timer) {
                clearInterval(this.timer);
                this.timer = 0;
            }
        },
        methods: {
            updateData() {
                $.getJSON('/userstatistics?group=ABC', data => this.stats = data);
            },
            fmt(duration) {
                console.log(duration);
                return formatDuration(duration * 1000);
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