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

        TextBox FeatureOpTextBox { get; }

        string FeaturePageHeaderText { get; set; }

        BindingSource BindingSource { get; set; }

        BindingSource SampleBindingSource { get; set; }

        int OpKey { get; }

        string OpStorage { get; set; }

        string PartStorage { get; set; }

        string OperationNumber { get; set; }

        string PartNumber { get; set; }

        int FeatureCount { get; }

        object LastRowFeaturePartNumberFK { get; set; }

        object LastRowFeatureOperationNumberFK { get; set; }

        object FeatureDataSource { get; set; }
        DataGridView FeatureGridView { get; }


    }
}
