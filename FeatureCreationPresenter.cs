﻿using System;
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

            
           
        }

        private void Initialize()
        {
            view.EditClicked += EditClick;
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

    }
}
