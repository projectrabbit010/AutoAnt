using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using GDataDB;
using GDataDB.Linq;

using UnityQuickSheet;

namespace HK.AutoAnt.Database.SpreadSheetData
{
    ///
    /// !!! Machine generated code !!!
    ///
    [CustomEditor(typeof(MasterDataHousingLevelParameter))]
    public class HousingLevelParameterEditor : BaseGoogleEditor<MasterDataHousingLevelParameter>
    {
        private const string WorkSheetName = "HousingLevelParameter";

        private const string SpreadSheetName = "AutoAnt";

        public override bool Load()
        {        
            var targetData = target as MasterDataHousingLevelParameter;
            var client = new DatabaseClient("", "");
            var error = string.Empty;
            var db = client.GetDatabase(SpreadSheetName, ref error);	
            var table = db.GetTable<HousingLevelParameterData>(WorkSheetName) ?? db.CreateTable<HousingLevelParameterData>(WorkSheetName);
            var myDataList = new List<MasterDataHousingLevelParameter.Record>();
            var all = table.FindAll();

            foreach(var element in all)
            {
                var data = new HousingLevelParameterData();
                data = Cloner.DeepCopy<HousingLevelParameterData>(element.Element);
                myDataList.Add(new MasterDataHousingLevelParameter.Record(data));
            }
                    
            targetData.Records = myDataList.ToArray();
            
            EditorUtility.SetDirty(targetData);
            AssetDatabase.SaveAssets();
            
            return true;
        }
    }
}
