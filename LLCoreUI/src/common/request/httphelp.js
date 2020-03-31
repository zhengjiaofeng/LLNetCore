import axios from 'axios'
// qs 是对 post 请求 data 进行处理，不然后台接收不了数据
// 因为axios post请求默认Content-type是 application/json
import qs from 'qs'

//api域名
axios.defaults.baseURL = 'https://localhost:44384/api';
//application/json
// 设置 post、put 默认 Content-Type
axios.defaults.headers.post['Content-Type'] = 'application/json';
// axios.defaults.headers.put['Content-Type'] = 'application/x-www-form-urlencoded';

axios.defaults.withCredentials=true;

// http request 拦截器 
axios.interceptors.request.use(
    request => {
        if (request.method === 'post' || request.method === 'put') {
            // post、put 提交时，将对象转换为string, 为处理后台解析问题
            request.data = qs.parse(request.data) 
          } 
        //   else if (request.method === 'get' && browser.isIE) {
        //     // 给GET 请求后追加时间戳， 解决IE GET 请求缓存问题
        //     let symbol = request.url.indexOf('?') >= 0 ? '&' : '?'
        //     request.url += symbol + '_=' + Date.now()
        //   }

        
          // 请求发送前进行处理
          return request
    },
    error => {
      return Promise.reject(error)
    }
  );

// 响应拦截器
axios.interceptors.response.use(    
    response => {   
        // 如果返回的状态码为200，说明接口请求成功，可以正常拿到数据     
        // 否则的话抛出错误
        if (response.status === 200) {            
            return Promise.resolve(response);        
        } else {            
            return Promise.reject(response);        
        }    
    },    
    // 服务器状态码不是2开头的的情况
    // 这里可以跟你们的后台开发人员协商好统一的错误状态码    
    // 然后根据返回的状态码进行一些操作，例如登录过期提示，错误提示等等
    // 下面列举几个常见的操作，其他需求可自行扩展
    error => {    

        console.log(error);
        if (error.response) {            
            switch (error.response.status) {                
                // 401: 未登录
                // 未登录则跳转登录页面，并携带当前页面的路径
                // 在登录成功后返回当前页面，这一步需要在登录页操作。                
                case 401:                    
                    "401"
                    break;
 
                // 403 token过期
                // 登录过期对用户进行提示
                // 清除本地token和清空vuex中token对象
                // 跳转登录页面                
                case 403:
                    "403"                 
                    break; 
 
                // 404请求不存在
                case 404:
                   "404"
                    break;
                // 其他错误，直接抛出错误提示
                default:
                   "default"
            }
            return Promise.reject(error.response);
        }else{
            console.log("错误的请求，跳转Error页面")
        }
    }   
);

export default axios