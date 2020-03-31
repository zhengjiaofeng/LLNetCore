import Vue from 'vue'
import VueRouter from 'vue-router'

Vue.use(VueRouter);

// 定义路由
const routes = [
    {
        name: 'LLVue1',
        path: '/',
        redirect: '/account',

    },
    {
        path: '/account',
        component: () => import('../views/Account/login.vue')
    }
];
const vuerouter = new VueRouter({
    routes// (缩写) 相当于 routes: routes
});

export default vuerouter;