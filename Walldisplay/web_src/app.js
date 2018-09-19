import './style/style.less';

import Vue from 'vue';
import VueRouter from 'vue-router';

Vue.use(VueRouter);

import App from './app.vue'

const routes = [
    { path: '/', component: require('./modules/user-statistics.vue').default },
    { path: '/statistics', component: require('./modules/statistics.vue').default },
];

const router = new VueRouter({
    routes
});


new Vue({
    el: '#app',
    render: h => h(App),
    router
});