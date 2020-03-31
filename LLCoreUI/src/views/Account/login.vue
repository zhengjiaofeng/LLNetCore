<!--  -->
<template>
    <div>
        <!-- vue 粒子背景-->
        <vue-particles color="#B0E0E6" :particleOpacity="0.7" :particlesNumber="80" shapeType="circle" :particleSize="4"
            linesColor="#B0E0E6" :linesWidth="1" :lineLinked="true" :lineOpacity="0.4" :linesDistance="150"
            :moveSpeed="3" :hoverEffect="true" hoverMode="grab" :clickEffect="true" clickMode="repulse">
        </vue-particles>

        <div class="center">
            <div class="login_div">
                <form>
                    <div class="input-group mb-4">
                        <div class="input-group-prepend">
                            <span class="input-group-text loginspan" id="inputGroup-sizing-default">U</span>
                        </div>
                        <input type="text" v-model="userName" class="form-control" aria-label="Sizing example input"
                            aria-describedby="inputGroup-sizing-default" placeholder="账号">
                    </div>

                    <div class="input-group mb-4">
                        <div class="input-group-prepend">
                            <span class="input-group-text loginspan" id="inputGroup-sizing-default">P</span>
                        </div>
                        <input type="password" v-model="userPwd" class="form-control" aria-label="Sizing example input"
                            aria-describedby="inputGroup-sizing-default" placeholder="密码">
                    </div>

                    <button type="button" class="btn btn-secondary loginbtn" v-on:click="DoLogin">Login</button>
                </form>
            </div>
        </div>
    </div>

</template>

<script>
    //这里可以导入其他文件（比如：组件，工具js，第三方插件js，json文件，图片文件等等）
    //例如：import 《组件名称》 from '《组件路径》';
    import md5 from 'js-md5';
    export default {
        name: 'Login',
        data() {
            return {
                msg: 'Welcome to your vueName',
                userName: "",
                userPwd: ""
            }
        },
        //监听属性 类似于data概念
        computed: {},
        //监控data中的数据变化
        watch: {},
        //方法集合
        methods: {
            DoLogin: function () {
                debugger;
                let params = Object();
                params.userName = this.userName;
                params.passWord = md5(this.userPwd);
                this.$http.post('/Account/Login', params).then((response) => {
                    debugger;
                    if (response.data.state == "200") {
                        var data = response.data.data;
                        console.log(data);
                        localStorage.clear();
                        localStorage.setItem("token", data.token);
                        localStorage.setItem("tokenExpired", data.tokenExpired);
                        localStorage.setItem("refreshToken", data.refreshToken);
                        localStorage.setItem("refTokenExpired", data.refTokenExpired);
                        localStorage.setItem("userId", data.userId);
                        localStorage.setItem("userAccount", data.userAccount);
                    }

                }).catch((error) => {
                    console.log(error)
                })

            },
            dotest: function () {
                console.log('test');
            }
        },
        //生命周期 - 创建完成（可以访问当前this实例）
        created() {
            //this.DoLogin();

        },
        //生命周期 - 挂载完成（可以访问DOM元素）
        mounted() {

        },
        beforeCreate() { }, //生命周期 - 创建之前
        beforeMount() { }, //生命周期 - 挂载之前
        beforeUpdate() { }, //生命周期 - 更新之前
        updated() { }, //生命周期 - 更新之后
        beforeDestroy() { }, //生命周期 - 销毁之前
        destroyed() { }, //生命周期 - 销毁完成
        activated() { }, //如果页面有keep-alive缓存功能，这个函数会触发
    }
</script>
<style lang='scss' scoped>
    /* @import url(); 引入公共css类 */
    #particles-js {
        background-image: url('../../assets/home/IndexPhoto.jpg');
    }

    .center {
        width: 300px;
        height: 300px;
        /*水平居中*/
        position: absolute;
        top: 30%;
        left: 44.5%;
        box-shadow: 0 -15px 30px #d0d0d0;
        border: 1px solid #d0d0d0;
        border-radius: 5px;
    }

    .login_div {
        margin: 0 auto;
        /*水平居中*/
        position: relative;
        /*设置position属性*/
        top: 25%;
        /*偏移*/
        width: 250px;
    }

    .loginbtn {
        width: 250px;
        background-color: #727473;
        border-color: #727473;
    }

    .loginspan {
        background-color: #727473;
        border-color: #727473;
    }
</style>