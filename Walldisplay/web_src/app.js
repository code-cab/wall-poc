import './style/style.less';
import Vue from 'vue';
import HighchartsVue from 'highcharts-vue';

import App from './app.vue'

Vue.use(HighchartsVue);

new Vue({
    el: '#app',
    render: h => h(App),
});