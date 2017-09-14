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

        public void DelteRow(object sender, DataGridViewCellMouseEventArgs e)
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
