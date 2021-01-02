﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Graphics.Capture;



using CaptureSampleCore;
using Composition.WindowsRuntimeHelpers;
using Windows.UI.Composition;
using System.Numerics;
using ContainerVisual = Windows.UI.Composition.ContainerVisual;
using CompositionTarget = Windows.UI.Composition.CompositionTarget;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace ScreenCapture
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Console.WriteLine("is ok?");       
        }

        public bool isClosed = false;

        private IntPtr hwnd;
        private Action callback;


        private BasicSampleApplication sample;

        private async Task test()
        {
            Console.WriteLine("@@@");
            var picker = new GraphicsCapturePicker();
            picker.SetWindow(hwnd);
            GraphicsCaptureItem item = await picker.PickSingleItemAsync();

            if(item != null)
            {
                IntPtr hWnd = GethWnd(item.DisplayName);

                if(hWnd == IntPtr.Zero)
                {
                    if (MessageBox.Show("해당 윈도우는 캡쳐할 수 없습니다" + System.Environment.NewLine + "윈도우가 활성화 되어 있는지 확인해 주세요", "오류", MessageBoxButton.OK)== MessageBoxResult.OK)
                    {
                        Close();
                    }
                    
                }
                else
                {

                    sample.StartCaptureFromItem(item, hWnd);
                }
            
               // callback();
            }
            else
            {
                Close();
            }

           
        }

        public void Start(Action callback)
        {
           
            //InitWindowList();
            //InitMonitorList();

            test();
        }

        public void DoCapture()
        {
            sample.StartDataCapture();
        }

        public bool GetData(ref byte[] array, ref int x, ref int y)
        {
            bool isSuccess = sample.GetData(ref array, ref x, ref y);         

            return isSuccess;
        }


        private IntPtr GethWnd(string name)
        {

            IntPtr hWnd = IntPtr.Zero;
            var processesWithWindows = from p in Process.GetProcesses()
                                       where !string.IsNullOrWhiteSpace(p.MainWindowTitle) && ScreenCapture.WindowEnumerationHelper.IsWindowValidForCapture(p.MainWindowHandle)
                                       select p;
            ObservableCollection<Process> processes = new ObservableCollection<Process>(processesWithWindows);


            foreach (var p in processes)
            {
                Console.WriteLine(p.MainWindowTitle);

                if (p.MainWindowTitle == name)
                {
                    hWnd = p.MainWindowHandle;
                    Console.WriteLine("!!!!!!GEt!!!!!!!!!!! " + p.MainWindowTitle);                }

                Console.WriteLine("Names " + p.MainWindowTitle);
            }

            return hWnd; //Should contain the handle but may be zero if the title doesn't match
        }




        private void InitComposition()
        {
            sample = new BasicSampleApplication();
        }

       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("is ok start?");
            var interopWindow = new WindowInteropHelper(this);
            hwnd = interopWindow.Handle;
            var presentationSource = PresentationSource.FromVisual(this);       
            InitComposition();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            isClosed = true;
            if (sample !=  null)
            {
                sample.Dispose();
            }
        }
    }
}
