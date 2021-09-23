﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfProgressDialog
{
    /// <summary>
    /// ProgressDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ProgressDialog : Window
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        const int GWL_STYLE = -16;
        const int WS_SYSMENU = 0x80000;

        private BackgroundWorker worker = new BackgroundWorker();

        private Action action;

        private CancellationTokenSource cancelToken;

        private bool isCanceled = false;
        public bool IsCanceled
        {
            get
            {
                return isCanceled;
            }
        }


        public ProgressDialog(object context, Action action, CancellationTokenSource cancelToken)
        {
            InitializeComponent();

            DataContext = context;
            this.action = action;
            this.cancelToken = cancelToken;

            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr handle = new WindowInteropHelper(this).Handle;
            int style = GetWindowLong(handle, GWL_STYLE);
            style = style & ~WS_SYSMENU;
            SetWindowLong(handle, GWL_STYLE, style);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if(action == null)
            {
                return;
            }

            Task task = Task.Factory.StartNew((obj) =>
            {
                action.Invoke();
            }, cancelToken);

            task.Wait();
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            cancelToken.Cancel();
            isCanceled = true;
        }
    }
}
