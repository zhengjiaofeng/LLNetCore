import Vue from 'vue'

import App from './App.vue'

/*载入路由*/
import vuerouter from '@/routes'


// 粒子背景插件
import VueParticles from 'vue-particles'
// Bootstrap-Vue
import BootstrapVue from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

import httprequest from './common/request/httphelp.js'
//使用组件
Vue.use(VueParticles,BootstrapVue)
Vue.config.productionTip = false

Vue.prototype.$http = httprequest

new Vue({
  render: h => h(App),
  router:vuerouter,
}).$mount('#app')
