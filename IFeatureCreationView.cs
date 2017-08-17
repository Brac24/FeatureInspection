using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature_Inspection
{
    public interface IFeatureCreationView
    {
        #region Show Methods

        void ShowRelatedFeatures(IList<Feature> relatedFeaures);

        void ShowJobInformation(Job job);

        #endregion

        

        event EventHandler EditClicked;
        event EventHandler EnterClicked;
        event EventHandler AddFeatureClicked;
        event EventHandler LotInspectionReadyClicked;

    }
}
