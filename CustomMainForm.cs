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

        public CustomMainForm()
        {
            InitializeComponent();

            ApiDispatcher = new ApiDispatch(GameInfoApi, EmuClientApi, EmulationApi, GuiApi, JoypadApi, MemoryApi);

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
            if (Owner != null)
            {
                Owner.Resize += (object sender, EventArgs e) =>
                {
                    var parent = (Form)sender;
                    if(parent.WindowState == FormWindowState.Minimized)
                    {
                        WindowState = FormWindowState.Minimized;
                    }

                    if(parent.WindowState == FormWindowState.Normal)
                    {
                        WindowState = FormWindowState.Normal;
                    }
                };
                Owner = null;
            }
            // Need to update the APIs that are being sent down to the Message Handler here
            ApiDispatcher.UpdateApis(GameInfoApi, EmuClientApi, EmulationApi, GuiApi, JoypadApi, MemoryApi);
        }

        public void UpdateValues(ToolFormUpdateType type)
        {
        }

        public bool AskSaveChanges() => true;
    }
}
