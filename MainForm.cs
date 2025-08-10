using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Measure.Forms;
using Measure.Helper;
using Measure.Logers;
using Measure.Models;
using NLog;
using Sunny.UI;

namespace Measure
{
    public partial class MainForm : UIForm
    {
        public static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public MainForm()
        {
            InitializeComponent();
            logger.Info("启动了程序");

        }
        private CancellationTokenSource _ctsTime = new CancellationTokenSource();

        private async void btn_Configure_Click(object sender, EventArgs e)
        {
            logger.Info("点击了设备配置选项");
            this.Hide();

            using (DeviceConfig deviceConfig = new DeviceConfig())
            {
                var result = deviceConfig.ShowDialog();

                if (deviceConfig.strings.Count > 0 && result != DialogResult.OK)
                {
                    var str = $"光母鸡宽度{deviceConfig.strings[0]}{Environment.NewLine}光母鸡高度{deviceConfig.strings[1]}{Environment.NewLine}滚筒的配置{deviceConfig.strings[2]},\r\n电子秤的配置{deviceConfig.strings[3]}";
                    var dialogResult = UIMessageBox.ShowMessageDialog(str, "是否保存当前配置", true, UIStyle.Colorful, true, UIMessageDialogButtons.Ok);

                    if (dialogResult == true)
                    {
                        await ReadWrite.WriteToLocal(deviceConfig.strings[0],
                             deviceConfig.strings[1],
                             deviceConfig.strings[2],
                             deviceConfig.strings[3]);
                        logger.Info("选择了保存当前配置");
                    }
                }
                else
                {
                    MessageBox.Show("点击了取消保存，你做的太对了;");
                    logger.Info("选择了取消保存当前配置");
                }
            }
            this.Show();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            //await Task.Delay(1000);
            await ShowTime(_ctsTime.Token);
           var config= ReadWrite.ReadFromLocal(ReadWrite.HEIGHTCONFIG);
        }

        private async Task ShowTime(CancellationToken ct)
        {
            try
            {
                await Task.Run(async () =>
                {
                    while (!ct.IsCancellationRequested)
                    {
                        this.DoOnUiThread(() =>
                        {
                            uiSymbolLabelTime.Text = DateTime.Now.ToString();
                        });
                        await Task.Delay(1000, ct);
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "时钟？");
            }

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _ctsTime?.Cancel();
            _ctsTime.Dispose();
            logger.Info("已经关闭了程序");
        }
    }
}
