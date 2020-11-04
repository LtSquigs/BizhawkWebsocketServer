using System;
using BizHawk.Client.Common;
using System.Reflection;

namespace BizHawkWebsocketServer
{
    class ApiDispatch
    {
        public event EventHandler RomLoaded;
        public event EventHandler BeforeQuickLoad;
        public event EventHandler BeforeQuickSave;
        public event EventHandler StateLoaded;
        public event EventHandler StateSaved;

        private IGameInfoApi gameInfoApi;
        private IEmuClientApi emuClientApi;
        private IEmulationApi emulationApi;
        private IGuiApi guiApi;
        private IJoypadApi joypadApi;
        private IMemoryApi memoryApi;

        public ApiDispatch(IGameInfoApi GameInfoApi, IEmuClientApi EmuClientApi, IEmulationApi EmulationApi, IGuiApi GuiApi, IJoypadApi JoypadApi, IMemoryApi MemoryApi)
        {
            UpdateApis(GameInfoApi, EmuClientApi, EmulationApi, GuiApi, JoypadApi, MemoryApi);
        }

        public void UpdateApis(IGameInfoApi GameInfoApi, IEmuClientApi EmuClientApi, IEmulationApi EmulationApi, IGuiApi GuiApi, IJoypadApi JoypadApi, IMemoryApi MemoryApi)
        {
            // De-register any events here
            if(emuClientApi != null)
            {
                emuClientApi.RomLoaded -= OnRomLoaded;
                emuClientApi.BeforeQuickLoad -= OnBeforeQuickLoad;
                emuClientApi.BeforeQuickSave -= OnBeforeQuickSave;
                emuClientApi.StateLoaded -= OnStateLoaded;
                emuClientApi.StateSaved -= OnStateSaved;
            }

            gameInfoApi = GameInfoApi;
            emuClientApi = EmuClientApi;
            emulationApi = EmulationApi;
            guiApi = GuiApi;
            joypadApi = JoypadApi;
            memoryApi = MemoryApi;

            // Re-register any events here
            if (emuClientApi != null)
            {
                emuClientApi.RomLoaded += OnRomLoaded;
                emuClientApi.BeforeQuickLoad += OnBeforeQuickLoad;
                emuClientApi.BeforeQuickSave += OnBeforeQuickSave;
                emuClientApi.StateLoaded += OnStateLoaded;
                emuClientApi.StateSaved += OnStateSaved;
            }
        }

        public object DispatchCall(string name, object[] arguments)
        {
            string[] parts = name.Split('.');

            if (parts.Length != 2)
            {
                throw new Exception($"Unknown method {name}. Should be <api>.<method>.");
            }

            string api = parts[0];
            string method = parts[1];

            MethodInfo methodFunc;
            object apiInstance = null;

            switch(api)
            {
                case "GameInfoApi":
                    apiInstance = gameInfoApi;
                    methodFunc = gameInfoApi.GetType().GetMethod(method);
                    break;
                case "EmuClientApi":
                    apiInstance = emuClientApi;
                    methodFunc = emuClientApi.GetType().GetMethod(method);
                    break;
                case "EmulationApi":
                    apiInstance = emulationApi;
                    methodFunc = emulationApi.GetType().GetMethod(method);
                    break;
                case "GuiApi":
                    apiInstance = guiApi;
                    methodFunc = guiApi.GetType().GetMethod(method);
                    break;
                case "JoypadApi":
                    apiInstance = joypadApi;
                    methodFunc = joypadApi.GetType().GetMethod(method);
                    break;
                case "MemoryApi":
                    apiInstance = memoryApi;
                    methodFunc = memoryApi.GetType().GetMethod(method);
                    break;
                default:
                    throw new Exception($"Unknown API {api}");
            }

            if (methodFunc == null)
            {
                throw new Exception($"Unknown Method {method} on API {api}");
            }

            return methodFunc.Invoke(apiInstance, arguments);
        }

        public void OnRomLoaded(object sender, EventArgs e)
        {
            EventHandler handler = RomLoaded;
            handler?.Invoke(sender, e);
        }

        public void OnBeforeQuickLoad(object sender, EventArgs e)
        {
            EventHandler handler = BeforeQuickLoad;
            handler?.Invoke(sender, e);
        }

        public void OnBeforeQuickSave(object sender, EventArgs e)
        {
            EventHandler handler = BeforeQuickSave;
            handler?.Invoke(sender, e);
        }

        public void OnStateLoaded(object sender, EventArgs e)
        {
            EventHandler handler = StateLoaded;
            handler?.Invoke(sender, e);
        }

        public void OnStateSaved(object sender, EventArgs e)
        {
            EventHandler handler = StateSaved;
            handler?.Invoke(sender, e);
        }
    }
}
