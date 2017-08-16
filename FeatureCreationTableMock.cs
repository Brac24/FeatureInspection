using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Feature_Inspection
{
    public partial class FeatureCreationTableMock : Form, IFeatureCreationView
    {
        public FeatureCreationTableMock()
        {
            InitializeComponent();
        }

        public FeatureCreationPresenter Presenter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler AddFeatureClicked;
        public event EventHandler EditClicked;
        public event EventHandler EnterClicked;
        public event EventHandler LotInspectionReadyClicked;

        public void ShowJobInformation(Job job)
        {
            throw new NotImplementedException();
        }

        public void ShowRelatedFeatures(IList<Feature> relatedFeaures)
        {
            throw new NotImplementedException();
        }
    }
}
