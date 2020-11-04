using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;

namespace BizHawkWebsocketServer
{
    class RequestMessage
    {
        public string message_type { get; set; }
        public string id { get; set; }
        public string function { get; set; }
        public object[] arguments { get; set; }
    }
    class ResultMessage
    {
        public const string message_type = "RESULT";
        public string id { get; set; }
        public object result { get; set; }
    }

    class EventResponse
    {
        public const string message_type = "EVENT_RESPONSE";
        public string event_name { get; set; }
        public object event_args { get; set; }
    }

    class ErrorMessage
    {
        public const string message_type = "ERROR";
        public string id { get; set; }
        public string error_string { get; set; }
    }

    class MessageHandler : WebSocketBehavior
    {
        ApiDispatch apiDispatch;

        public MessageHandler(ApiDispatch dispatch)
        {
            apiDispatch = dispatch;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            string currentMessageId = null;

            try
            {
                var request = JsonConvert.DeserializeObject<RequestMessage>(e.Data, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                });

                var message = new ResultMessage();
                message.id = currentMessageId;

                switch (request.message_type)
                {
                    case "METHOD":
                        currentMessageId = request.id;

                        var result = apiDispatch.DispatchCall(request.function, request.arguments);

                        message.result = result;

                        Send(JsonConvert.SerializeObject(message));

                        break;
                    case "REGISTER_EVENT":

                        RegisterEvent(request.function);

                        message.result = "Event Registered";

                        Send(JsonConvert.SerializeObject(message));

                        break;
                    case "UNREGISTER_EVENT":

                        UnregisterEvent(request.function);

                        message.result = "Event Unregistered";

                        Send(JsonConvert.SerializeObject(message));

                        break;
                    default:
                        throw new Exception("Unknown Message Type");
                }

            }
            catch (Exception ex)
            {
                var errorMessage = new ErrorMessage();
                errorMessage.id = currentMessageId;
                errorMessage.error_string = ex.Message;

                Send(JsonConvert.SerializeObject(errorMessage));
            }
        }

        protected void RegisterEvent(string eventName)
        {
            switch(eventName)
            {
                case "RomLoaded":
                    apiDispatch.RomLoaded += RomLoaded;
                    break;
                case "BeforeQuickLoad":
                    apiDispatch.RomLoaded += BeforeQuickLoad;
                    break;
                case "BeforeQuickSave":
                    apiDispatch.RomLoaded += BeforeQuickSave;
                    break;
                case "StateLoaded":
                    apiDispatch.RomLoaded += StateLoaded;
                    break;
                case "StateSaved":
                    apiDispatch.RomLoaded += StateSaved;
                    break;
                default:
                    throw new Exception($"Unknown event name {eventName}");
            }
        }

        protected void UnregisterEvent(string eventName)
        {
            switch (eventName)
            {
                case "RomLoaded":
                    apiDispatch.RomLoaded -= RomLoaded;
                    break;
                default:
                    throw new Exception($"Unknown event name {eventName}");
            }
        }

        protected void RomLoaded(object sender, EventArgs args)
        {
            EventSender("RomLoaded", args);
        }

        protected void BeforeQuickLoad(object sender, EventArgs args)
        {
            EventSender("BeforeQuickLoad", args);
        }

        protected void BeforeQuickSave(object sender, EventArgs args)
        {
            EventSender("BeforeQuickSave", args);
        }

        protected void StateLoaded(object sender, EventArgs args)
        {
            EventSender("StateLoaded", args);
        }

        protected void StateSaved(object sender, EventArgs args)
        {
            EventSender("StateSaved", args);
        }


        protected void EventSender(string name, EventArgs args)
        {
            var result = new EventResponse();
            result.event_name = name;
            result.event_args = args;

            Send(JsonConvert.SerializeObject(result));
        }

    }

    class WebsocketServer
    {
        readonly WebSocketServer wssv;

        public WebsocketServer(string url, int port, ApiDispatch apiDispatch)
        {
            wssv = new WebSocketServer(port);
            wssv.AddWebSocketService<MessageHandler>(url, () => new MessageHandler(apiDispatch));
        }

        public void Start()
        {
            wssv.Start();
        }

        public void Stop()
        {
            wssv.Stop();
        }
    }
}
