using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using Measure.Helper;
using Measure.Logers;
using Measure.Models;
using NLog;
using Sunny.UI;

namespace Measure.Forms
{
    public partial class DeviceConfig : XtraForm
    {

        public static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public List<string> strings = new List<string>();
        PortConfiguration curtainHeightJson= new PortConfiguration();
        PortConfiguration curtainWidthJson = new PortConfiguration();
        PortConfiguration rollerJson = new PortConfiguration();
        PortConfiguration scaleJson = new PortConfiguration();
        public DeviceConfig()
        {
            InitializeComponent();

            this.Load += DeviceConfig_Load;

        }


        private async void DeviceConfig_Load(object sender, EventArgs e)
        {
            await InitialElectronicScaleConfig();
            await InitialCurtainHeightConfig();
            await InitialCurtainWidthConfig();
            await InitialRollerConfig();
            logger.Info("设备配置窗体加载成功");
        }

        /// <summary>
        /// 滚筒参数
        /// </summary>
        private async Task InitialRollerConfig()
        {
            await Initial(comBoxRollerPort
                , comBoxRollerBaudRate
                , comBoxRollerDataBit
                , comBoxRollerParityBit
                , comBoxRollerStopBit);
        }
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="Port">控件的串口</param>
        /// <param name="baudRate">控件的波特率</param>
        /// <param name="dataBit">控件的数据位</param>
        /// <param name="parityBit">控件的校验位</param>
        /// <param name="stopBit">控件的停止位</param>
        /// <param name="tagline">我还得练</param>
        private async Task Initial(ComboBoxEdit Port,
            ComboBoxEdit baudRate,
            ComboBoxEdit dataBit,
            ComboBoxEdit parityBit,
            ComboBoxEdit stopBit,
            string tagline = "还可以优化")
        {

            try
            {
                await Task.Run(() =>
                {
                    int[] baudRates = { 9600, 19200, 38400, 57600, 115200 };
                    var DataBits = new int[] { 5, 6, 7, 8 };
                    this.DoOnUiThread(() =>
                    {
                        //串口
                        Port.Properties.Items.AddRange(SerialPort.GetPortNames());

                        Port.SelectedIndex = 0;


                        //波特率
                        baudRate.Properties.Items.AddRange(baudRates);

                        baudRate.SelectedIndex = 0;

                        //数据位
                        dataBit.Properties.Items.AddRange(DataBits);

                        dataBit.SelectedIndex = 0;
                        //strings[2] = dataBit.SelectedItem.ToString();

                        //校验位
                        parityBit.Properties.Items.AddRange(Enum.GetValues(typeof(Parity)));

                        parityBit.SelectedIndex = 0;
                        //strings[3] = parityBit.SelectedItem.ToString();

                        //停止位
                        var stopBitsValues = Enum.GetValues(typeof(StopBits));
                        stopBit.Properties.Items.AddRange(stopBitsValues);
                        //stopBit.Properties.Items.AddRange(Enum.GetValues(typeof(StopBits)));

                        stopBit.SelectedIndex = 0;
                        //strings[4] = stopBit.SelectedItem.ToString();

                    });
                });
            }
            catch (Exception ex)
            {
                Mylog.logger.Debug(ex, "串口添加出错？");
            }


        }

        /// <summary>
        /// 初始化光栅宽度配置
        /// </summary>
        private async Task InitialCurtainWidthConfig()
        {
            await Initial(comBoxCurtainWidthPort
                , comBoxCurtainWidthBaudRate
                , comBoxCurtainWidthDataBits
                , comBoxCurtainWidthParityBits
                , comBoxCurtainWidthStopBits);
        }
        /// <summary>
        /// 初始化光栅高度配置
        /// </summary>
        private async Task InitialCurtainHeightConfig()
        {
            await Initial(comBoxCurtainHeightPort
                , comBoxCurtainHeightBaudRate
                , comBoxCurtainHeightDataBit
                , comBoxCurtainHeighPrarityBit
                , comBoxCurtainHeightStopBit
                  );
        }
        /// <summary>
        /// 电子秤参数
        /// </summary>
        private async Task InitialElectronicScaleConfig()
        {
            await Initial(comBoxScaleSerialPort
                    , comBoxScaleBaudRate
                    , comBoxScaleDataBit
                    , comBoxScaleParityBit
                    , comBoxScaleStopBit
                    );

        }

        /// <summary>
        /// 保存所有设备的配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_ApplyConfig_Click(object sender, EventArgs e)
        {

            //电子秤的串口配置
            var scaleResult = GetDeviceConig(serialPortScale
                , comBoxScaleSerialPort
                , comBoxScaleBaudRate
                , comBoxScaleDataBit
                , comBoxScaleParityBit
                , comBoxScaleStopBit
                );
            //滚筒的串口配置
            var rollerResult = GetDeviceConig(serialPortRoller
                , comBoxRollerPort
                , comBoxRollerBaudRate
                , comBoxRollerDataBit
                , comBoxRollerParityBit
               , comBoxRollerStopBit
                 );
            //光栅高度的串口配置
            var curtainHeightResult = GetDeviceConig(serialPortCurtainHeight
                , comBoxCurtainHeightPort
                , comBoxCurtainHeightBaudRate
                , comBoxCurtainHeightDataBit
                , comBoxCurtainHeighPrarityBit
                , comBoxCurtainHeightStopBit
                  );
            //光栅宽度的串口配置
            var curtainWidthResult = GetDeviceConig(serialPortCurtainWidth
                   , comBoxCurtainWidthPort
                   , comBoxCurtainWidthBaudRate
                   , comBoxCurtainWidthDataBits
                   , comBoxCurtainWidthParityBits
                   , comBoxCurtainWidthStopBits
                     );



            logger.Info($"点击了保存配置\r\n 光栅高度的最新配置是{curtainHeightResult}"
                + Environment.NewLine + $"光栅宽度的最新配置是{curtainWidthResult}"
                + Environment.NewLine + $"滚筒的最新配置是{rollerResult}"
                + Environment.NewLine + $"电子秤的最新配置是{scaleResult} ");

            strings.Add(curtainHeightResult);
            strings.Add(curtainWidthResult);
            strings.Add(rollerResult);
            strings.Add(scaleResult);
            await ReadWrite.WriteToLocal(curtainHeightResult, curtainWidthResult, rollerResult, scaleResult);
            curtainHeightJson.PortNama = curtainHeightResult
             await ReadWrite.WriteToLocal()
            this.DialogResult = DialogResult.OK;
            //UIMessageBox.ShowAsk("已经保存");
            //UIMessageBox.ShowInfo("已经保存", true, 20);
        }
        private void DeviceConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            logger.Info($@"{this.Name}正在关闭");
            logger.Info($"{this.Name}is closing");
        }
        /// <summary>
        /// 返回每个设备的串口配置
        /// </summary>
        /// <param name="serialPort">设备对应的串口配置</param>
        /// <param name="Port">选择的串口</param>
        /// <param name="baudRate">选择的波特率</param>
        /// <param name="dataBit">选择的数据位</param>
        /// <param name="parityBit">选择的检验位</param>
        /// <param name="stopBit">选择的停止位<param>
        private SerialPort GetDeviceConig(SerialPort serialPort
            , ComboBoxEdit comBoxPort
            , ComboBoxEdit comBoxbaudRate
            , ComboBoxEdit comBoxdataBit
            , ComboBoxEdit comBoxparityBit
            , ComboBoxEdit comBoxstopBit)
        {

            serialPort.PortName = comBoxPort.SelectedItem.ToString();

            serialPort.BaudRate = (int)comBoxbaudRate.SelectedItem;

            serialPort.DataBits = (int)comBoxdataBit.SelectedItem;

            serialPort.Parity = (Parity)comBoxparityBit.SelectedItem;

            //EnumConverter enumConverter = new EnumConverter(typeof(StopBits));

            if (comBoxstopBit.SelectedItem is StopBits selectedStopBits)
            {
                serialPort.StopBits = comBoxstopBit.SelectedItem;
            }
            else
            {
                serialPort.StopBits = StopBits.None;
            }
            return serialPort;



            //    $"{serialPort.PortName}, 波特率={serialPort.BaudRate}, " +
            //$"数据位={serialPort.DataBits}, 校验位={serialPort.Parity}, 停止位={stopBit.SelectedItem}";
        }

        private void btnRefreshPort_Click(object sender, EventArgs e)
        {
            //串口

            RefreshPorts(comBoxScaleSerialPort);
            RefreshPorts(comBoxCurtainHeightPort);
            RefreshPorts(comBoxRollerPort);
            RefreshPorts(comBoxCurtainWidthPort);
        }

        private void RefreshPorts(ComboBoxEdit Port)
        {
            Port.Properties.Items.Clear();

            Port.Properties.Items.AddRange(SerialPort.GetPortNames());

        }

        private void uiHeaderButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}