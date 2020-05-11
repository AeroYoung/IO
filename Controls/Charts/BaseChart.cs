using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ExpertLib.Utils;

namespace ExpertLib.Controls
{
    public partial class BaseChart : UserControl
    {
        #region 字段定义

        [Description("计算方式"), Category("自定义属性")]
        public Func Func;

        [Description("日期字段"), Category("自定义属性")]
        public Field DateField;

        [Description("求和字段"), Category("自定义属性")]
        public Field SumField;

        [Description("分组字段"), Category("自定义属性")]
        public List<Field> GroupByFields;

        [Description("数据源"), Category("自定义属性")]
        public DataTable DataSource;

        #endregion

        #region 筛选条件

        [Description("时间单位"), Category("自定义属性")]
        public TimeUnit TimeUnit;

        [Description("年份, 为0则是所有年份"), Category("自定义属性")]
        public int Year;

        private DateTime _beginDate;

        private DateTime _endDate;

        public DateTime BeginDate
        {
            set => _beginDate = value;
            get
            {
                if (TimeUnit == TimeUnit.Year)
                    return new DateTime(_beginDate.Year, 1, 1); 
                else if (TimeUnit == TimeUnit.Month)
                    return new DateTime(_beginDate.Year, _beginDate.Month, 1);
                else if (TimeUnit == TimeUnit.Week)
                    return _beginDate.GetWeekFirstDayMon();
                else if (TimeUnit == TimeUnit.Season)
                    return _beginDate.GetSeasonBegin();
                else
                    return _beginDate;
            }
        }

        public DateTime EndDate
        {
            set => _endDate = value;
            get
            {
                if (TimeUnit == TimeUnit.Year)
                    return new DateTime(_endDate.Year, 12, 31);
                else if (TimeUnit == TimeUnit.Month)
                    return _endDate.AddMonths(1).AddDays(-1);
                else if (TimeUnit == TimeUnit.Week)
                    return _endDate.GetWeekLastDaySun();
                else if (TimeUnit == TimeUnit.Season)
                    return _endDate.GetSeasonEnd();
                else
                    return _endDate;
            }
        }

        #endregion

        #region 图表

        public SeriesChartType ChartType 
        {
            get
            {
                if (TimeUnit == TimeUnit.NoTimeUnit)
                    return SeriesChartType.Pie;
                else
                    return GroupByFields.Count > 1 ? SeriesChartType.StackedColumn : SeriesChartType.Column;
            }
        }

        public ChartArea Area => chart1.ChartAreas[0];

        #endregion

        public BaseChart()
        {
            InitializeComponent();
            GroupByFields = new List<Field>();

            if (DesignMode)
                return;

            if (chart1.ChartAreas.Count == 0)
                chart1.ChartAreas.Add(new ChartArea());
        }

    }
}
