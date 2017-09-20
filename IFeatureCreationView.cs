using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Feature_Inspection
{
    public interface IFeatureCreationView
    {

        TextBox PartTextBox { get; }

        TextBox OpTextBox { get; }

        string FeaturePageHeader { set; }
       
        BindingSource BindingSource { get; set; }

        BindingSource SampleBindingSource { get; set; }

        int OpKey { get; }

        string OperationNumber { get; }

        string PartNumber { get; }


        int FeatureCount { get; }

        object LastRowFeaturePartNumberFK { get; set; }

        object LastRowFeatureOperationNumberFK { get; set; }

        object FeatureDataSource { get; set; }
        DataGridView FeatureGridView { get; }

        #region Show Methods

        void ShowRelatedFeatures(IList<Feature> relatedFeaures);

        void ShowJobInformation(Job job);

        string CreateYesNoMessage(string message, string caption);

        #endregion

        
        
    }
}
