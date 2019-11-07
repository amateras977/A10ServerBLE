using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;

namespace A10ServerBLE
{
    public class A10APIServer
    {
        private bool isEnable = false;

        private TargetDeviceEventDispatcher EventDispatcher;

        private static string apiUrlPrefix = "/api";

        private Dictionary<string, Func<HttpListenerContext, HttpListenerResponse>> urlMap;

        public A10APIServer(TargetDeviceEventDispatcher dispatcher)
        {
            this.EventDispatcher = dispatcher; 

            urlMap = new Dictionary<string, System.Func<HttpListenerContext, HttpListenerResponse>>()
            {
                { $"{apiUrlPrefix}/addQueue", (context) =>
                    {
                        Logger.log("/addQueue");

                        string intervalStr = context.Request.QueryString.Get("interval");
                        float interval = float.Parse(intervalStr);

                        string directionStr = context.Request.QueryString.Get("direction");
                        int direction = int.Parse(directionStr);
                        direction = direction < 0 ? -1 : 1;

                        Logger.log($"/addQueue interval : {interval}, direction : {direction}");
                        DeviceCommand command = new DeviceCommand();
                        command.direction = direction;
                        command.interval = interval;
                        EventDispatcher.AddQueue(command);
                        return context.Response;
                    } },
                { $"{apiUrlPrefix}/clearQueue", (context) =>
                    {
                        Logger.log("/clearQueue");
                        EventDispatcher.ClearQueue();
                        return context.Response;
                    } },


            };
        }

        public void Start()
        {
            try
            {
                string hostName = "http://localhost:8080/";
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add(hostName);

                listener.Start();
                Logger.log($"A10Server Started. host: {hostName}");

                isEnable = true;

                var serverTask = Task.Run(async () =>
                {
                    while (isEnable)
                    {
                        HttpListenerContext context = listener.GetContext();

                        Dispatcher(context);
                        HttpListenerResponse res = context.Response;

                        res.StatusCode = 200;
                        byte[] content = Encoding.UTF8.GetBytes("Http Request Recieved.");
                        res.OutputStream.Write(content, 0, content.Length);
                        res.Close();
                    }
                });

                var eventDispatchTask = Task.Run(async () =>
                {
                    var sw = new Stopwatch();
                    sw.Start();

                    float lastTime = 0f;
                    while (isEnable)
                    {

                        float currentTime = (float)sw.ElapsedMilliseconds / 1000;

                        EventDispatcher.Sync(currentTime);

                        lastTime = currentTime;

                        await Task.Delay(TimeSpan.FromMilliseconds(1));
                    }
                });

                Logger.log("Start A10ApiServer.");
            }
            catch (Exception e)
            {
                Logger.log($"Error: {e.Message}");
            }
        }

        private void Dispatcher(HttpListenerContext context)
        {
            var request = context.Request;

            if (request.RawUrl.Contains(apiUrlPrefix))
            {
                string querySeparator = "?";
                string url = request.RawUrl.Contains(querySeparator) ? request.RawUrl.Split("?", 2)[0] : request.RawUrl;

                Func<HttpListenerContext, HttpListenerResponse> tgtFunc;
                this.urlMap.TryGetValue(url, out tgtFunc);

                if (tgtFunc != null) { tgtFunc.Invoke(context); }


                Logger.log($"url: {url}");
                if (request.QueryString.AllKeys.Length > 0)
                {
                    Logger.log($" keys: {request.QueryString.AllKeys.Aggregate((all, key) => all + key)}");
                }
                Logger.log($"raw url:{request.RawUrl}");
            }
        }
    }
}
