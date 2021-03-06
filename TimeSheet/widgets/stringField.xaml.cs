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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeSheet
{
    /// <summary>
    /// stringField.xaml 的交互逻辑
    /// </summary>
    public partial class stringField : UserControl
    {
        public stringField()
        {
            InitializeComponent();
        }

        
        public string Lable
        {
            get
            {
                return m_value.Text;
            }
            set
            {
                m_value.Text = value;
            }
        }

        public string Text
        {
            get
            {
                return m_value.Text;
            }
            set
            {
                m_value.Text = value;
            }
        }
    }
}
