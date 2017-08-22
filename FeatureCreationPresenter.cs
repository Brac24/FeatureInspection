using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;

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
            Initialize();
            
        }

        private void Initialize()
        {
            view.EditClicked += EditClick;
            view.DoneClicked += DoneClick;
        }

        public bool ViewExists()
        {
            if (view == null)
            {
                return false;
            }
            else
                return true;
        }

        public void EditClick(object sender, EventArgs e)
        {

        }

        public void DoneClick(object t, EventArgs e)
        {
            Feature feature = new Feature();
            var row = (DataGridViewRow)t;

           

        }

       
    }
}
