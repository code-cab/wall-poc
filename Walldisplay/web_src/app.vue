<template>
    <div v-if="data">
        <component v-bind:is="data.View"></component>
    </div>
</template>
<script>
    import $ from 'jquery';
    import queryString from 'query-string';

    const params = queryString.parse(location.search);
    console.log(JSON.stringify(params));

    export default {
        data() {
            return {
                data: null
            };
        },
        created() {
            this.updateData();
        },
        mounted() {
            if (this.timer) clearInterval(this.timer);
            this.timer = setInterval(() => this.updateData(), 5000);
        },
        destroyed() {
            if (this.timer) {
                clearInterval(this.timer);
                this.timer = 0;
            }
        },
        methods: {
            updateData() {
                $.getJSON('/api?displayId=' + params.displayId, data => this.data = data);
            },
        },
        components: {
            GroupUserStatistics: require('./modules/group-user-statistics.vue').default
        }
    }

</script>
