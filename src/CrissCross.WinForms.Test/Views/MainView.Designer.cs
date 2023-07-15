namespace CrissCross.WinForms.Test.Views;

partial class MainView
{
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        GotoFirst = new Button();
        GotoMain = new Button();
        label1 = new Label();
        SuspendLayout();
        // 
        // GotoFirst
        // 
        GotoFirst.Location = new Point(502, 338);
        GotoFirst.Name = "GotoFirst";
        GotoFirst.Size = new Size(112, 34);
        GotoFirst.TabIndex = 4;
        GotoFirst.Text = "Goto First";
        GotoFirst.UseVisualStyleBackColor = true;
        // 
        // GotoMain
        // 
        GotoMain.Location = new Point(501, 275);
        GotoMain.Name = "GotoMain";
        GotoMain.Size = new Size(112, 34);
        GotoMain.TabIndex = 3;
        GotoMain.Text = "Goto Main";
        GotoMain.UseVisualStyleBackColor = true;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(41, 35);
        label1.Name = "label1";
        label1.Size = new Size(93, 25);
        label1.TabIndex = 5;
        label1.Text = "Main View";
        // 
        // MainView
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Red;
        Controls.Add(label1);
        Controls.Add(GotoFirst);
        Controls.Add(GotoMain);
        Name = "MainView";
        Size = new Size(1115, 647);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button GotoFirst;
    private Button GotoMain;
    private Label label1;
}
