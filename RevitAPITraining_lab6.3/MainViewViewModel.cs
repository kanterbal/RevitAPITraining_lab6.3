using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining_lab6._3
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        public List<FamilySymbol> Furniture { get; } = new List<FamilySymbol>();
        public List<Level> Levels { get; } = new List<Level> { };
        public int ElementCount { get; set; }
        public DelegateCommand SaveCommand { get; }
        public XYZ InsertionPoint1 { get; }
        public XYZ InsertionPoint2 { get; }
        public FamilySymbol SelectedFurniture { get; set; }
        public Level SelectedLevel { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            Furniture = FurnitureUtils.GetFurniture(commandData);
            Levels = LevelsUtils.GetLevels(commandData);
            ElementCount = 0;
            SaveCommand = new DelegateCommand(onSaveCommand);
            InsertionPoint1 = SelectionUtils.GetPoint(_commandData, "Выберите первую точку", ObjectSnapTypes.Endpoints);
            InsertionPoint2 = SelectionUtils.GetPoint(_commandData, "Выберите вторую точку", ObjectSnapTypes.Endpoints);
        }
        private void onSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (SelectedFurniture == null || SelectedLevel == null)
                return;
            var oLevel = (Level)doc.GetElement(SelectedLevel.Id);
            var oFamSymb = (FamilySymbol)doc.GetElement(SelectedFurniture.Id);

            var p0X = InsertionPoint1.X;
            var p0Y = InsertionPoint1.Y;
            var pEX = InsertionPoint2.X;
            var pEY = InsertionPoint2.Y;

            for (int i = 1; i < ElementCount + 1; i++)
            {
                var pPX = pEX - i * ((pEX - p0X) / (ElementCount + 1));
                var pPY = pEY - i * ((pEY - p0Y) / (ElementCount + 1));
                XYZ pP = new XYZ(pPX, pPY, 0);

                FamilyInstanceUtils.CreateFamilyInstance(_commandData, oFamSymb, pP, oLevel);
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
