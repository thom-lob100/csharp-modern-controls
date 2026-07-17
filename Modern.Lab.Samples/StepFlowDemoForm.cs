using System;
using System.Data;
using System.Windows.Forms;

namespace Modern.Lab.Samples
{
    /// <summary>
    /// ModernStepIndicator 데모 — 스텝 수가 늘어나거나 가용 폭이 줄어들 때의
    /// 자동 강등 사다리(균등 축소 → 컴팩트 → 중간 접기)를 한 화면에서 확인한다.
    ///
    /// - Standard(5) / Long(12) / Very long(24) / Failed(16): 같은 폭에서 스텝
    ///   수에 따라 표시가 어떻게 달라지는지 비교. 24스텝은 이 폭에서도 이미
    ///   균등 축소·컴팩트로 내려간다.
    /// - Width test: 슬라이더로 컨테이너 폭을 140~1490px로 조절하며 모드 전환을
    ///   실시간으로 본다 — 어느 폭에서도 첫/현재/마지막 단계는 잘리지 않는다.
    ///   숨은 레이블은 노드에 마우스를 올리면 툴팁으로 확인된다.
    /// </summary>
    public partial class StepFlowDemoForm : Form
    {
        public StepFlowDemoForm()
        {
            this.InitializeComponent();

            // 컨테이너 폭 = min(슬라이더 값, 카드 가용 폭) — 슬라이더로도 줄일 수
            // 있고, 창을 줄여도 패널이 카드 밖으로 잘리지 않고 함께 줄어든다.
            this.trackWidth.ValueChanged += this.OnHostWidthChanged;
            this.cardResize.Resize += this.OnHostWidthChanged;

            this.LoadSampleFlows();
            this.OnHostWidthChanged(this, EventArgs.Empty);
        }

        // 각 인디케이터에 서로 다른 길이/상태 구성의 데모 흐름을 채운다.
        private void LoadSampleFlows()
        {
            // 표준 5단계 — 기존 화면들과 같은 짧은 수명주기 흐름.
            this.stepStd.DataSource = BuildFlow(
                    new string[] { "Created", "Released", "Job Prep", "Job Start", "Job End" },
                    3, 0);

            // 12단계 — SMT 라인 공정. 이 폭에서는 균등 축소(말줄임) 영역.
            this.stepLong.DataSource = BuildFlow(
                    new string[]
                    {
                        "Receiving", "Incoming QC", "Kitting", "SMT Top", "SMT Bottom",
                        "Reflow Inspection (AOI)", "Assembly", "Screw Fastening",
                        "Functional Test", "Burn-in", "Final QC", "Packing"
                    },
                    8, 0);

            // 24단계 — 긴 공정 라우트. 이 폭에서도 컴팩트/접힘까지 내려간다.
            string[] route = new string[24];

            for (int index = 0; index < route.Length; index++)
            {
                route[index] = "OP-" + ((index + 1) * 10).ToString("000") + " " + RouteName(index);
            }

            this.stepVeryLong.DataSource = BuildFlow(route, 15, 0);

            // 16단계 + 11단계 실패 — 실패 노드(X)가 기준 단계가 되는 구성.
            string[] failedRoute = new string[16];

            for (int index = 0; index < failedRoute.Length; index++)
            {
                failedRoute[index] = "Step " + (index + 1).ToString() + " — " + RouteName(index);
            }

            this.stepFailed.DataSource = BuildFlow(failedRoute, 0, 11);

            // 폭 테스트 14단계 — 슬라이더로 컨테이너 폭을 조절하며 모드 전환 확인.
            string[] resizeRoute = new string[14];

            for (int index = 0; index < resizeRoute.Length; index++)
            {
                resizeRoute[index] = RouteName(index);
            }

            this.stepResize.DataSource = BuildFlow(resizeRoute, 9, 0);
        }

        // 인디케이터 컨테이너 폭 = min(슬라이더 값, 카드 가용 폭).
        // 카드 제목에 실제 적용된 폭을 함께 표기한다.
        private void OnHostWidthChanged(object sender, EventArgs e)
        {
            int available = this.cardResize.ClientSize.Width
                    - this.cardResize.Padding.Left - this.cardResize.Padding.Right;
            int applied = Math.Max(120, Math.Min(this.trackWidth.Value, available));

            this.hostPanel.Width = applied;
            this.cardResize.Text = "Width test — 14 steps, container " + applied.ToString() + "px";
        }

        // 데모용 공정 이름 순환 목록 — 짧은 이름과 긴 이름을 섞어 말줄임도 보이게 한다.
        private static string RouteName(int index)
        {
            string[] names =
            {
                "Laser Marking", "Cleaning", "Coating", "Exposure", "Developing",
                "Etching", "Strip", "Inspection", "Deposition", "CMP",
                "Ion Implant", "Anneal", "Metrology", "Sort Test"
            };

            return names[index % names.Length];
        }

        // 단계 이름 배열로 LABEL/STATE DataTable을 만든다.
        // currentStep: 1-기준 현재 단계 번호(0 = 없음), 앞은 Completed, 뒤는 Pending.
        // failedStep: 1-기준 실패 단계 번호(0 = 없음) — 지정 시 currentStep은 무시된다.
        private static DataTable BuildFlow(string[] labels, int currentStep, int failedStep)
        {
            DataTable table = new DataTable();
            table.Columns.Add("LABEL", typeof(string));
            table.Columns.Add("STATE", typeof(string));

            int marker = failedStep > 0 ? failedStep : currentStep;

            for (int index = 0; index < labels.Length; index++)
            {
                int stepNumber = index + 1;
                string state;

                if (stepNumber < marker)
                {
                    state = "Completed";
                }
                else if (stepNumber == marker)
                {
                    state = failedStep > 0 ? "Failed" : "Current";
                }
                else
                {
                    state = "Pending";
                }

                table.Rows.Add(labels[index], state);
            }

            return table;
        }
    }
}
