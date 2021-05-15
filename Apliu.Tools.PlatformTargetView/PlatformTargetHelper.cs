using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Apliu.Tools.PlatformTargetView
{
    public enum PlatformTarget
    {
        None,
        AnyCpu,
        x86,
        x64 = 523,
        x86_AnyCpu = 267,
    }

    public enum CheckType
    {
        Assembly,
        PEArchitecture,
        PENoneFramework,
        VsDumpbin,
    }

    public class FilePlatformTarget
    {
        public string FileName { get; set; }
        public PlatformTarget PlatformTarget { get; set; }
        public CheckType CheckType { get; set; }
        public string MD5 { get; set; }
    }

    public class PlatformTargetHelper
    {
        private static string DumpbinPath;
        private static readonly Dictionary<string, FilePlatformTarget> FileTargetMap;
        static PlatformTargetHelper()
        {
            FileTargetMap = new Dictionary<string, FilePlatformTarget>();
        }

        /// <summary>
        /// 初始化Vs dumpbin.exe 文件路径
        /// </summary>
        /// <param name="dumpbinPath"></param>
        public static void InitVsDumpbinPath(string dumpbinPath)
        {
            DumpbinPath = dumpbinPath;
        }

        /// <summary>
        /// 获取指定文件的架构类型
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FilePlatformTarget GetPlatformTarget(string filePath)
        {
            var fileTarget = new FilePlatformTarget()
            {
                FileName = filePath,
                PlatformTarget = PlatformTarget.None,
            };

            try
            {
                PortableExecutableKinds peKind;
                ImageFileMachine machine;

                //var appDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
                var bytes = File.ReadAllBytes(filePath);
                fileTarget.MD5 = FileMd5.GetMD5(bytes);
                //var assembly = appDomain.Load(bytes);
                var assembly = Assembly.ReflectionOnlyLoad(bytes);
                assembly.ManifestModule.GetPEKind(out peKind, out machine);
                //AppDomain.Unload(appDomain);

                if (peKind == PortableExecutableKinds.ILOnly && machine == ImageFileMachine.I386)
                {
                    fileTarget.PlatformTarget = PlatformTarget.AnyCpu;
                }
                else if (machine == ImageFileMachine.AMD64 || machine == ImageFileMachine.IA64)
                {
                    fileTarget.PlatformTarget = PlatformTarget.x64;
                }
                else if (peKind == (PortableExecutableKinds.ILOnly | PortableExecutableKinds.Required32Bit) && machine == ImageFileMachine.I386)
                {
                    fileTarget.PlatformTarget = PlatformTarget.x86;
                }
                else if (peKind == PortableExecutableKinds.Required32Bit && machine == ImageFileMachine.I386)
                {
                    fileTarget.PlatformTarget = PlatformTarget.x86;
                }
                else
                {
                    fileTarget.PlatformTarget = PlatformTarget.None;
                }

                fileTarget.CheckType = CheckType.Assembly;
                if (!FileTargetMap.ContainsKey(fileTarget.MD5))
                {
                    FileTargetMap.Add(fileTarget.MD5, fileTarget);
                }
            }
            catch (FileLoadException ex)
            {
                //重复加载时会出现异常, 需要从原结果集中获取数据
                if (FileTargetMap.ContainsKey(fileTarget.MD5))
                {
                    fileTarget = FileTargetMap[fileTarget.MD5];
                }
            }
            catch (Exception ex)
            {
            }

            if (fileTarget.PlatformTarget == PlatformTarget.None)
            {
                fileTarget.PlatformTarget = (PlatformTarget)GetPEArchitecture(filePath);
                fileTarget.CheckType = CheckType.PEArchitecture;
                if (fileTarget.PlatformTarget == PlatformTarget.x86_AnyCpu)
                {
                    var fileStr = File.ReadAllText(filePath);
                    bool isFramework = fileStr.Contains("TargetFramewor") && fileStr.Contains(".NET Framework");
                    //只有Framework框架的应用才可能是AnyCpu
                    if (!isFramework)
                    {
                        fileTarget.CheckType = CheckType.PENoneFramework;
                        fileTarget.PlatformTarget = PlatformTarget.x86;
                    }
                }
                if (fileTarget.PlatformTarget == PlatformTarget.None || fileTarget.PlatformTarget == PlatformTarget.x86_AnyCpu)
                {
                    //避免因找不到dumpbin而覆盖了pe解析
                    var tempTarget = GetDumpbinHeadersTarget(filePath);
                    if (tempTarget != PlatformTarget.None)
                    {
                        fileTarget.CheckType = CheckType.VsDumpbin;
                        fileTarget.PlatformTarget = tempTarget;
                    }
                }
            }

            return fileTarget;
        }

        private static PlatformTarget GetDumpbinHeadersTarget(string filePath)
        {
            var target = PlatformTarget.None;
            var dumpbinPath = DumpbinPath;
            if (!string.IsNullOrEmpty(dumpbinPath) && File.Exists(dumpbinPath))
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.CreateNoWindow = true;         // 不创建新窗口    
                process.StartInfo.UseShellExecute = false;       //不启用shell启动进程  
                process.StartInfo.RedirectStandardInput = true;  // 重定向输入    
                process.StartInfo.RedirectStandardOutput = true; // 重定向标准输出    
                process.StartInfo.RedirectStandardError = true;  // 重定向错误输出
                process.Start();
                process.StandardInput.WriteLine("\"" + dumpbinPath + "\" /headers \"" + filePath + "\" " + "&exit"); //向cmd窗口发送输入信息  
                process.StandardInput.AutoFlush = true;
                var PESignature = process.StandardOutput.ReadToEnd();//获取返回值  
                process.WaitForExit();
                process.Close();

                if (PESignature.Contains("8664 machine (x64)"))
                {
                    target = PlatformTarget.x64;
                }
                else if (PESignature.Contains("14C machine (x86)") && PESignature.Contains("Application can handle large (>2GB) addresses"))
                {
                    target = PlatformTarget.AnyCpu;
                }
                else if (PESignature.Contains("14C machine (x86)") && PESignature.Contains("32 bit word machine"))
                {
                    target = PlatformTarget.x86;
                }
            }
            return target;
        }

        /// <summary>
        /// 获取指定文件的架构位数 523-64位, 267-32位/AnyCpu
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static ushort GetPEArchitecture(string filePath)
        {
            ushort architecture = 0;
            try
            {
                using (System.IO.FileStream fStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    using (System.IO.BinaryReader bReader = new System.IO.BinaryReader(fStream))
                    {
                        if (bReader.ReadUInt16() == 23117) //check the MZ signature
                        {
                            //Any CPU PE32 with 32BIT = 0
                            //x86 PE32 with 32BIT = 1
                            //x64 / Itanium(IA - 64) PE32 + with 32BIT = 0

                            fStream.Seek(0x3A, System.IO.SeekOrigin.Current); //seek to e_lfanew.
                            fStream.Seek(bReader.ReadUInt32(), System.IO.SeekOrigin.Begin); //seek to the start of the NT header.
                            if (bReader.ReadUInt32() == 17744) //check the PE\0\0 signature.
                            {
                                fStream.Seek(20, System.IO.SeekOrigin.Current); //seek past the file header,
                                architecture = bReader.ReadUInt16(); //read the magic number of the optional header.
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                /* TODO: Any exception handling you want to do, personally I just take 0 as a sign of failure */
            }
            //if architecture returns 0, there has been an error.
            return architecture;
        }
    }
}
