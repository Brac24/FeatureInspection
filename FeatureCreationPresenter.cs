using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature_Inspection
{
    public class FeatureCreationPresenter
    {
        private IFeatureCreationView view;
        private IFeaturesDataSource model;

        public FeatureCreationPresenter (IFeatureCreationView view, IFeaturesDataSource model)
        {
            this.view = view;
            this.model = model;
           
        }

       

       
    }
}
