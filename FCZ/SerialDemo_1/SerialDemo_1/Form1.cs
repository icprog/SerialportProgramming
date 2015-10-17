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
using System.IO;


namespace SerialDemo_1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		//定义一个串口对象
		SerialPort sp = new SerialPort();

		//初始化各个参数变量的值
		public static string strPortName = "";
		public static string strBaudRate = "";
		public static string strDataBits = "";
		public static string strStopBits = "";
//变量声明

		//是否处于发送状态
		bool IsSended = false;
		//定时器
		Timer AutoSendTimer = new Timer();

		//接收的数据字符串
		StringBuilder Recivestr = new StringBuilder();
		
		

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load(object sender, EventArgs e)
		{
			string[] ports = SerialPort.GetPortNames();
			Array.Sort(ports);
			cmbPorts.Items.AddRange(ports);
			sp.DataReceived += ReciveData;
			InitParam();
			AutoSendTimer.Tick  += new EventHandler(AutoSend);

			BindingSource bsYear = new BindingSource();
			bsYear.DataSource = InfoDictionary.dicYear;
			cmbYear.DataSource = bsYear;
			cmbYear.DisplayMember = "Value";
			cmbYear.ValueMember = "Key";



			BindingSource bsMonth = new BindingSource();
			bsMonth.DataSource = InfoDictionary.dicMonth;
			cmbMonth.DataSource = bsMonth;
			cmbMonth.DisplayMember = "Value";
			cmbMonth.ValueMember = "Key";

			BindingSource bsDay = new BindingSource();
			bsDay.DataSource = InfoDictionary.dicDay;
			cmbDay.DataSource = bsDay;
			cmbDay.DisplayMember = "Value";
			cmbDay.ValueMember = "Key";

			BindingSource bsHour = new BindingSource();
			bsHour.DataSource = InfoDictionary.dicHour;
			cmbHour.DataSource = bsHour;
			cmbHour.DisplayMember = "Value";
			cmbHour.ValueMember = "Key";

			BindingSource bsMinute = new BindingSource();
			bsMinute.DataSource = InfoDictionary.dicMinute;
			cmbMinute.DataSource = bsMinute;
			cmbMinute.DisplayMember = "Value";
			cmbMinute.ValueMember = "Key";

			BindingSource bsLang = new BindingSource();
			bsLang.DataSource = InfoDictionary.dicLang;
			cmbLang.DataSource = bsLang;
			cmbLang.DisplayMember = "Value";
			cmbLang.ValueMember = "Key";

		}

		//private void AddSysyemConfig(Dictionary<byte , int > dic)


		/// <summary>
		/// 设置参数,将参数赋值到串口中
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSetParam_Click(object sender, EventArgs e)
		{
			try
			{
				strPortName = cmbPorts.Text;
				strBaudRate = cmbBaudRate.Text;
				strDataBits = cmbDataBit.Text;
				strStopBits = cmbStopBit.Text;

				sp.PortName = strPortName;
				sp.BaudRate = Convert.ToInt32(strBaudRate);
				sp.DataBits = Convert.ToByte(strDataBits);
				sp.StopBits = StopBits.One;
				sp.ReadTimeout = 500;

				sp.Open();

				txtRecive.AppendText("串口成功开启!\n");
			}
			catch (Exception ex)
			{
				txtRecive.AppendText("异常: " + ex.Message);
				txtRecive.AppendText("\r\n");
			}
		}

		/// <summary>
		/// 初始化参数函数
		/// </summary>
		private void InitParam()
		{
			try
			{
				cmbPorts.SelectedIndex = 0;
				cmbBaudRate.SelectedIndex = 0;
				cmbDataBit.SelectedIndex = 0;
				cmbParity.SelectedIndex = 0;
				cmbStopBit.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				txtRecive.AppendText("异常:" + ex.Message + "\r\n");
			}
			
		}

		/// <summary>
		/// 关闭串口
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnClose_Click(object sender, EventArgs e)
		{
			if (sp.IsOpen)
			{
				sp.Close();
				txtRecive.AppendText("串口已关闭!\r\n");
			}
				
		}

		/// <summary>
		/// 退出程序
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnExit_Click(object sender, EventArgs e)
		{
			if(sp.IsOpen)
				sp.Close();
			this.Close();
		}

		/// <summary>
		/// 发送数据
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSend_Click(object sender, EventArgs e)
		{
			
				if (ckbAutoSend.Checked)
				{
					if (!IsSended)
					{
						btnSend.Text = "停止";
						int interval;
						if( ! int.TryParse(txtTimeCell.Text, out interval))
							interval = 1;
						AutoSendTimer.Interval = 1000 * interval;
						AutoSendTimer.Start();
						IsSended = true;
					}
					else
					{
						btnSend.Text = "发送";
						AutoSendTimer.Stop();
						txtSend.Clear();
						IsSended = false;
					}
				}
				else
				{
					try 
					{
						Send(txtSend.Text);
						txtRecive.AppendText(txtSend.Text + Environment.NewLine);
						txtSend.Clear();
					}
					catch (Exception ex) { txtRecive.AppendText("异常: " + ex.Message + "\r\n"); }
				}
		}


		/// <summary>
		/// 发送数据
		/// </summary>
		private void Send(String str)
		{
			if (sp != null)
			{
				str = str.Replace(" ", "");
				byte[] Sendbyte = new byte[str.Length / 2];
				for (int i = 0, j = 0; i < str.Length; i = i + 2, j++)
				{
					string mysubstring = str.Substring(i, 2);
					Sendbyte[j] = Convert.ToByte(mysubstring, 16);
				}

				sp.Write(Sendbyte, 0, Sendbyte.Length);
			}
		}

		/// <summary>
		/// 自动发送的内容
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AutoSend(object sender, EventArgs e) 
		{
			try 
			{
				Send(txtSend.Text);
				txtRecive.AppendText(txtSend.Text + Environment.NewLine);
			}
			catch (Exception ex) { txtRecive.AppendText("异常: " + ex.Message + "\r\n"); }
		}

		/// <summary>
		/// 初始化参数
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnInit_Click(object sender, EventArgs e)
		{
			InitParam();
		}

		/// <summary>
		/// 接收数据线程
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ReciveData(object sender, SerialDataReceivedEventArgs e)
		{
				DateTime dt = DateTime.Now;
				int n = sp.BytesToRead;			//缓冲区中的字节数
				byte[] buffer = new byte[n];
				sp.Read(buffer, 0, n);
				this.Invoke((EventHandler)(delegate
				{
					txtRecive.AppendText(dt.ToString() +" "+ TranString(buffer));
					txtRecive.AppendText("\r\n");
					
				}));
				FileStream fs = new FileStream(@"E:\光伏\串口\Data\" + "1.txt", FileMode.OpenOrCreate, FileAccess.Write);
				fs.Position = fs.Length;                  //将待写入内容追加到文件末尾  
				StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);//(fs, System.Text.Encoding.GetEncoding("GB2312"));
				sw.Flush();
				sw.BaseStream.Seek(fs.Position, SeekOrigin.Begin);
				sw.WriteLine(dt.ToString() + " " + TranString(buffer));
				sw.Flush();
				sw.Close();
		}

		/// <summary>
		/// 将接收到的数据转化成相应字符串
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		private StringBuilder  TranString(byte[] buffer)
		{
			Recivestr.Clear();
			//依次的拼接出16进制字符串  
			foreach (byte b in buffer)
			{
				Recivestr.Append(b.ToString("X2") + " ");
			}
			return Recivestr;
		}

		/// <summary>
		/// 是否循环发送
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ckbAutoSend_CheckedChanged(object sender, EventArgs e)
		{
			if(ckbAutoSend.Checked)
			{
				txtTimeCell.ReadOnly = false;
				btnSend.Text = "发送";
				AutoSendTimer.Stop();
				AutoSendTimer.Dispose();
				IsSended = false;
			}
			else
			{
				txtTimeCell.ReadOnly = true;
			}
		}

		private void btnReciveClear_Click(object sender, EventArgs e)
		{
			txtRecive.Clear();
		}
//====================    快捷键设置      ======================================================
		string strWeatherInfo = "01 03 00 00 F1 D8";
		string strHistoryWeather = "01 03 00 37 B0 0E";
		string strSystemInfo = "01 03 00 20 F0 00";
		string strPasswordInfo = "01 03 00 61 00 04 15 d7";

		private void button1_Click(object sender, EventArgs e)
		{
			Send(strWeatherInfo);
			txtRecive.AppendText(strWeatherInfo + "\r\n");
		}

		private void btnHistoryWeather_Click(object sender, EventArgs e)
		{
			Send(strHistoryWeather);
			txtRecive.AppendText(strHistoryWeather + "\r\n");
		}

		private void btnSysConfig_Click(object sender, EventArgs e)
		{
			Send(strSystemInfo);
			txtRecive.AppendText(strSystemInfo + "\r\n");
		}

		private void btnPassWord_Click(object sender, EventArgs e)
		{
			Send(strPasswordInfo);
			txtRecive.AppendText(strPasswordInfo + "\r\n");
		}

		private void btnSetSystemConfig_Click(object sender, EventArgs e)
		{
			string SendMsg = "01 10 00 20 00 04 08 "
						+ValueToString(cmbYear.SelectedValue) + " "
						+ ValueToString(cmbMonth.SelectedValue) + " "
						+ ValueToString(cmbDay.SelectedValue) + " "
						+ ValueToString(cmbHour.SelectedValue) + " "
						+ ValueToString(cmbMinute.SelectedValue) + " "
						+ ValueToString(txtSaveTime.Text) + " "
						+ ValueToString(cmbLang.SelectedValue)
						+ " 00 59 DD";
			Send(SendMsg);

			txtRecive.AppendText(SendMsg + "\r\n");
		}


//=============================================================================================
		private string ValueToString(object obj)
		{
			string str = Convert.ToString(Convert.ToInt32(obj), 16);
			if (str.Length%2 == 1)
			{
				str = "0" + str;
			}
			return str;
		}

//======================   设置密码   ==========================================

		string pswStr = "";

		private void btnPsw0_Click(object sender, EventArgs e)
		{
			if (txtPsw.Text.Length >= 4) { return; }
			pswStr += "30";
			txtPsw.AppendText("0");
		}

		private void btnPsw1_Click(object sender, EventArgs e)
		{
			if (txtPsw.Text.Length >= 4) { return; }
			pswStr += "31";
			txtPsw.AppendText("1");
		}
		private void btnPsw2_Click(object sender, EventArgs e)
		{
			if (txtPsw.Text.Length >= 4) { return; }
			pswStr += "32";
			txtPsw.AppendText("2");
		}

		private void btnPsw3_Click(object sender, EventArgs e)
		{
			if (txtPsw.Text.Length >= 4) { return; }
			pswStr += "33";
			txtPsw.AppendText("3");
		}

		private void btnPsw4_Click(object sender, EventArgs e)
		{
			if (txtPsw.Text.Length >= 4) { return; }
			pswStr += "34";
			txtPsw.AppendText("4");
		}

		private void btnPsw5_Click(object sender, EventArgs e)
		{
			if (txtPsw.Text.Length >= 4) { return; }
			pswStr += "35";
			txtPsw.AppendText("5");
		}

		private void btnPsw6_Click(object sender, EventArgs e)
		{
			if (txtPsw.Text.Length >= 4) { return; }
			pswStr += "36";
			txtPsw.AppendText("6");
		}

		private void btnPsw7_Click(object sender, EventArgs e)
		{
			if (txtPsw.Text.Length >= 4) { return; }
			pswStr += "37";
			txtPsw.AppendText("7");
		}

		private void btnPsw8_Click(object sender, EventArgs e)
		{
			if (txtPsw.Text.Length >= 4) { return; }
			pswStr += "38";
			txtPsw.AppendText("8");
		}

		private void btnPsw9_Click(object sender, EventArgs e)
		{
			if (txtPsw.Text.Length >= 4) { return; }
			pswStr += "39";
			txtPsw.AppendText("9");
		}

		private void btnPswBack_Click(object sender, EventArgs e)
		{
			if (txtPsw.Text.Length > 0)
			{
				pswStr = pswStr.Remove(pswStr.Length - 2);
				txtPsw.Text = txtPsw.Text.Remove(txtPsw.Text.Length - 1);
			}
			else
			{
				txtPsw.Clear();
				pswStr = "";
			}
		}

		private void btnSetPsw_Click(object sender, EventArgs e)
		{
			if (pswStr.Length != 8 || txtPsw.Text.Length != 4)
			{
				MessageBox.Show("密码长度为三位,请重新输入", "错误", MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
			else
			{
				Send(pswStr);
				txtRecive.AppendText(pswStr+Environment.NewLine);
				pswStr = "";
				txtPsw.Clear();
				txtRecive.AppendText("设置密码成功!\r\n");
			}
		}

	}
}
