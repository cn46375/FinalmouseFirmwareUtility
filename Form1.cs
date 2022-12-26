using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalmouseFirmwareUtility
{
    public partial class Form1 : Form
    {
        List<string> currentPorts = SerialPort.GetPortNames().ToList();
        List<string> dfuPorts = new List<string>();
        Dictionary<string, string> firmwareDictionary = GetFirmwareDictionary();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var text = "Welcome to the Finalmouse Firmware Update Utility. " +
                "If your Starlight-12 is already in DFU mode before running this utility, " +
                "please disconnect your mouse from the included micro-USB cable and restart this program again. " +
                "Otherwise, proceed to begin the firmware update.";

            var title = "Finalmouse Firmware Update Utility";

            MessageBox.Show(text, title);

            foreach (var firmware in firmwareDictionary)
            {
                comboBox1.Items.Add(firmware.Value);
            }

            comboBox1.SelectedIndex = firmwareDictionary.Count - 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var updatedPorts = SerialPort.GetPortNames().ToList();

            dfuPorts = updatedPorts.Where(x => !currentPorts.Contains(x)).ToList();

            if (dfuPorts.Count() > 0)
            {
                var msgText = "Found device in DFU mode. Click on the Update button below to begin the firmware update.";
                var msgTitle = "DFU Device Found";

                MessageBox.Show(msgText, msgTitle);

                label1.Text = $"Status: Current device in DFU Mode: {dfuPorts.First()}.\nClick on \"Update Firmware\" to begin the update.";

                button2.Enabled = true;
            }
            else
            {
                label1.Text = $"Status: No devices found in DFU Mode.";

                var msgText = "No devices found in DFU mode. Please make sure you have done the following:\n\n" +
                                "1. Unplug the wireless receiver from the micro-USB cable and turn the mouse off.\n" +
                                "2. Hold down M3 (middle-click) and the DPI button.\n" +
                                "3. Plug in your Starlight-12 with the included micro-USB cable while performing (2).";
                var msgTitle = "No Devices Found";

                MessageBox.Show(msgText, msgTitle);

                button2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var selectedFw = firmwareDictionary.ElementAt(comboBox1.SelectedIndex).Key;

            var startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                WorkingDirectory = Directory.GetCurrentDirectory() + "\\Resources",
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal,
                FileName = "cmd.exe",
                Arguments = $"/c nrfutil dfu usb-serial -pkg {selectedFw} -p {dfuPorts.First()} -b 115200 & PAUSE",
                RedirectStandardInput = false,
                UseShellExecute = false
            };

            var proc = new System.Diagnostics.Process();
            proc.StartInfo = startInfo;
            proc.Start();

            button2.Enabled = false;

            label1.Text = $"Status: No devices found in DFU Mode.";
        }

        private static Dictionary<string, string> GetFirmwareDictionary()
        {
            var firmwareDictionary = new Dictionary<string, string>();

            firmwareDictionary.Add("fm6_dfu_package_1.3.1_competition.zip", "Competition+");

            return firmwareDictionary;
        }
    }
}