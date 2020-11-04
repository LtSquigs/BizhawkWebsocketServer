# BizhawkWebsocketServer
 An External Tool for Bizhawk that Exposes various APIs over Websockets

# APIs Exposed

The APIs exposed over this websocket server are:

GameInfoApi
EmuClientApi
EmulationApi
GuiApi
JoypadApi
MemoryApi

The API Exposes all methods on these APIs, the interfaces for which can be found in Bizhawk's Code.

In addition, the API Also exposes several events that websockets can listen on:

RomLoaded
BeforeQuickLoad
BeforeQuickSave
StateLoaded
StateSaved
