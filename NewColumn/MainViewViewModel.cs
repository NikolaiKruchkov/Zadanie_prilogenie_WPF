using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewColumn
{
    class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        public FamilySymbol SelectedColumnType { get; set; }
        public Level SelectedLevel { get; set; }
        public List<FamilySymbol> TypeColumns { get; } = new List<FamilySymbol>();
        public List<Level> Levels { get; } = new List<Level>();
        public DelegateCommand SaveCommand { get; }
        public double ColumnOffset { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public double CoordinateZ { get; set; }
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            TypeColumns = ColumnUtils.GetColumnTypes(commandData);
            Levels = LevelsUtils.GetLevels(commandData);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            CoordinateX = 0;
            CoordinateY = 0;
            CoordinateZ = 0;
            ColumnOffset = 0;
        }
        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            XYZ point = new XYZ(CoordinateX, CoordinateY, CoordinateZ);
            if (SelectedColumnType == null || SelectedLevel == null)
                return;            
          FamilyInstance column = FamilyInstanceUtils.CreateFamilyInstance(_commandData, SelectedColumnType, point, SelectedLevel);         
            
            using (Transaction ts1 = new Transaction(doc, "Set parameter"))
            {
                ts1.Start();                
                Parameter parameterOffset = column.LookupParameter("Base Offset");
                parameterOffset.Set(UnitUtils.ConvertToInternalUnits(ColumnOffset, DisplayUnitType.DUT_MILLIMETERS));
                ts1.Commit();
            }
            RaiseCloseRequest();
        }
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}

