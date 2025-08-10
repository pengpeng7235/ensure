using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Models
{
    public class PortConfiguration
    {
        // COM123456...
        public string PortNama { get; set; }
        // 波特率：9600, 115200 ...
        public int BaudRates { get; set; } 
        // 校验位 none
        public Parity Parity { get; set; }
        // 数据位：8
        public int DataBits { get; set; }
        // 停止位
        public static StopBits StopBits { get; set; }

        /// <summary>
        /// 获取有效的串口哈
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAvailablePorts()
        {

            try
            {
                var ports = SerialPort.GetPortNames();
                return new List<string>(ports);
            }
            catch
            {
                return new List<string>(); // 出错时返回空列表
            }
        }
    }
}


