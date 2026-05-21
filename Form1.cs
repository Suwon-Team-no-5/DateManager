using System;
using System.Windows.Forms;

namespace DateManager
{
    public partial class Form1 : Form
    {
        private Trainer donkeyTrainer = new Trainer();

        public Form1()
        {
            InitializeComponent();

            // 폼이 닫힐 때 안전장치 연결
            this.FormClosing += (s, e) => donkeyTrainer.KillProcess();

            // 이벤트 연결은 여기서 딱 한 번만 깔끔하게 등록!
            donkeyTrainer.LogReceived += (logText) => {
                this.Invoke((MethodInvoker)delegate {
                    rtbTrainLog.AppendText(logText);
                    rtbTrainLog.SelectionStart = rtbTrainLog.TextLength;
                    rtbTrainLog.ScrollToCaret();
                });
            };

            donkeyTrainer.TrainingFinished += () => {
                this.Invoke((MethodInvoker)delegate { btnStart.Enabled = true; });
            };
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            rtbTrainLog.Clear();
            rtbTrainLog.AppendText(" 학습 연동 테스트를 시작합니다...\r\n");

            btnStart.Enabled = false;


            string pythonPath = "wsl.exe";
            string mycarDir = "/home/jaeseo03/mycar";

            // 백그라운드에서 안전하게 학습 프로세스 
            await System.Threading.Tasks.Task.Run(() => donkeyTrainer.StartTraining(pythonPath, mycarDir));
        }
    }
}