using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

internal class CustomCollectionEditor : CollectionEditor
{

    public CustomCollectionEditor(Type type) : base(type) { }

    public delegate void CustomCollectionEditorFormClosedEventHandler(object sender,
                                        FormClosedEventArgs e);

    public static event CustomCollectionEditorFormClosedEventHandler CustomCollectionEditorFormClosed;

    public delegate void CustomCollectionEditorPropertyValueChangedEventHandler(object sender,
                                    PropertyValueChangedEventArgs e);

    public static event CustomCollectionEditorPropertyValueChangedEventHandler CustomCollectionEditorPropertyValueChanged;

    protected override CollectionForm CreateCollectionForm()
    {
        CollectionForm collectionForm = base.CreateCollectionForm();
        Form form = collectionForm as Form;
        TableLayoutPanel tableLayoutPanel = form.Controls[0] as TableLayoutPanel;
        
        //this fires when the collecion is closed
        collectionForm.FormClosed += new FormClosedEventHandler(collection_FormClosed);
        if (tableLayoutPanel != null)
        {
            if (tableLayoutPanel.Controls[5] is PropertyGrid)
            {
                PropertyGrid propertyGrid = tableLayoutPanel.Controls[5] as PropertyGrid;
                //this fires when a value in the collection is updated
                propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid_PropertyValueChanged);
            }
        }
        return collectionForm;
    }

    private void collection_FormClosed(object sender, FormClosedEventArgs e)
    {
        if (CustomCollectionEditorFormClosed != null)
        {
            CustomCollectionEditorFormClosed(this, e);
        }
    }

    private void propertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
    {
        if (CustomCollectionEditor.CustomCollectionEditorPropertyValueChanged != null)
        {
            CustomCollectionEditor.CustomCollectionEditorPropertyValueChanged(this, e);
        }
    }

}