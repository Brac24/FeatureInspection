using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature_Inspection
{
    interface IFeatureCreationView
    {
        #region Show Methods

        void ShowRelatedFeatures(IList<Feature> relatedFeaures);

        void ShowJobInformation(Job job);

        #endregion

        FeatureCreationPresenter Presenter { get; }

        event EventHandler EditClicked;
        event EventHandler EnterClicked;
        event EventHandler AddFeatureClicked;
        event EventHandler LotInspectionReadyClicked;

    }
}
