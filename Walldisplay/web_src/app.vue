<template>
    <div>
        <div v-if="!displayId">
            <h2>No DisplayId.</h2>
            <p>Usage:</p>
            <pre>
/?displayId=wb1
/?displayId=wb2</pre>
        </div>
        <div v-if="data && data.error">
            <h2>Error retrieving data: {{data.error}}</h2>
        </div>
        <div v-if="data && !data.error">
            <component v-if="validateView(data.View)" v-bind:is="data.View"></component>
            <div v-if="!validateView(data.View)">
                <h2>Undefined view name: {{data.View}}</h2>
            </div>
        </div>
    </div>
</template>
<script>
    import $ from 'jquery';
    import queryString from 'query-string';

    const params = queryString.parse(location.search);

    const components = {
        GroupUserStatistics: require('./modules/group-user-statistics.vue').default,
        GroupStatistics: require('./modules/group-statistics.vue').default,
        MultiGroupStatistics: require('./modules/multi-group-statistics.vue').default,
        QueueStatistics: require('./modules/queue-statistics.vue').default,
        MultiQueueStatistics: require('./modules/multi-queue-statistics.vue').default,
        AggregateStatistics: require('./modules/aggregate-statistics.vue').default,
        HourlyQueueStatistics: require('./modules/hourly-queue-statistics.vue').default,
    };

    export default {
        data() {
            return {
                data: null,
                displayId: params.displayId
            };
        },
        created() {
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
                $.getJSON('/api/' + this.displayId, data => this.data = data)
                    .fail(e => this.data = {error: e.responseText || e.statusText});
                $('body').trigger('updated');
            },
            validateView(view) {
                return components[view] !== undefined;
            }
        },
        components: components
    }

</script>
