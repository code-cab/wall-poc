<template>
    <div class="group-user-statistics container-fluid">
        <div class="row header">
            <div class="col-xs-12">
                <h2>GROUP {{ data.GroupKeys[0].GroupName }} USER STATISTICS - REALTIME</h2>
            </div>
        </div>
        <ul class="entries row content">
            <li v-for="user in data.UserKeys" class="col-xs-12 col-md-6">
                <div class="grid entry" v-bind:class="user.State.toLowerCase()">
                    <div class="cell">{{user.Name}}</div>
                    <div class="cell text-center">{{user.State}}</div>
                    <div class="cell text-right">{{fmt(user.DurationSec)}}</div>
                    <div class="img"></div>
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
                return formatDuration(duration * 1000);
            },
        }
    }
</script>