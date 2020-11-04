using System;

using System.Windows.Forms;
using BizHawk.Client.Common;


namespace BizHawkWebsocketServer
{
    [ExternalTool("WebsocketServer", Description = "A server that exposes the BizHawk client API over websockets")]
    public partial class CustomMainForm : Form, IExternalToolForm
    {
        WebsocketServer ApiServer;
        ApiDispatch ApiDispatcher;

        [RequiredApi]
        public IGameInfoApi GameInfoApi { get; set; }

        [RequiredApi]
        public IEmuClientApi EmuClientApi { get; set; }

        [RequiredApi]
        public IEmulationApi EmulationApi { get; set; }

        [RequiredApi]
        public IGuiApi GuiApi { get; set; }

        [RequiredApi]
        public IJoypadApi JoypadApi { get; set; }

        [RequiredApi]
        public IMemoryApi MemoryApi { get; set; }

        [RequiredApi]
        public IMemoryEventsApi MemoryEventsApi { get; set; }

        public CustomMainForm()
        {
            InitializeComponent();

            ApiDispatcher = new ApiDispatch(GameInfoApi, EmuClientApi, EmulationApi, GuiApi, JoypadApi, MemoryApi, MemoryEventsApi);

            ApiServer = new WebsocketServer("/", 64646, ApiDispatcher);
            ApiServer.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            ApiServer.Stop();
            base.OnClosed(e);
        }

        public void Restart()
        {
            // Need to update the APIs that are being sent down to the Message Handler here
            ApiDispatcher.UpdateApis(GameInfoApi, EmuClientApi, EmulationApi, GuiApi, JoypadApi, MemoryApi, MemoryEventsApi);
        }

        public void UpdateValues(ToolFormUpdateType type)
        {
        }

        public bool AskSaveChanges() => true;
    }
}
