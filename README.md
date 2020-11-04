# BizhawkWebsocketServer
 An External Tool for Bizhawk that Exposes various APIs over Websockets

# APIs Exposed

The APIs exposed over this websocket server are:

```
GameInfoApi
EmuClientApi
EmulationApi
GuiApi
JoypadApi
MemoryApi
```

The API Exposes all methods on these APIs, the interfaces for which can be found in Bizhawk's Code.

In addition, the API Also exposes several events that websockets can listen on:

```
RomLoaded
BeforeQuickLoad
BeforeQuickSave
StateLoaded
StateSaved
```

# Connecting To Server

The server listens for any connection on the following address:

`ws://localhost:64646/`

Any standard Websocket Library should be able to connect to it.

# Message Format

There are several different messages that can be sent or recieved from the websocket server for requests that will be documented here.

All messages are in JSON format, and all messages (except for Events from the server) will have an ID associated with them. Requests from the client should generate this ID themselves, and the websocket server will echo this ID in response. This is useful to coordinate responses to function calls. Any ID will work as long as it is unique for each message call.

## Request Messages

### Method Request

To request a method call the following message format is expected

```
{
  message_type: "METHOD",
  id: "<unique-id>",
  function: "<api>.<method>",
  arguments: [ <arg1>, <arg2>, ...]
}
```

Where `<api>` is one of the APIs mentioned above, and `<method>` is a method on that interface. e.g. `GameInfoApi.GetRomName`.

The `arguments` array should still be sent even if the method has no arguments, send an empty array (e.g. `[]`)

### Event Register

To register to listen for an event send the message

```
{
  message_type: "REGISTER_EVENT",
  id: "<unique-id>",
  function: "<event>"
}
```

Where `<event>` is one of the events mentioned above (e.g. `RomLoaded`)

### Event De-Register

To register to listen for an event send the message

```
{
  message_type: "DEREGISTER_EVENT",
  id: "<unique-id>",
  function: "<event>"
}
```

Where `<event>` is one of the events mentioned above (e.g. `RomLoaded`)

## Response Messages

### Result Message

Result messages are sent in response to Method Requests and Event Register/De-Register events.

The general format is:

```
{
  message_type: "RESPONSE",
  id: "<unique-id>",
  result: <result-obj>
}
```

Where the `id` value is the corresponding ID that was sent in with the request.

`result` will be a JSON serialized object containing the return value from the method (or a string indicating success for event register/de-register)

### Event Response

After registering for an Event, the websocket server will emit Event Response messages every time the event happens.

Their format is:

```
{
   message_type: "EVENT_RESPONSE",
   event_name: "<name_of_event>",
   event_args: <event_args>
}
```

Where `event_name` is the name of the event (e.g. `RomLoaded`), and `event_args` is a JSON Serialization of the `EventArgs` passed to the event.

### Error Response

When an error occurs the websocket emits an error message in response.

Their format is:

```
{
   message_type: "ERROR",
   id: "<unique-id>",
   error_string: "<error-string>"
}
```
