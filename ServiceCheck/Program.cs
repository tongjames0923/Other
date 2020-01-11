using System;
using System.Collections.Generic;
using System.ServiceProcess;
using SysTool.WindowsSystem;
using static System.Console;
using System.IO;

namespace ServiceCheck
{
    internal class Program
    {
        private static List<ServiceController> services = new List<ServiceController>();

        private static void Main(string[] args)
        {
            int c = 0;
            TakeStep step = new TakeStep();
            step.OnSelect += Step_OnSelect;
            while (step.Chiose != TakeStep.Step.退出)
            {
                step.Chiose = TakeStep.Step.无;
                WriteLine("0.查看现有服务\t1.安装服务\t2.卸载服务\t3.启动服务\t4.停止服务\t5退出");
                string t = ReadLine();
                if (int.TryParse(t, out c))
                {
                    switch (c)
                    {
                        case 0:
                            step.Chiose = TakeStep.Step.无;
                            ShowAll();
                            break;
                        case 1:
                            step.Chiose = TakeStep.Step.安装;
                            break;
                        case 2:
                            step.Chiose = TakeStep.Step.卸载;
                            break;
                        case 3:
                            step.Chiose = TakeStep.Step.启动;
                            break;
                        case 4:
                            step.Chiose = TakeStep.Step.停止;
                            break;
                        case 5:
                            step.Chiose = TakeStep.Step.退出;
                            break;
                        default:
                            WriteLine("输入有误");
                            break;
                    }
                }
                else
                {
                    WriteLine("输入有误");
                }
                WriteLine("<-------------------------------------------------------------------------------------------------->");
            }
            WriteLine("按任意键退出");
            ReadKey();
        }

        private static bool Comfirm(int index)
        {
            int ok = 0;
            bool res = false;
            while (ok == 0)
            {
                WriteLine("请确认操作对象：服务名称：{0}。", services[index].ServiceName);
                WriteLine("Y:确认\tN:取消");
                switch (ReadLine().ToUpper())
                {
                    case "Y":
                        ok = 1;
                        res = true;
                        break;
                    case "N":
                        ok = 1;
                        break;
                    default:
                        ok = 0;
                        break;
                }
            }
            return res;
        }

        private static void ShowAll()
        {
            int i = 0;
            foreach (ServiceController controller in services)
            {
                WriteLine("{0}:服务名称：{1}\t服务状态：{2}", i, controller.ServiceName, controller.Status);
                i++;
            }
        }
        private static void Step_OnSelect(object sender, string e)
        {
            ServiceCase service = new ServiceCase();
            switch (e)
            {
                case "无":
                    services.Clear();
                    foreach (ServiceController controller in service.GetServices())
                    {
                        services.Add(controller);
                    }
                    break;
                case "安装":
                    WriteLine("请输入服务程序的路径：");
                    try
                    {
                        service.InstallService(ReadLine());
                        WriteLine("安装完成");
                    }
                    catch
                    {
                        WriteLine("安装失败");
                    }
                    break;
                case "卸载":
                    ShowAll();
                    int c = 0;
                    WriteLine("请输入服务序号：");
                    if (int.TryParse(ReadLine(), out c) && c >= 0 && c <= services.Count)
                    {
                        if (Comfirm(c) == true)
                        {
                            string path = service.GetWindowsServiceInstallPath(services[c].ServiceName);
                          string[] allfiles=  Directory.GetFiles(path,"*.exe",SearchOption.TopDirectoryOnly);
                            for (int i = 0; i < allfiles.Length; i++)
                                WriteLine("{0}:{1}", i, allfiles[i]);
                            WriteLine("请输入序号以选择服务源文件：");
                            try
                            {
                                int tmp = int.Parse(ReadLine());
                                if (tmp >= 0 && tmp <= allfiles.Length)
                                {
                                    path =  new FileInfo(allfiles[tmp]).FullName;
                                    service.UninstallService(path);
                                    WriteLine("卸载完成");
                                }

                            }
                            catch
                            {
                                WriteLine("输入有误。");
                            }
                        }
                        else
                        {
                            WriteLine("已取消");
                        }
                    }
                    else
                    {
                        WriteLine("输入有误");
                    }
                    break;
                case "启动":
                    ShowAll();
                    c = 0;
                    WriteLine("请输入服务序号：");
                    if (int.TryParse(ReadLine(), out c) && c >= 0 && c <= services.Count)
                    {
                        if (Comfirm(c) == true)
                        {
                            if (!services[c].Status.Equals(ServiceControllerStatus.Running))
                            {
                                try
                                {
                                    service.ServiceStart(services[c].ServiceName);
                                    WriteLine("启动完成");
                                }
                                catch
                                {
                                    WriteLine("启动失败。");
                                }
                            }
                            else
                            {
                                WriteLine("该服务已启动");
                            }
                        }
                        else
                        {
                            WriteLine("已取消");
                        }
                    }
                    else
                    {
                        WriteLine("输入错误");
                    }
                    break;
                case "停止":
                    ShowAll();
                    c = 0;
                    WriteLine("请输入服务序号：");
                    if (int.TryParse(ReadLine(), out c) && c >= 0 && c <= services.Count)
                    {
                        if (Comfirm(c) == true)
                        {
                            if (services[c].Status.Equals(ServiceControllerStatus.Running))
                            {
                                try
                                {
                                    service.ServiceStop(services[c].ServiceName);
                                    WriteLine("停止完成");
                                }
                                catch
                                {
                                    WriteLine("停止失败。");
                                }
                            }
                            else
                            {
                                WriteLine("该服务已停止");
                            }
                        }
                        else
                        {
                            WriteLine("已取消");
                        }
                    }
                    else
                    {
                        WriteLine("输入错误");
                    }
                    break;
                default: break;
            }
        }
    }
    internal class TakeStep
    {
        internal event EventHandler<string> OnSelect;
        internal enum Step
        {
            无, 安装, 卸载, 启动, 停止, 退出
        }

        private Step step = Step.无;
        internal Step Chiose
        {
            set
            {
                step = value;
                if (!OnSelect.Equals(null))
                {
                    OnSelect.Invoke(this, step.ToString());
                }
            }
            get => step;
        }
    }
}
