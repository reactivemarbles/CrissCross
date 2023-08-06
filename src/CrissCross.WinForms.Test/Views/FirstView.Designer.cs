namespace CrissCross.WinForms.Test.Views;

partial class FirstView
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
        label1 = new Label();
        GotoMain = new Button();
        GotoFirst = new Button();
        SuspendLayout();
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(34, 56);
        label1.Name = "label1";
        label1.Size = new Size(87, 25);
        label1.TabIndex = 0;
        label1.Text = "First View";
        // 
        // GotoMain
        // 
        GotoMain.Location = new Point(396, 108);
        GotoMain.Name = "GotoMain";
        GotoMain.Size = new Size(112, 34);
        GotoMain.TabIndex = 1;
        GotoMain.Text = "Goto Main";
        GotoMain.UseVisualStyleBackColor = true;
        // 
        // GotoFirst
        // 
        GotoFirst.Location = new Point(397, 171);
        GotoFirst.Name = "GotoFirst";
        GotoFirst.Size = new Size(112, 34);
        GotoFirst.TabIndex = 2;
        GotoFirst.Text = "Goto First";
        GotoFirst.UseVisualStyleBackColor = true;
        // 
        // FirstView
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Pink;
        Controls.Add(GotoFirst);
        Controls.Add(GotoMain);
        Controls.Add(label1);
        Name = "FirstView";
        Size = new Size(932, 570);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label label1;
    private Button GotoMain;
    private Button GotoFirst;
}
