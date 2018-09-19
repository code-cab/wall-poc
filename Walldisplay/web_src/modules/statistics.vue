<template>
    <div v-if="stats" class="user-statistics">
        <h2>GROUP {{ stats.group }} STATISTICS - REALTIME</h2>
        <div class="entries">
            <ul>
                <li v-for="user in stats.users">
                    <div>
                        <span>{{user.name}}</span>
                        <span>{{user.state}}</span>
                        <span>{{user.duration}}</span>
                    </div>
                </li>
            </ul>
        </div>
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
        methods: {
            updateData() {
                $.getJSON('static/statistics.json', data => {
                    this.stats = data;
                });
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