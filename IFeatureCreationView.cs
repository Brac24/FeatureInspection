﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature_Inspection
{
    public interface IFeatureCreationView
    {
        int OpKey { get; }

        string OperationNumber { get; }

        string PartNumber { get; }
        int FeatureCount { get; }
        object FeaturePartNumberFK { set; }
        object FeatureOperationNumberFK { set; }
        object FeatureDataSource { get; set; }
        #region Show Methods

        void ShowRelatedFeatures(IList<Feature> relatedFeaures);

        void ShowJobInformation(Job job);

        #endregion

        

        event EventHandler<EventArgs> EditClicked;
        event EventHandler<EventArgs> DoneClicked;
        event EventHandler EnterClicked;
        event EventHandler AddFeatureClicked;
        event EventHandler LotInspectionReadyClicked;

        
        
    }
}
