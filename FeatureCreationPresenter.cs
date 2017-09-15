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
            Initialize();
            
        }

        private void Initialize()
        {
            view.checkEnterKeyPressedFeatures += View_checkEnterKeyPressedFeatures;
            
        }

        private void View_checkEnterKeyPressedFeatures(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void DeleteDataGridViewRow(object sender, DataGridViewCellMouseEventArgs e)
        {
            var table = (DataGridView)sender;

            if (e.RowIndex != -1)
            {
                if (e.ColumnIndex == table.Columns[table.ColumnCount - 1].Index)
                {
                    table.Rows.Remove(table.Rows[e.RowIndex]);
                }
            }
        }

        public void AddFeatureRow(DataTable data)
        {
            
            data = AddTableRow(data);

            SetPartAndOpNumInTable();
        }

        private void SetPartAndOpNumInTable()
        {
            if (view.FeatureCount > 0)
            {
                //Set the last row Part_Number_FK and Operation_Number_FK to the same value as in the first row
                view.LastRowFeaturePartNumberFK = view.PartNumber;
                view.LastRowFeatureOperationNumberFK = view.OperationNumber;
            }

        }

        private DataTable AddTableRow(DataTable t)
        {
            if (view.FeatureDataSource == null)
            {
                return t;
            }
            DataRow newRow = t.NewRow();
            t.Rows.Add(newRow);

            return t;
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

        

       
    }
}
