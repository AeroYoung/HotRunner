using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HotRunner
{
    public class BtnManifoldPage : PropertyManagerPage2Handler9
    {
        #region Local Objects
        PropertyManagerPage2 swPropertyPage = null;
        SldWorks iSwApp = null;
        SwAddin userAddin = null;
        #endregion

        #region Property Manager Page Controls
        //Groups
        IPropertyManagerPageGroup group1;

        //Controls
        public PropertyManagerPageSelectionbox selection1;

        //Control IDs
        public const int group1ID = 0;
        public const int selection1ID = 8;
        #endregion

        #region Control

        public BtnManifoldPage(SwAddin addin)
        {
            userAddin = addin;
            if (userAddin != null)
            {
                iSwApp = (SldWorks)userAddin.SwApp;
                CreatePropertyManagerPage();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("SwAddin not set.");
            }
        }
        
        protected void CreatePropertyManagerPage()
        {
            int errors = -1;
            int options = (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
                (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton;
            
            swPropertyPage = (PropertyManagerPage2)iSwApp.CreatePropertyManagerPage("创建分流板主体", (int)options, (PropertyManagerPage2Handler9)this, ref errors);
            if (swPropertyPage != null && errors == (int)swPropertyManagerPageStatus_e.swPropertyManagerPage_Okay)
            {
                try
                {
                    AddControls();
                }
                catch (Exception e)
                {
                    iSwApp.SendMsgToUser2(e.Message, 0, 0);
                }
            }
        }
        
        protected void AddControls()
        {
            short controlType = -1;
            short align = -1;
            int options = -1;
            
            //Add the groups
            options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                      (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;

            group1 = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(group1ID, "选择点位草图", options);


            //Add the controls to group1

            //selection1
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                (int)swAddControlOptions_e.swControlOptions_Visible;

            selection1 = (PropertyManagerPageSelectionbox)group1.AddControl(selection1ID, controlType, "Sample Selection", align, options, "Displays features selected in main view");
            if (selection1 != null)
            {
                int[] filter = { (int)swSelectType_e.swSelSKETCHES };
                selection1.Height = 40;
                selection1.SetSelectionFilters(filter);
                selection1.SingleEntityOnly = true;
            }

        }

        public void Show()
        {
            if (swPropertyPage != null)
            {
                swPropertyPage.Show();
            }
        }

        #endregion

        Sketch swSketch = null;
        int Count = 0;

        #region Handler 接口

        public bool OnSubmitSelection(int Id, object Selection, int SelType, ref string ItemText)
        {
            if (Id == selection1ID)
            {
                //ISketch sketch1 = (ISketch)Selection;
                Feature f = (Feature)Selection;
                swSketch = (Sketch)f.GetSpecificFeature2();
                
                f.Name = "Basic";
            }
            return true;
        }

        #region SelectionBox

        public void OnSelectionboxCalloutCreated(int Id)
        {
            ////throw new NotImplementedException();
        }

        public void OnSelectionboxCalloutDestroyed(int Id)
        {
            ////throw new NotImplementedException();
        }

        public void OnSelectionboxFocusChanged(int Id)
        {
            ////throw new NotImplementedException();
        }
        
        public void OnSelectionboxListChanged(int Id, int Count)
        {
            this.Count = Count;
        }

        #endregion

        public void AfterActivation()
        {
            ////throw new NotImplementedException();
        }

        public void AfterClose()
        {
            //throw new NotImplementedException();
        }

        public int OnActiveXControlCreated(int Id, bool Status)
        {
            //throw new NotImplementedException();
            return -1;
        }

        public void OnButtonPress(int Id)
        {
            MessageBox.Show(Id.ToString());
        }

        public void OnCheckboxCheck(int Id, bool Checked)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Reason">1:OK 2:Cancel</param>
        public void OnClose(int Reason)
        {
            if (Reason == (int)swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                if (Count == 0 || swSketch==null) return;

                //TODO
            }
        }

        public void OnComboboxEditChanged(int Id, string Text)
        {
            //throw new NotImplementedException();
        }

        public void OnComboboxSelectionChanged(int Id, int Item)
        {
            //throw new NotImplementedException();
        }

        public void OnGainedFocus(int Id)
        {
            //throw new NotImplementedException();
        }

        public void OnGroupCheck(int Id, bool Checked)
        {
            //throw new NotImplementedException();
        }

        public void OnGroupExpand(int Id, bool Expanded)
        {
            //throw new NotImplementedException();
        }

        public bool OnHelp()
        {
            //throw new NotImplementedException();
            return true;
        }

        public bool OnKeystroke(int Wparam, int Message, int Lparam, int Id)
        {
            //throw new NotImplementedException();
            return true;
        }

        public void OnListboxRMBUp(int Id, int PosX, int PosY)
        {
            //throw new NotImplementedException();
        }

        public void OnListboxSelectionChanged(int Id, int Item)
        {
            //throw new NotImplementedException();
        }

        public void OnLostFocus(int Id)
        {
            //throw new NotImplementedException();
        }

        public bool OnNextPage()
        {
            return true;
            //throw new NotImplementedException();
        }

        public void OnNumberboxChanged(int Id, double Value)
        {
            //throw new NotImplementedException();
        }

        public void OnNumberBoxTrackingCompleted(int Id, double Value)
        {
            //throw new NotImplementedException();
        }

        public void OnOptionCheck(int Id)
        {
            //throw new NotImplementedException();
        }

        public void OnPopupMenuItem(int Id)
        {
            //throw new NotImplementedException();
        }

        public void OnPopupMenuItemUpdate(int Id, ref int retval)
        {
            //throw new NotImplementedException();
        }

        public bool OnPreview()
        {
            //throw new NotImplementedException();
            return true;
        }

        public bool OnPreviousPage()
        {
            //throw new NotImplementedException();
            return true;
        }

        public void OnRedo()
        {
            //throw new NotImplementedException();
        }
        
        public void OnSliderPositionChanged(int Id, double Value)
        {
            //throw new NotImplementedException();
        }

        public void OnSliderTrackingCompleted(int Id, double Value)
        {
            ////throw new NotImplementedException();
        }

        public bool OnTabClicked(int Id)
        {
            //throw new NotImplementedException();
            return true;
        }

        public void OnTextboxChanged(int Id, string Text)
        {
            //throw new NotImplementedException();
        }

        public void OnUndo()
        {
            //throw new NotImplementedException();
        }

        public void OnWhatsNew()
        {
            //throw new NotImplementedException();
        }

        public int OnWindowFromHandleControlCreated(int Id, bool Status)
        {
            //throw new NotImplementedException();
            return 0;
        }

        #endregion
    }
}
