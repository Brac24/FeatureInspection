using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature_Inspection
{
    class ReportPresenter
    {
        private IReportView view;
        private IReportDataSource model;

        public ReportPresenter(IReportView view, IReportDataSource model)
        {
            this.view = view;
            this.model = model;
            Initialize();
        }

        private void Initialize()
        {

        }
    }
}
