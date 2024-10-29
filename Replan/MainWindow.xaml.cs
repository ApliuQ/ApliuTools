using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

namespace Replan
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        bool isload = false;
        bool isCustom = false;
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtReturnType.ItemsSource = System.Enum.GetValues(typeof(EnumReturnType));
            txtReturnType.SelectedItem = EnumReturnType.等额本金;
            txtLoanBackDayType.ItemsSource = System.Enum.GetValues(typeof(EnumLoanBackType));
            txtLoanBackDayType.SelectedItem = EnumLoanBackType.算头不算尾;
            txtInitialChargeType.ItemsSource = System.Enum.GetValues(typeof(EnumInitialChargeType));
            txtInitialChargeType.SelectedItem = EnumInitialChargeType.当月放款当期还款;
            txtInterestMonthOrDay.ItemsSource = System.Enum.GetValues(typeof(EnumInterestMonthOrDay));
            txtInterestMonthOrDay.SelectedItem = EnumInterestMonthOrDay.按日付息;

            //cmTypePlan.ItemsSource = System.Enum.GetValues(typeof(IousType));
            //cmTypeReturn.ItemsSource = System.Enum.GetValues(typeof(IousType));
            //cmTypeAdvance.ItemsSource = System.Enum.GetValues(typeof(IousType));
            //cmTypeExtend.ItemsSource = System.Enum.GetValues(typeof(IousType));

            Exhibition.Visibility = Visibility.Hidden;
            TQPrepayment.Visibility = Visibility.Hidden;
            Repayment.Visibility = Visibility.Hidden;
            ReturnCz.IsEnabled = false;

            colRetrieveAmount.Visibility = Visibility.Collapsed;
            colRetrieveInterest.Visibility = Visibility.Collapsed;
            colRetrieveIAddA.Visibility = Visibility.Collapsed;
            colRetrieveManagementFee.Visibility = Visibility.Collapsed;

            txtAdvanceDate.Text = DateTime.Now.ToShortDateString();
            AdvanceCz.IsEnabled = false;
            txtExtendDate.Text = DateTime.Now.ToShortDateString();
            ExtendCz.IsEnabled = false;

            setReadOnly(true);
            isload = true;
        }

        private void Alculate_Click(object sender, RoutedEventArgs e)
        {
            decimal PayAmount;
            decimal InterestOfMonth;
            int RepaymentNum;//总期次
            decimal InterestOfFee;
            DateTime Begindate;
            int RepaymentMonthNum = 1;//每期月数
            int InterestSetDay;
            string LoanBackDayType;
            string InitialChargeType;
            DateTime EndDate;
            LoanBackDayType = txtLoanBackDayType.Text.ToString();
            InitialChargeType = txtInitialChargeType.Text.ToString();

            //PlanGrid.ItemsSource = 等额本金ReturnRepaymentPlan(500000, 10, 6, 0, Convert.ToDateTime("2016-01-01"), 1, 0,
            //  EnumLoanBackType.算头又算尾.ToString(), EnumInitialChargeType.当月放款下期还款.ToString(),
            //  Convert.ToDateTime("2016-07-01"));
            //return;

            if (
            decimal.TryParse(txtPayAmount.Text, out PayAmount) &&
            decimal.TryParse(txtInterestOfMonth.Text, out InterestOfMonth) &&
            int.TryParse(txtRepaymentNum.Text, out RepaymentNum) &&
            decimal.TryParse(txtInterestOfFee.Text, out InterestOfFee) &&
            DateTime.TryParse(txtBegindate.Text, out Begindate) &&
            //int.TryParse(txtRepaymentMonthNum.Text,out RepaymentMonthNum)&&
            int.TryParse(txtInterestSetDay.Text, out InterestSetDay) &&
            DateTime.TryParse(txtEndDate.Text, out EndDate) &&
            !string.IsNullOrEmpty(LoanBackDayType) &&
            !string.IsNullOrEmpty(InitialChargeType))
            {
                if (EndDate < Begindate)
                {
                    MessageBox.Show("结束日期必须大于等于开始日期");
                    return;
                }

                if (isCustom)
                {
                    PlanGrid.ItemsSource = CalculatePlan.分期付息一次还本ReturnRepaymentPlan(PayAmount, InterestOfMonth, RepaymentNum, InterestOfFee,
                        Begindate, 1, InterestSetDay, LoanBackDayType, InitialChargeType, EndDate);
                    isCustom = false;
                    return;
                }
                switch ((EnumReturnType)txtReturnType.SelectedItem)
                {
                    case EnumReturnType.等额本金:
                        PlanGrid.ItemsSource = CalculatePlan.等额本金ReturnRepaymentPlan(PayAmount, InterestOfMonth, RepaymentNum, InterestOfFee,
    Begindate, 1, InterestSetDay, LoanBackDayType, InitialChargeType, EndDate);
                        break;
                    case EnumReturnType.等额本息:
                        PlanGrid.ItemsSource = CalculatePlan.等额本息ReturnRepaymentPlan(PayAmount, InterestOfMonth, RepaymentNum, InterestOfFee,
    Begindate, 1, InterestSetDay, LoanBackDayType, InitialChargeType, EndDate);
                        break;
                    case EnumReturnType.分期付息一次还本:
                        PlanGrid.ItemsSource = CalculatePlan.分期付息一次还本ReturnRepaymentPlan(PayAmount, InterestOfMonth, RepaymentNum, InterestOfFee,
    Begindate, 1, InterestSetDay, LoanBackDayType, InitialChargeType, EndDate);
                        break;
                    case EnumReturnType.到期一次性还本付息:
                        PlanGrid.ItemsSource = CalculatePlan.到期一次性还本付息ReturnRepaymentPlan(PayAmount, InterestOfMonth, RepaymentNum, InterestOfFee,
    Begindate, 1, InterestSetDay, LoanBackDayType, InitialChargeType, EndDate);
                        break;
                    case EnumReturnType.分期付息按还款计划表分期还本:
                        PlanGrid.ItemsSource = CalculatePlan.分期付息按还款计划表分期还本ReturnRepaymentPlan(PayAmount, InterestOfMonth, RepaymentNum, InterestOfFee,
    Begindate, 1, InterestSetDay, LoanBackDayType, InitialChargeType, EndDate, PlanGrid.ItemsSource);
                        break;
                    case EnumReturnType.自定义还款:
                        if (PlanGrid.ItemsSource == null) PlanGrid.ItemsSource = new ObservableCollection<AfterLoanRepaymentPlan> { };
                        MessageBox.Show("自定义还款系统不做计算！");
                        break;
                    default:
                        break;
                }
                ObservableCollection<AfterLoanRepaymentPlan> CurMainRP = PlanGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;
                txtRepaymentNum.Text = CurMainRP.Count.ToString();
                //if (CurMainRP.Count > 0) txtEndDate.Text = CurMainRP[CurMainRP.Count - 1].PlanDate.ToShortDateString();
            }
            else
            {
                MessageBox.Show("有必输项为空或者格式错误！");
            }
        }

        private void txtEditValueChanged(object sender, RoutedEventArgs e)
        {
            if (!isload) return;

            DateTime Begindate;
            DateTime EndDate;
            EnumReturnType LoanBackDayType = (EnumReturnType)txtReturnType.SelectedItem;
            EnumInitialChargeType InitialChargeType = (EnumInitialChargeType)txtInitialChargeType.SelectedItem;
            int InterestSetDay = 0;

            DateTime.TryParse(txtBegindate.Text, out Begindate);
            DateTime.TryParse(txtEndDate.Text, out EndDate);
            int.TryParse(txtInterestSetDay.Text, out InterestSetDay);

            #region
            int zjYear = EndDate.Year - Begindate.Year;
            int zjMonth = EndDate.Month - Begindate.Month;
            int zjDay = EndDate.Day - Begindate.Day;
            TimeSpan tSpan = Begindate - EndDate;
            #endregion

            int RepaymentNum = 0;
            switch (LoanBackDayType)
            {
                case EnumReturnType.到期一次性还本付息:
                    txtRepaymentNum.Text = "1";
                    setReadOnly(true);
                    break;
                default:
                    RepaymentNum = zjYear * 12 + zjMonth;
                    if (InitialChargeType == EnumInitialChargeType.当月放款当期还款)
                    {
                        if (InterestSetDay != 0)
                        {

                            if (Begindate.Day < InterestSetDay) RepaymentNum = RepaymentNum + 1;
                        }
                    }
                    if (InterestSetDay == 0)
                    {
                        if (zjDay > 0) RepaymentNum = RepaymentNum + 1;
                    }
                    else
                    {
                        if (EndDate.Day > InterestSetDay) RepaymentNum = RepaymentNum + 1;
                    }

                    txtRepaymentNum.Text = RepaymentNum.ToString();
                    setReadOnly(true);
                    break;
            }
            if (sender.Equals(this.txtReturnType) || sender.Equals(this.txtEndDate))
            {
                if (LoanBackDayType == EnumReturnType.分期付息按还款计划表分期还本)
                {
                    isCustom = true;
                    Alculate_Click(null, null);
                }
            }

            if (LoanBackDayType == EnumReturnType.自定义还款 ||
                LoanBackDayType == EnumReturnType.分期付息按还款计划表分期还本) setReadOnly(false);
            ContextMenu.Focus();
        }

        private void setReadOnly(bool isReadOnly)
        {
            foreach (var GridColumn in PlanGrid.Columns)
            {
                GridColumn.IsReadOnly = isReadOnly;
            }
            ContextMenu.Visibility = isReadOnly ? Visibility.Collapsed : Visibility.Visible;
        }

        private void MenuInsertItem_Click_1(object sender, RoutedEventArgs e)
        {
            EnumReturnType LoanBackDayType = (EnumReturnType)txtReturnType.SelectedItem;
            if (LoanBackDayType != EnumReturnType.分期付息按还款计划表分期还本 &&
                LoanBackDayType != EnumReturnType.自定义还款) return;

            ObservableCollection<AfterLoanRepaymentPlan> listOCALRP = this.PlanGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;
            try
            {
                int intCurALRP = listOCALRP.IndexOf((AfterLoanRepaymentPlan)PlanGrid.SelectedItem);
                if (intCurALRP >= 0) listOCALRP.Insert(intCurALRP, new AfterLoanRepaymentPlan());
                else listOCALRP.Add(new AfterLoanRepaymentPlan());
            }
            catch (Exception)
            {
                listOCALRP = new ObservableCollection<AfterLoanRepaymentPlan> { };
                listOCALRP.Add(new AfterLoanRepaymentPlan());
            }


            for (int i = 0; i < listOCALRP.Count; i++)
            {
                listOCALRP[i].Num = i + 1;
            }
            PlanGrid.ItemsSource = listOCALRP;

            txtRepaymentNum.Text = listOCALRP.Count.ToString();
            isCustom = false;
        }

        private void MenuDeletetem_Click_1(object sender, RoutedEventArgs e)
        {
            EnumReturnType LoanBackDayType = (EnumReturnType)txtReturnType.SelectedItem;
            if (LoanBackDayType != EnumReturnType.分期付息按还款计划表分期还本 &&
                LoanBackDayType != EnumReturnType.自定义还款) return;

            if (PlanGrid.SelectedItem == null || MessageBox.Show(this, "您将删除选择的记录,是否继续？", "提示", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                return;

            ObservableCollection<AfterLoanRepaymentPlan> listOCALRP = this.PlanGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;

            if (((AfterLoanRepaymentPlan)PlanGrid.SelectedItem).Type != IousType.未还款)
            {
                MessageBox.Show(this, "该期次" + ((AfterLoanRepaymentPlan)PlanGrid.SelectedItem).Type + ",不允许删除！");
                return;
            }
            listOCALRP.Remove((AfterLoanRepaymentPlan)PlanGrid.SelectedItem);

            for (int i = 0; i < listOCALRP.Count; i++)
            {
                listOCALRP[i].Num = i + 1;
            }
            PlanGrid.ItemsSource = listOCALRP;

            txtRepaymentNum.Text = listOCALRP.Count.ToString();
            isCustom = false;
        }

        private void txtInterestMonthOrDay_EditValueChanged_1(object sender, RoutedEventArgs e)
        {
            CalculatePlan.DefaultInterest = (EnumInterestMonthOrDay)txtInterestMonthOrDay.SelectedItem;
        }

        //展期
        private void Exhibition_Click_1(object sender, RoutedEventArgs e)
        {
            if (((EnumReturnType)txtReturnType.SelectedItem) == EnumReturnType.自定义还款
                || ((EnumReturnType)txtReturnType.SelectedItem) == EnumReturnType.分期付息按还款计划表分期还本)
            {
                MessageBox.Show(txtReturnType.Text + "方式无法系统计算后续还款计划，暂不支持展期！");
                return;
            }

            DateTime dtExtendDate = DateTime.Now;
            DateTime.TryParse(txtExtendDate.Text, out dtExtendDate);
            DateTime dtEndDate = DateTime.Now;
            DateTime.TryParse(txtEndDate.Text, out dtEndDate);
            if (dtExtendDate <= dtEndDate)
            {
                MessageBox.Show("展期后到期日应大于原本的到期日");
                return;
            }

            ObservableCollection<AfterLoanRepaymentPlan> PlanGridOCALRP = PlanGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;
            int i = 0;
            for (i = 0; i < PlanGridOCALRP.Count; i++)
            {
                if (PlanGridOCALRP[i].Type == IousType.未结清)
                {
                    MessageBox.Show("存在未结清期次，无法进行展期！");
                    return;
                }
                if (PlanGridOCALRP[i].Type == IousType.未还款) break;
            }

            decimal PayAmount = 0;
            decimal.TryParse(txtPayAmount.Text, out PayAmount);
            DateTime dtBegindate = DateTime.Parse("0221-12-21");
            DateTime dtBigDe = DateTime.Now;
            DateTime.TryParse(txtBegindate.Text, out dtBigDe);

            for (int j = 0; j < PlanGridOCALRP.Count; j++)
            {
                PayAmount -= PlanGridOCALRP[j].RetrieveAmount;
                if (PlanGridOCALRP[j].Type != IousType.已结清)
                {
                    if (dtBegindate == DateTime.Parse("0221-12-21"))
                    {
                        if (j - 1 >= 0) dtBegindate = PlanGridOCALRP[j - 1].PlanDate;
                        else dtBegindate = dtBigDe;
                    }
                }
            }

            int InterestSetDay;
            int.TryParse(txtInterestSetDay.Text, out InterestSetDay);
            if (InterestSetDay == 0) InterestSetDay = dtBigDe.Day;
            int RepaymentNum = 0;
            RepaymentNum = CurRepaymentNum(dtBegindate, dtExtendDate);
            decimal decExtendInterestOfMonth = 0;
            decimal.TryParse(txtExtendInterestOfMonth.Text, out decExtendInterestOfMonth);
            ObservableCollection<AfterLoanRepaymentPlan> newOcAlrpAdd = CurPlanAmount(i - 1, InterestSetDay, PayAmount, RepaymentNum, dtBegindate, dtExtendDate, decExtendInterestOfMonth);
            //将期次相同的还款计划，替换成已还款的计划
            for (int m = PlanGridOCALRP.Count - 1; m >= 0; m--)
            {
                if (PlanGridOCALRP[m].Type != IousType.未还款) newOcAlrpAdd.Insert(0, PlanGridOCALRP[m].Copy());
            }
            PlanGrid.ItemsSource = newOcAlrpAdd;

            txtEndDate.Text = txtExtendDate.Text;
            txtInterestOfMonth.Text = txtExtendInterestOfMonth.Text;
            ExtendBigEnd.Text = "    原终止日期：" + dtExtendDate.ToString("yyyy-MM-dd") + "   原月利率‰：" + txtInterestOfMonth.Text;
        }

        //放款
        private void Loan_Click_1(object sender, RoutedEventArgs e)
        {
            Alculate_Click(null, null);
            ObservableCollection<AfterLoanRepaymentPlan> PlanGridOCALRP = PlanGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;

            if (PlanGridOCALRP == null || PlanGridOCALRP.Count <= 0)
            {
                MessageBox.Show("必须需要有还款计划，才能进行放款！");
                return;
            }

            foreach (AfterLoanRepaymentPlan item in PlanGridOCALRP)
            {
                if (item.PlanIAddA <= 0)
                {
                    MessageBox.Show("还款本金重新计算有误，请手动修改本金，再重新点击计算");
                    return;
                }
            }

            //if (MessageBox.Show("进行放款之后，就不能再更改还款计划了。确定执行吗？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
            //PlanGrid.Visibility = Visibility.Collapsed;
            LCloaninfo.IsEnabled = false;

            ObservableCollection<AfterLoanRepaymentPlan> newOCalrp = new ObservableCollection<AfterLoanRepaymentPlan> { };

            foreach (var Planalrp in PlanGridOCALRP)
            {
                newOCalrp.Add(Planalrp.Copy());
            }
            ReturnGrid.ItemsSource = newOCalrp;

            ReturnGrid.Visibility = Visibility.Visible;
            Alculate.IsEnabled = false;
            Loan.IsEnabled = false;
            Repayment.Visibility = Visibility.Visible;
            Exhibition.Visibility = Visibility.Visible;
            TQPrepayment.Visibility = Visibility.Visible;
            ReturnCz.IsEnabled = true;
            AdvanceCz.IsEnabled = true;
            ExtendCz.IsEnabled = true;

            colRetrieveAmount.Visibility = Visibility.Collapsed;
            colRetrieveInterest.Visibility = Visibility.Collapsed;
            colRetrieveIAddA.Visibility = Visibility.Collapsed;
            colRetrieveManagementFee.Visibility = Visibility.Collapsed;

            txtExtendInterestOfMonth.Text = txtInterestOfMonth.Text;
            AdvanceBigEnd.Text = "    起始日期：" + txtBegindate.Text;
            ExtendBigEnd.Text = "    原终止日期：" + txtEndDate.Text + "   原月利率‰：" + txtInterestOfMonth.Text;
            //}
        }

        //提前还款
        private void TQPrepayment_Click_1(object sender, RoutedEventArgs e)
        {
            if (((EnumReturnType)txtReturnType.SelectedItem) == EnumReturnType.自定义还款)
            {
                MessageBox.Show(txtReturnType.Text + "方式无法系统计算后续还款计划，暂不支持提前还款！");
                return;
            }

            string strMessage = strAdvance_EditValueChanged(false);
            if (!string.IsNullOrEmpty(strMessage))
            {
                MessageBox.Show(strMessage);
                return;
            }
            decimal decAdvanceAmoun = 0;
            decimal.TryParse(txtAdvanceAmount.Text, out decAdvanceAmoun);
            if (decAdvanceAmoun == 0)
            {
                MessageBox.Show("提前还款本金不能为0");
                return;
            }
            DateTime dtAdvanceDate = DateTime.Now;
            DateTime.TryParse(txtAdvanceDate.Text, out dtAdvanceDate);
            decimal decAdvanceInterest = 0;
            decimal.TryParse(txtAdvanceInterest.Text, out decAdvanceInterest);
            decAdvanceRetrieveGlobal = decAdvanceRetrieveGlobal - decAdvanceInterest;

            ObservableCollection<AfterLoanRepaymentPlan> PlanGridOCALRP = PlanGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;
            int i = 0;
            for (i = 0; i < PlanGridOCALRP.Count; i++)
            {
                if (PlanGridOCALRP[i].Type == IousType.未还款) break;
            }
            AfterLoanRepaymentPlan newAlrp = new AfterLoanRepaymentPlan();
            newAlrp.PlanAmount = decAdvanceAmoun;
            newAlrp.PlanDate = dtAdvanceDate;
            newAlrp.PlanInterest = decAdvanceInterest;
            newAlrp.RetrieveAmount = decAdvanceAmoun;
            newAlrp.RetrieveInterest = decAdvanceInterest;
            newAlrp.RetrieveDate = dtAdvanceDate;
            newAlrp.Type = IousType.已结清;
            PlanGridOCALRP.Insert(i, newAlrp);

            decimal PayAmount = 0;
            decimal.TryParse(txtPayAmount.Text, out PayAmount);
            decimal AllAmount = PayAmount;

            int RepaymentNum = 0;
            DateTime dtBegindate = DateTime.Parse("0221-12-21");

            for (int j = 0; j < PlanGridOCALRP.Count; j++)
            {
                PayAmount -= PlanGridOCALRP[j].RetrieveAmount;
                PlanGridOCALRP[j].Num = j + 1;
                if (PlanGridOCALRP[j].Type != IousType.已结清)
                {
                    if (dtBegindate == DateTime.Parse("0221-12-21")) dtBegindate = PlanGridOCALRP[j - 1].PlanDate;
                    RepaymentNum++;
                }
            }
            if (decAdvanceAmoun + PayAmount > AllAmount)
            {
                MessageBox.Show("提前还款不能大于剩余未还款金额");
                return;
            }

            int InterestSetDay;
            DateTime dtBigDe = DateTime.Now;
            DateTime.TryParse(txtBegindate.Text, out dtBigDe);
            int.TryParse(txtInterestSetDay.Text, out InterestSetDay);
            if (InterestSetDay == 0) InterestSetDay = dtBigDe.Day;
            DateTime dtEndDate = DateTime.Now;
            DateTime.TryParse(txtEndDate.Text, out dtEndDate);
            decimal decInterestOfMonth = 0;
            decimal.TryParse(txtInterestOfMonth.Text, out decInterestOfMonth);

            ObservableCollection<AfterLoanRepaymentPlan> newOcAlrpAdd = CurPlanAmount(i, InterestSetDay, PayAmount, RepaymentNum, dtBegindate, dtEndDate, decInterestOfMonth);
            if (newOcAlrpAdd.Count > 0) newOcAlrpAdd[0].PlanInterest += decAdvanceRetrieveGlobal;
            //将期次相同的还款计划，替换成已还款的计划
            for (int m = 0; m < PlanGridOCALRP.Count; m++)
            {
                for (int n = 0; n < newOcAlrpAdd.Count; n++)
                {
                    if (PlanGridOCALRP[m].Num == newOcAlrpAdd[n].Num)
                    {
                        PlanGridOCALRP[m] = newOcAlrpAdd[n].Copy();
                    }
                }
            }
            ObservableCollection<AfterLoanRepaymentPlan> newLinshi = new ObservableCollection<AfterLoanRepaymentPlan> { };
            foreach (AfterLoanRepaymentPlan alrp in PlanGridOCALRP)
            {
                if (alrp.PlanIAddA > 0) newLinshi.Add(alrp.Copy());
            }
            PlanGrid.ItemsSource = newLinshi;

            decAdvanceRetrieveGlobal = 0;
            txtAdvanceAmount.Text = "0";
            txtAdvanceInterest.Text = "0";
        }

        //还款
        private void Repayment_Click_1(object sender, RoutedEventArgs e)
        {
            ObservableCollection<AfterLoanRepaymentPlan> PlanGridOCALRP = PlanGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;
            ObservableCollection<AfterLoanRepaymentPlan> ReturnGridOCALRP = ReturnGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;
            //PlanGrid ReturnGrid

            //将期次相同的还款计划，替换成已还款的计划
            for (int i = 0; i < PlanGridOCALRP.Count; i++)
            {
                for (int j = 0; j < ReturnGridOCALRP.Count; j++)
                {
                    if (PlanGridOCALRP[i].Num == ReturnGridOCALRP[j].Num)
                    {
                        if (ReturnGridOCALRP[j].RetrieveAmount < PlanGridOCALRP[i].RetrieveAmount
                            || ReturnGridOCALRP[j].RetrieveInterest < PlanGridOCALRP[i].RetrieveInterest
                            || ReturnGridOCALRP[j].RetrieveManagementFee < PlanGridOCALRP[i].RetrieveManagementFee)
                        {
                            MessageBox.Show("别乱改表格，没做限制，已回收金额只能改大不能改小！");
                            return;
                        }
                        PlanGridOCALRP[i] = ReturnGridOCALRP[j].Copy();
                    }
                }
            }

            ObservableCollection<AfterLoanRepaymentPlan> newOCalrp = new ObservableCollection<AfterLoanRepaymentPlan> { };
            foreach (var alrp in PlanGridOCALRP)
            {
                if (alrp.Type != IousType.已结清)
                {
                    newOCalrp.Add(alrp.Copy());
                }
            }
            PlanGrid.ItemsSource = PlanGridOCALRP;


            ObservableCollection<AfterLoanRepaymentPlan> OCALRP = PlanGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;
            decRetrieve = 0;
            foreach (var alrp in OCALRP)
            {
                if (alrp.Type != IousType.已结清)
                {
                    decRetrieve += alrp.RetrieveAmount;
                    decRetrieve += alrp.RetrieveInterest;
                    decRetrieve += alrp.RetrieveManagementFee;
                }
            }
            txtzReturnAmount.Text = "0";

            ReturnGrid.ItemsSource = newOCalrp;
            PlanGrid_ItemsSourceChanged_1(null, null);
        }

        decimal decRetrieve = 0;
        //将还款总额自动分配到总金额中
        private void txtzReturnAmount_EditValueChanged_1(object sender, RoutedEventArgs e)
        {
            decimal dezReturnAmount = 0;
            decimal.TryParse(txtzReturnAmount.Text, out dezReturnAmount);
            dezReturnAmount += decRetrieve;

            ObservableCollection<AfterLoanRepaymentPlan> OCALRP = ReturnGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;
            for (int i = 0; i < OCALRP.Count; i++)
            {
                //利息
                if (dezReturnAmount - OCALRP[i].PlanInterest > 0)
                {
                    OCALRP[i].RetrieveInterest = OCALRP[i].PlanInterest;
                    dezReturnAmount -= OCALRP[i].PlanInterest;
                }
                else
                {
                    if (dezReturnAmount > 0)
                    {
                        OCALRP[i].RetrieveInterest = dezReturnAmount;
                        dezReturnAmount -= OCALRP[i].PlanInterest;
                    }
                    else OCALRP[i].RetrieveInterest = 0;
                }

                //本金
                if (dezReturnAmount - OCALRP[i].PlanAmount > 0)
                {
                    OCALRP[i].RetrieveAmount = OCALRP[i].PlanAmount;
                    dezReturnAmount -= OCALRP[i].PlanAmount;
                }
                else
                {
                    if (dezReturnAmount > 0)
                    {
                        OCALRP[i].RetrieveAmount = dezReturnAmount;
                        dezReturnAmount -= OCALRP[i].PlanAmount;
                    }
                    else OCALRP[i].RetrieveAmount = 0;
                }

                //管理费
                if (dezReturnAmount - OCALRP[i].PlanManagementFee > 0)
                {
                    OCALRP[i].RetrieveManagementFee = OCALRP[i].PlanManagementFee;
                    dezReturnAmount -= OCALRP[i].PlanManagementFee;
                }
                else
                {
                    if (dezReturnAmount > 0)
                    {
                        OCALRP[i].RetrieveManagementFee = dezReturnAmount;
                        dezReturnAmount -= OCALRP[i].PlanManagementFee;
                    }
                    else OCALRP[i].RetrieveManagementFee = 0;
                }

                //本息合计
                //OCALRP[i].RetrieveIAddA = OCALRP[i].RetrieveAmount + OCALRP[i].RetrieveInterest;

                if (OCALRP[i].RetrieveIAddA == 0) OCALRP[i].Type = IousType.未还款;
                if (OCALRP[i].RetrieveIAddA > 0) OCALRP[i].Type = IousType.未结清;
                if (OCALRP[i].RetrieveIAddA == OCALRP[i].PlanIAddA && OCALRP[i].PlanManagementFee == OCALRP[i].RetrieveManagementFee)
                    OCALRP[i].Type = IousType.已结清;

            }

            ReturnGrid.ItemsSource = OCALRP;
        }

        private void txtAdvance_EditValueChanged_1(object sender, RoutedEventArgs e)
        {
            strAdvance_EditValueChanged(true);
        }

        decimal decAdvanceRetrieveGlobal = 0;
        //根据提前还款日期、本金，自动计算利息，以及各类判断
        private string strAdvance_EditValueChanged(bool isSet)
        {
            //txtAdvanceDate txtAdvanceAmount txtAdvanceInterest
            decimal decAdvanceAmoun = 0;
            //decimal.TryParse(txtAdvanceAmount.Text, out decAdvanceAmoun);
            //（1）5月15日应还金额X=200万元本金+按1000万元支付的期间利息（4.21-5.15）
            //（2）5月15日应还金额Y=200万元本金+按200万元支付的期间利息（4.21-5.15）
            //重新修改符合现有系统规则，改用(1)方案

            DateTime dtAdvanceDate = DateTime.Now;
            if (!DateTime.TryParse(txtAdvanceDate.Text, out dtAdvanceDate)) return "日期格式错误！";
            DateTime dtBegindate = DateTime.Now;
            if (!DateTime.TryParse(txtBegindate.Text, out dtBegindate)) return "日期格式错误！";
            decimal decInterestOfMonth = 0;
            decimal.TryParse(txtInterestOfMonth.Text, out decInterestOfMonth);

            try
            {
                ObservableCollection<AfterLoanRepaymentPlan> PlanGridOCALRP = PlanGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;
                ObservableCollection<AfterLoanRepaymentPlan> ReturnGridOCALRP = ReturnGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;
                if (ReturnGridOCALRP == null || PlanGridOCALRP == null || ReturnGridOCALRP.Count <= 0 || ReturnGridOCALRP[0].PlanDate < dtAdvanceDate
                    || ReturnGridOCALRP[0].Type != IousType.未还款 || PlanGridOCALRP.Count < ReturnGridOCALRP[0].Num)
                    return "未找到还款计划或上期状态为 未结清";

                decimal.TryParse(txtPayAmount.Text, out decAdvanceAmoun);
                foreach (AfterLoanRepaymentPlan itAlrp in PlanGridOCALRP)
                {
                    decAdvanceAmoun -= itAlrp.RetrieveAmount;
                }

                DateTime dtJiSuanDate = DateTime.Now;
                if (ReturnGridOCALRP[0].Num - 2 >= 0) dtJiSuanDate = PlanGridOCALRP[ReturnGridOCALRP[0].Num - 2].PlanDate;
                else dtJiSuanDate = dtBegindate;
                int intDay = ((TimeSpan)(dtAdvanceDate - dtJiSuanDate)).Days;
                if (intDay < 0) return "提前还款日期小于上期日期！";
                if (isSet)
                {
                    decAdvanceRetrieveGlobal = Math.Round(decInterestOfMonth / 30 / 1000 * decAdvanceAmoun * intDay, 2);
                    txtAdvanceInterest.Text = decAdvanceRetrieveGlobal.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        private int CurRepaymentNum(DateTime Begindate, DateTime EndDate)
        {
            EnumReturnType LoanBackDayType = (EnumReturnType)txtReturnType.SelectedItem;
            EnumInitialChargeType InitialChargeType = (EnumInitialChargeType)txtInitialChargeType.SelectedItem;
            int InterestSetDay = 0;

            int.TryParse(txtInterestSetDay.Text, out InterestSetDay);

            #region
            int zjYear = EndDate.Year - Begindate.Year;
            int zjMonth = EndDate.Month - Begindate.Month;
            int zjDay = EndDate.Day - Begindate.Day;
            TimeSpan tSpan = Begindate - EndDate;
            #endregion

            int RepaymentNum = 0;
            switch (LoanBackDayType)
            {
                case EnumReturnType.到期一次性还本付息:
                    RepaymentNum = 1;
                    break;
                default:
                    RepaymentNum = zjYear * 12 + zjMonth;
                    if (InitialChargeType == EnumInitialChargeType.当月放款当期还款)
                    {
                        if (InterestSetDay != 0)
                        {

                            if (Begindate.Day < InterestSetDay) RepaymentNum = RepaymentNum + 1;
                        }
                    }
                    if (InterestSetDay == 0)
                    {
                        if (zjDay > 0) RepaymentNum = RepaymentNum + 1;
                    }
                    else
                    {
                        if (EndDate.Day > InterestSetDay) RepaymentNum = RepaymentNum + 1;
                    }
                    break;
            }

            return RepaymentNum;
        }

        /// <summary>
        /// 提前还款或展期之后，根据条件重新计算还款日期
        /// </summary>
        /// <param name="i">开始期次</param>
        /// <param name="InterestSetDay">结息日</param>
        /// <param name="PayAmount"></param>
        /// <param name="RepaymentNum"></param>
        /// <param name="Begindate"></param>
        /// <returns></returns>
        private ObservableCollection<AfterLoanRepaymentPlan> CurPlanAmount(int i, int InterestSetDay, decimal PayAmount, int RepaymentNum, DateTime Begindate, DateTime EndDate, decimal InterestOfMonth)
        {
            decimal InterestOfFee;
            int RepaymentMonthNum = 1;//每期月数
            string LoanBackDayType;
            string InitialChargeType;
            LoanBackDayType = txtLoanBackDayType.Text.ToString();
            InitialChargeType = txtInitialChargeType.Text.ToString();
            ObservableCollection<AfterLoanRepaymentPlan> newOcAlrp = new ObservableCollection<AfterLoanRepaymentPlan> { };

            if (
            decimal.TryParse(txtInterestOfFee.Text, out InterestOfFee) &&
            !string.IsNullOrEmpty(LoanBackDayType) &&
            !string.IsNullOrEmpty(InitialChargeType))
            {
                switch ((EnumReturnType)txtReturnType.SelectedItem)
                {
                    case EnumReturnType.等额本金:
                        newOcAlrp = CalculatePlan.等额本金ReturnRepaymentPlan(PayAmount, InterestOfMonth, RepaymentNum, InterestOfFee,
    Begindate, 1, InterestSetDay, LoanBackDayType, InitialChargeType, EndDate);
                        break;
                    case EnumReturnType.等额本息:
                        newOcAlrp = CalculatePlan.等额本息ReturnRepaymentPlan(PayAmount, InterestOfMonth, RepaymentNum, InterestOfFee,
    Begindate, 1, InterestSetDay, LoanBackDayType, InitialChargeType, EndDate);
                        break;
                    case EnumReturnType.分期付息一次还本:
                        newOcAlrp = CalculatePlan.分期付息一次还本ReturnRepaymentPlan(PayAmount, InterestOfMonth, RepaymentNum, InterestOfFee,
    Begindate, 1, InterestSetDay, LoanBackDayType, InitialChargeType, EndDate);
                        break;
                    case EnumReturnType.到期一次性还本付息:
                        newOcAlrp = CalculatePlan.到期一次性还本付息ReturnRepaymentPlan(PayAmount, InterestOfMonth, RepaymentNum, InterestOfFee,
    Begindate, 1, InterestSetDay, LoanBackDayType, InitialChargeType, EndDate);
                        break;
                    case EnumReturnType.分期付息按还款计划表分期还本:
                        ObservableCollection<AfterLoanRepaymentPlan> PlanGridOCALRP = PlanGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;
                        ObservableCollection<AfterLoanRepaymentPlan> newLinshi = new ObservableCollection<AfterLoanRepaymentPlan> { };
                        foreach (AfterLoanRepaymentPlan alrp in PlanGridOCALRP)
                        {
                            if (alrp.Type == IousType.未还款) newLinshi.Add(alrp.Copy());
                        }
                        newOcAlrp = CalculatePlan.分期付息按还款计划表分期还本ReturnRepaymentPlan(PayAmount, InterestOfMonth, RepaymentNum, InterestOfFee,
Begindate, 1, InterestSetDay, LoanBackDayType, InitialChargeType, EndDate, newLinshi);
                        break;
                    case EnumReturnType.自定义还款:
                        break;
                    default:
                        break;
                }
            }
            foreach (AfterLoanRepaymentPlan itAlrp in newOcAlrp)
            {
                itAlrp.Num += i + 1;
            }
            return newOcAlrp;
        }

        private void PlanGrid_ItemsSourceChanged_1(object sender, RoutedEventArgs e)
        {
            ObservableCollection<AfterLoanRepaymentPlan> PlanGridOCALRP = PlanGrid.ItemsSource as ObservableCollection<AfterLoanRepaymentPlan>;
            ObservableCollection<AfterLoanRepaymentPlan> newAdvanceOCalrp = new ObservableCollection<AfterLoanRepaymentPlan> { };
            ObservableCollection<AfterLoanRepaymentPlan> newReturnOCalrp = new ObservableCollection<AfterLoanRepaymentPlan> { };
            ObservableCollection<AfterLoanRepaymentPlan> newExtendOCalrp = new ObservableCollection<AfterLoanRepaymentPlan> { };
            foreach (AfterLoanRepaymentPlan alrp in PlanGridOCALRP)
            {
                newAdvanceOCalrp.Add(alrp.Copy());
                if (alrp.Type != IousType.已结清)
                {
                    newReturnOCalrp.Add(alrp.Copy());
                }
                newExtendOCalrp.Add(alrp.Copy());
            }
            AdvanceGrid.ItemsSource = newAdvanceOCalrp;

            ReturnGrid.ItemsSource = newReturnOCalrp;

            ExtendGrid.ItemsSource = newExtendOCalrp;
        }
    }

    public class CalculatePlan
    {
        public static EnumInterestMonthOrDay DefaultInterest = EnumInterestMonthOrDay.按日付息;
        /// <summary>
        /// 等额本金还款方式
        /// </summary>
        /// <param name="PayAmount">本金</param>
        /// <param name="InterestOfMonth">月利率‰</param>
        /// <param name="RepaymentNum">总期次</param>
        /// <param name="InterestOfFee">管理费月利率‰</param>
        /// <param name="Begindate">开始日期</param>
        /// <param name="RepaymentMonthNum">每期月数</param>
        /// <param name="InterestSetDay">结息日</param>
        /// <param name="LoanBackDayType">算头算尾类型：1算头不算尾，2头尾，3无，4尾</param>
        /// <param name="InitialChargeType">首期结息类型：1当月放款当期还款，2下期</param>
        /// <param name="EndDate">借据终止日期</param>
        /// <returns></returns>
        public static ObservableCollection<AfterLoanRepaymentPlan> 等额本金ReturnRepaymentPlan(decimal PayAmount, decimal InterestOfMonth,
    int RepaymentNum, decimal InterestOfFee, DateTime Begindate, int RepaymentMonthNum, int InterestSetDay, string LoanBackDayType, string InitialChargeType, DateTime EndDate)
        {
            #region 第一步先进行计算每一期的应还本金、利息以及管理费
            //每期应还本金
            decimal OnceAmount = PayAmount / RepaymentNum;

            //每期应还利息，考虑算头算尾类型，第一期和最后一期可能会多加一天利息
            decimal DayInterest = InterestOfMonth / 30;
            #endregion

            ObservableCollection<AfterLoanRepaymentPlan> OCAfterLoanRepaymentPlan = new ObservableCollection<AfterLoanRepaymentPlan>();
            decimal SurplusAmount = PayAmount;
            DateTime nowDateTime = Begindate;

            for (int i = 0; i < RepaymentNum; i++)
            {
                AfterLoanRepaymentPlan alrp = new AfterLoanRepaymentPlan();
                alrp.Num = i + 1;

                DateTime PlanDate = Begindate.AddMonths(i + 1);
                if (InterestSetDay != 0)
                {
                    if (InitialChargeType.Equals(EnumInitialChargeType.当月放款当期还款.ToString()))
                    {
                        if (PlanDate.Day < InterestSetDay) PlanDate = PlanDate.AddMonths(-1);
                    }
                    PlanDate = PlanDate.AddDays(InterestSetDay - PlanDate.Day);
                }

                int bezjDay = ((TimeSpan)(PlanDate - EndDate)).Days;
                if (bezjDay > 0) PlanDate = EndDate;
                alrp.PlanDate = PlanDate;
                alrp.Type = IousType.未还款;
                alrp.PlanAmount = OnceAmount;

                decimal PlanInterest = SurplusAmount * InterestOfMonth * RepaymentMonthNum / 1000;
                if (DefaultInterest == EnumInterestMonthOrDay.按日付息)
                {
                    int cjDay = ((TimeSpan)(PlanDate - nowDateTime)).Days;
                    PlanInterest = SurplusAmount * DayInterest * cjDay / 1000;
                }
                alrp.PlanInterest = PlanInterest;
                alrp.IsAdvance = false;

                if ((LoanBackDayType.Equals(EnumLoanBackType.不算头不算尾.ToString()) || LoanBackDayType.Equals(EnumLoanBackType.算尾不算头.ToString()))
                    && i == 0)
                {
                    alrp.PlanInterest -= DayInterest * SurplusAmount / 1000;
                }

                if ((LoanBackDayType.Equals(EnumLoanBackType.算尾不算头.ToString()) || LoanBackDayType.Equals(EnumLoanBackType.算头又算尾.ToString()))
                    && i == RepaymentNum - 1)
                {
                    alrp.PlanInterest += DayInterest * SurplusAmount / 1000;
                }
                //alrp.PlanIAddA = alrp.PlanInterest + alrp.PlanAmount;
                alrp.PlanManagementFee = InterestOfMonth != 0 ? alrp.PlanInterest / InterestOfMonth * InterestOfFee : 0;
                //SurplusAmount * InterestOfFee * RepaymentMonthNum / 1000;
                SurplusAmount -= OnceAmount;

                nowDateTime = PlanDate;
                if (i == RepaymentNum - 1) alrp.PlanAmount += PayAmount - Math.Round(OnceAmount, 2) * RepaymentNum;
                OCAfterLoanRepaymentPlan.Add(alrp);
            }

            return OCAfterLoanRepaymentPlan;
        }

        /// <summary>
        /// 等额本息
        /// </summary>
        /// <param name="PayAmount"></param>
        /// <param name="InterestOfMonth"></param>
        /// <param name="RepaymentNum"></param>
        /// <param name="InterestOfFee"></param>
        /// <param name="Begindate"></param>
        /// <param name="RepaymentMonthNum"></param>
        /// <returns></returns>
        public static ObservableCollection<AfterLoanRepaymentPlan> 等额本息ReturnRepaymentPlan(decimal PayAmount, decimal InterestOfMonth,
    int RepaymentNum, decimal InterestOfFee, DateTime Begindate, int RepaymentMonthNum, int InterestSetDay, string LoanBackDayType, string InitialChargeType, DateTime EndDate)
        {
            decimal Amount = 0;//还款计划总额
            decimal Interest = InterestOfMonth / 1000 * RepaymentMonthNum;//月利率
            decimal DayInterest = InterestOfMonth / 30;
            decimal MathPow = (decimal)Math.Pow(1 + (double)InterestOfMonth / 1000, RepaymentNum);
            decimal onceIAddA = Math.Round(PayAmount * InterestOfMonth / 1000 * MathPow / (MathPow - 1), 2);

            decimal SurplusAmount = PayAmount;
            DateTime nowDateTime = Begindate;
            ObservableCollection<AfterLoanRepaymentPlan> OCAfterLoanRepaymentPlan = new ObservableCollection<AfterLoanRepaymentPlan>();
            for (int i = 0; i < RepaymentNum; i++)
            {
                AfterLoanRepaymentPlan alrp = new AfterLoanRepaymentPlan();
                alrp.Num = i + 1;

                DateTime PlanDate = Begindate.AddMonths(i + 1);
                if (InterestSetDay != 0)
                {
                    if (InitialChargeType.Equals(EnumInitialChargeType.当月放款当期还款.ToString()))
                    {
                        if (PlanDate.Day < InterestSetDay) PlanDate = PlanDate.AddMonths(-1);
                    }
                    PlanDate = PlanDate.AddDays(InterestSetDay - PlanDate.Day);
                }

                int bezjDay = ((TimeSpan)(PlanDate - EndDate)).Days;
                if (bezjDay > 0) PlanDate = EndDate;
                alrp.PlanDate = PlanDate;
                alrp.Type = IousType.未还款;

                decimal PlanInterest = SurplusAmount * InterestOfMonth * RepaymentMonthNum / 1000;
                #region 等额本息一律默认一月30天，按月计息
                //if (DefaultInterest == EnumInterestMonthOrDay.按日付息)
                //{
                //    int cjDay = ((TimeSpan)(PlanDate - nowDateTime)).Days;
                //    PlanInterest = SurplusAmount * DayInterest * cjDay / 1000;
                //}
                #endregion

                alrp.PlanInterest = PlanInterest;
                alrp.IsAdvance = false;

                #region 等额本息无视算头算尾类型
                //if ((LoanBackDayType.Equals(EnumLoanBackType.不算头不算尾.ToString()) || LoanBackDayType.Equals(EnumLoanBackType.算尾不算头.ToString()))
                //    && i == 0)
                //{
                //    alrp.PlanInterest -= DayInterest * SurplusAmount / 1000;
                //}

                //if ((LoanBackDayType.Equals(EnumLoanBackType.算尾不算头.ToString()) || LoanBackDayType.Equals(EnumLoanBackType.算头又算尾.ToString()))
                //    && i == RepaymentNum - 1)
                //{
                //    alrp.PlanInterest += DayInterest * SurplusAmount / 1000;
                //}
                #endregion

                alrp.PlanAmount = onceIAddA - alrp.PlanInterest;
                //alrp.PlanIAddA = alrp.PlanInterest + alrp.PlanAmount;
                alrp.PlanManagementFee = InterestOfMonth != 0 ? alrp.PlanInterest / InterestOfMonth * InterestOfFee : 0;//SurplusAmount * InterestOfFee * RepaymentMonthNum / 1000;
                SurplusAmount -= alrp.PlanAmount;

                nowDateTime = PlanDate;
                Amount += alrp.PlanAmount;
                if (i == RepaymentNum - 1) alrp.PlanAmount += PayAmount - Amount;
                OCAfterLoanRepaymentPlan.Add(alrp);
            }
            return OCAfterLoanRepaymentPlan;
        }

        public static ObservableCollection<AfterLoanRepaymentPlan> 分期付息一次还本ReturnRepaymentPlan(decimal PayAmount, decimal InterestOfMonth,
int RepaymentNum, decimal InterestOfFee, DateTime Begindate, int RepaymentMonthNum, int InterestSetDay, string LoanBackDayType, string InitialChargeType, DateTime EndDate)
        {
            decimal Amount = PayAmount;//借款本金
            decimal Interest = InterestOfMonth / 1000 * RepaymentMonthNum;//月利率
            decimal DayInterest = InterestOfMonth / 30;

            decimal SurplusAmount = PayAmount;
            DateTime nowDateTime = Begindate;
            ObservableCollection<AfterLoanRepaymentPlan> OCAfterLoanRepaymentPlan = new ObservableCollection<AfterLoanRepaymentPlan>();
            for (int i = 0; i < RepaymentNum; i++)
            {
                AfterLoanRepaymentPlan alrp = new AfterLoanRepaymentPlan();
                alrp.Num = i + 1;

                DateTime PlanDate = Begindate.AddMonths(i + 1);
                if (InterestSetDay != 0)
                {
                    if (InitialChargeType.Equals(EnumInitialChargeType.当月放款当期还款.ToString()))
                    {
                        if (PlanDate.Day < InterestSetDay) PlanDate = PlanDate.AddMonths(-1);
                    }
                    PlanDate = PlanDate.AddDays(InterestSetDay - PlanDate.Day);
                }

                int bezjDay = ((TimeSpan)(PlanDate - EndDate)).Days;
                if (bezjDay > 0) PlanDate = EndDate;
                alrp.PlanDate = PlanDate;
                alrp.Type = IousType.未还款;

                decimal PlanInterest = SurplusAmount * InterestOfMonth * RepaymentMonthNum / 1000;
                if (DefaultInterest == EnumInterestMonthOrDay.按日付息)
                {
                    int cjDay = ((TimeSpan)(PlanDate - nowDateTime)).Days;
                    PlanInterest = SurplusAmount * DayInterest * cjDay / 1000;
                }
                if (i == RepaymentNum - 1) alrp.PlanAmount = PayAmount;
                else alrp.PlanAmount = 0;
                alrp.PlanInterest = PlanInterest;

                alrp.IsAdvance = false;


                if ((LoanBackDayType.Equals(EnumLoanBackType.不算头不算尾.ToString()) || LoanBackDayType.Equals(EnumLoanBackType.算尾不算头.ToString()))
                    && i == 0)
                {
                    alrp.PlanInterest -= DayInterest * SurplusAmount / 1000;
                }

                if ((LoanBackDayType.Equals(EnumLoanBackType.算尾不算头.ToString()) || LoanBackDayType.Equals(EnumLoanBackType.算头又算尾.ToString()))
                    && i == RepaymentNum - 1)
                {
                    alrp.PlanInterest += DayInterest * SurplusAmount / 1000;
                }
                //alrp.PlanIAddA = alrp.PlanInterest + alrp.PlanAmount;
                alrp.PlanManagementFee = InterestOfMonth != 0 ? alrp.PlanInterest / InterestOfMonth * InterestOfFee : 0;//SurplusAmount * InterestOfFee * RepaymentMonthNum / 1000;
                SurplusAmount -= alrp.PlanAmount;

                nowDateTime = PlanDate;

                OCAfterLoanRepaymentPlan.Add(alrp);
            }
            return OCAfterLoanRepaymentPlan;
        }

        public static ObservableCollection<AfterLoanRepaymentPlan> 到期一次性还本付息ReturnRepaymentPlan(decimal PayAmount, decimal InterestOfMonth,
int RepaymentNum, decimal InterestOfFee, DateTime Begindate, int RepaymentMonthNum, int InterestSetDay, string LoanBackDayType, string InitialChargeType, DateTime EndDate)
        {
            decimal Amount = PayAmount;//借款本金
            decimal Interest = InterestOfMonth / 1000 * RepaymentMonthNum;//月利率
            decimal DayInterest = InterestOfMonth / 30;

            ObservableCollection<AfterLoanRepaymentPlan> OCAfterLoanRepaymentPlan = new ObservableCollection<AfterLoanRepaymentPlan>();
            for (int i = 0; i < RepaymentNum; i++)
            {
                AfterLoanRepaymentPlan alrp = new AfterLoanRepaymentPlan();
                alrp.Num = i + 1;
                alrp.PlanDate = EndDate;
                alrp.Type = IousType.未还款;
                alrp.PlanAmount = PayAmount;

                decimal planInt = PayAmount * DayInterest / 1000 * ((TimeSpan)(EndDate - Begindate)).Days;
                if (DefaultInterest == EnumInterestMonthOrDay.按月付息)
                {
                    int intYear = EndDate.Year - Begindate.Year;
                    int intMonth = EndDate.Month - Begindate.Month;
                    intMonth = intYear * 12 + intMonth;
                    if (EndDate.Day - Begindate.Day > 0) intMonth++;
                    planInt = PayAmount * Interest * intMonth;
                }
                if (LoanBackDayType.Equals(EnumLoanBackType.算头又算尾.ToString()))
                {
                    planInt += DayInterest * PayAmount / 1000;
                }
                if (LoanBackDayType.Equals(EnumLoanBackType.不算头不算尾.ToString()))
                {
                    planInt -= DayInterest * PayAmount / 1000;
                }

                alrp.PlanInterest = planInt;
                alrp.PlanManagementFee = InterestOfMonth != 0 ? alrp.PlanInterest / InterestOfMonth * InterestOfFee : 0;//SurplusAmount * InterestOfFee * RepaymentMonthNum / 1000;
                alrp.IsAdvance = false;
                //alrp.PlanIAddA = alrp.PlanInterest + alrp.PlanAmount;

                OCAfterLoanRepaymentPlan.Add(alrp);
            }
            return OCAfterLoanRepaymentPlan;
        }

        public static ObservableCollection<AfterLoanRepaymentPlan> 分期付息按还款计划表分期还本ReturnRepaymentPlan(decimal PayAmount, decimal InterestOfMonth,
int RepaymentNum, decimal InterestOfFee, DateTime Begindate, int RepaymentMonthNum, int InterestSetDay, string LoanBackDayType, string InitialChargeType, DateTime EndDate,
            object CurMainOB)
        {
            string strMessage = string.Empty;
            if (!CalculatePlan.YanZhengDate(CurMainOB as ObservableCollection<AfterLoanRepaymentPlan>, Begindate, EndDate, out strMessage))
            {
                MessageBox.Show(strMessage);
                return CurMainOB as ObservableCollection<AfterLoanRepaymentPlan>;
            }

            decimal SurplusAmount = PayAmount;//借款本金
            decimal Interest = InterestOfMonth / 1000 * RepaymentMonthNum;//月利率
            decimal DayInterest = InterestOfMonth / 30;
            DateTime nowDateTime = Begindate;

            ObservableCollection<AfterLoanRepaymentPlan> CurMainRP = CurMainOB as ObservableCollection<AfterLoanRepaymentPlan>;
            ObservableCollection<AfterLoanRepaymentPlan> OCAfterLoanRepaymentPlan = new ObservableCollection<AfterLoanRepaymentPlan>();
            for (int i = 0; i < RepaymentNum; i++)
            {
                AfterLoanRepaymentPlan CurAlrp = CurMainRP[i];
                if (SurplusAmount == 0)
                {
                    break;
                }

                AfterLoanRepaymentPlan alrp = new AfterLoanRepaymentPlan();
                alrp.Num = i + 1;
                alrp.PlanDate = CurAlrp.PlanDate;
                alrp.Type = IousType.未还款;
                alrp.PlanAmount = CurAlrp.PlanAmount;
                if (i == RepaymentNum - 1) alrp.PlanAmount = SurplusAmount;

                decimal PlanInterest = SurplusAmount * InterestOfMonth * RepaymentMonthNum / 1000;
                if (DefaultInterest == EnumInterestMonthOrDay.按日付息)
                {
                    int cjDay = ((TimeSpan)(alrp.PlanDate - nowDateTime)).Days;
                    PlanInterest = SurplusAmount * DayInterest * cjDay / 1000;
                }
                alrp.PlanInterest = PlanInterest;
                alrp.IsAdvance = false;

                if ((LoanBackDayType.Equals(EnumLoanBackType.不算头不算尾.ToString()) || LoanBackDayType.Equals(EnumLoanBackType.算尾不算头.ToString()))
                    && i == 0)
                {
                    alrp.PlanInterest -= DayInterest * SurplusAmount / 1000;
                }

                if ((LoanBackDayType.Equals(EnumLoanBackType.算尾不算头.ToString()) || LoanBackDayType.Equals(EnumLoanBackType.算头又算尾.ToString()))
                    && i == RepaymentNum - 1)
                {
                    alrp.PlanInterest += DayInterest * SurplusAmount / 1000;
                }
                //alrp.PlanIAddA = alrp.PlanInterest + alrp.PlanAmount;
                alrp.PlanManagementFee = InterestOfMonth != 0 ? alrp.PlanInterest / InterestOfMonth * InterestOfFee : 0;//SurplusAmount * InterestOfFee * RepaymentMonthNum / 1000;
                SurplusAmount -= alrp.PlanAmount;

                nowDateTime = alrp.PlanDate;

                OCAfterLoanRepaymentPlan.Add(alrp);
            }

            foreach (AfterLoanRepaymentPlan item in OCAfterLoanRepaymentPlan)
            {
                if (item.PlanIAddA <= 0) MessageBox.Show("还款本金重新计算有误，请手动修改本金，再重新点击计算");
            }

            return OCAfterLoanRepaymentPlan;
        }

        public static bool YanZhengDate(ObservableCollection<AfterLoanRepaymentPlan> CurMainOB, DateTime Begindate, DateTime EndDate, out string Message)
        {
            bool isok = true;
            Message = string.Empty;
            DateTime dtBig = Begindate;

            foreach (AfterLoanRepaymentPlan itemALRP in CurMainOB)
            {
                //计划还款时间在有效期内
                if (itemALRP.PlanDate < Begindate || itemALRP.PlanDate > EndDate)
                {
                    Message = "计划还款时间必须在有效期内！";
                    isok = false;
                }

                //此次还款计划时间大于等于上次还款时间
                if (itemALRP.PlanDate < dtBig)
                {
                    Message = "第" + (CurMainOB.IndexOf(itemALRP) + 1) + "期计划还款时间(" + itemALRP.PlanDate.ToString("yyyy-MM-dd")
                        + ")必须在大于或等于上一期时间(" + dtBig.ToString("yyyy-MM-dd") + ")";
                    isok = false;
                }
                dtBig = itemALRP.PlanDate;
            }
            return isok;
        }
    }

    /// <summary>
    /// 算头算尾类型
    /// </summary>
    public enum EnumLoanBackType
    {
        算头不算尾 = 1,
        算头又算尾 = 2,
        不算头不算尾 = 3,
        算尾不算头 = 4
    }

    /// <summary>
    /// 首期结息类型
    /// </summary>
    public enum EnumInitialChargeType
    {
        当月放款当期还款 = 1,
        当月放款下期还款 = 2
    }

    /// <summary>
    /// 还款方式
    /// </summary>
    public enum EnumReturnType
    {
        等额本息 = 1,
        等额本金 = 2,
        分期付息一次还本 = 3,
        到期一次性还本付息 = 4,
        分期付息按还款计划表分期还本 = 5,
        自定义还款 = 6
    }

    /// <summary>
    /// 付息方式
    /// </summary>
    public enum EnumInterestMonthOrDay
    {
        按日付息 = 2,
        按月付息 = 1
    }

    /// <summary>
    /// 当期状态
    /// </summary>
    public enum IousType
    {
        未还款,
        已结清,
        未结清
    };

    public class AfterLoanRepaymentPlan
    {
        /// <summary>
        /// 期号
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// 计划还款日期
        /// </summary>
        public DateTime PlanDate { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public IousType Type { get; set; }
        /// <summary>
        /// 应收本金
        /// </summary>
        private decimal _PlanAmount;
        public decimal PlanAmount
        {
            get { return _PlanAmount; }
            set { _PlanAmount = Math.Round(value, 2); }
        }
        /// <summary>
        /// 应收利息
        /// </summary>
        private decimal _PlanInterest;
        public decimal PlanInterest
        {
            get { return _PlanInterest; }
            set { _PlanInterest = Math.Round(value, 2); }
        }
        /// <summary>
        /// 应收本息合计
        /// </summary>
        //private decimal _PlanIAddA;
        public decimal PlanIAddA
        {
            get { return _PlanAmount + _PlanInterest; }
            //set { _PlanIAddA = Math.Round(value, 2); }
        }
        /// <summary>
        /// 应收管理费
        /// </summary>
        private decimal _PlanManagementFee = 0;
        public decimal PlanManagementFee
        {
            get { return _PlanManagementFee; }
            set { _PlanManagementFee = Math.Round(value, 2); }
        }
        /// <summary>
        /// 是否提前还款
        /// </summary>
        public bool IsAdvance { get; set; }
        /// <summary>
        /// 实际还款日期
        /// </summary>
        public DateTime RetrieveDate { get; set; }
        /// <summary>
        /// 实收本金
        /// </summary>
        private decimal _RetrieveAmount;
        public decimal RetrieveAmount
        {
            get { return _RetrieveAmount; }
            set { _RetrieveAmount = Math.Round(value, 2); }
        }
        /// <summary>
        /// 实收利息
        /// </summary>
        private decimal _RetrieveInterest;
        public decimal RetrieveInterest
        {
            get { return _RetrieveInterest; }
            set { _RetrieveInterest = Math.Round(value, 2); }
        }
        /// <summary>
        /// 实收本息合计
        /// </summary>
        //private decimal _RetrieveIAddA;
        public decimal RetrieveIAddA
        {
            get { return _RetrieveAmount + _RetrieveInterest; }
            //set { _RetrieveIAddA = Math.Round(value, 2); }
        }
        /// <summary>
        /// 实收管理费
        /// </summary>
        private decimal _RetrieveManagementFee = 0;
        public decimal RetrieveManagementFee
        {
            get { return _RetrieveManagementFee; }
            set { _RetrieveManagementFee = Math.Round(value, 2); }
        }

        //复制、克隆，而非赋值，直接等于其实用的是同一个对象。A、B同时改变
        public AfterLoanRepaymentPlan Copy()
        {
            AfterLoanRepaymentPlan newAlrp = new AfterLoanRepaymentPlan();
            newAlrp.Num = this.Num;
            newAlrp.IsAdvance = this.IsAdvance;
            newAlrp.PlanAmount = this.PlanAmount;
            newAlrp.PlanDate = this.PlanDate;
            //newAlrp.PlanIAddA = this.PlanIAddA;
            newAlrp.PlanInterest = this.PlanInterest;
            newAlrp.PlanManagementFee = this.PlanManagementFee;
            newAlrp.RetrieveAmount = this.RetrieveAmount;
            //newAlrp.RetrieveIAddA = this.RetrieveIAddA;
            newAlrp.RetrieveInterest = this.RetrieveInterest;
            newAlrp.RetrieveManagementFee = this.RetrieveManagementFee;
            newAlrp.Type = this.Type;
            return newAlrp;
        }
    }
}
