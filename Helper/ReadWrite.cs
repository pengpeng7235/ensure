using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using Measure.Logers;

namespace Measure.Models
{
    static class ReadWrite
    {

        static string backupDir;
        public const string WIDTHCONFIG = "光幕宽度配置.json";
        public const string HEIGHTCONFIG = "光幕高度配置.json";
        public const string SCALECONFIG = "电子秤配置.json";
        public const string ROLLERCONFIG = "滚筒配置.json";

        /// <summary>
        /// 保存配置问件
        /// </summary>
        /// <param name="curtainHeightResult">光幕的高度通信配置</param>
        /// <param name="curtainWidthFile">光幕的宽度通信配置</param>
        /// <param name="rollerFile">滚筒的通信配置</param>
        /// <param name="ScaleFile">电子秤的通信配置</param>
        /// <returns>啥都没有</returns>
        public static async Task WriteToLocal
            (string curtainHeightResult,
            string curtainWidthFile,
            string rollerFile,
            string ScaleFile
            )
        {
            await Task.Run(() =>
           {
               try
               {
                   // 当前应用程序根目录
                   var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                   //文件名
                   backupDir = Path.Combine(baseDirectory, "设备配置文件");


                   if (!Directory.Exists(backupDir))
                       Directory.CreateDirectory(backupDir);

                   var width = Path.Combine(backupDir, WIDTHCONFIG);
                   var height = Path.Combine(backupDir, HEIGHTCONFIG);
                   var scale = Path.Combine(backupDir, SCALECONFIG);
                   var roller = Path.Combine(backupDir, ROLLERCONFIG);

                   var curtainHeightResultJson = JsonConvert.SerializeObject(curtainHeightResult, Formatting.Indented);
                   var curtainWidthResultJson = JsonConvert.SerializeObject(curtainWidthFile, Formatting.Indented);
                   var rollerResultJson = JsonConvert.SerializeObject(rollerFile, Formatting.Indented);
                   var scaleResultJson = JsonConvert.SerializeObject(ScaleFile, Formatting.Indented);

                   File.WriteAllText(width, curtainHeightResultJson);
                   File.WriteAllText(height, curtainWidthResultJson);
                   File.WriteAllText(scale, rollerResultJson);
                   File.WriteAllText(roller, scaleResultJson);

                   Mylog.logger.Info($"光幕宽度配置已保存到本地缓存路径：{width}");
                   Mylog.logger.Info($"光幕高度配置已保存到本地缓存路径：{height}");
                   Mylog.logger.Info($"电子秤配置已保存到本地缓存路径：{scale}");
                   Mylog.logger.Info($"滚筒配置已保存到本地缓存路径：{roller}");
                   //Mylog.logger.Info("滚筒配置已保存到本地缓存路径"+roller);

               }
               catch (Exception ex)
               {
                   Mylog.logger.Warn(ex.Message, "保存文件也会出错？？？", "我是菜鸡");

               }
           });
        }
        /// <summary>
        /// 返回json文件
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static string ReadFromLocal(string searchPattern)
        {
            // 查找最新的“光幕宽度配置”文件

            var files = Directory.GetFiles(backupDir, searchPattern)
                                 .Select(f => new FileInfo(f))
                                 .OrderByDescending(f => f.CreationTime)
                                 .FirstOrDefault();


            if (files != null)
            {
                string json = File.ReadAllText(files.FullName, Encoding.UTF8);
                var config = JsonConvert.DeserializeObject<string>(json);
                return config;
            }
            return null;
        }
    }




}
