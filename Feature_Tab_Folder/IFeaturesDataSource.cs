﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Feature_Inspection
{
    public interface IFeaturesDataSource
    {
        //Job JobInfo { get; }


        //Will bind data directly to view. No need to go through presenter
        DataTable GetFeaturesOnPartAndOpNumber(string partnumber, string operationNum);
        DataTable AdapterUpdate(DataTable table);
        DataTable GetFeaturesOnPartIndex(int pieceID, int opKey);
        bool PartNumberExists(string partNumber);
        bool OpExists(string operationNumber, string partNumber);
        DataTable GetSampleChoices();
        DataTable GetChartData(int opKey, int featureKey);
        DataTable AdapterUpdateInspection(DataTable table);
        DataTable GetInfoFromOpKeyEntry(int opkey);
        DataTable GetPartsList(int opkey);
        DataTable GetFeatureList(int opkey);
        DataTable GetFeaturesOnOpKey(int opkey);
        bool GetInspectionExistsOnOpKey(int opkey);
        string GetLotSize(int opkey);
        void InsertLotSizeToInspectionTable(int lotSize, int opkey);
        void InsertPartsToPositionTable(int opkey, int lotSize);
        void CreateInspectionInInspectionTable(int opkey);
    }
}
