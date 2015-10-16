﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO.Ports;

namespace SerialDemo_1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		SerialPort sp = new SerialPort();
		public static string strPortName = "";
		public static string strBaudRate = "";
		public static string strDataBits = "";
		public static string strStopBits = "";
		//public static string strPortName = "";


		private void Form1_Load(object sender, EventArgs e)
		{
			
		}

		private void btnSetParam_Click(object sender, EventArgs e)
		{
			strPortName = cmbPorts.Text;
			strBaudRate = cmbBaudRate.Text;
			strDataBits = cmbDataBit.Text;
			strStopBits = cmbStopBit.Text;

			sp.PortName = strPortName;
			sp.BaudRate = Convert.ToInt32(strBaudRate);
			sp.DataBits = Convert.ToByte(strDataBits);
			sp.StopBits = StopBits.One;

			//sp.ReadTimeout = 500;
		}

		private void btnOpen_Click(object sender, EventArgs e)
		{
			try
			{
				sp.Open();
			}
			catch (Exception ex)
			{
				txtRecive.AppendText("异常: " + ex.Message + "\r\n");
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			sp.Close();
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			sp.Close();
			this.Close();
		}

		private void btnSend_Click(object sender, EventArgs e)
		{
			sp.Write(txtSend.Text);
			string strRecivel = sp.ReadExisting();
			txtRecive.AppendText(strRecivel + Environment.NewLine); 
		}

		private void btnInit_Click(object sender, EventArgs e)
		{
			try
			{
				string[] ports = SerialPort.GetPortNames();
				foreach (string p in ports) { cmbPorts.Items.Add(p); }
				cmbPorts.SelectedIndex = 0;
				cmbBaudRate.SelectedIndex = 0;
				cmbDataBit.SelectedIndex = 0;
				cmbParity.SelectedIndex = 0;
				cmbStopBit.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				txtRecive.AppendText("异常:" + ex.Message + Environment.NewLine);
			}
		}
	}
}
